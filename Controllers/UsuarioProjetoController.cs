using DiarioDeEspecime.Data;
using DiarioDeEspecime.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace DiarioDeEspecime.Controllers
{
    public class UsuarioProjetoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public UsuarioProjetoController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // List users in a project
        public async Task<IActionResult> Index(int projetoId, string search)
        {
            if (!await UsuarioEhCriadorAsync(projetoId))
                return Forbid();

            var query = _context.UsuarioProjeto
                .Include(up => up.Usuario)
                .Where(up => up.ProjetoId == projetoId);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(up =>
                    up.Usuario.UserName.Contains(search) ||
                    up.Usuario.Email.Contains(search)
                );
            }

            ViewBag.ProjetoId = projetoId;
            return View(await query.ToListAsync());
        }

        // GET: Add user to project
        public IActionResult Create(int projetoId)
        {
            if (!UsuarioEhCriadorAsync(projetoId).Result)
                return Forbid();
            ViewBag.ProjetoId = projetoId;
            return View(new UsuarioProjeto { ProjetoId = projetoId });
        }

        // POST: Add user to project
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioProjeto usuarioProjeto)
        {
            if (ModelState.IsValid)
            {
                _context.UsuarioProjeto.Add(usuarioProjeto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { projetoId = usuarioProjeto.ProjetoId });
            }
            ViewBag.ProjetoId = usuarioProjeto.ProjetoId;
            return View(usuarioProjeto);
        }

        // GET: Edit user in project
        public async Task<IActionResult> Edit(int projetoId, string usuarioId)
        {
            var usuarioProjeto = await _context.UsuarioProjeto
                .Include(up => up.Projeto)
                .Include(up => up.Usuario)
                .FirstOrDefaultAsync(up => up.ProjetoId == projetoId && up.UsuarioId == usuarioId);

            if (usuarioProjeto == null)
            {
                TempData["Error"] = "Usuário ou projeto não encontrado.";
                return RedirectToAction(nameof(Index), new { projetoId });
            }

            return View(usuarioProjeto);
        }

        // POST: Edit user in project
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UsuarioProjeto usuarioProjeto)
        {
            if (ModelState.IsValid)
            {
                _context.Update(usuarioProjeto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { projetoId = usuarioProjeto.ProjetoId });
            }
            return View(usuarioProjeto);
        }

        // GET: Delete user from project
        public async Task<IActionResult> Delete(int projetoId, string usuarioId)
        {
            var usuarioProjeto = await _context.UsuarioProjeto
                .Include(up => up.Usuario)
                .FirstOrDefaultAsync(up => up.ProjetoId == projetoId && up.UsuarioId == usuarioId);

            if (usuarioProjeto == null)
                return NotFound();

            return View(usuarioProjeto);
        }

        // POST: Delete user from project
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int projetoId, string usuarioId)
        {
            var usuarioProjeto = await _context.UsuarioProjeto
                .FirstOrDefaultAsync(up => up.ProjetoId == projetoId && up.UsuarioId == usuarioId);

            if (usuarioProjeto != null)
            {
                _context.UsuarioProjeto.Remove(usuarioProjeto);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { projetoId });
        }

        private async Task<bool> UsuarioEhCriadorAsync(int projetoId)
        {
            var userId = _userManager.GetUserId(User);
            var projeto = await _context.Projetos.FindAsync(projetoId);
            return projeto != null && projeto.CriadorId == userId;
        }

        public async Task<IActionResult> BuscarUsuarios(string termo)
        {
            // Certifique-se de que o termo pode ser nulo ou vazio para retornar todos os usuários
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(termo))
            {
                query = query.Where(u => u.UserName.Contains(termo) || u.Email.Contains(termo));
            }

            var usuarios = await query
                .Select(u => new
                {
                    u.Id,
                    u.Nome,
                    u.Sobrenome,
                    u.Email
                })
                .ToListAsync();

            return Json(new { success = true, data = usuarios });
        }


    }
}
