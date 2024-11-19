// Some properties are not init only because they are set by EF Core.
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Common;

public class Member
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public Gender Gender { get; set; }
    public Guid? FatherId { get; set; }
    public Guid? MotherId { get; set; }

    public Member? Father { get; set; }
    public Member? Mother { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is not Member member)
        {
            return false;
        }

        return Id == member.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}