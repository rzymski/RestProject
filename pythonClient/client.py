import requests
from requests.exceptions import ConnectionError
from urllib3.exceptions import NewConnectionError, MaxRetryError
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
    #     self.checkServerAvailability(ipAddress, serverPort)
    #
    # def checkServerAvailability(self, ipAddress, serverPort):
    #     baseServerUrl = f"https://{ipAddress}:{serverPort}"
    #     try:
    #         checkResponse = requests.get(baseServerUrl, verify=self.certificate)
    #     except(ConnectionRefusedError, ConnectionError, NewConnectionError, MaxRetryError) as e:
    #         raise ValueError(f"Nie udało się połączyć z adresem URL = {baseServerUrl}. Najprawdopodobniej serwer jest wyłączony.")
    #     except Exception as e:
    #         raise ValueError(f"Wystąpił nieoczekiwany błąd: {e}")

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
    def service(self, serviceName, serviceMethod, pathParameter="", data=None, json=None, parameters=None, headers={}, matrixParameters=[], expectedResponseFormat="json", valueFieldName="value"):
        serviceUrl = f"{self.baseUrl}/{serviceName}/" + str(pathParameter) + ''.join([f";{matrixParam}" for matrixParam in matrixParameters])
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
            self.responseHeaders.update(AirportClient.getHeadersFromResponse(requestResponse, ("userValidation",)))
            ic(self.responseHeaders)
            AirportClient.printRequest(requestResponse.request)  # Wyswietlanie danych wyslanego requesta
            requestResponse.raise_for_status()
            if not requestResponse.text:
                return None
            if expectedResponseFormat.lower() == "json":
                responseJson = requestResponse.json()
                if isinstance(responseJson, int):
                    return responseJson
                if valueFieldName in responseJson and 'links' in responseJson:
                    return responseJson[valueFieldName]
                else:
                    return responseJson
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

    def getHeaderValidationValue(self, serviceName, *args, **kwargs):
        serviceResponse = self.service(serviceName, *args, **kwargs)
        ic(self.responseHeaders)
        if "userValidation" in self.responseHeaders:
            return self.responseHeaders["userValidation"]
        return None


if __name__ == "__main__":
    client = AirportClient("localhost", 8080, "Airport", certificate="certificate.pem")
    client.setUser("adminUser", "pass")

    response = client.service("GetFlightById", "GET", pathParameter=100)
    # response = client.service("GetAllQualifyingFlights", "GET", parameters={"departureAirport": "Tokyo", "destinationAirport": "warsaw", "departureStartDateRange": "2024-05-18T00:00:00", "departureEndDateRange": "2024-05-21T00:00:00"})
    # client.generatePDF(2101)
    # response = client.service("GetAvailableAirports", "GET", expectedResponseFormat="text")
    # response = client.service("CreateUser", "POST", json={"login": "xxx", "password": "xd", "email": "xx@wp.pl"})
    # response = client.service("ReserveFlight", "POST", pathParameter="505", json=5)
    # response = client.service("CheckFlightReservation", "GET", pathParameter=2101)
    # response = client.service("CancelFlightReservation", "DELETE", pathParameter=2752)
    # response = client.service("CancelUserReservationInConcreteFlight", "DELETE", pathParameter=1700)
    # response = client.service("GetUserReservations", "GET", pathParameter="adminUser")
    # response = client.service("GetFlightAvailableSeats", "GET", pathParameter=1700)
    # response = client.service("Echo", "POST", json="x", expectedResponseFormat="text")
