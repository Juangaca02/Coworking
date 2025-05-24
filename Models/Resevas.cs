using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFG.Models
{
    public class Reservas
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int SalaId { get; set; }  // Relación con la sala
        public int UsuarioId { get; set; }  // Relación con el usuario
        public DateTime Fecha { get; set; }
        public string Hora { get; set; }  // Hora de la reserva
    }
}