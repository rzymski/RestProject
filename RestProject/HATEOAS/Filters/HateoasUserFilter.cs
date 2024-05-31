using DB.Dto.User;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using RestProject.HATEOAS.Services;

namespace RestProject.HATEOAS.Filters;

public class HateoasUserFilter : IActionFilter
{
    private readonly HateoasUserService _hateoasService;

    public HateoasUserFilter(HateoasUserService hateoasService)
    {
        _hateoasService = hateoasService;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        // No logic needed before the action executes
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is OkObjectResult okObjectResult)
        {
            string actionName = context.ActionDescriptor.RouteValues["action"]?.ToString() ?? "";
            if (okObjectResult.Value is UserDto userDto)
                userDto.Links = _hateoasService.CreateLinksForUser(userDto.Id, userDto.Login, actionName);
            else if (okObjectResult.Value is List<UserDto> users)
            {
                foreach (var user in users)
                    user.Links = _hateoasService.CreateLinksForUser(user.Id, user.Login, actionName);
                okObjectResult.Value = _hateoasService.CreateLinksForUsers(users, actionName);
            }
        }
    }
}
