
using CamperoAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CamperoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpleadosController : ControllerBase
    {
        private readonly DbCamperoContext _context;

        public EmpleadosController(DbCamperoContext context)
        {
            _context = context;
        }

        // Consultar todos los empleados
        [HttpGet]
        public async Task<IActionResult> ListarEmpleados()
        {
            var empleados = await _context.Empleados
                .AsNoTracking()
                .Select(e => new
                {
                    e.IdEmpleado,
                    e.IdPuesto,
                    e.Nombre,
                    e.Apellidos
                })
                .ToListAsync();

            return Ok(empleados);
        }

        // Consultar un empleado por ID
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerEmpleado(int id)
        {
            var empleado = await _context.Empleados
                .AsNoTracking()
                .Where(e => e.IdEmpleado == id)
                .Select(e => new
                {
                    e.IdEmpleado,
                    e.IdPuesto,
                    e.Nombre,
                    e.Apellidos
                })
                .FirstOrDefaultAsync();

            if (empleado == null)
            {
                return NotFound("Empleado no encontrado.");
            }

            return Ok(empleado);
        }

        // Registrar un nuevo empleado
        [HttpPost]
        public async Task<IActionResult> CrearEmpleado(
            [FromBody] EmpleadoDatos datos)
        {
            var puestoExiste = await _context.Puestos
                .AnyAsync(p => p.IdPuesto == datos.IdPuesto);

            if (!puestoExiste)
            {
                return BadRequest("El puesto indicado no existe.");
            }

            var empleado = new Empleado
            {
                IdPuesto = datos.IdPuesto,
                Nombre = datos.Nombre,
                Apellidos = datos.Apellidos
            };

            _context.Empleados.Add(empleado);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(ObtenerEmpleado),
                new { id = empleado.IdEmpleado },
                new
                {
                    empleado.IdEmpleado,
                    empleado.IdPuesto,
                    empleado.Nombre,
                    empleado.Apellidos
                }
            );
        }

        // Actualizar un empleado
        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarEmpleado(
            int id, [FromBody] EmpleadoDatos datos)
        {
            if (id != datos.IdEmpleado)
            {
                return BadRequest("Los ID del empleado no coinciden.");
            }

            var empleado = await _context.Empleados.FindAsync(id);

            if (empleado == null)
            {
                return NotFound("Empleado no encontrado.");
            }

            var puestoExiste = await _context.Puestos
                .AnyAsync(p => p.IdPuesto == datos.IdPuesto);

            if (!puestoExiste)
            {
                return BadRequest("El puesto indicado no existe.");
            }

            empleado.IdPuesto = datos.IdPuesto;
            empleado.Nombre = datos.Nombre;
            empleado.Apellidos = datos.Apellidos;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Eliminar un empleado
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarEmpleado(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);

            if (empleado == null)
            {
                return NotFound("Empleado no encontrado.");
            }

            _context.Empleados.Remove(empleado);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class EmpleadoDatos
    {
        public int IdEmpleado { get; set; }

        public int IdPuesto { get; set; }

        [StringLength(45)]
        public string? Nombre { get; set; }

        [StringLength(45)]
        public string? Apellidos { get; set; }
    }
}
