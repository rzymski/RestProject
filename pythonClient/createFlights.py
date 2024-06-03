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
            combinationPorts = itertools.combinations(selected_ports, 2)
            departureTime = datetime.now() + timedelta(days=day, minutes=5*b)
            arrivalTime = departureTime + timedelta(hours=b + 1)
            now = datetime.now()
            for combination in combinationPorts:
                capacity = random.randint(50, 150)
                counter += 1
                flight = {#'id': f'{10000+counter}',
                          'flightCode': f'LOT {counter}',
                          'departureAirport': combination[0].upper(),
                          'departureTime': departureTime.strftime("%Y-%m-%dT%H:%M:%S"),
                          'destinationAirport': combination[1].upper(),
                          'arrivalTime': arrivalTime.strftime("%Y-%m-%dT%H:%M:%S"),
                          'capacity': capacity
                          }
                flights.append(flight)


client = AirportClient("localhost", 8080, "Flight", certificate=False)

response = client.service("AddList", "POST", json=flights)

print(f"Liczba lotów = {len(flights)}")
