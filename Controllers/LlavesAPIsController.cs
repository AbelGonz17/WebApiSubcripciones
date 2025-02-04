using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.DTOs;
using WebAPIAutores.Servicios;

namespace WebAPIAutores.Controllers
{
    [ApiController]
    [Route("api/llavesapi")]
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]  
    public class LlavesAPIsController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly ServicioLlaves servicioLlaves;

        public LlavesAPIsController(ApplicationDbContext context, IMapper mapper, ServicioLlaves servicioLlaves)
        {
            this.context = context;
            this.mapper = mapper;
            this.servicioLlaves = servicioLlaves;
        }

        [HttpGet]
        public async Task<List<LlaveDTO>> MisLlaves()
        {
            var usuarioId = ObtenerUsuarioId();
            var llaves = await context.llaveAPIs
                .Include(x => x.RestriccionesDominio)
                .Include(x => x.RestriccionesIP)
                .Where(x => x.Usuarioid == usuarioId)
                .ToListAsync();
            return mapper.Map<List<LlaveDTO>>(llaves);
        }

        [HttpPost]
        public async Task<ActionResult> CrearLlave(CrearLlaveDTO crearLlaveDTO)
        {
            var usuarioId = ObtenerUsuarioId();

            if(crearLlaveDTO.tipoLlave == TipoLlave.Gratuita)
            {
                var elUsuarioYaTieneUnaLlaveGratuita = await context.llaveAPIs
                    .AnyAsync(x => x.Usuarioid == usuarioId && x.tipoLlave == TipoLlave.Gratuita);

                if(elUsuarioYaTieneUnaLlaveGratuita)
                {
                    return BadRequest("El usuario ya tiene una llave gratuita");
                }
            }

            await servicioLlaves.CrearLlave(usuarioId, crearLlaveDTO.tipoLlave);
            return NoContent();

        }
        [HttpPut]
        public async Task<ActionResult> ActualizarLlave( ActualizarLlaveDTO actualizarLlaveDTO)
        {
            var usuarioId = ObtenerUsuarioId();

            var llaveDB = await context.llaveAPIs.FirstOrDefaultAsync(x => x.Id == actualizarLlaveDTO.LlaveId);

            if(llaveDB == null ) { return NotFound(); }

            if(usuarioId != llaveDB.Usuarioid)
            {
                return Forbid();
            }

            if(actualizarLlaveDTO.ActualizarLlave)
            {
                llaveDB.Llave = servicioLlaves.GenerarLlave();
            }

            llaveDB.Activa = actualizarLlaveDTO.Activa;
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
