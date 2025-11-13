# NoSQL Project - Ticket Management Systeem

Een ASP.NET Core MVC applicatie voor het beheren van incident- en servicetickets met MongoDB als database.

## 📋 Overzicht

Dit project is een ticket management systeem waarbij gebruikers tickets kunnen aanmaken, bekijken en beheren. Het systeem ondersteunt twee rollen:
- **RegularEmployee**: Kan eigen tickets aanmaken en bekijken
- **ServiceDeskEmployee**: Kan alle tickets beheren en gebruikers beheren

## 🛠️ Vereisten

- .NET 8.0 SDK
- MongoDB (lokaal of via MongoDB Atlas)
- Visual Studio 2022 of Visual Studio Code

## ⚙️ Installatie

1. **Clone het project**
   ```bash
   git clone <repository-url>
   cd NoSQL_projectFinal
   ```

2. **MongoDB instellen**
   - Installeer MongoDB lokaal, of
   - Maak een account aan bij [MongoDB Atlas](https://www.mongodb.com/cloud/atlas)

3. **Environment variabelen configureren**
   - Maak een `.env` bestand in de root van het project
   - Voeg de volgende regel toe:
     ```
     Mongo__ConnectionString=your_mongodb_connection_string
     ```
   - Voorbeeld lokaal: `Mongo__ConnectionString=mongodb://localhost:27017`
   - Voorbeeld Atlas: `Mongo__ConnectionString=mongodb+srv://username:password@cluster.mongodb.net/`

4. **Dependencies installeren**
   ```bash
   cd NoSQL_project/NoSQL_project
   dotnet restore
   ```

## 🚀 Project starten

1. **Start MongoDB** (als je lokaal werkt)
   ```bash
   mongod
   ```

2. **Run het project**
   ```bash
   dotnet run
   ```

3. **Open de applicatie**
   - Navigeer naar: `https://localhost:5001` of `http://localhost:5000`

## 👤 Standaard gebruikers

Bij de eerste start worden automatisch de volgende gebruikers aangemaakt:

| Username | Password | Rol |
|----------|----------|-----|
| admin | admin123 | ServiceDeskEmployee |
| johndoe | password123 | RegularEmployee |
| janesmith | password123 | RegularEmployee |

## 📁 Project Structuur

```
NoSQL_project/
├── Controllers/          # MVC Controllers
│   ├── DashboardController.cs
│   ├── TicketController.cs
│   └── UserController.cs
├── Models/               # Data modellen
│   ├── User.cs
│   ├── Ticket.cs
│   └── ViewModels/      # ViewModels voor views
├── Repositories/         # Data access layer
│   ├── Interfaces/
│   ├── UserRepository.cs
│   └── TicketRepository.cs
├── Services/            # Business logic layer
│   ├── Interfaces/
│   ├── UserService.cs
│   ├── TicketService.cs
│   └── TicketSearchService.cs
├── Enum/                # Enumeraties
│   ├── UserRoles.cs
│   ├── TicketStatus.cs
│   ├── TicketPrioritys.cs
│   └── TicketIncidentType.cs
└── Views/               # Razor views
```

## 🎯 Functionaliteiten

### Voor alle gebruikers:
- ✅ Account aanmaken en inloggen
- ✅ Tickets aanmaken, bekijken en bewerken
- ✅ Tickets zoeken met AND/OR operatoren
- ✅ Dashboard met ticket statistieken

### Voor ServiceDeskEmployee:
- ✅ Alle tickets bekijken en beheren
- ✅ Gebruikers beheren (CRUD)
- ✅ Tickets escaleren, sluiten en oplossen
- ✅ Tickets verwijderen

## 🔍 Zoekfunctionaliteit

Het systeem ondersteunt geavanceerd zoeken in tickets:
- **Enkele woorden**: `probleem`
- **AND operator**: `probleem AND server`
- **OR operator**: `bug OR issue`
- **Gecombineerd**: `probleem AND server OR database`

Zoeken gebeurt in:
- Incident Subject
- Description
- Incident Type

## 🏗️ Architectuur

Het project volgt een **MVC + Service** architectuur:

- **Controllers**: Handelen HTTP requests af, minimale logica
- **Services**: Bevatten business logic
- **Repositories**: Data access naar MongoDB
- **Models**: Data modellen en ViewModels

## 📦 Belangrijke Packages

- `MongoDB.Driver` - MongoDB database driver
- `BCrypt.Net-Next` - Password hashing
- `DotNetEnv` - Environment variabelen beheer

## 🔐 Beveiliging

- Cookie-based authenticatie
- Password hashing met BCrypt
- Role-based autorisatie (RBAC)
- CSRF protection

## 📝 Notities

- De applicatie seed automatisch standaard gebruikers bij de eerste start
- Tickets worden opgeslagen in MongoDB collectie `Tickets`
- Gebruikers worden opgeslagen in MongoDB collectie `Users`
- Database naam: `NoSQL_Project` (configureerbaar in `appsettings.json`)

## 🐛 Troubleshooting

**MongoDB connection error:**
- Controleer of MongoDB draait (lokaal) of je connection string correct is
- Controleer of de `.env` file bestaat en correct geconfigureerd is

**Build errors:**
- Run `dotnet clean` en daarna `dotnet restore`
- Controleer of .NET 8.0 SDK geïnstalleerd is

## 👥 Groep

[Voeg hier jullie namen toe]

---

**Let op**: Zorg ervoor dat de `.env` file niet gecommit wordt naar Git (staat in `.gitignore`).
