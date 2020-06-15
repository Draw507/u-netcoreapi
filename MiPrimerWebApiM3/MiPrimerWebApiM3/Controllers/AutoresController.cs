using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    public class AutoresController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IClaseB claseB;
        private readonly ILogger<AutoresController> logger;
        private readonly IMapper mapper;

        public AutoresController(ApplicationDbContext context, IClaseB claseB, ILogger<AutoresController> logger, IMapper mapper)
        {
            this.context = context;
            this.claseB = claseB;
            this.logger = logger;
            this.mapper = mapper;
        }

        //Multiple endpoint
        [HttpGet("/listado")] // ignora [Route("api/[controller]")]
        [HttpGet("listado")]
        [HttpGet]
        [ServiceFilter(typeof(MiFiltroDeAccion))] // Esto es por la inyeccion de dependencias en el filtro de accion
        public ActionResult<IEnumerable<Autor>> Get()
        {
            //throw new NotImplementedException();

            logger.LogInformation("Obteniendo los autores");

            return context.Autores.ToList();
        }

        // GET api/autores/5 o api/autores/5/david
        [HttpGet("{id}/{param2?}", Name = "ObtenerAutor")] // param2 es opcional
        //[HttpGet("{id}/{param2=David}", Name = "ObtenerAutor")] // param2 con parametro por defecto
        public ActionResult<AutorDTO> Get(int id, string param2)
        {
            var autor = context.Autores.FirstOrDefault(x => x.Id == id);

            if(autor == null)
            {
                return NotFound();
            }

            var autorDTO = mapper.Map<AutorDTO>(autor);

            return autorDTO;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacion)
        {
            var autor = mapper.Map<Autor>(autorCreacion);

            context.Autores.Add(autor);
            await context.SaveChangesAsync();

            var autorDTO = mapper.Map<AutorDTO>(autor);

            return new CreatedAtRouteResult("ObtenerAutor", new { id = autor.Id }, autorDTO);
        }

        [HttpPut]
        public ActionResult Put(int id, [FromBody] Autor value)
        {
            if(id != value.Id)
            {
                return BadRequest();
            }

            context.Entry(value).State = EntityState.Modified;
            context.SaveChanges();

            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult<Autor> Delete(int id)
        {
            var autor = context.Autores.FirstOrDefault(x => x.Id == id);

            if(autor == null)
            {
                return NotFound();
            }

            context.Autores.Remove(autor);
            context.SaveChanges();

            return autor;
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
