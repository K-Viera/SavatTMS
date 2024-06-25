CREATE DATABASE TmsDb;
GO

-- Use the database
USE TmsDb;
GO
-- Create the Shipments table
CREATE TABLE Shipments (
    ShipmentId INT PRIMARY KEY IDENTITY,
    TrackingNumber NVARCHAR(50) NOT NULL UNIQUE,
    Status NVARCHAR(50) NOT NULL,
    Origin NVARCHAR(100) NOT NULL,
    Destination NVARCHAR(100) NOT NULL,
    Weight DECIMAL(10, 2) NOT NULL,
    ShippingDate DATETIME NOT NULL,
    DeliveryDate DATETIME
);

-- Create the Users table
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    Role NVARCHAR(50) NOT NULL
);