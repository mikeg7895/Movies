using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.Models;
using System.Security.Claims;

namespace Movies.Controllers
{
    [Authorize]
    public class MovieController : Controller
    {
        private readonly ContextApp _context;

        public MovieController(ContextApp context)
        {
            _context = context;
        }

        public async Task<User?> GetUserLogged()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            return await _context.Users.FindAsync(int.Parse(id));
        }

        public async Task<IActionResult> Index(string? search)
        {
            var user = await GetUserLogged();
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }
            IEnumerable<Movie> movies;
            if (!string.IsNullOrEmpty(search))
            {
                movies = await _context.Movies.Where(m => m.Title.Contains(search) && m.UserId == user.Id)
                    .OrderBy(m => m.Id).ToListAsync();
                return View(movies);
            }
            movies = await _context.Movies.Where(m => m.UserId == user.Id).OrderBy(m => m.Id).ToListAsync();
            return View(movies);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Movie movie)
        {
            if (ModelState.IsValid)
            {
                var user = await GetUserLogged();
                if (user == null)
                {
                    return RedirectToAction("Login", "User");
                }
                movie.UserId = user.Id;
                await _context.AddAsync(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        public async Task<IActionResult> Edit(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var movie = await _context.Movies.FindAsync(Id);
            if(movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Movie movie)
        {
            if(ModelState.IsValid)
            {
                _context.Update(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        public async Task<IActionResult> Delete(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var movie = await _context.Movies.FindAsync(Id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> Delete(int Id)
        {
            var movie = await _context.Movies.FindAsync(Id);
            if (movie == null)
            {
                return NotFound();
            }
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
