import requests
from icecream import ic
from datetime import datetime, timedelta


class AirportClient:
    def __init__(self, baseUrl, certificate=False):
        self.baseUrl = baseUrl
        self.certificate = certificate

    @staticmethod
    def printRequest(request):
        ic(request.url)
        ic(request.body)
        ic(request.headers)

    @staticmethod
    def printService(func):
        def wrapper(self, *args, **kwargs):
            serviceResponse = func(self, *args, **kwargs)
            ic(args, kwargs, serviceResponse)
            return serviceResponse
        return wrapper

    @printService
    def service(self, serviceName, serviceMethod, pathParameter="", data=None, json=None, parameters=None, headers=None, matrixParameters=[], expectedResponseFormat="json"):
        serviceUrl = f"{self.baseUrl}/{serviceName}/" + pathParameter + ''.join([f";{matrixParam}" for matrixParam in matrixParameters])
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
            # AirportClient.printRequest(requestResponse.request)  # Wyswietlanie danych wyslanego requesta
            requestResponse.raise_for_status()
            if expectedResponseFormat.lower() == "json":
                return requestResponse.json() if requestResponse.text else None
            elif expectedResponseFormat.lower() == "xml" or expectedResponseFormat.lower() == "text":
                return requestResponse.text
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


if __name__ == "__main__":
    client = AirportClient("https://localhost:8080/Flight", certificate="certificate.pem")
    # Get method
    # response = client.service("GetList", "GET")
    response = client.service("GetOne", "GET", pathParameter="1824")
    # response = client.service("GetByValues", "GET", parameters={"author": "Adam", "content": "Secret", "priority": 1})
    # response = client.service("GetHeaderValues", "GET", headers={"username": "admin", "password": "pass"}, expectedResponseFormat="text")
    # response = client.service("GetMatrixParams", "GET", pathParameter="exampleParams", matrixParameters=["arg0=1", "arg1=Kot", "arg2=Pies", "arg3=1950"])


    # Add method
    # message = {
    #     "author": "Peter",
    #     "content": "PIZZA TIME",
    #     "priority": 5,
    #     "created": datetime.now().isoformat()
    # }
    # response = client.service("Add", "POST", json=message)

    # Update method
    # editedMessage = {
    #     "author": "Darius",
    #     "content": "PIZZA IS THE BEST",
    #     "priority": 3,
    #     "created": (datetime.now() + timedelta(days=2, hours=3)).isoformat()
    # }
    # response = client.service("Update", "PUT", pathParameter="4003", json=editedMessage)

    # Delete method
    # response = client.service("Delete", "DELETE", pathParameter="4002")

    # XML response
    # client = RestClient("https://localhost:8080/rest/api/Hello")
    # response = client.service("messages", "get", pathParameter="xml", expectedResponseFormat="xml")


# response = requests.get("https://localhost:8080/GetList", verify="D:/programowanie/C#/aplikacjeKonsolowe/rest/RESTfulWebServices/pythonClient/localhost.pem")
# print(response)
