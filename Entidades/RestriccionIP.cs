namespace WebAPIAutores.Entidades
{
    //Una IP (Internet Protocol Address) es un número único que identifica un dispositivo en una red.
    public class RestriccionIP
    {
        public int Id { get; set; }
        public int llaveId { get; set; }
        public string IP { get; set; }
        public LlaveAPI llave { get; set; }
    }
}
