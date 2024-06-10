from tkinter import Tk
from interface import AirportInterface
from logic import AirportLogic
from client import AirportClient

if __name__ == "__main__":
    try:
        rootInterface = Tk()
        # app = AirportInterface(rootInterface, AirportLogic(AirportClient("localhost", 8080, "Airport", certificate=False)))
        app = AirportInterface(rootInterface, AirportLogic(AirportClient("192.168.95.131", 8080, "Airport", certificate="certificate.pem")))
        rootInterface.mainloop()
    except ValueError as e:
        print(e)
