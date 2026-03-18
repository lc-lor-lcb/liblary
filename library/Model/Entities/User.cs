namespace LibrarySystem.Model.Entities;

/// <summary>利用者エンティティ</summary>
public class User
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    /// <summary>性別（false:男性 true:女性）</summary>
    public bool Gender { get; set; }
    public DateOnly Birth { get; set; }
    public string Mail { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}
