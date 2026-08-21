using Folivora.Scaffold;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.DbModels;
using Project.Models;
using System.Net;
using System.Text;
using Telegram.Bot;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project.RestControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestRestController : ControllerBase
    {
        private CartridgeDbContext _context;
        private ITelegramBotClient _botClient;
        public RequestRestController(CartridgeDbContext context, ITelegramBotClient botClient)
        {
            _context = context;
            _botClient = botClient;
        }
        // GET: api/<ZaiavkaRestController>
        [HttpGet]
        public IEnumerable<Request> Get()
        {
            return _context.Requests;
        }

        // GET api/<ZaiavkaRestController>/5
        [HttpGet("{Id}")]
        public IActionResult Get(int id)
        {
            var request = _context.Requests.Find(id);

            if (request == null)
            {
                return NotFound($"Запись не найдена.");
            }

            return Ok(request);
        }

        // GET api/<ZaiavkaRestController>/new
        [HttpGet("new")]
        public IActionResult CreateNew(int id) => CreateNewRequest(new Request());

        // POST api/<ZaiavkaRestController>
        [HttpPost("{Id}")]
        public IActionResult Post(Request request)
        {
            return CreateNewRequest(request);
        }
        protected IActionResult CreateNewRequest(Request request)
        {
            string? printerId = HttpContext.Request.Query.FirstOrDefault(q => q.Key == "printer").Value;

            string? ip = "";

            if (string.IsNullOrEmpty(printerId))
            {
                ip = Response.HttpContext.Connection.RemoteIpAddress?.ToString();

                if (ip == "::1")
                {
                    ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
                }
            }
            else
            {
                var printerFromQuery = _context.Printers.FirstOrDefault(u => u.IdPrint == int.Parse(printerId));
                if (printerFromQuery != null)
                {
                    ip = printerFromQuery.Ip;
                }
            }

            if (_context.Requests.FirstOrDefault(u => u.IpPrinter == ip) != null)
            {
                return NotFound("Такой IP уже существует.");
            }

            request.IpPrinter = ip;

            Printer? print = _context.Printers.FirstOrDefault(u => u.Ip == request.IpPrinter);
            if (print == null)
            {
                return NotFound();
            }

            request.IdPrinterFk = print.IdPrint;

            Office? office = _context.Offices.FirstOrDefault(u => u.Id == print.OffId);
            if (office == null)
            {
                return NotFound("Офис не найден.");
            }

            var officeToStringCommand = new OfficeToStringCommand(_context, office);
            string officeLocation = officeToStringCommand.Execute();

            if (print.CartridgeId != null)
            {
                Cartridge? cartridg = _context.Cartridges.FirstOrDefault(c => c.IdCr == print.CartridgeId);
                if (cartridg == null)
                {
                    return NotFound("Картридж не найден.");
                }

                Model? model = _context.Models.FirstOrDefault(m => m.Id == cartridg.ModelId);
                if (model == null)
                {
                    return NotFound("Модель картриджа не найдена.");
                }

                Vendor? firm = _context.Vendors.FirstOrDefault(f => f.Id == model.FirmId);

                string cartridgeModelName = model.ModelNumber;
                if (firm != null)
                {
                    cartridgeModelName = firm.Name + " " + model.ModelNumber;
                }

                request.CartridgeModel = cartridgeModelName;
            }

            request.StatusZv = RequestStatus.Open;

            _context.Requests.Add(request);
            _context.SaveChanges();

            SendTelegramNotification(request, officeLocation);

            async Task SendTelegramNotification(Request newRequest, string officeLocation)
            {
                long chatId = 929453196;

                var sb = new StringBuilder();
                sb.AppendLine("Новая заявка!");
                sb.Append("ID: ").AppendLine(newRequest.IdZv.ToString());
                sb.Append("Статус: ").AppendLine(newRequest.StatusZv.ToString());
                sb.Append("Модель картриджа: ").AppendLine(newRequest.CartridgeModel);
                sb.Append("Местоположение офиса: ").AppendLine(officeLocation);

                string message = sb.ToString();

                await _botClient.SendTextMessageAsync(chatId, message);
            }

            return Redirect(
                "https://cartridgesmonitoring.azurewebsites.net/Request/Details/" + request.IdZv
            );
        }


        // PUT api/<ZaiavkaRestController>/5
        [HttpPut("{Id}")]
        public IActionResult Put(int id, Request request)
        {
            var zv = _context.Requests.FirstOrDefault(u => u.IdZv == id);
            if (zv != null)
            {
                zv.StatusZv = RequestStatus.Closed;

                _context.Entry(zv).State = EntityState.Modified;
                _context.SaveChanges();
                return Ok(zv);
            }
            return NotFound();
        }

        // DELETE api/<ZaiavkaRestController>/5
        [HttpDelete("{Id}")]
        public IActionResult Delete(int id)
        {
            var request = _context.Requests.Find(id);

            if (request != null)
            {
                _context.Remove(request);
                _context.SaveChanges();
                return NoContent();
            }

            return NotFound();
        }

        [HttpPost("AssignTo")]
        public async Task<ActionResult> AssignTo([FromBody] AssignToRequest assignTo)
        {
            foreach (var req in _context.Requests.Where(r => assignTo.Requests.Contains(r.IdZv)))
            {
                req.LastName = assignTo.Assign;
                req.StatusZv = RequestStatus.AtWork;

                await _botClient.SendTextMessageAsync(
                    929453196,
                    $"Заявка {req.IdZv} взята в работу\n" +
                    $"Кто: {assignTo.Assign}\n" +
                    $"Статус: В работе"
                );
            }

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
