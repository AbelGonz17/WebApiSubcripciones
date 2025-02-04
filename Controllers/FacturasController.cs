using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WebAPIAutores.DTOs;

namespace WebAPIAutores.Controllers
{
    [ApiController]
    [Route("api/facturas")]
 
    public class FacturasController:ControllerBase
    {
        private readonly ApplicationDbContext context;

        public FacturasController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Pagar(PagarFacturaDTO pagarFacturaDTO)
        {
            var facturaDB = await context.Facturas
                                 .Include(x => x.Usuario)
                                 .FirstOrDefaultAsync(x => x.ID == pagarFacturaDTO.facturaId);

            if(facturaDB == null)
            {
                return NotFound();
            }

            if(facturaDB.pagada)
            {
                return BadRequest("La Factura ya fue saldada");

            }

            //logica para pagar la factura

            facturaDB.pagada = true;
            await context.SaveChangesAsync();

            var hayFacturaPendientesVencidad = await context.Facturas
                .AnyAsync(x => x.UsuarioId == facturaDB.UsuarioId && 
                x.pagada && x.FechaLimeteDePago < DateTime.Today );

            if(!hayFacturaPendientesVencidad)
            {
                facturaDB.Usuario.MalaPaga = false;
                await context.SaveChangesAsync();        
            }

            return NoContent();
        }
    }
}
