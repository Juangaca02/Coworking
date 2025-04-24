using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TFG.Models;

namespace TFG.Services
{
    public class SalaServicio
    {
        private readonly SQLiteAsyncConnection _database;
        private static SalaServicio instance;

        private SalaServicio(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Salas>().Wait();
            _database.CreateTableAsync<Reservas>().Wait();
        }

        public static SalaServicio GetInstancia()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "salas.db");
            return instance ??= new SalaServicio(dbPath);
        }

        public async Task<List<Salas>> ObtenerTodasLasSalas()
        {
            return await _database.Table<Salas>().ToListAsync();
        }

        public async Task EliminarSala(Salas sala)
        {
            await _database.DeleteAsync(sala);
        }

        public async Task InsertarSala(Salas sala)
        {
            await _database.InsertAsync(sala);
        }
        public async Task InsertarReserva(Reservas reserva)
        {
            await _database.InsertAsync(reserva);
        }

        public async Task<List<Reservas>> ObtenerReservasPorSala(int salaId)
        {
            return await _database.Table<Reservas>().Where(r => r.SalaId == salaId).ToListAsync();
        }

        public async Task InicializarDatos()
        {
            if ((await ObtenerTodasLasSalas()).Count == 0)
            {
                var listaInicial = new List<Salas>
                {
                    new() { Nombre = "Sala 1", Capacidad = 10, Equipamiento = "Proyector", Estado = "Disponible", Imagen = "dotnet_bot.png" },
                    new() { Nombre = "Sala 2", Capacidad = 20, Equipamiento = "Pantalla", Estado = "Disponible", Imagen = "dotnet_bot.png" },
                    new() { Nombre = "Sala 3", Capacidad = 15, Equipamiento = "Televisor", Estado = "Ocupada", Imagen = "dotnet_bot.png" },
                    new() { Nombre = "Sala 4", Capacidad = 12, Equipamiento = "Proyector", Estado = "Disponible", Imagen = "dotnet_bot.png" },
                    new() { Nombre = "Sala 5", Capacidad = 18, Equipamiento = "Pantalla", Estado = "Disponible", Imagen = "dotnet_bot.png" }
                };

                foreach (var sala in listaInicial)
                {
                    await InsertarSala(sala);
                }
            }
        }
    }
}
