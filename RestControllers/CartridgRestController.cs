using Folivora.Scaffold;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.DbModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project.RestControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartridgRestController : ControllerBase

    {
        private CartridgeDbContext _context;
        public CartridgRestController(CartridgeDbContext context)
        {
            _context = context;
        }
        // GET: api/<CartridgRestController>
        [HttpGet]
        public IEnumerable<Cartridge> Get()
        {
            return _context.Cartridges;
        }

        // GET api/<CartridgRestController>/5
        [HttpGet("{Id}")]
        public IActionResult Get(int id)
        {
            var cartridg = _context.Cartridges.Find(id)!;

            if (cartridg == null)
            {
                return NotFound($"Запись не найдена.");
            }
            return Ok(cartridg);
        }

        // POST api/<CartridgRestController>
        [HttpPost("{Id}")]
        public IActionResult Post(Cartridge cartridg)
        {
            if (_context.Cartridges.FirstOrDefault(u => u.IdCr == cartridg.IdCr) == null)
            {
                _context.Cartridges.Add(cartridg);
                _context.SaveChanges();
                return Ok(cartridg);
            }
            return BadRequest();
        }

        // PUT api/<CartridgRestController>/5
        [HttpPut("{Id}")]
        public IActionResult Put(int id, Cartridge cartridg)
        {
            var cr = _context.Cartridges.FirstOrDefault(u => u.IdCr == id);
            if (cr != null)
            {
                cr.StatusCr = cartridg.StatusCr;
                cr.LocationCr = cartridg.LocationCr;

                _context.Entry(cr).State = EntityState.Modified;
                _context.SaveChanges();
                return Ok(cr);
            }
            return NotFound($"Запись не найдена.");
        }

        // DELETE api/<CartridgRestController>/5
        [HttpDelete("{Id}")]
        public IActionResult Delete(int id)
        {
            var cartridg = _context.Cartridges.Find(id);
            if (cartridg != null)
            {
                _context.Cartridges.Remove(cartridg);
                _context.SaveChanges();
                return NoContent();
            }
            return NotFound($"Запись не найдена.");
        }
    }
}
