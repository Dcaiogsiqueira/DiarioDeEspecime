using DiarioDeEspecime.Data;
using DiarioDeEspecime.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DiarioDeEspecime.Controllers
{
    [Authorize]
    public class EspecimeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EspecimeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Especime
        public async Task<IActionResult> Index(string search,string creatorId,int? projetoId,string sortOrder)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Base query: all especimes for the current user (home) or all especimes of the project (project details)
            IQueryable<Especime> query = _context.Especimes
                .Include(e => e.Especie)
                .Include(e => e.Projeto)
                .Include(e => e.Criador);

            if (projetoId.HasValue)
            {
                // Project context: show all especimes of the project, regardless of user
                query = query.Where(e => e.ProjetoId == projetoId.Value);
            }
            else
            {
                // Home context: show only especimes created by the user
                query = query.Where(e => e.CriadorId == userId);
            }

            // Filtering by search (local, espécie, coletor)
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e =>
                    (e.LocalEncontro != null && e.LocalEncontro.Contains(search)) ||
                    (e.Especie != null && e.Especie.NomeCientifico.Contains(search)) ||
                    (e.Coletor != null && e.Coletor.Contains(search))
                );
            }

            // Filtering by creator
            if (!string.IsNullOrEmpty(creatorId))
            {
                query = query.Where(e => e.CriadorId == creatorId);
            }

            // Sorting
            switch (sortOrder)
            {
                case "inicio_asc":
                    query = query.OrderBy(e => e.DataCadastro);
                    break;
                case "inicio_desc":
                    query = query.OrderByDescending(e => e.DataCadastro);
                    break;
                case "termino_asc":
                    query = query.OrderBy(e => e.DataHoraEncontro);
                    break;
                case "termino_desc":
                    query = query.OrderByDescending(e => e.DataHoraEncontro);
                    break;
                default:
                    query = query.OrderByDescending(e => e.DataCadastro);
                    break;
            }

            // Populate dropdowns
            var creators = await query
                .Select(e => e.Criador)
                .Where(u => u != null)
                .Distinct()
                .ToListAsync();

            var projetos = await _context.Projetos
                .Where(p => p.UsuariosProjeto.Any(up => up.UsuarioId == userId))
                .ToListAsync();

            // Set ViewBag for dropdowns and current selections
            ViewBag.CurrentFilter = search;
            ViewBag.CurrentCreator = creatorId;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.Creators = creators;
            ViewBag.CurrentProjetoId = projetoId;
            ViewBag.Projetos = projetos;

            return View(await query.ToListAsync());
        }


        public async Task<IActionResult> Create(int? especieId, int? projetoId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (especieId == null)
                return RedirectToAction("SelectEspecie", new { projetoId });

            var especie = await _context.Especies.FindAsync(especieId);

            ViewData["EspecieId"] = new SelectList(_context.Especies, "Id", "NomeCientifico", especieId);

            if (projetoId.HasValue)
            {
                ViewData["ProjetoId"] = new SelectList(
                    _context.Projetos.Where(p => p.Id == projetoId.Value),
                    "Id", "Nome", projetoId.Value
                );
                ViewBag.ProjetoFixo = true;
            }
            else
            {
                // Busca projetos do usuário
                var projetos = await _context.UsuarioProjeto
                    .Where(up => up.UsuarioId == userId)
                    .Include(up => up.Projeto)
                    .Select(up => up.Projeto)
                    .ToListAsync();

                // Adiciona a opção "Sem Projeto"
                projetos.Insert(0, new DiarioDeEspecime.Models.Projeto { Id = 0, Nome = "Sem Projeto" });

                ViewData["ProjetoId"] = new SelectList(projetos, "Id", "Nome");
                ViewBag.ProjetoFixo = false;
            }

            var especime = new DiarioDeEspecime.Models.Especime
            {
                EspecieId = especieId.Value,
                ProjetoId = projetoId
            };

            ViewBag.EspecieNome = especie?.NomeCientifico;

            return View(especime);
        }



        // POST: Especime/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        public async Task<IActionResult> CreatePost(Especime especime)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            especime.CriadorId = userId;
            especime.DataCadastro = DateTime.Now;

            if (especime.ProjetoId == 0)
                especime.ProjetoId = null;

            if (ModelState.IsValid)
            {
                _context.Add(especime);
                await _context.SaveChangesAsync();
                if (especime.ProjetoId.HasValue)
                    return RedirectToAction(nameof(Index), new { projetoId = especime.ProjetoId });
                return RedirectToAction(nameof(Index));
            }

            // Se houver erro, recarrega selects
            ViewData["EspecieId"] = new SelectList(_context.Especies, "Id", "NomeCientifico", especime.EspecieId);

            if (especime.ProjetoId.HasValue)
            {
                ViewData["ProjetoId"] = new SelectList(_context.Projetos.Where(p => p.Id == especime.ProjetoId.Value), "Id", "Nome", especime.ProjetoId);
                ViewBag.ProjetoFixo = true;
            }
            else
            {
                var projetos = await _context.UsuarioProjeto
                    .Where(up => up.UsuarioId == userId)
                    .Select(up => up.Projeto)
                    .ToListAsync();
                ViewData["ProjetoId"] = new SelectList(projetos, "Id", "Nome");
                ViewBag.ProjetoFixo = false;
            }

            return View(especime);
        }


        // GET: Especime/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var especime = await _context.Especimes.FindAsync(id);

            if (especime == null || especime.CriadorId != userId)
                return NotFound();

            ViewData["EspecieId"] = new SelectList(_context.Especies, "Id", "NomeCientifico", especime.EspecieId);

            if (especime.ProjetoId.HasValue)
            {
                ViewData["ProjetoId"] = new SelectList(_context.Projetos.Where(p => p.Id == especime.ProjetoId.Value), "Id", "Nome", especime.ProjetoId);
                ViewBag.ProjetoFixo = true;
            }
            else
            {
                var projetos = await _context.UsuarioProjeto
                    .Where(up => up.UsuarioId == userId)
                    .Select(up => up.Projeto)
                    .ToListAsync();
                ViewData["ProjetoId"] = new SelectList(projetos, "Id", "Nome");
                ViewBag.ProjetoFixo = false;
            }

            return View(especime);
        }

        // POST: Especime/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Especime especime)
        {
            if (id != especime.Id) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var especimeDb = await _context.Especimes.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id && e.CriadorId == userId);

            if (especimeDb == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    especime.CriadorId = userId;
                    especime.DataCadastro = especimeDb.DataCadastro; // preserve original
                    _context.Update(especime);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Especimes.Any(e => e.Id == especime.Id && e.CriadorId == userId))
                        return NotFound();
                    throw;
                }
                if (especime.ProjetoId.HasValue)
                    return RedirectToAction(nameof(Index), new { projetoId = especime.ProjetoId });
                return RedirectToAction(nameof(Index));
            }

            ViewData["EspecieId"] = new SelectList(_context.Especies, "Id", "NomeCientifico", especime.EspecieId);

            if (especime.ProjetoId.HasValue)
            {
                ViewData["ProjetoId"] = new SelectList(_context.Projetos.Where(p => p.Id == especime.ProjetoId.Value), "Id", "Nome", especime.ProjetoId);
                ViewBag.ProjetoFixo = true;
            }
            else
            {
                var projetos = await _context.UsuarioProjeto
                    .Where(up => up.UsuarioId == userId)
                    .Select(up => up.Projeto)
                    .ToListAsync();
                ViewData["ProjetoId"] = new SelectList(projetos, "Id", "Nome");
                ViewBag.ProjetoFixo = false;
            }

            return View(especime);
        }

        // GET: Especime/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var especime = await _context.Especimes
                .Include(e => e.Projeto)
                .Include(e => e.Especie)
                .FirstOrDefaultAsync(m => m.Id == id && m.CriadorId == userId);

            if (especime == null)
                return NotFound();

            return View(especime);
        }

        // POST: Especime/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var especime = await _context.Especimes.FirstOrDefaultAsync(e => e.Id == id && e.CriadorId == userId);

            if (especime == null)
                return NotFound();

            _context.Especimes.Remove(especime);
            await _context.SaveChangesAsync();

            if (especime.ProjetoId.HasValue)
                return RedirectToAction(nameof(Index), new { projetoId = especime.ProjetoId });
            return RedirectToAction(nameof(Index));
        }

        // GET: Especime/SelectEspecie
        public async Task<IActionResult> SelectEspecie(int? projetoId)
        {
            var especies = await _context.Especies.ToListAsync();
            ViewBag.ProjetoId = projetoId;
            return View(especies);
        }

        // POST: Especime/SelectEspecie
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SelectEspecie(int especieId, int? projetoId)
        {
            if (projetoId.HasValue)
                return RedirectToAction("Create", new { especieId, projetoId });
            return RedirectToAction("Create", new { especieId });
        }

    }
}
