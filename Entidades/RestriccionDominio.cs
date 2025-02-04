namespace WebAPIAutores.Entidades
{
    //Un dominio es el nombre que se usa para identificar un sitio web en Internet.
    public class RestriccionDominio
    {
        public int Id { get; set; }
        public int LlaveId { get; set; }
        public string  Dominio { get; set; }
        public LlaveAPI llave {  get; set; }

    }
}
