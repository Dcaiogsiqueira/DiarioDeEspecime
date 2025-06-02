using DiarioDeEspecime.Data;
using DiarioDeEspecime.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiarioDeEspecime.Controllers
{
    [Authorize]
    public class ConfiguracoesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public ConfiguracoesController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Aparencia()
        {
            var biomas = await _context.Biomas.ToListAsync();
            var user = await _userManager.GetUserAsync(User);
            ViewBag.Biomas = biomas;
            ViewBag.BiomaId = user.BiomaId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Aparencia(int biomaId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                user.BiomaId = biomaId;
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction("Index");
        }
    }
}
