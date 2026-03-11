using System;
using System.Collections.Generic;
using System.Text;
using LibrarySystem.Model.Common;
using LibrarySystem.Model.Dto;
using LibrarySystem.Model.Entities;
using LibrarySystem.Model.Repositories;
using System.Text.RegularExpressions;

namespace LibrarySystem.Model.Services;

/// <summary>利用者管理サービスインターフェース</summary>
public interface IUserService
{
    /// <summary>利用者を新規登録する。</summary>
    Task<Result<User>> CreateUserAsync(CreateUserDto dto);

    /// <summary>利用者情報を更新する。</summary>
    Task<Result<User>> UpdateUserAsync(int userId, UpdateUserDto dto);

    /// <summary>氏名で利用者を検索する（部分一致）。</summary>
    Task<IList<User>> SearchAsync(string name);

    /// <summary>利用者IDで1件取得する。存在しない場合は null。</summary>
    Task<User?> GetByIdAsync(int userId);
}

/// <summary>利用者管理サービス実装</summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    // 設計書5.8.1: 電話番号は数字・ハイフンのみ、7〜15桁（ハイフン除く）
    private static readonly Regex PhoneRegex =
        new(@"^[\d\-]{7,15}$", RegexOptions.Compiled);

    // RFC5322準拠の簡易メールアドレスパターン
    private static readonly Regex MailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <inheritdoc/>
    public async Task<Result<User>> CreateUserAsync(CreateUserDto dto)
    {
        var validationError = ValidateUserDto(dto.Name, dto.Birth, dto.Mail, dto.Phone, dto.Address);
        if (validationError != null)
            return Result<User>.Failure(validationError);

        // メールアドレス重複チェック
        if (await _userRepository.ExistsMailAsync(dto.Mail))
            return Result<User>.Failure("このメールアドレスは既に登録されています。");

        var user = new User
        {
            Name = dto.Name.Trim(),
            Gender = dto.Gender,
            Birth = dto.Birth,
            Mail = dto.Mail.Trim(),
            Phone = dto.Phone.Trim(),
            Address = dto.Address.Trim(),
            IsActive = true,
            CreatedAt = DateTime.Now,
        };

        var inserted = await _userRepository.InsertAsync(user);
        return Result<User>.Success(inserted);
    }

    /// <inheritdoc/>
    public async Task<Result<User>> UpdateUserAsync(int userId, UpdateUserDto dto)
    {
        var existing = await _userRepository.FindByIdAsync(userId);
        if (existing == null || !existing.IsActive)
            return Result<User>.Failure("利用者IDが存在しないか、無効な利用者です。");

        var validationError = ValidateUserDto(dto.Name, dto.Birth, dto.Mail, dto.Phone, dto.Address);
        if (validationError != null)
            return Result<User>.Failure(validationError);

        // 他の利用者と重複するメールでないか確認（自分自身は除外）
        if (await _userRepository.ExistsMailAsync(dto.Mail, excludeId: userId))
            return Result<User>.Failure("このメールアドレスは既に他の利用者に登録されています。");

        existing.Name = dto.Name.Trim();
        existing.Gender = dto.Gender;
        existing.Birth = dto.Birth;
        existing.Mail = dto.Mail.Trim();
        existing.Phone = dto.Phone.Trim();
        existing.Address = dto.Address.Trim();

        var updated = await _userRepository.UpdateAsync(existing);
        return Result<User>.Success(updated);
    }

    /// <inheritdoc/>
    public async Task<IList<User>> SearchAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Array.Empty<User>();

        return await _userRepository.SearchByNameAsync(name.Trim());
    }

    /// <inheritdoc/>
    public async Task<User?> GetByIdAsync(int userId)
    {
        return await _userRepository.FindByIdAsync(userId);
    }

    // ----------------------------------------------------------------
    // バリデーション（設計書5.8.1準拠）
    // ----------------------------------------------------------------
    private static string? ValidateUserDto(
        string name, DateOnly birth, string mail, string phone, string address)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 100)
            return "氏名を1〜100文字で入力してください。";

        if (birth >= DateOnly.FromDateTime(DateTime.Today))
            return "生年月日は過去の日付を入力してください。";

        if (string.IsNullOrWhiteSpace(mail) || !MailRegex.IsMatch(mail.Trim()))
            return "正しい形式のメールアドレスを入力してください。";

        if (mail.Trim().Length > 254)
            return "メールアドレスは254文字以内で入力してください。";

        var phoneTrimmed = phone?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(phoneTrimmed) || !PhoneRegex.IsMatch(phoneTrimmed))
            return "電話番号は数字・ハイフンのみ、7〜15桁で入力してください。";

        if (string.IsNullOrWhiteSpace(address) || address.Trim().Length > 500)
            return "住所を1〜500文字で入力してください。";

        return null;
    }
}