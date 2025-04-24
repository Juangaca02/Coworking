using SQLite;

namespace TFG.Models
{
    public class Salas
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Capacidad { get; set; }
        public string Equipamiento { get; set; }
        public string Estado { get; set; } // Disponible, Ocupada, En Mantenimiento

        public string Imagen { get; set; }
    }
}
