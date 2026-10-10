
using CamperoAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CamperoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly DbCamperoContext _context;

        public ClientesController(DbCamperoContext context)
        {
            _context = context;
        }

        // Consultar todos los clientes
        [HttpGet]
        public async Task<ActionResult<List<Cliente>>> ListarClientes()
        {
            var clientes = await _context.Clientes
                .AsNoTracking()
                .ToListAsync();

            return Ok(clientes);
        }

        // Consultar un cliente por ID
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Cliente>> ObtenerCliente(int id)
        {
            var cliente = await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente == null)
            {
                return NotFound("Cliente no encontrado.");
            }

            return Ok(cliente);
        }

        // Registrar un nuevo cliente
        [HttpPost]
        public async Task<ActionResult<Cliente>> CrearCliente([FromBody] Cliente cliente)
        {
            cliente.IdCliente = 0;

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(ObtenerCliente),
                new { id = cliente.IdCliente },
                cliente
            );
        }

        // Actualizar un cliente existente
        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarCliente(
            int id, [FromBody] Cliente cliente)
        {
            if (id != cliente.IdCliente)
            {
                return BadRequest("Los ID del cliente no coinciden.");
            }

            var clienteExistente = await _context.Clientes.FindAsync(id);

            if (clienteExistente == null)
            {
                return NotFound("Cliente no encontrado.");
            }

            clienteExistente.Nombre = cliente.Nombre;
            clienteExistente.Apellidos = cliente.Apellidos;
            clienteExistente.Telefono = cliente.Telefono;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Eliminar un cliente por ID
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound("Cliente no encontrado.");
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
