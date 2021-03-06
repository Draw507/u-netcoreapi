﻿using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiPrimerWebApiM3.Context;
using MiPrimerWebApiM3.Entities;
using MiPrimerWebApiM3.Helpers;
using MiPrimerWebApiM3.Models;
using MiPrimerWebApiM3.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiPrimerWebApiM3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("PermitirApiRequest")]
    [HttpHeaderIsPresent("x-version", "1")]
    public class AutoresController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IClaseB claseB;
        private readonly ILogger<AutoresController> logger;
        private readonly IMapper mapper;
        private readonly Microsoft.Extensions.Configuration.IConfiguration configuration;
        private readonly HashServices hashServices;
        private readonly IDataProtector _protector;

        public AutoresController(ApplicationDbContext context, IClaseB claseB, ILogger<AutoresController> logger, IMapper mapper,
            Microsoft.Extensions.Configuration.IConfiguration configuration, IDataProtectionProvider protectionProvider, HashServices hashServices)
        {
            this.context = context;
            this.claseB = claseB;
            this.logger = logger;
            this.mapper = mapper;
            this.configuration = configuration;
            this.hashServices = hashServices;
            this._protector = protectionProvider.CreateProtector("valor_unico_y_quizas_secreto");
        }

        //Multiple endpoint
        //[HttpGet("/listado")] // ignora [Route("api/[controller]")]
        [HttpGet("listado")]
        //[HttpGet]
        [ServiceFilter(typeof(MiFiltroDeAccion))] // Esto es por la inyeccion de dependencias en el filtro de accion
        public ActionResult<IEnumerable<Autor>> Get()
        {
            //throw new NotImplementedException();

            //logger.LogInformation("Obteniendo los autores");

            //return context.Autores.ToList();

            var protectorLimitadoTiempo = _protector.ToTimeLimitedDataProtector();

            string textoPlano = "David";
            //string textoCifrado = _protector.Protect(textoPlano);
            //string textoDesencriptado = _protector.Unprotect(textoCifrado);
            string textoCifrado = protectorLimitadoTiempo.Protect(textoPlano, TimeSpan.FromSeconds(5));
            string textoDesencriptado = protectorLimitadoTiempo.Unprotect(textoCifrado);

            return Ok(new { textoPlano, textoCifrado, textoDesencriptado });
        }

        [HttpGet("GenerarHash")]
        public ActionResult<string> GetHash()
        {
            string textoPlano = "David";
            var hashResult1 = hashServices.Hash(textoPlano).Hash;
            var hashResult2 = hashServices.Hash(textoPlano).Hash;

            return Ok(new { textoPlano, hashResult1, hashResult2 });
        }

        /// <summary>
        /// Obtiene un autor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param2"></param>
        /// <returns></returns>
        // GET api/autores/5 o api/autores/5/david
        [HttpGet("{id}/{param2?}", Name = "ObtenerAutor")] // param2 es opcional
        //[HttpGet("{id}/{param2=David}", Name = "ObtenerAutor")] // param2 con parametro por defecto
        //[ProducesResponseType(404)]
        //[ProducesResponseType(typeof(AutorDTO), 200)]
        public ActionResult<AutorDTO> Get(int id, string param2)
        {
            var autor = context.Autores.FirstOrDefault(x => x.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            var autorDTO = mapper.Map<AutorDTO>(autor);

            GenerarEnlaces(autorDTO);

            return autorDTO;
        }

        private void GenerarEnlaces(AutorDTO autor)
        {

            autor.Enlaces.Add(new Enlace(href: Url.Link("ActualizarAutor", new { id = autor.Id }), rel: "self", metodo: "PUT"));
            autor.Enlaces.Add(new Enlace(href: Url.Link("ObtenerAutor", new { id = autor.Id }), rel: "self", metodo: "GET"));
            autor.Enlaces.Add(new Enlace(href: Url.Link("BorrarAutor", new { id = autor.Id }), rel: "self", metodo: "DELETE"));
        }

        /// <summary>
        /// Lee la configuracion de appsettings
        /// </summary>
        /// <returns></returns>
        [HttpGet("LeerConfiguracion")]
        public ActionResult<string> Configuracion()
        {
            return configuration["apellido"];
            //return configuration["connectionStrings:DefaultConnection"]; //Busqueda interna
        }

        [HttpPost(Name = "CrearAutor")]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacion)
        {
            var autor = mapper.Map<Autor>(autorCreacion);

            context.Autores.Add(autor);
            await context.SaveChangesAsync();

            var autorDTO = mapper.Map<AutorDTO>(autor);

            return new CreatedAtRouteResult("ObtenerAutor", new { id = autor.Id }, autorDTO);
        }

        [HttpPut("{id}", Name = "ActualizarAutor")]
        public async Task<ActionResult> Put(int id, [FromBody] AutorCreacionDTO autorActualizacion)
        {
            var autor = mapper.Map<Autor>(autorActualizacion);
            autor.Id = id;

            context.Entry(autor).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return NoContent();
        }

        //JSON Patch RFC 6902
        [HttpPatch("{id}", Name = "ActualizarParcialAutor")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<AutorCreacionDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var autorDeLaDB = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);

            if (autorDeLaDB == null)
            {
                return NotFound();
            }

            var autorDTO = mapper.Map<AutorCreacionDTO>(autorDeLaDB);

            patchDocument.ApplyTo(autorDTO, ModelState);

            mapper.Map(autorDTO, autorDeLaDB);

            var isValid = TryValidateModel(autorDeLaDB);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}", Name = "BorrarAutor")]
        public async Task<ActionResult<Autor>> Delete(int id)
        {
            var autorId = await context.Autores.Select(x => x.Id).FirstOrDefaultAsync(x => x == id);

            if (autorId == default(int))
            {
                return NotFound();
            }

            context.Autores.Remove(new Autor { Id = autorId });
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("Caching")]
        [Authorize]
        [ResponseCache(Duration = 15)]
        public ActionResult<string> Caching()
        {
            return DateTime.Now.Second.ToString();
        }

    }
}
