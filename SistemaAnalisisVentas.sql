-- Creación de la base de datos Sistema de Análisis de Ventas DBA
-- Se definen filegroups para separar tablas de dimensiones y hechos
CREATE DATABASE SistemaAnalisisVentasDBA
ON PRIMARY (
    NAME = SistemaVentas_Primary,
    FILENAME = 'C:\SQLData\SistemaVentas_Primary.mdf',
    SIZE = 100MB,
    FILEGROWTH = 20MB
),
-- Filegroup para tablas descriptivas (dimensiones)
FILEGROUP FG_Dimensiones (
    NAME = SistemaVentas_Dimensiones,
    FILENAME = 'C:\SQLData\SistemaVentas_Dimensiones.ndf',
    SIZE = 150MB,
    FILEGROWTH = 30MB
),
-- Filegroup para tablas transaccionales (hechos)
FILEGROUP FG_Hechos (
    NAME = SistemaVentas_Hechos,
    FILENAME = 'C:\SQLData\SistemaVentas_Hechos.ndf',
    SIZE = 300MB,
    FILEGROWTH = 50MB
)
LOG ON (
-- Archivo de log de la base de datos
    NAME = SistemaVentas_Log,
    FILENAME = 'C:\SQLData\SistemaVentas_Log.ldf',
    SIZE = 100MB,
    FILEGROWTH = 20MB
);
GO
--Seleccion de la base de datos para trabajar en ella 
USE SistemaAnalisisVentasDBA;
GO

---Creacion de las tablas que consideré necesarias
--Tabla de países
CREATE TABLE Countries (
    CountryID INT IDENTITY PRIMARY KEY,
    CountryName VARCHAR(100) NOT NULL
) ON FG_Dimensiones;

--Tabla ciudades que se relaciona con países
CREATE TABLE Cities (
    CityID INT IDENTITY PRIMARY KEY,
    CityName VARCHAR(100) NOT NULL,
    CountryID INT NOT NULL,
    CONSTRAINT FK_Cities_Countries
        FOREIGN KEY (CountryID) REFERENCES Countries(CountryID)
) ON FG_Dimensiones;

--Tabla de Clientes 
CREATE TABLE Customers (
    CustomerID INT PRIMARY KEY,
    FirstName VARCHAR(100),
    LastName VARCHAR(100),
    Email VARCHAR(150),
    Phone VARCHAR(50),
    CityID INT,
    CONSTRAINT FK_Customers_Cities
        FOREIGN KEY (CityID) REFERENCES Cities(CityID)
) ON FG_Dimensiones;

--Tabla de categorías de productos
CREATE TABLE Categories (
    CategoryID INT IDENTITY PRIMARY KEY,
    CategoryName VARCHAR(100) NOT NULL
) ON FG_Dimensiones;

--- Tabla de productos 
CREATE TABLE Products (
    ProductID INT PRIMARY KEY,
    ProductName VARCHAR(150),
    CategoryID INT,
    Price DECIMAL(10,2),
    Stock INT,
    CONSTRAINT FK_Products_Categories
        FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
) ON FG_Dimensiones;

--- Tabla de órdenes de venta
CREATE TABLE Orders (
    OrderID INT PRIMARY KEY,
    CustomerID INT,
    OrderDate DATE,
    Status VARCHAR(50),
    CONSTRAINT FK_Orders_Customers
        FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID)
) ON FG_Hechos;

--- Tabla de detalle de órdenes
CREATE TABLE Order_Details (
    OrderDetailID INT IDENTITY PRIMARY KEY,
    OrderID INT,
    ProductID INT,
    Quantity INT,
    TotalPrice DECIMAL(10,2),
    CONSTRAINT FK_OrderDetails_Orders
        FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    CONSTRAINT FK_OrderDetails_Products
        FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
) ON FG_Hechos;
