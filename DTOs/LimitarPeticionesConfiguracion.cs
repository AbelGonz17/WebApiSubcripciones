﻿namespace WebAPIAutores.DTOs
{
    public class LimitarPeticionesConfiguracion
    {
        public int PeticionesPorDiaGratuito { get; set; }
        public string[] listaBlancaRutas  { get; set; }
    }
}
