CREATE DATABASE GymProDB;
USE GymProDB;

DROP TABLE Usuarios;
CREATE TABLE Usuarios(
	UserId INT PRIMARY KEY NOT NULL IDENTITY (1,1),
	Username VARCHAR(35) UNIQUE NOT NULL,
	Clave VARCHAR(35) NOT NULL, 
	Rol varchar(10) NOT NULL,
	FechaCreacion DATETIME NOT NULL
);

DROP TABLE Productos;
CREATE TABLE Productos(
	ProductoId INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	Nombre VARCHAR(30) NOT NULL,
	Categoria VARCHAR(30) NOT NULL,
	Precio FLOAT NOT NULL
);

DROP TABLE Suscripciones;
CREATE TABLE Suscripciones(
	SuscripcionId INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	Nombre VARCHAR(25) NOT NULL,
	Descripcion VARCHAR(100) NOT NULL,
	Precio FLOAT NOT NULL,
	ClientesSuscritos INT NOT NULL
);
INSERT INTO Suscripciones (Nombre,Descripcion,Precio,ClientesSuscritos) VALUES('Suscripcion basica','Membresia del gym',1000 , 0) ;
INSERT INTO Suscripciones (Nombre,Descripcion,Precio,ClientesSuscritos) VALUES('Suscripcion Avanzada','Membresia del gym + Entrenador',1500 , 0) ;
INSERT INTO Suscripciones (Nombre,Descripcion,Precio,ClientesSuscritos) VALUES('Suscripcion Premium','Membresia del gym + Entrenador + Plan de dieta + Rutina',2000 , 0) ;



DROP TABLE Entrenadores;
CREATE TABLE Entrenadores(
	EntrenadorId INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	UserId INT NOT NULL,
	Rango VARCHAR(20) NOT NULL, 
	ClientesInscritos INT NOT NULL
);

DROP TABLE Clientes;
CREATE TABLE Clientes(
	ClienteId INT PRIMARY KEY NOT NULL IDENTITY(1,1),
	UserId INT NOT NULL,        --Username
	SuscripcionId INT,          --Suscripcion
	EntrenadorId INT,           --Entrenador

	FechaNacimiento DATE NOT NULL,
	NoIdentificacion VARCHAR(100) NOT NULL,

	Peso Float NOT NULL,
	Altura FLOAT NOT NULL,
	Genero VARCHAR(1) NOT NULL
);

DROP TABLE Equipamientos;
CREATE TABLE Equipamientos(
	EquipoId INT PRIMARY KEY NOT NULL IDENTITY(1,1),
	Nombre VARCHAR(30) NOT NULL,
	Descripcion VARCHAR(150) NOT NULL
);
