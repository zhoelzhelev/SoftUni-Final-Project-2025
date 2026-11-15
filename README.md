# Schedulefy – Event Management Platform

## About the Project

Schedulefy is an ASP.NET Core MVC web application designed to make event management easier. The platform allows users to discover events, publish their own events, and participate in activities organized by other users.

The project was developed as a personal project to improve my knowledge of C#, ASP.NET Core MVC, Entity Framework Core, database management, and web application development.

## Features

- **User Authentication** – Registration, login, and account management using ASP.NET Core Identity.
- **Event Management** – Users can create, publish, view, and manage events.
- **Event Participation** – Users can mark themselves as attending events organized by other users.
- **Ticket Management** – Support for purchasing tickets for events.
- **Premium Events** – Support for premium events.
- **Categories** – Events can be organized into different categories.
- **Comments** – Users can interact through comments.
- **Role-Based Authorization** – Different permissions for administrators and regular users.

## Technologies Used

- C#
- ASP.NET Core MVC
- Entity Framework Core
- Microsoft SQL Server
- ASP.NET Core Identity
- HTML
- CSS
- JavaScript
- Bootstrap

## Project Setup and Installation

**Important – Please read before running the project.**

I would like to apologize for the initial project structure and the way I approached organizing the solution. The project was not structured as cleanly as it should have been, which means a few manual adjustments are necessary before it can run correctly.

Thank you for your understanding.

Please follow these steps:

### 1. Clone the Repository

Download the entire repository to your local machine:

```bash
git clone YOUR_REPOSITORY_URL
```

Alternatively, you can use GitHub Desktop or Visual Studio to clone the repository.

### 2. Correct the Project Structure

After downloading the repository, locate the `Schedulefy.slnx` solution file.

The `Controllers`, `Views`, and `wwwroot` folders need to be moved into the main Schedulefy project directory, alongside the `Schedulefy.csproj` file.

Make sure the folders are not left outside the actual project directory.

### 3. Correct the Namespaces

After moving the folders, some namespaces and references might need to be adjusted.

Please check the namespaces in the Controllers, Models, Services, and other related classes to ensure that they match the project's actual namespace structure.

Also verify that the references between controllers, services, models, and views are correct.

### 4. Configure the Database

Open `appsettings.json` and configure the SQL Server connection string according to your local database setup.

Create the database and apply the Entity Framework Core migrations.

```bash
dotnet ef database update
```

### 5. Run the Application

Open `Schedulefy.slnx` in Visual Studio.

Restore the NuGet packages and build the solution.

You can then run the application using Visual Studio or the .NET CLI:

```bash
dotnet restore
dotnet build
dotnet run
```

## Project Structure

The intended project structure should look similar to this:

```text
Schedulefy/
│
├── Controllers/
│
├── Data/
│   ├── SchedulefyDbContext.cs
│   └── ...
│
├── Models/
│
├── Services/
│
├── Views/
│
├── wwwroot/
│
├── appsettings.json
├── Program.cs
├── Schedulefy.csproj
└── Schedulefy.slnx
```

## Final Note

Schedulefy represents an important part of my learning journey as a developer. Throughout its development, I gained practical experience with ASP.NET Core MVC, database relationships, authentication, authorization, and application architecture.

I acknowledge that the initial organization of the repository could have been handled better, and I sincerely apologize for any inconvenience this may cause when setting up the project.

Thank you for taking the time to explore my work!
