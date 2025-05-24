using SQLite;

namespace TFG.Models
{
    public class Salas
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Capacidad { get; set; }

        // Cambiado de string a lista de strings (serializada manualmente)
        public string EquipamientoSerializado { get; set; }

        [Ignore]
        public string EquipamientoTexto => string.Join(", ", Equipamiento);

        [Ignore]
        public List<string> Equipamiento
        {
            get => EquipamientoSerializado?.Split(',').ToList() ?? new List<string>();
            set => EquipamientoSerializado = string.Join(",", value);
        }

        public string Estado { get; set; }
        public string Imagen { get; set; }
    }
}
