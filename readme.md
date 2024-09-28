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

## 10. Para modificar el LOGIN, REGISTRAR, ETC

Agrega lo siguiente en el <nameApp>.csproj, ejemplo boseapp.csproj

<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.8" />
<PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="8.0.5" />

dotnet restore

dotnet tool install --global dotnet.aspnet.codegenerator --version 8.0.5

dotnet aspnet-codegenerator identity -dc <AplicationName>.Data.ApplicationDbContext --files "Account.Register;Account.Login"
Ejemplo:
dotnet aspnet-codegenerator identity -dc boseapp.Data.ApplicationDbContext --files "Account.Register;Account.Login"

Otras opciones:
Account.ForgotPassword: Página para solicitar un correo de recuperación de contraseña.
Account.ResetPassword: Página para restablecer la contraseña usando el enlace enviado al correo.
Account.ConfirmEmail: Página para confirmar la dirección de correo electrónico después del registro.
Account.AccessDenied: Página que muestra cuando un usuario intenta acceder a un recurso restringido.
Account.ExternalLogin: Página para manejar logins externos, como Google, Facebook, etc.
Account.Manage: Página para que los usuarios administren su cuenta (cambiar contraseña, etc.).
Account.Logout: Acción para cerrar sesión.
Account.Lockout: Página que muestra cuando una cuenta ha sido bloqueada.

### INICIAR TU PROYECTO

dotnet watch run
