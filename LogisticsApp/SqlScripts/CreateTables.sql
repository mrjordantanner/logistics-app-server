-- Locations Table
 DROP TABLE IF EXISTS Locations;
 CREATE TABLE Locations (
     Id INT PRIMARY KEY IDENTITY(1,1),
     Name NVARCHAR(255),     
     City NVARCHAR(100) NOT NULL,     
     State NVARCHAR(50) NOT NULL,     
     PostalCode NVARCHAR(20) NOT NULL,
     Country NVARCHAR(100) NOT NULL,
  	 Latitude DECIMAL(9, 6), 
  	 Longitude DECIMAL(9, 6)
 );

-- Users Table
DROP TABLE IF EXISTS Users;
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Role NVARCHAR(10) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(15) NOT NULL, 
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME,

    -- Driver-specific columns (nullable for Admins)
    CurrentPostalCode INT NULL,
    Status NVARCHAR(10) DEFAULT 'Available'
);

-- Items Table
 DROP TABLE IF EXISTS Items;
 CREATE TABLE Items (
     Id INT PRIMARY KEY IDENTITY(1,1),
     Name NVARCHAR(100) NOT NULL,
     Description NVARCHAR(255),
 	 Weight INT NOT NULL,
     Value INT NOT NULL
 );

 -- Orders Table
 DROP TABLE IF EXISTS Orders;
CREATE TABLE Orders (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Origin INT,
    Destination INT,
    DeliveryId INT,
    OrderDate DATETIME DEFAULT GETDATE(),
    OrderStatus NVARCHAR(50) CHECK (OrderStatus IN ('Pending', 'Shipped', 'Delivered', 'Cancelled')),
    FOREIGN KEY (Origin) REFERENCES Locations(Id),
    FOREIGN KEY (Destination) REFERENCES Locations(Id),
    FOREIGN KEY (DeliveryId) REFERENCES Deliveries(Id)
);

 -- Deliveries Table
 DROP TABLE IF EXISTS Deliveries;
CREATE TABLE Deliveries (
    Id INT PRIMARY KEY IDENTITY(1,1),
    DriverId INT FOREIGN KEY REFERENCES Users(Id),  
    OrderId INT FOREIGN KEY REFERENCES Orders(Id), 
    Origin INT FOREIGN KEY REFERENCES Locations(Id),
    Destination INT FOREIGN KEY REFERENCES Locations(Id),
    TargetDeliveryDate DATETIME,
    ActualDeliveryDate DATETIME,
    Status NVARCHAR(50) CHECK (Status IN ('Scheduled', 'InTransit', 'Delivered', 'Delayed', 'Failed')),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME,
);
CREATE INDEX idx_deliveries_driverid ON Deliveries (DriverId);



-- OrderItems Table with Index on OrderId
 DROP TABLE IF EXISTS OrderItems;
 CREATE TABLE OrderItems (
     Id INT PRIMARY KEY IDENTITY(1,1),
     OrderId INT NOT NULL,
     ItemId INT NOT NULL,
     Quantity INT NOT NULL,
     FOREIGN KEY (OrderId) REFERENCES Orders(Id),
     FOREIGN KEY (ItemId) REFERENCES Items(Id)
 );
 CREATE INDEX idx_orderitems_orderid ON OrderItems (OrderId);



