using DB.Dto.FlightReservation;
using DB.Dto.HATEOAS;
using RestProject.Controllers;

namespace RestProject.HATEOAS.Services
{
    public class HateoasFlightReservationService
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HateoasFlightReservationService(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        private string? GetControllerName()
        {
            return _httpContextAccessor?.HttpContext?.Request?.RouteValues["controller"]?.ToString();
        }

        public List<Link> CreateLinksForFlightReservation(int id, string actionName)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                throw new InvalidOperationException("HTTP context is not available can't add links in HATEOAS service.");
            var controllerName = GetControllerName();
            if (controllerName == null)
                throw new InvalidOperationException("Controller name is not available, can't add links in HATEOAS service.");
            return new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(httpContext, nameof(FlightReservationController.GetOne), controllerName, new { id }), actionName == nameof(FlightReservationController.GetOne) ? "self" : "get_flightReservation", "GET"),
                new Link(_linkGenerator.GetUriByAction(httpContext, nameof(FlightReservationController.GetFlightReservationAllData), controllerName, new { id }), actionName == nameof(FlightReservationController.GetFlightReservationAllData) ? "self" : "get_flightReservation_all_data", "GET"),
                // usunalem ten link, bo nie generuje href prawidłowo dla asynchronicznych metod
                //new Link(_linkGenerator.GetUriByAction(httpContext, nameof(FlightReservationController.GetAsynchronouslyFlightReservationAllDataAsync), controllerName, new { id }), actionName == nameof(FlightReservationController.GetAsynchronouslyFlightReservationAllDataAsync) ? "self" : "get_flightReservation_all_data_asynchronously", "GET"),
                new Link(_linkGenerator.GetUriByAction(httpContext, nameof(FlightReservationController.Add), controllerName), "add_flightReservation", "POST"),
                new Link(_linkGenerator.GetUriByAction(httpContext, nameof(FlightReservationController.Update), controllerName, new { id }), "update_flightReservation", "PUT"),
                new Link(_linkGenerator.GetUriByAction(httpContext, nameof(FlightReservationController.Delete), controllerName, new { id }), "delete_flightReservation", "DELETE"),
                new Link(_linkGenerator.GetUriByAction(httpContext, nameof(FlightReservationController.ChangeNumberOfReservedSeats), controllerName, new { id }), "edit_flightReservation", "PATCH"),
            };
        }

        public LinkCollectionWrapper<FlightReservationDto> CreateLinksForFlightReservations(List<FlightReservationDto> flightReservations, string actionName)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                throw new InvalidOperationException("HTTP context is not available can't add links in HATEOAS service.");
            var controllerName = GetControllerName();
            if (controllerName == null)
                throw new InvalidOperationException("Controller name is not available, can't add links in HATEOAS service.");
            var wrapper = new LinkCollectionWrapper<FlightReservationDto>(flightReservations);
            wrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext, nameof(FlightReservationController.GetList), controllerName), actionName == nameof(FlightReservationController.GetList) ? "self" : "get_flightReservations", "GET"));
            wrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext, nameof(FlightReservationController.GetByValues), controllerName, new { flightId = 0, userId = 0, numberOfReservedSeats = 0 }), actionName == nameof(FlightReservationController.GetByValues) ? "self" : "get_flightReservations_by_values", "GET"));
            wrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext, nameof(FlightReservationController.AddList), controllerName), "add_flightReservations", "POST"));
            return wrapper;
        }
    }
}
