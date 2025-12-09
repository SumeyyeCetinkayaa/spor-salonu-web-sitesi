using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessCenter.Data;
using FitnessCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenter.Controllers.Api
{
    [Route("api/services")]
    [ApiController]
    public class ServicesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ServicesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/services
        // Aktif hizmetleri fiyatına göre sıralı döner
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Service>>> GetAll()
        {
            var services = await _context.Services
                .Where(s => s.IsActive)                   // LINQ filtresi
                .OrderBy(s => s.Price)                    // fiyata göre artan
                .ThenBy(s => s.Name)
                .ToListAsync();

            return Ok(services);
        }

        // GET: api/services/5
        // Belirli hizmeti döner
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Service>> GetById(int id)
        {
            var service = await _context.Services
                .FirstOrDefaultAsync(s => s.Id == id);

            if (service == null)
                return NotFound();

            return Ok(service);
        }

        // GET: api/services/by-trainer/3
        // Belirli bir antrenörün verdiği tüm hizmetler
        [HttpGet("by-trainer/{trainerId:int}")]
        public async Task<ActionResult<IEnumerable<Service>>> GetByTrainer(int trainerId)
        {
            var services = await _context.TrainerServices
                .Where(ts => ts.TrainerId == trainerId)   // önce köprü tablo
                .Select(ts => ts.Service)                 // ilgili Service'e git
                .Where(s => s.IsActive)                   // sadece aktifler
                .Distinct()
                .OrderBy(s => s.Name)
                .ToListAsync();

            return Ok(services);
        }
    }
}
