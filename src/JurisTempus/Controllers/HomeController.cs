using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using JurisTempus.Data;
using Microsoft.EntityFrameworkCore;
using JurisTempus.ViewModels;
using AutoMapper;
using JurisTempus.Data.Entities;

namespace JurisTempus.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BillingContext _context;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, BillingContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var result = _context.Clients
            .Include(c => c.Address)
            .Include(c => c.Cases)
            // CODICE MAPPING A MANO (SOSTITUITO DA AutoMapper)
            // .Select(c => new ClientDTO()
            // {
            //     Id = c.Id,
            //     Name = c.Name,
            //     ContactName = c.Contact,
            //     Phone = c.Phone
            // })
            .ToArray();

            var vms = _mapper.Map<Client[], ClientDTO[]>(result); // SFRUTTO AUTOMAPPER source -> dest types
            //var vms = _mapper.Map<ClientDTO[]>(result);
            return View(vms);
        }

        [HttpGet("editor/{id:int}")]
        public async Task<IActionResult> ClientEditor(int id)
        {
            var result = await _context.Clients
              .Include(c => c.Address)
              .Where(c => c.Id == id)
              .FirstOrDefaultAsync();

            return View(_mapper.Map<ClientDTO>(result)); // SFRUTTO AUTOMAPPER (basta specificare DEST type, source inferred da parametro passato)
        }

        [HttpPost("editor/{id:int}")]
        public async Task<IActionResult> ClientEditor(int id, ClientDTO model)
        {
            /* // ESEMPIO LOGICA BUSINESS/VALIDAZIONE CUSTOM FATTA DIRETTAMENTE
            if (!string.IsNullOrEmpty(model.ContactName) && string.IsNullOrEmpty(model.Phone))
            {
                ModelState.AddModelError("Phone", "If you specify Contact you MUST specify even Phone number!");
            } */

            if (ModelState.IsValid) // CONTROLLA VALIDITA' REGOLE ModelBinding 
            {

                // Save changes to database
                var old = await _context.Clients
                  // .Include(c => c.Address) // SE VOGLIO AGGIORNARE ANCHE Address (ma devo includere i campi nella form)
                  .Where(c => c.Id == id)
                  .FirstOrDefaultAsync();

                if (old != null)
                {
                    // Update the database 
                    _mapper.Map(model, old); // Copy changes -> SFRUTTO AUTOMAPPER PER COPIARE I DATI source , dest
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    // Create a new one
                    var newClient = _mapper.Map<Client>(model);
                    _context.Add(newClient);
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }

            return View();
        }

        [HttpGet("timesheet")]
        public IActionResult Timesheet()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
