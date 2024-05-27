___
**RESTProject**
___

# Konfiguracja
<details>
<summary><h3>Stworzenie pustej bazy danych</h3></summary>
  Tworzymy bazę danych za pomocą <b><code>SQL Server Object Explorer</code></b>.<br>
  View -> SQL Server Object Explorer -> SQL Server -> Database -> Add new Database<br>
  Po stworzeniu bazy danych, odświeżamy ją klikając na nią prawym przyciskiem i Refresh.<br>
  Nastepnie klikamy na nią prawym przyciskiem, wybieramy <b>Properties</b><br>
  z <b>Properties</b> kopiujemy wartość <b>Connection string</b>, które umieszczmy w <b><code>appsettings.json</code></b> w <b>"DefaultConnectionString"</b><br>
</details>
<details>
<summary><h3>Utworzenie tabel z Entities</h3></summary>
  Otwieramy <b><code>Package Manage Console</code></b><br>
  View -> Other Windows -> Package Manage Console<br>
  W <b>Package Manage Console</b> w <b><code>Default project:</code></b>  wybieramy <b><code>DB</code></b><br><br>
  
  Dodanie nowej migracji *opcjonalne*
  ```sh
  add-migration InitialCreateDatabase
  ```
  Wdrożenie migracji i zaktualizowanie bazy danych
  ```sh
  update-database
  ```
</details>

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
