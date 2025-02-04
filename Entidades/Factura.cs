using System;

namespace WebAPIAutores.Entidades
{
    public class Factura
    {
        public  int ID{ get; set; }
        public string UsuarioId { get; set; }
        public Usuario Usuario { get; set; }    
        public bool pagada { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaLimeteDePago { get; set; }
    
    }
}
