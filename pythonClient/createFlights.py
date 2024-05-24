import random
import itertools
from datetime import datetime, timedelta
from client import AirportClient

flights = []

ports = ['Warsaw', 'Paris', 'Rome', 'Moscow', 'Berlin', 'London', 'Los Angeles', 'New York', 'Tokyo', 'Beijing', 'Kair', "Madrid", 'Brasilia', 'Seul']

counter = 0
for day in range(10):
    for hour in range(24):
        b = random.randint(0, 11)
        if (b*day*hour) % 3 == 0:
            selected_ports = random.sample(ports, 5)
            comibnation_ports = itertools.combinations(selected_ports, 2)
            departureTime = datetime(2024, 5, 16+day, hour, 5*b, 0)
            arrivalTime = departureTime + timedelta(hours=b + 1)
            now = datetime.now()
            for combination in comibnation_ports:
                capacity = random.randint(70, 150)
                occupiedSeats = capacity - random.randint(0, 20)
                counter += 1
                flight = {#'id': f'{10000+counter}',
                          'flightCode': f'LOT {1000+counter}',
                          'departureAirport': combination[0].upper(),
                          'departureTime': departureTime.isoformat(),
                          'destinationAirport': combination[1].upper(),
                          'arrivalTime': arrivalTime.isoformat(),
                          # 'createDate': now,
                          # 'updateDate': now,
                          'capacity': capacity
                          }
                flights.append(flight)


client = AirportClient("https://localhost:8080/Flight", certificate="certificate.pem")

response = client.service("AddList", "POST", json=flights)

print(f"Liczba lotów = {len(flights)}")
