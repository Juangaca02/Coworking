using SQLite;
using TFG.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace TFG.Services

{
    public class DatabaseService
    {
        private static SQLiteAsyncConnection _database;

        public static async Task InitializeAsync()
        {
            if (_database != null)
                return;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "TFG.db");
            _database = new SQLiteAsyncConnection(dbPath);
            await _database.CreateTableAsync<Salas>();

            // Datos de ejemplo solo si la tabla está vacía
            var count = await _database.Table<Salas>().CountAsync();
            if (count == 0)
            {
                await _database.InsertAllAsync(new List<Salas>
                {
                    new Salas { Nombre = "Sala 1", Capacidad = 10, Equipamiento = new List<string> { "Proyector", "Pizarra" }, Estado = "Disponible", Imagen = "dotnet_bot.png" },
                    new Salas { Nombre = "Sala 2", Capacidad = 20, Equipamiento = new List<string> { "Proyector", "Pizarra" }, Estado = "Disponible", Imagen = "dotnet_bot.png" },
                    new Salas { Nombre = "Sala 3", Capacidad = 15, Equipamiento = new List < string > { "Proyector", "Pizarra" }, Estado = "Ocupada", Imagen = "dotnet_bot.png" },
                    new Salas { Nombre = "Sala 4", Capacidad = 12, Equipamiento = new List < string > { "Proyector", "Pizarra" }, Estado = "Disponible", Imagen = "dotnet_bot.png" },
                    new Salas { Nombre = "Sala 5", Capacidad = 18, Equipamiento = new List < string > { "Proyector", "Pizarra" }, Estado = "Disponible", Imagen = "dotnet_bot.png" }
                });
            }
        }

        public static Task<List<Salas>> GetSalasAsync()
        {
            return _database.Table<Salas>().ToListAsync();
        }
    }
}