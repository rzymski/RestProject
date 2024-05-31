using DB.Dto.User;
using DB.Dto.HATEOAS;
using RestProject.Controllers;

namespace RestProject.HATEOAS.Services
{
    public class HateoasUserService
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HateoasUserService(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        private string? GetControllerName()
        {
            return _httpContextAccessor?.HttpContext?.Request?.RouteValues["controller"]?.ToString();
        }

        public List<Link> CreateLinksForUser(int id, string login, string actionName)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                throw new InvalidOperationException("HTTP context is not available can't add links in HATEOAS service.");
            var controllerName = GetControllerName();
            if (controllerName == null)
                throw new InvalidOperationException("Controller name is not available, can't add links in HATEOAS service.");
            return new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(httpContext, nameof(UserController.GetOne), controllerName, new { id }), actionName == nameof(UserController.GetOne) ? "self" : "get_user", "GET"),
                new Link(_linkGenerator.GetUriByAction(httpContext, nameof(UserController.GetByLogin), controllerName, new { login }), actionName == nameof(UserController.GetByLogin) ? "self" : "get_user", "GET"),
                new Link(_linkGenerator.GetUriByAction(httpContext, nameof(UserController.Add), controllerName), "add_user", "POST"),
                new Link(_linkGenerator.GetUriByAction(httpContext, nameof(UserController.Update), controllerName, new { id }), "update_user", "PUT"),
                new Link(_linkGenerator.GetUriByAction(httpContext, nameof(UserController.Delete), controllerName, new { id }), "delete_user", "DELETE"),
            };
        }

        public LinkCollectionWrapper<UserDto> CreateLinksForUsers(List<UserDto> users, string actionName)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                throw new InvalidOperationException("HTTP context is not available can't add links in HATEOAS service.");
            var controllerName = GetControllerName();
            if (controllerName == null)
                throw new InvalidOperationException("Controller name is not available, can't add links in HATEOAS service.");
            var wrapper = new LinkCollectionWrapper<UserDto>(users);
            wrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext, nameof(UserController.GetList), controllerName), actionName == nameof(UserController.GetList) ? "self" : "get_users", "GET"));
            wrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext, nameof(UserController.AddList), controllerName), "add_users", "POST"));
            return wrapper;
        }
    }
}
