
using CamperoAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CamperoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PuestosController : ControllerBase
    {
        private readonly DbCamperoContext _context;

        public PuestosController(DbCamperoContext context)
        {
            _context = context;
        }

        // Consultar todos los puestos
        [HttpGet]
        public async Task<IActionResult> ListarPuestos()
        {
            var puestos = await _context.Puestos
                .AsNoTracking()
                .Select(p => new
                {
                    p.IdPuesto,
                    p.Nombre
                })
                .ToListAsync();

            return Ok(puestos);
        }

        // Consultar un puesto por ID
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPuesto(int id)
        {
            var puesto = await _context.Puestos
                .AsNoTracking()
                .Where(p => p.IdPuesto == id)
                .Select(p => new
                {
                    p.IdPuesto,
                    p.Nombre
                })
                .FirstOrDefaultAsync();

            if (puesto == null)
            {
                return NotFound("Puesto no encontrado.");
            }

            return Ok(puesto);
        }

        // Registrar un nuevo puesto
        [HttpPost]
        public async Task<IActionResult> CrearPuesto(
            [FromBody] PuestoDatos datos)
        {
            var puesto = new Puesto
            {
                Nombre = datos.Nombre
            };

            _context.Puestos.Add(puesto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(ObtenerPuesto),
                new { id = puesto.IdPuesto },
                new
                {
                    puesto.IdPuesto,
                    puesto.Nombre
                }
            );
        }

        // Actualizar un puesto
        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarPuesto(
            int id, [FromBody] PuestoDatos datos)
        {
            if (id != datos.IdPuesto)
            {
                return BadRequest("Los ID del puesto no coinciden.");
            }

            var puesto = await _context.Puestos.FindAsync(id);

            if (puesto == null)
            {
                return NotFound("Puesto no encontrado.");
            }

            puesto.Nombre = datos.Nombre;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Eliminar un puesto
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarPuesto(int id)
        {
            var puesto = await _context.Puestos.FindAsync(id);

            if (puesto == null)
            {
                return NotFound("Puesto no encontrado.");
            }

            var tieneEmpleados = await _context.Empleados
                .AnyAsync(e => e.IdPuesto == id);

            if (tieneEmpleados)
            {
                return Conflict(
                    "No se puede eliminar el puesto porque tiene empleados asignados."
                );
            }

            _context.Puestos.Remove(puesto);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class PuestoDatos
    {
        public int IdPuesto { get; set; }

        [StringLength(45)]
        public string? Nombre { get; set; }
    }
}
