from datetime import datetime
from functools import wraps
from zeep.helpers import serialize_object as serialize
from icecream import ic
import json


class AirportLogic:
    javaDateFormat = "%Y-%m-%dT%H:%M:%S"

    def __init__(self, client):
        self.client = client
        self.username, self.email, self.password = [None] * 3

    def getAllAirports(self):
        return [airport.title() for airport in self.client.service("GetAvailableAirports", "GET", valueFieldName="airports")]

    def validateUser(self, username, password):
        ic("Logika walidacja: ", username, password)
        self.client.setUser(username, password)
        result = self.client.getHeaderValidationValue("Echo", "POST", json="Check if user is correct", expectedResponseFormat="text") == "True"
        if result:
            self.username = username
            self.password = password
        return result

    def createUser(self, username, password, email):
        ic("Logika tworzenie uzytkownika: ", username, password, email)
        result = self.client.service("CreateUser", "POST", json={"login": username, "password": password, "email": email})
        if result:
            self.username = username
            self.password = password
            self.email = email
            self.client.setUser(username, password)
        return result

    def logoutUser(self):
        ic("Logika wylogowano uzytkownika")
        self.username, self.email, self.password = [None] * 3
        self.client.setUser(None, None)

    @staticmethod
    def refactorDate(date):
        dateStr = datetime.strptime(date, AirportLogic.javaDateFormat).strftime("%H:%M %d/%m/%Y")
        refactoredDateStr = dateStr if dateStr[0] != "0" else dateStr[1:]
        return refactoredDateStr

    @staticmethod
    def refactorFlight(flightData):
        departureTime = AirportLogic.refactorDate(flightData['departureTime'])
        arrivalTime = AirportLogic.refactorDate(flightData['arrivalTime'])
        return {"id": flightData['id'], "flightCode": flightData['flightCode'], "departureAirport": flightData['departureAirport'], "departureTime": departureTime, "destinationAirport": flightData['destinationAirport'], "arrivalTime": arrivalTime}

    @staticmethod
    def refactorFlightList(flightsData):
        flights = []
        for flightData in flightsData:
            flight = AirportLogic.refactorFlight(flightData)
            flights.append(flight)
        return flights

    @staticmethod
    def convertEmptyToNoneDecorator(func):
        @wraps(func)
        def wrapper(*args, **kwargs):
            # Zamień wszystkie puste ciągi na None w argumentach pozycyjnych
            newArgs = [None if arg == "" else arg for arg in args]
            # Zamień wszystkie puste ciągi na None w argumentach nazwanych
            newKwargs = {k: (None if v == "" else v) for k, v in kwargs.items()}
            # Wywołaj oryginalną funkcję z nowymi argumentami
            return func(*newArgs, **newKwargs)
        return wrapper

    def getAllFlights(self):
        flightsData = self.client.service("GetFlightsData", "GET")
        return AirportLogic.refactorFlightList(flightsData)

    @convertEmptyToNoneDecorator
    def getFlightsWithParameters(self, departureAirport, destinationAirport, departureTime, arrivalTime):
        flightsData = self.client.service("GetAllQualifyingFlights", "GET", parameters={"departureAirport": departureAirport, "destinationAirport": destinationAirport, "departureStartDateRange": departureTime, "departureEndDateRange": arrivalTime})
        return AirportLogic.refactorFlightList(flightsData)

    @staticmethod
    def refactorReservation(reservationData):
        arrivalTime = AirportLogic.refactorDate(reservationData['arrivalTime'])
        return {"flightId": reservationData['flightId'],
                "reservationId": reservationData['reservationId'],
                "flightCode": reservationData['flightCode'],
                "airports": f"{reservationData['departureAirport']} ==> {reservationData['destinationAirport']}",
                "dates": f"{arrivalTime}", "seats": f"{reservationData['numberOfReservedSeats']}"}  # "dates": f"{arrivalTime}", "seats": f"{reservationData['numberOfReservedSeats']}/{reservationData['capacity']}"}

    @staticmethod
    def refactorReservationList(reservationsData):
        reservationsData = reservationsData if reservationsData else []
        reservations = []
        for reservationData in reservationsData:
            reservation = AirportLogic.refactorReservation(reservationData)
            reservations.append(reservation)
        return reservations

    def getFlightReservations(self):
        reservationsData = self.client.service("GetUserReservations", "GET", pathParameter=self.username)
        return AirportLogic.refactorReservationList(reservationsData)

    def numberOfAvailableSeatsInFlight(self, flightId):
        return self.client.service("GetFlightAvailableSeats", "GET", pathParameter=flightId)  # , valueFieldName="availableSeats"

    def reserveFlight(self, flightId, numberOfReservedSeats):
        return self.client.service("ReserveFlight", "POST", pathParameter=flightId, json=numberOfReservedSeats)

    def generatePDF(self, reservationId):
        self.client.generatePDF(reservationId)

    def checkReservation(self, reservationId):
        reservation = self.client.service("CheckFlightReservation", "GET", pathParameter=reservationId)
        ic(reservation)
        reservation = serialize(reservation)
        reservation['arrivalTime'] = AirportLogic.refactorDate(reservation['arrivalTime'])
        reservation['departureTime'] = AirportLogic.refactorDate(reservation['departureTime'])
        reservation.pop("links")
        return reservation

    def cancelReservation(self, flightReservationId):
        self.client.service("CancelFlightReservation", "DELETE", pathParameter=flightReservationId)
