CREATE DATABASE GymProDB;
USE GymProDB;

CREATE TABLE Usuarios(
	UserId INT PRIMARY KEY NOT NULL IDENTITY (1,1),
	Username VARCHAR(35) UNIQUE NOT NULL,
	Clave VARCHAR(35) NOT NULL, 
	Rol varchar(10) NOT NULL,
	FechaCreacion DATETIME NOT NULL
);

CREATE TABLE Productos(
	ProductoId INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	Nombre VARCHAR(30) NOT NULL,
	Categoria VARCHAR(30) NOT NULL,
	Precio FLOAT NOT NULL
);
Insert into Productos(Nombre, Categoria, Precio) VALUES
('Proteina Whey','Suplemento',5000),
('Creatina Optimun Nutrition','Suplemento',3000),
('Guantes','Accesorio',800),
('Suerter under armor','',1200),


CREATE TABLE Suscripciones(
	SuscripcionId INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	Nombre VARCHAR(25) NOT NULL,
	Descripcion VARCHAR(100) NOT NULL,
	Precio FLOAT NOT NULL,
	ClientesSuscritos INT NOT NULL
);
INSERT INTO Suscripciones (Nombre,Descripcion,Precio,ClientesSuscritos) VALUES('Suscripcion Basica','Membresia del gym',1000 , 0) ;
INSERT INTO Suscripciones (Nombre,Descripcion,Precio,ClientesSuscritos) VALUES('Suscripcion Avanzada','Membresia del gym + Entrenador',1500 , 0) ;
INSERT INTO Suscripciones (Nombre,Descripcion,Precio,ClientesSuscritos) VALUES('Suscripcion Premium','Membresia del gym + Entrenador + Plan de dieta + Rutina',2000 , 0) ;



CREATE TABLE Entrenadores(
	EntrenadorId INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	UserId INT NOT NULL,
	Rango VARCHAR(20) NOT NULL, 
	ClientesInscritos INT NOT NULL
);

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

CREATE TABLE Equipamientos(
	EquipoId INT PRIMARY KEY NOT NULL IDENTITY(1,1),
	Nombre VARCHAR(30) NOT NULL,
	Descripcion VARCHAR(150) NOT NULL
);


INSERT INTO Equipamientos(
Nombre, Descripcion
) VALUES 
('Mancuerna 5 lbs','Excelente para principantes'),
('Mancuerna 15 lbs','Excelente para principantes'),
('Mancuerna 25 lbs','Excelente para principantes'),
('Mancuerna 35 lbs','Excelente para Intermedios'),
('Mancuerna 45 lbs','Excelente para Intermedios'),
('Mancuerna 55 lbs','Excelente para Intermedios'),
('Mancuerna 65 lbs','Excelente para avanzados'),
('Mancuerna 75 lbs','Excelente para avanzados'),
('Suiza','Excelente para trabajar cardio / Cuerpo completo')

