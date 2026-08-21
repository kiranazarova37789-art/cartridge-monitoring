using Folivora.Scaffold;
using Microsoft.AspNetCore.Mvc;
using Project.DbModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project.RestControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfficeRestController : ControllerBase

    {

        private CartridgeDbContext _context;
        public OfficeRestController(CartridgeDbContext context)
        {
            _context = context;
        }

        // GET: api/<OfficeRestController>
        [HttpGet]
        public IEnumerable<Office> Get()
        {
            return _context.Offices;
        }

        [HttpGet("{Id}")]
        public IActionResult Get(int id)
        {
            var office = _context.Offices.FirstOrDefault(o => o.Id == id);

            if (office == null)
            {
                return NotFound("Запись не найдена.");
            }

            Office? parentOffice = null;
            if (office.Parent != 0)
            {
                parentOffice = _context.Offices.FirstOrDefault(o => o.Id == office.Parent);
            }

            var result = new
            {
                office = office,
                parentOffice = parentOffice
            };

            return Ok(result);
        }

        // POST api/<OfficeRestController>
        [HttpPost("{Id}")]
        public ActionResult Post(Office office)
        {
            if (office == null)
            {
                return BadRequest("Некорректные данные.");
            }

            var newOffice = new Office
            {
                Name = office.Name,
                Parent = office.Parent,
                Level = office.Level
            };

            _context.Offices.Add(newOffice);

            _context.SaveChangesAsync();

            return Ok(newOffice);
        }


        // PUT api/<OfficeRestController>/5
        [HttpPut("{Id}")]
        public IActionResult Put(int id, Office newOffice)
        {
            var office = _context.Offices.FirstOrDefault(o => o.Id == id);

            if (office == null)
            {
                return NotFound("Офис не найден.");
            }

            office.Name = newOffice.Name;
            office.Parent = newOffice.Parent;
            office.Level = newOffice.Level;

            _context.SaveChanges();

            return Ok(office);
        }


        // DELETE api/<OfficeRestController>/5
        [HttpDelete("{Id}")]
        public IActionResult Delete(int id)
        {
            var office = _context.Offices.Find(id);
            if (office != null)
            {
                _context.Offices.Remove(office);
                _context.SaveChanges();
                return NoContent();
            }
            return NotFound($"Запись не найдена.");
        }

    }
}
