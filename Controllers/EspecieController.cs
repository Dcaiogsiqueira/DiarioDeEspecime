using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DiarioDeEspecime.Models;
using DiarioDeEspecime.Data;


namespace DiarioDeEspecime.Controllers
{
    public class EspecieController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EspecieController(ApplicationDbContext context)
        {
            _context = context;
        }

  
        public async Task<IActionResult> Index(string search, string ordem, string reino, string filo, string classe, string familia, string genero)
        {
            var especies = _context.Especies.AsQueryable();

            // Busca por nome científico ou popular
            if (!string.IsNullOrWhiteSpace(search))
            {
                especies = especies.Where(e =>
                    e.NomeCientifico.Contains(search) ||
                    e.NomeEspecie.Contains(search) ||
                    e.Genero.Contains(search) ||
                    e.Familia.Contains(search));
            }

            // Filtros taxonômicos
            if (!string.IsNullOrWhiteSpace(reino))
                especies = especies.Where(e => e.Reino == reino);
            if (!string.IsNullOrWhiteSpace(filo))
                especies = especies.Where(e => e.Filo == filo);
            if (!string.IsNullOrWhiteSpace(classe))
                especies = especies.Where(e => e.Classe == classe);
            if (!string.IsNullOrWhiteSpace(familia))
                especies = especies.Where(e => e.Familia == familia);
            if (!string.IsNullOrWhiteSpace(genero))
                especies = especies.Where(e => e.Genero == genero);

            // Ordenação alfabética
            especies = especies.OrderBy(e => e.NomeCientifico);

            // Carregar listas distintas para filtros (para dropdowns na view)
            ViewBag.Reinos = await _context.Especies.Select(e => e.Reino).Distinct().OrderBy(x => x).ToListAsync();
            ViewBag.Filos = await _context.Especies.Select(e => e.Filo).Distinct().OrderBy(x => x).ToListAsync();
            ViewBag.Classes = await _context.Especies.Select(e => e.Classe).Distinct().OrderBy(x => x).ToListAsync();
            ViewBag.Familias = await _context.Especies.Select(e => e.Familia).Distinct().OrderBy(x => x).ToListAsync();
            ViewBag.Generos = await _context.Especies.Select(e => e.Genero).Distinct().OrderBy(x => x).ToListAsync();

            return View(await especies.ToListAsync());
        }

        // GET: Especie/Create
        public IActionResult Create(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Especie/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Especie especie, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                especie.CriadorId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                _context.Add(especie);
                await _context.SaveChangesAsync();
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    // Redireciona para a URL de retorno com a nova espécie selecionada
                    return RedirectToAction("SelectEspecie", "Especime", new { search = especie.NomeCientifico });
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(especie);
        }

        // GET: Especie/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var especie = await _context.Especies.FindAsync(id);
            if (especie == null)
                return NotFound();

            return View(especie);
        }

        // POST: Especie/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Especie especie)
        {
            if (id != especie.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(especie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EspecieExists(especie.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(especie);
        }

        // GET: Especie/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var especie = await _context.Especies
                .Include(e => e.Criador)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (especie == null)
                return NotFound();

            return View(especie);
        }

        // GET: Especie/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var especie = await _context.Especies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (especie == null)
                return NotFound();

            return View(especie);
        }

        // POST: Especie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var especie = await _context.Especies.FindAsync(id);
            if (especie != null)
            {
                _context.Especies.Remove(especie);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EspecieExists(int id)
        {
            return _context.Especies.Any(e => e.Id == id);
        }
    }
}
