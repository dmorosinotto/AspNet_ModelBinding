using AutoMapper;
using JurisTempus.Data;
using JurisTempus.Data.Entities;
using JurisTempus.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JurisTempus.Controllers
{
    [Route("api/timebills")]
    [ApiController] // USARE QUESTO ATTRIBUTO PER AVERE CONTROLLO AUTOMATICO ModelState.IsValid -> BadRequest FORMATO PROBLEM CON errors
    public class TimeBillsController : ControllerBase
    {
        private readonly ILogger<TimeBillsController> _logger;
        private readonly BillingContext _ctx;
        private readonly IMapper _mapper;

        public TimeBillsController(
            ILogger<TimeBillsController> logger,
            BillingContext ctx,
            IMapper mapper)
        {
            _logger = logger;
            _ctx = ctx;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<TimeBillDTO[]>> Get()
        {
            var result = await _ctx.TimeBills
              .Include(t => t.Case)
              .Include(t => t.Employee)
              .ToArrayAsync();

            return Ok(_mapper.Map<TimeBillDTO[]>(result)); // USO AUTOMAPPER PER MAPPARE DTO IN USCITA (Collection)
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TimeBill>> Get(int id) // PER SCELTA RITORNO INTERA ENTITY
        //public async Task<ActionResult<TimeBillDTO>> Get(int id)
        {
            var result = await _ctx.TimeBills
              .Include(t => t.Case)
              .Include(t => t.Employee)
              .Where(t => t.Id == id)
              .FirstOrDefaultAsync();

            return Ok(result); // PER SCELTA RITORNO INTERA ENTITY
            //return Ok(_mapper.Map<TimeBillDTO>(result)); // USO AUTOMAPPER PER MAPPARE DTO IN USCITA
        }

        /* 
        //CODICE POST ORIGINALE CHE USA Entity 
                [HttpPost]
                public async Task<ActionResult<TimeBill>> Post([FromBody] TimeBill bill)
                {
                    var theCase = await _ctx.Cases
                      .Where(c => c.Id == bill.Case.Id)
                      .FirstOrDefaultAsync();

                    var theEmployee = await _ctx.Employees
                      .Where(e => e.Id == bill.Employee.Id)
                      .FirstOrDefaultAsync();

                    bill.Case = theCase;
                    bill.Employee = theEmployee;

                    _ctx.Add(bill);
                    if (await _ctx.SaveChangesAsync() > 0)
                    {
                        return CreatedAtAction("Get", new { id = bill.Id }, bill);
                    }

                    return BadRequest("Failed to save new timebill");
                }
        */
        [HttpPost]
        public async Task<ActionResult<TimeBill>> Post([FromBody] TimeBillDTO model) // PER SCELTA RITORNO INTERA ENTITY
        //public async Task<ActionResult<TimeBillDTO>> Post([FromBody] TimeBillDTO model)
        {
            // CONTROLLO MANUALE DEL ModelState E RITORNO ELENCO ERRORI SEMPLIFICATO
            // OPPURE BASTA AGGIUNGERE ATTRIBUTO [ApiController] ALLA CLASSE E LO FA DI DEFAULT
            // PERO' RITORNA UN FORAMTO ERRORE PIU' COMPLESSO STANDARD ERROR... 
            // if (!ModelState.IsValid) return BadRequest(ModelState);

            var bill = _mapper.Map<TimeBill>(model); // Copy fields DTO -> Entity

            // RIEMPIO RELATED Entities (SearchById DA model.relID -> EntityREL) USO EF
            var refCase = await _ctx.Cases
              .Where(c => c.Id == model.CaseId)
              .FirstOrDefaultAsync();

            var refEmployee = await _ctx.Employees
              .Where(e => e.Id == model.EmployeeId)
              .FirstOrDefaultAsync();

            // AGGIUNGO CONTROLLO COERENZA DATI PASSATI -> SE NON TROVO relENTITY RITORNO BadRequest
            if (refCase == null || refEmployee == null)
            {
                return BadRequest("CAN'T FIND Case OR Employee");
            }

            bill.Case = refCase;
            bill.Employee = refEmployee;

            _ctx.Add(bill);
            if (await _ctx.SaveChangesAsync() > 0)
            {
                //return CreatedAtAction("Get", new { id = bill.Id }, _mapper.Map<TimeBillDTO>(bill)); //AUTOMAPPER PER RITORNARE DTO aggiornato 
                return CreatedAtAction("Get", new { id = bill.Id }, bill); //PER SCELTA RITORNO INTERA ENTITY
            }

            return BadRequest("Failed to save new timebill");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TimeBillDTO>> Put(int id, [FromBody] TimeBillDTO model)
        {
            var oldBill = await _ctx.TimeBills
              .Where(b => b.Id == id)
              .FirstOrDefaultAsync();

            if (oldBill == null) return NotFound($"Invalid ID {id}"); //BadRequest("Invalid ID");

            _mapper.Map(model, oldBill); // Copy properties to update using AUTOMAPPER
            // oldBill.Rate = bill.Rate;
            // oldBill.TimeSegments = bill.TimeSegments;
            // oldBill.WorkDate = bill.WorkDate;
            // oldBill.WorkDescription = bill.WorkDescription;

            var refCase = await _ctx.Cases
              .Where(c => c.Id == model.CaseId)
              .FirstOrDefaultAsync();

            var refEmployee = await _ctx.Employees
              .Where(e => e.Id == model.EmployeeId)
              .FirstOrDefaultAsync();

            oldBill.Case = refCase;
            oldBill.Employee = refEmployee;

            if (await _ctx.SaveChangesAsync() > 0)
            {
                return Ok(_mapper.Map<TimeBillDTO>(oldBill)); //USE AUTOMAPPER TO RETURN DTO with Updated Data
            }

            return BadRequest("Failed to save new timebill");
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var oldBill = await _ctx.TimeBills
              .Where(b => b.Id == id)
              .FirstOrDefaultAsync();

            if (oldBill == null) return NotFound();

            _ctx.Remove(oldBill);

            if (await _ctx.SaveChangesAsync() > 0)
            {
                return Ok();
            }

            return BadRequest("Failed to save new timebill");
        }

    }
}
