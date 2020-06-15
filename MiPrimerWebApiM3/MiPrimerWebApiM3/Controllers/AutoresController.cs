using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] AutorCreacionDTO autorActualizacion)
        {
            var autor = mapper.Map<Autor>(autorActualizacion);
            autor.Id = id;

            context.Entry(autor).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return NoContent();
        }

        //JSON Patch RFC 6902
        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<AutorCreacionDTO> patchDocument)
        {
            if(patchDocument == null)
            {
                return BadRequest();
            }

            var autorDeLaDB = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);

            if(autorDeLaDB == null)
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

        [HttpDelete("{id}")]
        public async Task<ActionResult<Autor>> Delete(int id)
        {
            var autorId = await context.Autores.Select(x => x.Id).FirstOrDefaultAsync(x => x == id);

            if(autorId == default(int))
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
