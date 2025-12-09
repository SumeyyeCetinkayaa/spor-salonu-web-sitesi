using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessCenter.Data;
using FitnessCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenter.Controllers.Api
{
    [Route("api/trainers")]
    [ApiController]
    public class TrainersApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TrainersApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/trainers
        // Aktif eğitmenleri isim sırasına göre döner
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Trainer>>> GetAll()
        {
            var trainers = await _context.Trainers
                .Where(t => t.IsActive)                    // LINQ filtresi
                .OrderBy(t => t.FirstName)                 // LINQ sıralama
                .ThenBy(t => t.LastName)
                .ToListAsync();

            return Ok(trainers);
        }

        // GET: api/trainers/5
        // Belirli eğitmeni, verdiği hizmetlerle birlikte döner
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Trainer>> GetById(int id)
        {
            var trainer = await _context.Trainers
                .Include(t => t.TrainerServices)
                    .ThenInclude(ts => ts.Service)         // LINQ + Include
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trainer == null)
                return NotFound();

            return Ok(trainer);
        }

        // GET: api/trainers/search?name=er
        // İsme göre içinde geçen harfe göre arama
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Trainer>>> SearchByName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Name query parameter is required.");

            var trainers = await _context.Trainers
                .Where(t =>
                    (t.FirstName + " " + t.LastName)
                        .ToLower()
                        .Contains(name.ToLower()))        // LINQ Contains
                .OrderBy(t => t.FirstName)
                .ThenBy(t => t.LastName)
                .ToListAsync();

            return Ok(trainers);
        }
    }
}
