using Common;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class MemberService(DatabaseContext context) : IMemberService
{
    private DbSet<Member> Members => context.Members;

    public async Task<Member?> GetMemberById(Guid id)
    {
        return await Members.FindAsync(id);
    }

    public async Task<IEnumerable<Member>> GetAllMembers()
    {
        return await Members.ToArrayAsync();
    }

    public async Task AddMember(Member member)
    {
        await ValidateAddMember(member);

        await Members.AddAsync(member);
        await context.SaveChangesAsync();
    }

    public async Task EditMember(Member member)
    {
        var existingMember = await CheckForExistingMember(member);

        UpdateMemberDetails(member, existingMember);

        await context.SaveChangesAsync();
    }

    public async Task DeleteMember(Member member)
    {
        var existingMember = await CheckForExistingMember(member);

        Members.Remove(existingMember);
        await context.SaveChangesAsync();
    }

    private async Task ValidateAddMember(Member member)
    {
        if (member.Id == Guid.Empty)
        {
            member.Id = Guid.NewGuid();
        }

        var existingMember = await GetMemberById(member.Id);
        if (existingMember is not null)
        {
            throw new ArgumentException("Member with the same ID already exists.");
        }

        if (member.Id == member.FatherId || member.Id == member.MotherId)
        {
            throw new ArgumentException("A member cannot be their own parent.");
        }

        var invalidParents = new List<string>();
        if (member.FatherId is not null)
        {
            var father = await Members.FindAsync(member.FatherId);
            if (father is null)
            {
                invalidParents.Add("Father not found.");
            }
        }

        if (member.MotherId is not null)
        {
            var mother = await Members.FindAsync(member.MotherId);
            if (mother is null)
            {
                invalidParents.Add("Mother not found.");
            }
        }

        if (invalidParents.Count != 0)
        {
            throw new AggregateException(invalidParents.Select(e => new ArgumentException(e)));
        }
    }

    private static void UpdateMemberDetails(Member member, Member existingMember)
    {
        if (member.FirstName != existingMember.FirstName)
        {
            existingMember.FirstName = member.FirstName;
        }

        if (member.LastName != existingMember.LastName)
        {
            existingMember.LastName = member.LastName;
        }

        if (member.Gender != existingMember.Gender)
        {
            existingMember.Gender = member.Gender;
        }

        if (member.FatherId is not null && member.FatherId != existingMember.FatherId)
        {
            existingMember.FatherId = member.FatherId;
        }

        if (member.MotherId is not null && member.MotherId != existingMember.MotherId)
        {
            existingMember.MotherId = member.MotherId;
        }
    }

    private async Task<Member> CheckForExistingMember(Member member)
    {
        var existingMember = await GetMemberById(member.Id);

        if (existingMember is null)
        {
            throw new ArgumentException("Member not found.");
        }

        return existingMember;
    }
}