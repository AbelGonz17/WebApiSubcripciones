using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Threading.Tasks;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entidades;

namespace WebAPIAutores.Controllers
{
    [ApiController]
    [Route("api/restriccionesdominio")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RestriccionesDeDominioController : CustomBaseController
    {
        private readonly ApplicationDbContext context;

        public RestriccionesDeDominioController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Post(CrearRestriccionesDominioDTO crearRestriccionesDominioDTO)
        {
            var llaveDB = await context.llaveAPIs.FirstOrDefaultAsync(x => x.Id == crearRestriccionesDominioDTO.LlaveId);

            if (llaveDB == null)
            {
                return NotFound();
            }

            var usuarioId = ObtenerUsuarioId();

            if (llaveDB.Usuarioid != usuarioId)
            {
                return Forbid();
            }

            var restriccionDominio = new RestriccionDominio
            {
                Dominio = crearRestriccionesDominioDTO.Dominio,
                LlaveId = crearRestriccionesDominioDTO.LlaveId
            };

            context.Add(restriccionDominio);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, ActualizarRestriccionesDominioDTO actualizarDominioDTO)
        {
            var restriccionDominio = await context.RestriccionesDominio.Include(x => x.llave)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (restriccionDominio == null)
            {
                return NotFound();
            }

            var usuarioId = ObtenerUsuarioId();
            if (restriccionDominio.llave.Usuarioid != usuarioId)
            {
                return Forbid();
            }

            restriccionDominio.Dominio = actualizarDominioDTO.Dominio;

            await context.SaveChangesAsync();
            return NoContent();

        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var restriccionDominio = await context.RestriccionesDominio.Include(x => x.llave)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (restriccionDominio == null)
            {
                return NotFound();
            }
            var usuarioId = ObtenerUsuarioId();
            if (restriccionDominio.llave.Usuarioid != usuarioId)
            {
                return Forbid();
            }
            context.Remove(restriccionDominio);
            await context.SaveChangesAsync();
            return NoContent(); 
        }
    }
}
