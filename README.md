# Saucisse_bot

Saucisse_bot is a C# Discord bot based on the Dsharp+ library.

## 1. Database
### 1.1. Creation

> Start by installing dotnet-ef tools : <br>
```batch
dotnet tool install --global dotnet-ef
```

> Then add the Microsoft.EntityFrameworkCore.Design package in the Saucisse_bot.Bots project :
```batch
cd Saucisse_bot.bots
dotnet add package Microsoft.EntityFrameworkCore.Design
```

### 1.2. Migration
> Create a migration commit
```batch
dotnet-ef migrations add [name_of_migration] -p ../Saucisse_bot.DAL.Migrations/Saucisse_bot.DAL.Migrations.csproj --context Saucisse_bot.DAL.RPGContext
```

> Update the database
```batch
dotnet-ef database update -p ../Saucisse_bot.DAL.Migrations/Saucisse_bot.DAL.Migrations.csproj --context Saucisse_bot.DAL.RPGContext
```

## 2. Building the solution
To build the solution simply double click the "build.bat" script.

## 3. Commands
### 3.1. Database

### 3.2. Debug

### 3.3. Profile
#### 3.3.1. User commands

#### 3.3.2. Admin commands

### 3.4. Random

## 4. Other features
### 4.1. Message handler

### 4.2. Dictatorship censure
#### 4.2.1. Words and sentences

#### 4.2.2. Images and gifs