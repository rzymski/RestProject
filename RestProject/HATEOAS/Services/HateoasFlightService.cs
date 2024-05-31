using DB.Dto.Flight;
using DB.Dto.HATEOAS;
using Microsoft.AspNetCore.Mvc;
using RestProject.Controllers;
using System.Runtime.CompilerServices;

namespace RestProject.HATEOAS.Services
{
    public class HateoasFlightService
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HateoasFlightService(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        private string? GetControllerName()
        {
            return _httpContextAccessor?.HttpContext?.Request?.RouteValues["controller"]?.ToString();
        }

        public List<Link> CreateLinksForFlight(int id, string actionName)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                throw new InvalidOperationException("HTTP context is not available can't add links in HATEOAS service.");
            var controllerName = GetControllerName();
            if (controllerName == null)
                throw new InvalidOperationException("Controller name is not available, can't add links in HATEOAS service.");
            return new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(httpContext, nameof(FlightController.GetOne), controllerName, new { id }), actionName == nameof(FlightController.GetOne) ? "self" : "get_flight", "GET"),
                new Link(_linkGenerator.GetUriByAction(httpContext, nameof(FlightController.Add), controllerName), "add_flight", "POST"),
                new Link(_linkGenerator.GetUriByAction(httpContext, nameof(FlightController.Update), controllerName, new { id }), "update_flight", "PUT"),
                new Link(_linkGenerator.GetUriByAction(httpContext, nameof(FlightController.Delete), controllerName, new { id }), "delete_flight", "DELETE"),
            };
        }

        public LinkCollectionWrapper<FlightDto> CreateLinksForFlights(List<FlightDto> flights, string actionName)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                throw new InvalidOperationException("HTTP context is not available can't add links in HATEOAS service.");
            var controllerName = GetControllerName();
            if (controllerName == null)
                throw new InvalidOperationException("Controller name is not available, can't add links in HATEOAS service.");
            var wrapper = new LinkCollectionWrapper<FlightDto>(flights);
            wrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext, nameof(FlightController.GetList), controllerName), actionName == nameof(FlightController.GetList) ? "self" : "get_flights", "GET"));
            wrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext, nameof(FlightController.GetByValues), controllerName, new { departureAirport = "city", destinationAirport = "city", departureTime = DateTime.Now, arrivalTime = DateTime.Now, capacity = 0 }), actionName == nameof(FlightController.GetByValues) ? "self" : "get_flights_by_values", "GET"));
            wrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext, nameof(FlightController.AddList), controllerName), "add_flights", "POST"));
            return wrapper;
        }
    }
}
