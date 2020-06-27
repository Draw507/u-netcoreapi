using Microsoft.AspNetCore.Mvc;
using MiPrimerWebApiM3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiPrimerWebApiM3.Controllers
{
    [ApiController]
    [Route("api")]
    public class RootController: ControllerBase
    {
        [HttpGet(Name="GetRoot")]
        public ActionResult<IEnumerable<Enlace>> Get()
        {
            List<Enlace> enlaces = new List<Enlace>();

            enlaces.Add(new Enlace(href: Url.Link("GetRoot", new { }), rel: "self", metodo: "GET"));
            enlaces.Add(new Enlace(href: Url.Link("ObtenerAutor", new { }), rel: "obtener-autor", metodo: "GET"));
            enlaces.Add(new Enlace(href: Url.Link("CrearAutor", new { }), rel: "crear-autor", metodo: "POST"));

            return Ok();
        }
    }
}
