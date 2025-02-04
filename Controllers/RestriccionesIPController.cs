using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entidades;

namespace WebAPIAutores.Controllers
{
    [ApiController]
    [Route("api/restriccionesip")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RestriccionesIPController : CustomBaseController
    {
        private readonly ApplicationDbContext context;

        public RestriccionesIPController(ApplicationDbContext context)
        {
            this.context = context;
        }


        [HttpPost]
        public async Task<ActionResult> Post(CrearRestriccioneIpDTO crearRestriccioneIpDTO)
        {
            var llaveDB = await context.llaveAPIs.FirstOrDefaultAsync(x => x.Id == crearRestriccioneIpDTO.llaveId);
            if (llaveDB == null)
            {
                return NotFound();
            }

            var usuarioId = ObtenerUsuarioId();
            if (llaveDB.Usuarioid != usuarioId)
            {
                return Forbid();
            }

            var restriccionIP = new RestriccionIP
            {
                IP = crearRestriccioneIpDTO.IP,
                llaveId = llaveDB.Id
            };

            context.Add(restriccionIP);
            await context.SaveChangesAsync();
            return NoContent();

        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, ActualizarRestriccionIPDTO actualizarRestriccionesIpDTO)
        {
            var restriccionIP = await context.RestriccionesIPs.Include(x => x.llave)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (restriccionIP == null)
            {
                return NotFound();
            }
            var usuarioId = ObtenerUsuarioId();
            if (restriccionIP.llave.Usuarioid != usuarioId)
            {
                return Forbid();
            }
            restriccionIP.IP = actualizarRestriccionesIpDTO.IP;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var restriccionIP = await context.RestriccionesIPs.Include(x => x.llave)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (restriccionIP == null)
            {
                return NotFound();
            }
            var usuarioId = ObtenerUsuarioId();
            if (restriccionIP.llave.Usuarioid != usuarioId)
            {
                return Forbid();
            }
            context.Remove(restriccionIP);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
