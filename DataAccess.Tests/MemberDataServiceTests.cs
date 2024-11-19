using Common;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Tests;

public class MemberDataServiceTests
{
    private DatabaseContext _context;
    private IMemberService _memberService;
    private Member _papaDoe;
    private Member _mamaDoe;
    private Member _johnDoe;
    private IEnumerable<Member> _family;

    [SetUp]
    public async Task Setup()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlite("Data Source=Ancestry_test.db")
            .Options;

        _context = new DatabaseContext(options);
        await _context.InitializeDatabase();

        _memberService = new MemberService(_context);

        var id = Guid.NewGuid();
        var fatherId = Guid.NewGuid();
        var motherId = Guid.NewGuid();

        _papaDoe = new Member
        {
            Id = fatherId,
            FirstName = "Papa",
            LastName = "Doe",
            Gender = Gender.Male
        };

        _mamaDoe = new Member
        {
            Id = motherId,
            FirstName = "Mama",
            LastName = "Doe",
            Gender = Gender.Female
        };

        _context.Members.Add(_papaDoe);
        _context.Members.Add(_mamaDoe);

        _johnDoe = new Member
        {
            Id = id,
            FirstName = "John",
            LastName = "Doe",
            Gender = Gender.Male,
            FatherId = fatherId,
            MotherId = motherId
        };

        _family = [_papaDoe, _mamaDoe, _johnDoe];
    }

    [Test]
    public async Task GetMemberById_ShouldReturnMember()
    {
        await _memberService.AddMember(_johnDoe);

        var member = await _memberService.GetMemberById(_johnDoe.Id);

        using (new AssertionScope())
        {
            member.Should().NotBeNull();
            member.FirstName.Should().Be("John");
            member.LastName.Should().Be("Doe");
            member.Gender.Should().Be(Gender.Male);
            member.FatherId.Should().Be(_papaDoe.Id);
            member.MotherId.Should().Be(_mamaDoe.Id);
        }
    }

    [Test]
    public async Task GetAllMembers_ShouldReturnAllMembers()
    {
        await _memberService.AddMember(_johnDoe);

        var members = await _memberService.GetAllMembers();
        var membersArray = members.ToArray();

        membersArray.Length.Should().Be(3);
        membersArray.Should().Contain(_family);
    }

    [Test]
    public async Task AddMember_ShouldAddMemberToDatabase()
    {
        await _memberService.AddMember(_johnDoe);

        var addedMember = await _context.Members.FindAsync(_johnDoe.Id);
        using (new AssertionScope())
        {
            addedMember.Should().NotBeNull();
            addedMember.FirstName.Should().Be("John");
            addedMember.LastName.Should().Be("Doe");
            addedMember.Gender.Should().Be(Gender.Male);
            addedMember.FatherId.Should().Be(_papaDoe.Id);
            addedMember.MotherId.Should().Be(_mamaDoe.Id);
        }
    }

    [Test]
    public async Task EditMember_ShouldUpdateMember()
    {
        var newMamaDoe = new Member
        {
            Id = Guid.NewGuid(),
            FirstName = "NewMana",
            LastName = "Doe",
            Gender = Gender.Female
        };

        await _memberService.AddMember(newMamaDoe);
        await _memberService.AddMember(_johnDoe);

        var modifiedMember = new Member
        {
            Id = _johnDoe.Id,
            FirstName = _johnDoe.FirstName,
            LastName = _johnDoe.LastName,
            FatherId = _papaDoe.Id,
            MotherId = newMamaDoe.Id,
            Gender = Gender.Male,
        };
        await _memberService.EditMember(modifiedMember);

        using (new AssertionScope())
        {
            var updatedMember = await _context.Members.FindAsync(modifiedMember.Id);
            updatedMember.Should().NotBeNull();
            updatedMember.FirstName.Should().Be("John");
            updatedMember.LastName.Should().Be("Doe");
            updatedMember.Gender.Should().Be(Gender.Male);
            updatedMember.FatherId.Should().Be(modifiedMember.FatherId);
            updatedMember.MotherId.Should().Be(modifiedMember.MotherId);
        }
    }

    [Test]
    public async Task DeleteMember_ShouldRemoveMemberFromDatabase()
    {
        await _memberService.AddMember(_johnDoe);
        await _memberService.DeleteMember(_johnDoe);

        var deletedMember = await _context.Members.FindAsync(_johnDoe.Id);
        deletedMember.Should().BeNull();
    }

    [TearDown]
    public async Task TearDown()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }
}