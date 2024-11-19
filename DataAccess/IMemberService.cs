using Common;

namespace DataAccess;

public interface IMemberService
{
    Task<Member?> GetMemberById(Guid id);
    Task<IEnumerable<Member>> GetAllMembers();
    Task AddMember(Member member);
    Task EditMember(Member member);
    Task DeleteMember(Member member);
}