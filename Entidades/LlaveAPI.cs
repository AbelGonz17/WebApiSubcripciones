using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace WebAPIAutores.Entidades
{
    public class LlaveAPI
    {
        public int Id{ get; set; }
        public string Llave{ get; set; }
        public TipoLlave tipoLlave  { get; set; }
        public bool Activa { get; set; }   
        public string  Usuarioid { get; set; }
        public Usuario Usuario { get; set; }
        public List<RestriccionDominio> RestriccionesDominio { get; set; }
        public List<RestriccionIP> RestriccionesIP { get; set; }
    }
}

    public enum TipoLlave
    {
        Gratuita = 1,
        Profesional = 2
    }

