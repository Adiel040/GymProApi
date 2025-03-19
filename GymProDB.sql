CREATE DATABASE GymProDB;
USE GymProDB;

--DROP TABLE Usuarios;
CREATE TABLE Usuarios(
	UserId INT PRIMARY KEY NOT NULL IDENTITY (1,1),
	Username VARCHAR(35) UNIQUE NOT NULL,
	Clave VARCHAR(35) NOT NULL, 
	Rol varchar(10) NOT NULL,
	FechaCreacion DATETIME NOT NULL
);

--DROP TABLE Productos;
CREATE TABLE Productos(
	ProductoId INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	Nombre VARCHAR(30) NOT NULL,
	Categoria VARCHAR(30) NOT NULL,
	Precio FLOAT NOT NULL
);

--DROP TABLE Suscripciones;
CREATE TABLE Suscripciones(
	SuscripcionId INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	Nombre VARCHAR(25) NOT NULL,
	Descripcion VARCHAR(100) NOT NULL,
	Precio FLOAT NOT NULL,
	ClientesSuscritos INT NOT NULL
);

--DROP TABLE Entrenadores;
CREATE TABLE Entrenadores(
	EntrenadorId INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	UserId INT NOT NULL,
	Rango VARCHAR(20) NOT NULL, 
	ClientesInscritos INT NOT NULL
);

--DROP TABLE Clientes;
CREATE TABLE Clientes(
	ClienteId INT PRIMARY KEY NOT NULL IDENTITY(1,1),
	UserId INT NOT NULL,        --Username
	SuscripcionId INT,          --Suscripcion
	EntrenadorId INT,           --Entrenador

	CorreoElectronico VARCHAR(100) NOT NULL,
	NoIdentificacion VARCHAR(100) NOT NULL,
);

--DROP TABLE Equipamientos;
CREATE TABLE Equipamientos(
	EquipoId INT PRIMARY KEY NOT NULL IDENTITY(1,1),
	Nombre VARCHAR(30) NOT NULL,
	Descripcion VARCHAR(150) NOT NULL
);
