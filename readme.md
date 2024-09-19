### PASOS PARA CREAR SU PROYECTO

## 1. Crear proyecto

dotnet new mvc --auth Individual

## 2. Git init

git init

## 3. Add .gitignore

bin/
obj/

## 4. git config

git config user.name "su nombre"
git config user.email "su correo"

## 5. dotnet EF

dotnet tool install --global dotnet-ef

## 6. Agregar lo siguiente

En appsetting.json
"PostgreSQLConnection": "Host=dpg-crlknnlumphs73e94hog-a.oregon-postgres.render.com; Database=boseappdb; Username=boseappdb_user; Password=DjgHp4zSMiK2gUXjoWnmdetp6UyMc3A9; Port=5432"

En aplicationDBcontext
public DbSet<boseapp.Models.Cliente> DataCliente { get; set; }

## 7. migrations

dotnet ef migrations add <PASO DE MIGRACION> --context <Nombre del aplicativo>.Data.ApplicationDbContext -o "<RUTA DEL DEL APLICATIVO>\Data\Migrations"

dotnet ef database update

Ejemplo:
dotnet ef migrations add InitialMigration --context boseapp.Data.ApplicationDbContext -o "C:\Users\Anthony\Desktop\PROGRAMACION\boseapp\Data\Migrations"
dotnet ef database update

## 8. Instalar Driver Postgress

dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.4
dotnet add package

## 9. Cambiar a la DB de PostgreSQL

En el archivo Program.cs, cambiar el connectionString por la DB de postgreSQL igual al que esta en appsettings.json, tambien modificar el options.UseSQLite por options.UseNpgsql;

### INICIAR TU PROYECTO

dotnet watch run
