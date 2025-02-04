using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace WebAPIAutores.DTOs
{
    public class CrearRestriccioneIpDTO
    {
        public int llaveId { get; set; }
        [Required]
        public  string IP{ get; set; }
    }
}
