using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAzureApiDbHw.Models;
using MyAzureApiDbHW.Data;

namespace MyAzureApiDb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StudentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _context.Students.ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Create(Student s)
        {
            _context.Students.Add(s);
            await _context.SaveChangesAsync();
            return Ok(s);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
                return NotFound();

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
