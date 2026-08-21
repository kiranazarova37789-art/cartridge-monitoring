using Folivora.Scaffold;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.DbModels;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project.RestControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrinterRestController : ControllerBase
    {
        private CartridgeDbContext _context;
        public PrinterRestController(CartridgeDbContext context)
        {
            _context = context;
        }
        // GET: api/<PrinterRestController>
        [HttpGet]
        public IEnumerable<Printer> Get()
        {
            return _context.Printers;
        }

        // GET api/<PrinterRestController>/5
        [HttpGet("{Id}")]
        public IActionResult Get(int id)
        {
            var printer = _context.Printers.Find(id)!;

            if (printer == null)
            {
                return NotFound($"Запись не найдена.");
            }
            return Ok(printer);
        }

        // POST api/<PrinterRestController>
        [HttpPost("{Id}")]
        public IActionResult Post(Printer printer)
        {
            string? ip = Response.HttpContext.Connection.RemoteIpAddress?.ToString();

            if (ip == "::1")
            {
                // Перебираем все IP-адреса устройства и выбираем первый доступный IPv4-адрес
                ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString(); // По умолчанию localhost
            }


            printer.Ip = ip.ToString();

            if (printer.CartridgeId != null)
            {
                bool cartridgeExists = _context.Printers.Any(p => p.CartridgeId == printer.CartridgeId);
                if (cartridgeExists)
                {
                    return BadRequest("Этот картридж уже установлен в другом принтере.");
                }
            }

            _context.Printers.Add(printer);
            _context.SaveChanges();
            return Ok(printer);
        }

        // PUT api/<PrinterRestController>/5
        [HttpPut("{Id}")]
        public IActionResult Put(int id, Printer printer)
        {
            var print = _context.Printers.FirstOrDefault(u => u.IdPrint == id);
            if (print != null)
            {
                print.OffId = printer.OffId;
                print.CartridgeId = printer.CartridgeId;

                _context.Entry(print).State = EntityState.Modified;
                _context.SaveChanges();

                return Ok(print);
            }
            return NotFound($"Запись не найдена.");
        }

        // DELETE api/<PrinterRestController>/5
        [HttpDelete("{Id}")]
        public IActionResult Delete(int id)
        {
            var printer = _context.Printers.Find(id);
            if (printer != null)
            {
                _context.Printers.Remove(printer);
                _context.SaveChanges();
                return NoContent();
            }
            return NotFound($"Запись не найдена.");
        }
    }
}
