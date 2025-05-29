using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace TFG.Models
{
    public class Salas
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Nombre { get; set; }

        public int Capacidad { get; set; }

        // Campo que se guarda en la BD como string serializado (equipamientos separados por coma)
        public string EquipamientoSerializado { get; set; }

        // Propiedad ignorada por SQLite para binding / uso en código
        [Ignore]
        public List<string> Equipamiento
        {
            get
            {
                // Separar por coma y eliminar espacios al inicio/final de cada elemento
                return string.IsNullOrEmpty(EquipamientoSerializado)
                    ? new List<string>()
                    : EquipamientoSerializado.Split(',')
                        .Select(e => e.Trim())
                        .ToList();
            }
            set
            {
                EquipamientoSerializado = value == null ? string.Empty : string.Join(", ", value);
            }
        }

        [Ignore]
        public string EquipamientoTexto => string.Join(", ", Equipamiento);

        public string Estado { get; set; }

        public string Imagen { get; set; }
    }
}
