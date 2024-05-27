import requests
from icecream import ic
from datetime import datetime, timedelta
import tkinter as tk
from tkinter import filedialog


class AirportClient:
    def __init__(self, ipAddress, serverPort, controllerName, username=None, password=None, proxyPorts=(), proxyInClientSide=True, certificate=False):
        self.baseUrl = f"https://{ipAddress}:{serverPort}/{controllerName}"
        self.certificate = certificate
        self.username = username
        self.password = password
        self.responseHeaders = {}

    @staticmethod
    def printRequest(request):
        ic(request.url)
        ic(request.body)
        ic(request.headers)

    @staticmethod
    def printService(func):
        def wrapper(self, serviceName, serviceMethod, *args, **kwargs):
            serviceResponse = func(self, serviceName, serviceMethod, *args, **kwargs)
            if "generatePDF" in serviceName:
                ic(serviceName, serviceMethod, args, kwargs)
            else:
                ic(serviceName, serviceMethod, args, kwargs, serviceResponse)
            return serviceResponse
        return wrapper

    def setUser(self, username, password):
        self.username = username
        self.password = password

    @printService
    def service(self, serviceName, serviceMethod, pathParameter="", data=None, json=None, parameters=None, headers={}, matrixParameters=[], expectedResponseFormat="json"):
        serviceUrl = f"{self.baseUrl}/{serviceName}/" + pathParameter + ''.join([f";{matrixParam}" for matrixParam in matrixParameters])
        headers.update({"username": self.username, "password": self.password})
        try:
            requestResponse = requests.request(
                url=serviceUrl,
                method=serviceMethod,
                headers=headers,
                params=parameters,
                data=data,  # Używany do wysyłania surowych danych
                json=json,  # Używany do wysyłania danych jako JSON
                verify=self.certificate
            )
            self.responseHeaders = AirportClient.getHeadersFromResponse(requestResponse, ("userValidation",))
            # AirportClient.printRequest(requestResponse.request)  # Wyswietlanie danych wyslanego requesta
            requestResponse.raise_for_status()
            if expectedResponseFormat.lower() == "json":
                return requestResponse.json() if requestResponse.text else None
            elif expectedResponseFormat.lower() == "xml" or expectedResponseFormat.lower() == "text":
                return requestResponse.text
            elif expectedResponseFormat.lower() == "binary":
                return requestResponse.content
            else:
                raise ValueError(f"Niewspierany format odpowiedzi: {expectedResponseFormat}")
        except ValueError:
            print(f"Odpowiedź nie jest w spodziewanym formacie {expectedResponseFormat}")
            return requestResponse.text
        except requests.exceptions.HTTPError as e:
            print(f"W service wystąpił błąd z statusem {e.response.status_code}\nKomunikat błędu: {e}")
            return None
        except requests.exceptions.RequestException as e:
            print(f"W service wystąpił błąd z komunikatem: {e}")
            return None

    def generatePDF(self, reservationID):
        pdfBytes = self.service("generatePDFAsynchronously", "GET", str(reservationID), expectedResponseFormat="binary")
        root = tk.Tk()
        root.withdraw()
        if pdfBytes:
            filePath = filedialog.asksaveasfilename(defaultextension=".pdf", filetypes=[("Pliki PDF", "*.pdf")], initialfile=f"ticketConfirmation{reservationID}", title="Zapisz pdf-a", initialdir="../pdfs")
            if filePath:
                try:
                    with open(filePath, 'wb') as file:
                        file.write(pdfBytes)
                        print("Save pdf file at " + filePath)
                except IOError as e:
                    print("Błąd podczas zapisywania pliku:", e)
            else:
                print("Zapis pliku został anulowany.")
        root.destroy()

    @staticmethod
    def getHeadersFromResponse(serviceResponse, headerNames):
        headers = {}
        for headerName in headerNames:
            if headerName in serviceResponse.headers:
                headers.update({headerName: serviceResponse.headers[headerName]})
        return headers


if __name__ == "__main__":
    client = AirportClient("localhost", 8080, "Airport", certificate="certificate.pem")
    # client.setUser("adminUser", "pass")

    response = client.service("GetFlightById", "GET", pathParameter="100")
    # response = client.service("GetAllQualifyingFlights", "GET", parameters={"departureAirport": "Tokyo", "destinationAirport": "warsaw", "departureStartDateRange": "2024-05-18T00:00:00", "departureEndDateRange": "2024-05-21T00:00:00"})
    # client.generatePDF(2652)
    # response = client.service("GetAvailableAirports", "GET", expectedResponseFormat="text")
    # response = client.service("CreateUser", "POST", json={"login": "adminUser", "password": "pass", "email": "email@wp.pl"})
    # response = client.service("ReserveFlight", "POST", pathParameter="888", json=8)
    # response = client.service("CheckFlightReservation", "GET", pathParameter="2752")
    # response = client.service("CancelFlightReservation", "DELETE", pathParameter="2752")
    # response = client.service("CancelUserReservationInConcreteFlight", "DELETE", pathParameter="1700")
    # response = client.service("GetUserReservations", "GET", pathParameter="adminUser")
    # response = client.service("GetFlightAvailableSeats", "GET", pathParameter="1700")

    ic(client.responseHeaders)

