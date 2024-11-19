using Common;
using DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MemberController(IMemberService service) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetMemberById(Guid id)
    {
        var member = await service.GetMemberById(id);
        return member is null ? NotFound() : Ok(member);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllMembers()
    {
        var members = await service.GetAllMembers();
        return Ok(members);
    }

    [HttpPost("Add")]
    public async Task<IActionResult> AddMember(Member member)
    {
        try
        {
            await service.AddMember(member);
            return CreatedAtAction(nameof(GetMemberById), new { id = member.Id }, member);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
        catch (Exception exception)
        {
            return StatusCode(500, new { error = exception.Message });
        }
    }

    [HttpPost("Edit")]
    public async Task<IActionResult> EditMember(Member member)
    {
        try
        {
            await service.EditMember(member);
            return Ok();
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
        catch (Exception exception)
        {
            return StatusCode(500, new { error = exception.Message });
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteMember(Member member)
    {
        try
        {
            await service.DeleteMember(member);
            return Ok();
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
        catch (Exception exception)
        {
            return StatusCode(500, new { error = exception.Message });
        }
    }
}