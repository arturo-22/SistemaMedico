# Sistema de Gestión Médica 🏥

Este es un sistema integral para la gestión de pacientes y citas médicas, desarrollado como parte de la evaluación técnica. El proyecto utiliza una arquitectura moderna separando el Backend (API) del Frontend (Cliente).

## 🚀 Tecnologías Utilizadas

* **Backend:** .NET 8 Web API con Dapper.
* **Frontend:** Angular 17+.
* **Base de Datos:** SQL Server (Stored Procedures, tables).
* **Control de Versiones:** Git & GitHub.

## 📁 Estructura del Proyecto

* `/Backend`: API REST construida con C# y .NET 8.
* `/Frontend`: Aplicación SPA desarrollada en Angular.

## 🛠️ Instalación y Ejecución

### Backend
1. Abrir la solución en **Visual Studio 2022**.
2. Configurar la cadena de conexión en `appsettings.json`.
   > Nota importante:** Mi cadena de conexión actual utiliza **Autenticación de Windows**: (`Integrated Security=True`),
   > por lo que no incluye usuario ni contraseña. Si tu instancia de SQL Server requiere credenciales (SQL Server Authentication), por favor añadelas manualmente: `User Id=tu_usuario;Password=tu_clave;`.
3. Ejecutar el proyecto (F5).

### Frontend
1. Abrir una terminal en la carpeta `/Frontend`.
2. Ejecutar `npm install` para instalar las dependencias.
3. Ejecutar `ng serve` para levantar el servidor de desarrollo.
4. Abrir `http://localhost:4200` en el navegador.

## 📝 Características
* Gestión de Pacientes (CRUD).
* Gestión de Citas Médicas (CRUD).
* Campos de auditoría automáticos (FechaCreacion, UsuarioModificacion, etc.).

## ⚠️ Notas de la Entrega
* **Autenticación (Login):** Siguiendo las instrucciones de la evaluación, el módulo de Login se mantuvo como **opcional** y no ha sido implementado en esta versión para priorizar la lógica de negocio en los módulos de Pacientes y Citas.
