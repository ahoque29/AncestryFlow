using Common;
using Controllers.Controllers;
using DataAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ClearExtensions;

namespace Controllers.Tests;

[TestFixture]
public class MemberControllerTests
{
    private IMemberService _service;
    private MemberController _controller;
    private Member _johnDoe;

    [SetUp]
    public void SetUp()
    {
        _service = Substitute.For<IMemberService>();
        _controller = new MemberController(_service);

        _johnDoe = new Member
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Gender = Gender.Male
        };
    }

    [Test]
    public async Task GetMemberById_ShouldReturnMember_WhenMemberExists()
    {
        _service.GetMemberById(_johnDoe.Id).Returns(_johnDoe);

        var result = await _controller.GetMemberById(_johnDoe.Id);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.Value.Should().Be(_johnDoe);
    }

    [Test]
    public async Task GetMemberById_ShouldReturnNotFound_WhenMemberDoesNotExist()
    {
        _service.GetMemberById(Guid.NewGuid()).Returns((Member?)null);

        var result = await _controller.GetMemberById(Guid.NewGuid());
        result.Should().BeOfType<NotFoundResult>();
    }

    [Test]
    public async Task GetAllMembers_ShouldReturnAllMembers()
    {
        Member[] members = [_johnDoe];
        _service.GetAllMembers().Returns(members);

        var result = await _controller.GetAllMembers();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.Value.Should().BeEquivalentTo(members);
    }

    [Test]
    public async Task AddMember_ShouldReturnCreatedAtAction_WhenMemberIsAddedSuccessfully()
    {
        _service.AddMember(_johnDoe).Returns(Task.CompletedTask);

        var result = await _controller.AddMember(_johnDoe);

        var createdResult = result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult.ActionName.Should().Be(nameof(MemberController.GetMemberById));
        createdResult.RouteValues!["id"].Should().Be(_johnDoe.Id);
        createdResult.Value.Should().Be(_johnDoe);
    }

    [Test]
    public async Task AddMember_ShouldReturnBadRequest_WhenArgumentExceptionIsThrown()
    {
        // Arrange
        _service.When(s => s.AddMember(_johnDoe))
            .Do(_ => throw new ArgumentException("Invalid data"));

        // Act
        var result = await _controller.AddMember(_johnDoe);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.Value.Should().BeEquivalentTo(new { error = "Invalid data" });
    }

    [Test]
    public async Task EditMember_ShouldReturnOk_WhenEditIsSuccessful()
    {
        _service.EditMember(_johnDoe).Returns(Task.CompletedTask);

        var result = await _controller.EditMember(_johnDoe);

        result.Should().BeOfType<OkResult>();
    }

    [Test]
    public async Task EditMember_ShouldReturnBadRequest_WhenArgumentExceptionIsThrown()
    {
        _service.When(s => s.EditMember(_johnDoe))
            .Do(_ => throw new ArgumentException("Member not found"));

        var result = await _controller.EditMember(_johnDoe);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.Value.Should().BeEquivalentTo(new { error = "Member not found" });
    }

    [Test]
    public async Task DeleteMember_ShouldReturnOk_WhenDeleteIsSuccessful()
    {
        _service.DeleteMember(_johnDoe).Returns(Task.CompletedTask);

        var result = await _controller.DeleteMember(_johnDoe);

        result.Should().BeOfType<OkResult>();
    }

    [Test]
    public async Task DeleteMember_ShouldReturnBadRequest_WhenArgumentExceptionIsThrown()
    {
        _service.When(s => s.DeleteMember(_johnDoe))
            .Do(_ => throw new ArgumentException("Member not found"));

        var result = await _controller.DeleteMember(_johnDoe);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.Value.Should().BeEquivalentTo(new { error = "Member not found" });
    }

    [TearDown]
    public void TearDown()
    {
        _service.ClearSubstitute();
    }
}