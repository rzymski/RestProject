using DB.Dto.User;
using DB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using RestProject.HATEOAS.Filters;

namespace RestProject.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[ServiceFilter(typeof(HateoasUserFilter))]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> logger;
    private readonly IUserService userService;

    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        this.logger = logger;
        this.userService = userService;
    }

    [HttpGet("{id}")]
    public ActionResult GetOne([FromRoute] int id)
    {
        var result = userService.GetByIdDtoObject(id);
        if (result == null)
            return NotFound(new { user = $"User not found with id = {id}." });
        return Ok(result);
    }

    [HttpGet]
    public ActionResult<List<UserDto>> GetList()
    {
        return Ok(userService.GetAllDtoList());
    }

    [HttpPost]
    public ActionResult Add([FromBody] UserAddEditDto user)
    {
        try
        {
            var id = userService.Add(user);
            return Ok(id);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult AddList([FromBody] List<UserAddEditDto> users)
    {
        try
        {
            var ids = userService.AddList(users);
            return Ok(ids);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public ActionResult Update([FromRoute] int id, [FromBody] UserAddEditDto user)
    {
        try
        {
            var result = userService.Update(id, user);
            if (result)
                return NoContent();
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Delete([FromRoute] int id)
    {
        var result = userService.Delete(id);
        if (result)
            return NoContent();
        return NotFound();
    }

    [HttpGet]
    public ActionResult<string> GetByLogin([FromQuery] string login)
    {
        var user = userService.GetByLogin(login);
        if (user != null)
            return Ok(user);
        return NotFound();
    }
}