using DB.Dto.Flight;
using DB.Dto.HATEOAS;
using DB.Dto.User;
using DB.Services;
using DB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Org.BouncyCastle.Crypto;
using System.Runtime.CompilerServices;

namespace RestProject.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> logger;
        private readonly IUserService userService;
        private LinkGenerator _linkGenerator;

        public UserController(ILogger<UserController> logger, IUserService userService, LinkGenerator linkGenerator)
        {
            this.logger = logger;
            this.userService = userService;
            _linkGenerator = linkGenerator;
        }

        [HttpGet("{id}")]
        public ActionResult GetOne([FromRoute] int id)
        {
            var result = userService.GetByIdDtoObject(id);
            if (result == null)
                return NotFound(new { user = $"User not found with id = {id}." });
            result.Links = CreateLinksForUser(result.Id, result.Login);
            return Ok(result);
        }

        [HttpGet]
        public ActionResult<List<UserDto>> GetList()
        {
            List<UserDto> users = userService.GetAllDtoList();
            foreach (var user in users)
                user.Links = CreateLinksForUser(user.Id, user.Login);
            return Ok(CreateLinksForUsers(users));
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
            {
                user.Links = CreateLinksForUser(user.Id, login);
                return Ok(user);
            }
            return NotFound();
        }


        //HATEOAS
        private List<Link> CreateLinksForUser(int id, string login, [CallerMemberName] string actionName = "")
        {
            return new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetOne), values: new { id = id }), "self", "GET"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetByLogin), values: new { login=login }), "self", "GET"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Add), values: new { }), "add_user", "POST"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Update), values: new { id }), "update_user", "PUT"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Delete), values: new { id }), "delete_user", "DELETE"),
            };
        }
        private LinkCollectionWrapper<UserDto> CreateLinksForUsers(List<UserDto> users)
        {
            LinkCollectionWrapper<UserDto> wrapper = new LinkCollectionWrapper<UserDto>(users);
            wrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetList), values: new { }), "self", "GET"));
            wrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(AddList), values: new { }), "add_users", "POST"));
            return wrapper;
        }
    }
}
