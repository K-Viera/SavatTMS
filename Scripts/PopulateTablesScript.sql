-- Use the database
USE TmsDb;
GO

-- Insert sample data into the Users table
INSERT INTO Users (Email, Password, Role) VALUES 
('admin@example.com', 'adminpassword', 'Admin')

-- Insert sample data into the Shipments table
INSERT INTO Shipments (TrackingNumber, Status, Origin, Destination, Weight, ShippingDate, DeliveryDate) VALUES 
('1Z999AA10123456784', 'In Transit', 'New York, NY', 'Los Angeles, CA', 10.5, '2024-06-01 10:00:00', '2024-06-05 15:30:00'),
('1Z999BB10234567895', 'Delivered', 'Chicago, IL', 'Houston, TX', 20.75, '2024-06-02 11:00:00', '2024-06-06 14:00:00'),
('1Z999CC10345678906', 'Pending', 'San Francisco, CA', 'Seattle, WA', 5.0, '2024-06-03 12:00:00', NULL),
('1Z999DD10456789017', 'Shipped', 'Miami, FL', 'Boston, MA', 15.2, '2024-06-04 13:00:00', '2024-06-09 16:00:00'),
('1Z999EE10567890128', 'Delivered', 'Dallas, TX', 'Atlanta, GA', 8.9, '2024-06-05 14:00:00', '2024-06-10 13:30:00');