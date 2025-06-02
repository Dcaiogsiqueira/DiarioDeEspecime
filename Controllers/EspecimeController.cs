using DiarioDeEspecime.Data;
using DiarioDeEspecime.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiarioDeEspecime.Controllers
{
    public class EspecimeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EspecimeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Seleção de Espécie (primeiro passo)
        public async Task<IActionResult> SelectEspecie(string search)
        {
            var especies = _context.Especies.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                especies = especies.Where(e =>
                    e.NomeCientifico.Contains(search) ||
                    e.NomeEspecie.Contains(search) ||
                    e.Genero.Contains(search));
            }

            return View(await especies.ToListAsync());
        }

        // 2. Criação do Espécime (segundo passo)
        // GET: Especime/Create
        public async Task<IActionResult> Create(int? especieId)
        {
            if (especieId == null)
                return RedirectToAction(nameof(SelectEspecie));

            var especie = await _context.Especies.FindAsync(especieId);
            if (especie == null)
                return RedirectToAction(nameof(SelectEspecie));

            ViewBag.EspecieNome = especie.NomeCientifico;
            var especime = new Especime { EspecieId = especie.Id };
            return View(especime);
        }

        // POST: Especime/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Especime especime)
        {
            if (ModelState.IsValid)
            {
                _context.Add(especime);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Preenche o nome da espécie para exibir novamente em caso de erro
            var especie = await _context.Especies.FindAsync(especime.EspecieId);
            ViewBag.EspecieNome = especie?.NomeCientifico;
            return View(especime);
        }

        // 3. Listagem com busca, filtro e ordenação
        public async Task<IActionResult> Index(string search, string genero, string sortOrder)
        {
            var especimes = _context.Especimes
                .Include(e => e.Especie)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                especimes = especimes.Where(e =>
                    e.Especie.NomeCientifico.Contains(search) ||
                    e.Especie.NomeEspecie.Contains(search) ||
                    e.LocalEncontro.Contains(search) ||
                    e.Coletor.Contains(search) ||
                    e.Especie.Genero.Contains(search));
            }

            if (!string.IsNullOrEmpty(genero))
            {
                especimes = especimes.Where(e => e.Sexo == genero);
            }

            especimes = sortOrder switch
            {
                "asc" => especimes.OrderBy(e => e.Especie.NomeCientifico),
                "desc" => especimes.OrderByDescending(e => e.Especie.NomeCientifico),
                _ => especimes
            };

            return View(await especimes.ToListAsync());
        }

        // 4. Detalhes
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var especime = await _context.Especimes
                .Include(e => e.Especie)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (especime == null)
                return NotFound();

            return View(especime);
        }

        // 5. Edição
        // GET: Especime/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var especime = await _context.Especimes.FindAsync(id);
            if (especime == null)
                return NotFound();

            var especie = await _context.Especies.FindAsync(especime.EspecieId);
            ViewBag.EspecieNome = especie?.NomeCientifico;
            return View(especime);
        }

        // POST: Especime/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Especime especime)
        {
            if (id != especime.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(especime);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await EspecimeExists(especime.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var especie = await _context.Especies.FindAsync(especime.EspecieId);
            ViewBag.EspecieNome = especie?.NomeCientifico;
            return View(especime);
        }

        // 6. Exclusão
        // GET: Especime/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var especime = await _context.Especimes
                .Include(e => e.Especie)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (especime == null)
                return NotFound();

            return View(especime);
        }

        // POST: Especime/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var especime = await _context.Especimes.FindAsync(id);
            if (especime != null)
            {
                _context.Especimes.Remove(especime);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> EspecimeExists(int id)
        {
            return await _context.Especimes.AnyAsync(e => e.Id == id);
        }
    }
}
