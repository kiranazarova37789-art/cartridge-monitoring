using Folivora.Scaffold;
using Microsoft.AspNetCore.Mvc;
using Project.DbModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project.RestControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChangeRestController : ControllerBase
    {

        private CartridgeDbContext _context;
        public ChangeRestController(CartridgeDbContext context)
        {
            _context = context;
        }
        // GET: api/<ChangeRestController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ChangeRestController>/5
        [HttpGet("{Id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ChangeRestController>
        [HttpPost("{Id}")]
        public IActionResult Post(Changes changes)
        {
            try
            {
                changes.CommentStatus = "Поступили";
                _context.Change.Add(changes);
                _context.SaveChanges();

                var command = new ApplyChangesCommand(_context, changes);
                command.Execute();

                changes.CommentStatus = "Выполнено";
                _context.SaveChanges();

                return Ok(changes);
            }

            catch
            {
                changes.CommentStatus = "Ошибка";
                _context.SaveChanges();
                return StatusCode(500);
            }
        }

        // PUT api/<ChangeRestController>/5
        [HttpPut("{Id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ChangeRestController>/5
        [HttpDelete("{Id}")]
        public void Delete(int id)
        {
        }
    }
}
