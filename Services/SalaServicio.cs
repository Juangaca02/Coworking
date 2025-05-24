using SQLite;
using TFG.Models;

namespace TFG.Services
{
    // Servicio que maneja operaciones relacionadas con las salas y reservas en la base de datos SQLite
    public class SalaServicio
    {
        private readonly SQLiteAsyncConnection _database; // Conexión a la base de datos SQLite
        private static SalaServicio instance; // Instancia única para aplicar el patrón Singleton

        // Constructor privado para asegurar que solo se cree una instancia (Singleton)
        private SalaServicio(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath); // Inicializa la conexión con la base de datos
            _database.CreateTableAsync<Salas>().Wait();     // Crea la tabla Salas si no existe
            _database.CreateTableAsync<Reservas>().Wait();
            _database.CreateTableAsync<Usuarios>().Wait();// Crea la tabla Reservas si no existe
        }

        // Método para obtener la instancia única del servicio
        public static SalaServicio GetInstancia()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "salas1.db"); // Ruta del archivo de base de datos
            return instance ??= new SalaServicio(dbPath); // Retorna la instancia, o la crea si no existe
        }
        /***************************************************************************
        **************************************************************************** 
        ---------------------------------USUARIOS-----------------------------------
        ****************************************************************************
        ***************************************************************************/

        // Autenticación de usuario
        public Task<Usuarios> Autenticar(string nombreUsuario, string contraseña)
        {
            return _database.Table<Usuarios>().FirstOrDefaultAsync(u => u.Nombre == nombreUsuario && u.Contrasena == contraseña);
        }

        // Registro de usuario
        public async Task RegistrarUsuario(Usuarios usuario)
        {
            await _database.InsertAsync(usuario);
        }

        /***************************************************************************
        **************************************************************************** 
        -----------------------------------SALAS------------------------------------
        ****************************************************************************
        ***************************************************************************/

        // Obtiene todas las salas de la base de datos
        public async Task<List<Salas>> ObtenerTodasLasSalas()
        {
            return await _database.Table<Salas>().ToListAsync();
        }

        // Elimina una sala específica de la base de datos
        public async Task EliminarSala(Salas sala)
        {
            await _database.DeleteAsync(sala);
        }

        // Inserta una nueva sala en la base de datos
        public async Task InsertarSala(Salas sala)
        {
            await _database.InsertAsync(sala);
        }

        // Obtiene el nombre de una sala dado su ID
        public async Task<string> ObtenerNombreSalaPorId(int salaId)
        {
            var sala = await _database.Table<Salas>().Where(s => s.Id == salaId).FirstOrDefaultAsync();
            return sala?.Nombre ?? "Sala No Encontrada"; // Si no se encuentra, retorna mensaje por defecto
        }

        // Obtiene todas las reservas asociadas a una sala específica
        public async Task<List<Reservas>> ObtenerReservasPorSala(int salaId)
        {
            return await _database.Table<Reservas>().Where(r => r.SalaId == salaId).ToListAsync();
        }


        /****************************************************************************
        *****************************************************************************
        ---------------------------------RESERVAS------------------------------------
        *****************************************************************************
        ****************************************************************************/

        // Inserta una nueva reserva en la base de datos
        public async Task InsertarReserva(Reservas reserva)
        {
            await _database.InsertAsync(reserva);
        }

        // Obtiene las reservas realizadas entre dos fechas específicas
        public async Task<List<Reservas>> ObtenerReservasEntreFechas(DateTime desde, DateTime hasta)
        {
            return await _database.Table<Reservas>()
                .Where(r => r.Fecha >= desde && r.Fecha <= hasta).ToListAsync();
        }

        // Alternativa al método anterior para obtener reservas entre un rango de fechas
        public async Task<List<Reservas>> ObtenerReservasPorRangoFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _database.Table<Reservas>()
                .Where(r => r.Fecha >= fechaInicio && r.Fecha <= fechaFin)
                .ToListAsync();
        }
        // Obtiene todas las reservas sin filtro
        public async Task<List<Reservas>> ObtenerTodasLasReservas()
        {
            return await _database.Table<Reservas>().ToListAsync();
        }

        // Obtiene el nombre del usuario por su ID
        public async Task<string> ObtenerNombreUsuarioPorId(int usuarioId)
        {
            var usuario = await _database.Table<Usuarios>().Where(u => u.Id == usuarioId).FirstOrDefaultAsync();
            return usuario?.Nombre ?? "Usuario desconocido";
        }

        /****************************************************************************
        *****************************************************************************
        ------------------------INICIALIZACIÓN RESERVAS------------------------------
        *****************************************************************************
        ****************************************************************************/
        public async Task InicializarDatos()
        {
            if ((await ObtenerTodasLasSalas()).Count == 0)
            {
                var listaInicial = new List<Salas>
                {
                    new() { Nombre = "Sala 1", Capacidad = 10, Equipamiento = new List<string> { "Proyector", "Pizarra" }, Estado = "Disponible", Imagen = "dotnet_bot.png" },
                    new() { Nombre = "Sala 2", Capacidad = 10, Equipamiento = new List<string> { "Proyector" }, Estado = "Disponible", Imagen = "dotnet_bot.png" },
                    new() { Nombre = "Sala 3", Capacidad = 10, Equipamiento = new List<string> { "Pizarra" }, Estado = "Disponible", Imagen = "dotnet_bot.png" },
                    new() { Nombre = "Sala 4", Capacidad = 10, Equipamiento = new List<string> { "Proyector", "Pizarra" }, Estado = "Disponible", Imagen = "dotnet_bot.png" },
                };

                foreach (var sala in listaInicial)
                {
                    await InsertarSala(sala);
                }
            }
        }
    }
}
