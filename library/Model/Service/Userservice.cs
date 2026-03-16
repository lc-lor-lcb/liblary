using library.Model.Common;
using library.Model.Dto;
using library.Model.Entities;
using library.Model.Repositories;
using System.Text.RegularExpressions;

namespace library.Model.Services;

public interface IUserService
{
    Task<Result<User>> CreateUserAsync(CreateUserDto dto);
    Task<Result<User>> UpdateUserAsync(int userId, UpdateUserDto dto);
    Task<IList<User>> SearchAsync(string name);
    Task<User?> GetByIdAsync(int userId);
    Task<bool> DeactivateUserAsync(int userId);   // ← 追加
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    private static readonly Regex PhoneRegex =
        new(@"^[\d\-]{7,15}$", RegexOptions.Compiled);
    private static readonly Regex MailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<User>> CreateUserAsync(CreateUserDto dto)
    {
        var err = ValidateUserDto(dto.Name, dto.Birth, dto.Mail, dto.Phone, dto.Address);
        if (err != null) return Result<User>.Failure(err);

        if (await _userRepository.GetByMailAsync(dto.Mail) != null)
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

        var id = await _userRepository.InsertAsync(user);
        var inserted = await _userRepository.GetByIdAsync(id);
        return inserted != null
            ? Result<User>.Success(inserted)
            : Result<User>.Failure("利用者の登録に失敗しました。");
    }

    public async Task<Result<User>> UpdateUserAsync(int userId, UpdateUserDto dto)
    {
        var existing = await _userRepository.GetByIdAsync(userId);
        if (existing == null || !existing.IsActive)
            return Result<User>.Failure("利用者IDが存在しないか、無効な利用者です。");

        var err = ValidateUserDto(dto.Name, dto.Birth, dto.Mail, dto.Phone, dto.Address);
        if (err != null) return Result<User>.Failure(err);

        var other = await _userRepository.GetByMailAsync(dto.Mail);
        if (other != null && other.Id != userId)
            return Result<User>.Failure("このメールアドレスは既に他の利用者に登録されています。");

        existing.Name = dto.Name.Trim(); existing.Gender = dto.Gender;
        existing.Birth = dto.Birth; existing.Mail = dto.Mail.Trim();
        existing.Phone = dto.Phone.Trim(); existing.Address = dto.Address.Trim();

        await _userRepository.UpdateAsync(existing);

        var updated = await _userRepository.GetByIdAsync(userId);
        return updated != null
            ? Result<User>.Success(updated)
            : Result<User>.Failure("利用者情報の更新に失敗しました。");
    }

    public async Task<IList<User>> SearchAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return Array.Empty<User>();
        return await _userRepository.SearchByNameAsync(name.Trim());
    }

    public async Task<User?> GetByIdAsync(int userId)
        => await _userRepository.GetByIdAsync(userId);

    public async Task<bool> DeactivateUserAsync(int userId)   // ← 追加
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || !user.IsActive) return false;
        user.IsActive = false;
        await _userRepository.UpdateAsync(user);
        return true;
    }

    private static string? ValidateUserDto(
        string name, DateOnly birth, string mail, string phone, string address)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 100)
            return "氏名を1〜100文字で入力してください。";
        if (birth >= DateOnly.FromDateTime(DateTime.Today))
            return "生年月日は過去の日付を入力してください。";
        if (string.IsNullOrWhiteSpace(mail) || !MailRegex.IsMatch(mail.Trim()) || mail.Trim().Length > 254)
            return "正しい形式のメールアドレスを入力してください。";
        if (string.IsNullOrWhiteSpace(phone) || !PhoneRegex.IsMatch(phone.Trim()))
            return "電話番号は数字・ハイフンのみ、7〜15桁で入力してください。";
        if (string.IsNullOrWhiteSpace(address) || address.Trim().Length > 500)
            return "住所を1〜500文字で入力してください。";
        return null;
    }
}