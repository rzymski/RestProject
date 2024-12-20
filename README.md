___
**Serwer stworzony w ASP.NET CORE Web API .NET 8.0 i klient w python 3.11**
___

## Zawartość REDME
1. :airplane: [Funkcjonalność](#funkcjonalność)
2. :computer: [Działanie serwera](#działanie-serwera)
3. :wrench: [Konfiguracja serwera](#instrukcja-konfiguracji-serwera)
4. :dragon: [Konfiguracja klienta w pythonie](#instrukcja-konfiguracji-klienta)
5. :lock: [Konfiguracja certifikatu SSL dla klienta](#instrukcja-konfiguracji-certifikatu-ssl-dla-klienta-w-pythonie)
6. :satellite: [Modyfikowanie bazy danych](#użycie-skryptu-pythonowego-do-modyfikowania-bazy-danych)
8. :godmode: [Przykład](#przykład)

# Funkcjonalność
**System rezerwacji biletów lotniczych**
1) [x] Baza lotów (Miasto od , Miasto do, dzień, godzina)
2) [x] Wyszukiwanie lotów
3) [x] Kupno biletu
4) [x] Odbiór potwierdzenia kupna w formacie PDF
5) [x] Sprawdzenie rezerwacji na podstawie podanego numeru

# Działanie Serwera
Można sprawdzić działanie serwera w:  
- https://localhost:8080/nameOfTheSelectedService/{optionalPathParameter}

SWAGGER:  
- https://localhost:8080/swagger/index.html

Klient pythonowy znajduje się w folderze pythonClient:

W postmanie importując dane z:  
- https://localhost:8080/swagger/v1/swagger.json

# Instrukcja konfiguracji serwera
<details>
<summary><h3>Stworzenie pustej bazy danych</h3></summary>
  Tworzymy bazę danych za pomocą <b><code>SQL Server Object Explorer</code></b>.<br>
  View -> SQL Server Object Explorer -> SQL Server -> Database -> Add new Database<br><br>
  Po stworzeniu bazy danych, odświeżamy ją klikając na nią prawym przyciskiem i Refresh.<br>
  Nastepnie klikamy na nią prawym przyciskiem, wybieramy <b>Properties</b><br>
  z <b>Properties</b> kopiujemy wartość <b>Connection string</b>, które umieszczmy w <b><code>appsettings.json</code></b> w <b>"DefaultConnectionString"</b><br>
</details>
<details>
<summary><h3>Utworzenie tabel z Entities</h3></summary>
  Otwieramy <b><code>Package Manage Console</code></b><br>
  View -> Other Windows -> Package Manage Console<br>
  W <b>Package Manage Console</b> w <b><code>Default project:</code></b>  wybieramy <b><code>DB</code></b><br><br>
  
  Dodanie nowej migracji *\*opcjonalne\**
  ```sh
  add-migration InitialCreateDatabase
  ```
  Wdrożenie migracji i zaktualizowanie bazy danych **\*wymagane\***
  ```sh
  update-database
  ```
</details>

# Instrukcja konfiguracji klienta
Przechodzimy w konsoli do folderu **`pythonClient`**

Tworzymy wirtualne środowisko
```sh
python -m venv venv
```

Aktywujemy wirtualne środowisko
```sh
.\venv\Scripts\activate
```
Instalujemy wszystkie potrzebne biblioteki z pliku `requirements.txt`
```sh
pip install -r .\requirements.txt
```

# Instrukcja konfiguracji certifikatu SSL dla klienta w pythonie
Wymaga Windows Powershell i OpenSSL
<details>
<summary><h3>Utworzenie certifikatu ssl dla klienta w pythonie</h3></summary>
  Otwieramy <b>Windows PowerShell jako administrator</b><br><br>

  Sprawdzamy klucz certifikatu z visual studio i zapisujemy certifakt do zmiennej:
  ```sh
  # Krok 1: Pobierz thumbprint certyfikatu
  $certInfo = dotnet dev-certs https --check
  $certId = $certInfo | Select-String -Pattern "A valid certificate was found: ([A-F0-9]{40})" | ForEach-Object { $_.Matches[0].Groups[1].Value }
  # Write-Output $certId powienien wyświetlić w konsoli 40-znakowy ciąg liter i cyfr będący thumbprintem certifikatu
  Write-Output $certId
  # Krok 2: Użyj zmiennej $certId, aby znaleźć certyfikat w magazynie certyfikatów
  $cert = Get-ChildItem -Path Cert:\CurrentUser\My | Where-Object {$_.Thumbprint -eq $certId}
  # Opcjonalnie: Wyświetl informacje o znalezionym certyfikacie
  if ($cert) {
      Write-Output "Certyfikat znaleziony:"
      Write-Output $cert
  } else {
      Write-Output "Certyfikat z thumbprintem $certId nie został znaleziony."
  }
  ```
  
  Podajemy folder gdzie ma wyeksportować klucz i dowolne haslo :
  ```sh
  $path = "D:\pathToProject\pythonClient"
  Export-PfxCertificate -Cert $cert -FilePath "$path\localhost.pfx" -Password (ConvertTo-SecureString -String twojeDowolneHaslo -Force -AsPlainText)
  ```
  
  Otwieramy openssl np. w <b>C:\Program Files\Git\usr\bin\openssl.exe</b>
  ```sh
  pkcs12 -in D:\pathToProject\pythonClient\localhost.pfx -out D:\pathToProject\pythonClient\certificate.pem -nodes
  ```
  Po wpisaniu polecenia należy podać wcześniej wybrane hasło w tym przykładzie było to twojeDowolneHaslo
</details>

# Użycie skryptu pythonowego do modyfikowania bazy danych
Skrypt pythonowy znajduje się w pliku <b><code>createFlights.py</code></b> w folderze <b>pythonClient</b>

# Przykład

Przykład w postmanie użycia servisu ReserveFlight:		
<h3><details>
    <summary>ReserveFlight body</summary>           
        <img src="screens/example1.png?raw=true" alt="Postman example ReserveFlight body">
</details></h3>
<h3><details>
    <summary>ReserveFlight authentication</summary>           
        <img src="screens/example2.png?raw=true" alt="Postman example ReserveFlight authentication">
</details></h3>
<h3><details>
    <summary>ReserveFlight response body</summary>           
        <img src="screens/example3.png?raw=true" alt="Postman example ReserveFlight response body">
</details></h3>
<h3><details>
    <summary>ReserveFlight response headers</summary>           
        <img src="screens/example4.png?raw=true" alt="Postman example ReserveFlight response headers">
</details></h3>
