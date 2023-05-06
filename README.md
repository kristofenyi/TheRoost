Disclosure: I had to copy this repo manually to make it public, due to very bad managment at GreenFox Academy that did not allow me and my collegues to make the original repo public. 

# The Roost <img width="35" alt="the_roost" src="https://user-images.githubusercontent.com/109298912/211288308-85880674-796b-4238-90b3-975bb41e9ec2.png">

## Project summary
An accommodation reservation booking web API developed as part of a Greenfox Academy project module (Dusycion cohort - class Midnight). Application is developed according to the MVC pattern using agile methodology. 

##  Features

- login and registration of users incl. account management
- booking/cancelling/viewing reservations
- adding/managing accommodations and rooms by managers
- searching functionality
- support for three languages (EN, CS, SK)
- admin access

##  Technologies and tools
### Base
- ASP.NET Core 7.0
- Entity Framework Core
- Microsoft SQL Server
- xUnit.net

### Feature dependent
- JSON Web Token/JWT (role-based authorization)
- Swagger (API frontend)
- MailTrap.io (development SMTP server)
- Serilog (logging)
- Global exception handling
- SQLite (in-memory database for testing purposes)
- Resource files (localization)

### APIs used
- Azure Cognitive Services Translator

### Management
- JIRA
- Miro
- Discord

### Authors
- developers:
	- [Krištof Kelecsényi](https://github.com/kristofenyi)
	- [Jiří Volenský](https://github.com/jirivls)
	- [Jakub Jůzl](https://github.com/Jkbjzl)
- project mentor: 
	- Erin (Maggie) McDonald
