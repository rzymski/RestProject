using DB.Dto.Flight;
using DB.Dto.FlightReservation;
using DB.Dto.HATEOAS;
using RestProject.Controllers;

namespace RestProject.HATEOAS.Services
{
    public class HateoasAirportService
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HateoasAirportService(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        private string? GetControllerName()
        {
            return _httpContextAccessor?.HttpContext?.Request?.RouteValues["controller"]?.ToString();
        }

        public List<Link> CreateLinksForFlight(int flightId, string actionName)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                throw new InvalidOperationException("HTTP context is not available can't add links in HATEOAS service.");
            var controllerName = GetControllerName();
            if (controllerName == null)
                throw new InvalidOperationException("Controller name is not available, can't add links in HATEOAS service.");
            return new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(httpContext, nameof(AirportController.GetFlightById), controllerName, new { flightId }), actionName == nameof(AirportController.GetFlightById) ? "self" : "get_flight", "GET"),
                new Link(_linkGenerator.GetUriByAction(httpContext, nameof(AirportController.GetAvailableAirports), controllerName, new { }), actionName == nameof(AirportController.GetAvailableAirports) ? "self" : "get_airports", "GET"),
                new Link(_linkGenerator.GetUriByAction(httpContext, nameof(AirportController.GetFlightAvailableSeats), controllerName, new { flightId }), actionName == nameof(AirportController.GetFlightAvailableSeats) ? "self" : "get_flight_available_seats", "GET"),
                new Link(_linkGenerator.GetUriByAction(httpContext, nameof(AirportController.ReserveFlight), controllerName, new { flightId }), actionName == nameof(AirportController.ReserveFlight) ? "self" : "reserve_fligh", "POST"),
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
            wrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext, nameof(AirportController.GetFlightsData), controllerName), actionName == nameof(AirportController.GetFlightsData) ? "self" : "get_all_flights", "GET"));
            wrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext, nameof(AirportController.GetAllQualifyingFlights), controllerName, new { departureAirport = "city", destinationAirport = "city", departureStartDateRange = DateTime.Now.ToString(), departureEndDateRange = DateTime.Now.ToString() }), actionName == nameof(AirportController.GetAllQualifyingFlights) ? "self" : "get_flights_with_parameters", "GET"));
            return wrapper;
        }

        public List<Link> CreateLinksForFlightReservation(int flightId, int reservationId, string actionName)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                throw new InvalidOperationException("HTTP context is not available, can't add links in HATEOAS service.");
            var controllerName = GetControllerName();
            if (controllerName == null)
                throw new InvalidOperationException("Controller name is not available, can't add links in HATEOAS service.");
            return new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(httpContext, nameof(AirportController.CheckFlightReservation), controllerName, new { flightReservationId = reservationId }), actionName == nameof(AirportController.CheckFlightReservation) ? "self" : "get_flightReservation", "GET"),
                new Link(_linkGenerator.GetUriByAction(httpContext, nameof(AirportController.ReserveFlight), controllerName, new { flightId = flightId }), "reserve_flight", "POST"),
                new Link(_linkGenerator.GetUriByAction(httpContext, nameof(AirportController.CancelFlightReservation), controllerName, new { flightReservationId = reservationId }), "cancel_flight_reservation", "DELETE"),
                new Link(_linkGenerator.GetUriByAction(httpContext, nameof(AirportController.CancelUserReservationInConcreteFlight), controllerName, new { flightId = flightId }), "cancel_user_reservation_in_concrete_flight", "DELETE"),
                new Link(_linkGenerator.GetUriByAction(httpContext, nameof(AirportController.GeneratePDF), controllerName, new { flightReservationId = reservationId }), actionName == nameof(AirportController.GeneratePDF) ? "self" : "generate_pdf", "GET")
            };
        }

        public LinkCollectionWrapper<FlightReservationAllFieldsDto> CreateLinksForFlightReservations(List<FlightReservationAllFieldsDto> flightReservations, string actionName)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                throw new InvalidOperationException("HTTP context is not available, can't add links in HATEOAS service.");
            var controllerName = GetControllerName();
            if (controllerName == null)
                throw new InvalidOperationException("Controller name is not available, can't add links in HATEOAS service.");
            string login = flightReservations.Count != 0 ? flightReservations.First().Login : "username";
            var wrapper = new LinkCollectionWrapper<FlightReservationAllFieldsDto>(flightReservations);
            wrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext, nameof(AirportController.GetUserReservations), controllerName, new { username = login }), actionName == nameof(AirportController.GetUserReservations) ? "self" : "get_user_reservations", "GET"));
            wrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext, nameof(AirportController.GetFlightsData), controllerName), actionName == nameof(AirportController.GetFlightsData) ? "self" : "get_all_flights", "GET"));
            wrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext, nameof(AirportController.CreateUser), controllerName), "create_user", "POST"));
            return wrapper;
        }
    }
}
