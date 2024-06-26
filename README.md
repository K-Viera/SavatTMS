# SavatTMS Project

## Overview
SavatTMS is a Transportation Management System (TMS) designed to streamline the operations of shipping and logistics. The system allows for efficient management of shipments, including tracking, scheduling, and reporting functionalities.

## Getting Started

### Prerequisites
- .NET 5.0 SDK or later
- Microsoft SQL Server
- Visual Studio 2019 or later (for development)

### Installation

1. Clone the repository to your local machine:
   ```bash
   git clone https://github.com/your-username/SavatTMS.git
   ```

2. Navigate to the project directory:
   ```bash
   cd SavatTMS
   ```

3. Restore the .NET packages:
   ```bash
   dotnet restore
   ```

4. Set up the database by running the SQL scripts located in the `Scripts` directory. First, create the database and tables:
   ```sql
   -- Run the contents of CreateDatabaseAndTables.sql
   ```

5. Populate the database with initial data:
   ```sql
   -- Run the contents of PopulateTablesScript.sql
   ```

6. Update the `appsettings.json` file in the `TMSApi` directory with your database connection string.

### Running the Application

1. Navigate to the `TMSApi` directory:
   ```bash
   cd TMSApi
   ```

2. Run the application:
   ```bash
   dotnet run
   ```

   The API will start, and you can access it through [http://localhost:5000](http://localhost:5000) by default.

## Project Structure

- **TMSApi/**: The main project directory containing the ASP.NET Core Web API application.
  - **Controllers/**: Contains the API controllers.
  - **DTOs/**: Data Transfer Objects used for data exchange between the client and server.
  - **Models/**: Entity models representing the database tables.
  - **Services/**: Business logic services.
- **TMSTests/**: Contains unit tests for the TMSApi project.
  - **ShipmentControllerTests.cs**: Tests for the Shipment controller.
  - **ShipmentServiceTest.cs**: Tests for the Shipment service.
- **Scripts/**: SQL scripts for database creation and initial data population.