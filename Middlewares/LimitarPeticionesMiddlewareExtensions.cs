
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entidades;

namespace WebAPIAutores.Middlewares
{
    public static  class LimitarPeticionesMiddlewareExtensions
    {
        public static IApplicationBuilder UseLimitarPeticiones(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LimitarPeticionesMiddleware>();
        }
    }
}

public class LimitarPeticionesMiddleware
{
    private readonly RequestDelegate siguiente;
    private readonly IConfiguration configuration;

    public LimitarPeticionesMiddleware(RequestDelegate siguiente, IConfiguration configuration)
    {
        this.siguiente = siguiente;
        this.configuration = configuration;
    }
    public async Task InvokeAsync(HttpContext httpContext, ApplicationDbContext context)
    {
        var limitePeticionesConfiguracion = new LimitarPeticionesConfiguracion();
        configuration.GetRequiredSection("LimitarPeticiones").Bind(limitePeticionesConfiguracion);

        var ruta = httpContext.Request.Path.ToString();
        var estaLaRutaEnListaBlanca = limitePeticionesConfiguracion.listaBlancaRutas.Any(x => ruta.Contains(x));

        if(estaLaRutaEnListaBlanca)
        {
            await siguiente(httpContext);
            return;
        }

        var llaveStringValues = httpContext.Request.Headers["X-API-Key"];

        if(llaveStringValues.Count == 0)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("Debe proveer la llave en la cabecera X-Api-Key");
            return;
        }

        if(llaveStringValues.Count > 1)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("Solo una Llave debe estar presente");
            return;
        }

        var llave = llaveStringValues[0];

        var llaveDb = await context.llaveAPIs
            .Include(x => x.RestriccionesDominio)
            .Include(x => x.RestriccionesIP)
            .Include(x => x.Usuario)
            .FirstOrDefaultAsync(x => x.Llave == llave);
        if(llaveDb == null)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("La Llave no existe");
            return;
        }

        if (!llaveDb .Activa)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("La Llave se encuentra inactiva");
            return;
        }

        if (llaveDb.tipoLlave == TipoLlave.Gratuita)
        {
            var hoy = DateTime.Today;
            var manana = hoy.AddDays(1);
            var cantidadPeticionesRealisadasHoy = await context.Peticiones.
                CountAsync(x => x.LlaveId == llaveDb.Id && x.FechaPeticion >= hoy && x.FechaPeticion < manana);

            if (cantidadPeticionesRealisadasHoy >= limitePeticionesConfiguracion.PeticionesPorDiaGratuito)
            {
                httpContext.Response.StatusCode = 429;//too many request
                await httpContext.Response.WriteAsync("Ha superado el limite de peticiones diarias. " +
                    "Si desea Realizar mas peticiones, actualice su suscripcion a una cuenta profesional");
                return;
            }
        }
        else if (llaveDb.Usuario.MalaPaga)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("La cuenta se encuentra suspendida por falta de pago");
            return;
        }
       
        var superaRestricciones = PeticionSuperaAlgunaDeLasRestricciones(llaveDb, httpContext);

        if (!superaRestricciones)
        {
            httpContext.Response.StatusCode = 403;
            return;
        }

        var peticion = new Peticion() { LlaveId = llaveDb.Id, FechaPeticion = DateTime.UtcNow };
        context.Add(peticion);
        await context.SaveChangesAsync();

        await siguiente(httpContext);
    }

    private bool PeticionSuperaAlgunaDeLasRestricciones(LlaveAPI llaveAPI,HttpContext httpContext)
    {
        var hayRestricciones = llaveAPI.RestriccionesDominio.Any() || llaveAPI.RestriccionesIP.Any();

        if (!hayRestricciones)
        {
            return true;
        }

        var peticionSuperaLasRestriccionesDeDominio =
                PeticionSuperaLasPeticionesDeDomino(llaveAPI.RestriccionesDominio, httpContext);

        var peticionSuperaLasRestriccionesDeIP =
                PeticionSuperaLasRestriccionesDeIP(llaveAPI.RestriccionesIP, httpContext);

        return peticionSuperaLasRestriccionesDeDominio || peticionSuperaLasRestriccionesDeIP;
    }

    
       
    

    private bool PeticionSuperaLasRestriccionesDeIP(List<RestriccionIP> restriccionIPs, HttpContext httpContext)
    {
        if (restriccionIPs == null || restriccionIPs.Count == 0)
        {
            return false;
        }
        var ip = httpContext.Connection.RemoteIpAddress.ToString();
        if(ip == string.Empty)
        {
            return false;
        }
        var superaRestriccion = restriccionIPs.Any(x => x.IP == ip);
        return superaRestriccion;
    }

    private bool PeticionSuperaLasPeticionesDeDomino(List<RestriccionDominio> restricciones, 
        HttpContext httpContext)
    {
        if(restricciones == null || restricciones.Count == 0)
        {
            return false;
        }

        //referer/URL es de hacia donde viene la peticion
        var referer = httpContext.Request.Headers["Referer"].ToString();

        if(referer == string.Empty)
        {
            return false;
        }

        Uri myUri = new Uri(referer);
        string host = myUri.Host;
        
        var superaRestriccion = restricciones.Any(x => x.Dominio == host);
        return superaRestriccion;
    }
}
        

