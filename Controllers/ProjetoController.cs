using DiarioDeEspecime.Data;
using DiarioDeEspecime.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using DiarioDeEspecime.Models.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace DiarioDeEspecime.Controllers
{
    public class ProjetoController : Controller
    {
        private readonly IProjetoRepository _projetoRepository;
        private readonly UserManager<Usuario> _userManager;
        private readonly ApplicationDbContext _context;

        public ProjetoController(IProjetoRepository projetoRepository, UserManager<Usuario> userManager, ApplicationDbContext context)
        {
            _context = context;
            _projetoRepository = projetoRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string search, string creatorId, string sortOrder)
        {
            var userId = _userManager.GetUserId(User);

            // Projetos onde o usuário é criador
            var projetosCriados = await _projetoRepository.GetByCreatorAsync(userId);

            // Projetos onde o usuário é participante
            var projetosParticipa = await _projetoRepository.GetByParticipantAsync(userId);

            // Unir e remover duplicatas
            var projetos = projetosCriados.Concat(projetosParticipa).Distinct().AsQueryable();

            // Filtro por nome (case-insensitive)
            if (!string.IsNullOrEmpty(search))
            {
                projetos = projetos.Where(p => p.Nome != null && p.Nome.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            // Filtro por criador
            if (!string.IsNullOrEmpty(creatorId))
            {
                projetos = projetos.Where(p => p.CriadorId == creatorId);
            }

            // Ordenação
            switch (sortOrder)
            {
                case "inicio_desc":
                    projetos = projetos.OrderByDescending(p => p.DataInicio);
                    break;
                case "inicio_asc":
                    projetos = projetos.OrderBy(p => p.DataInicio);
                    break;
                case "termino_desc":
                    projetos = projetos.OrderByDescending(p => p.DataFim);
                    break;
                case "termino_asc":
                    projetos = projetos.OrderBy(p => p.DataFim);
                    break;
                default:
                    projetos = projetos.OrderByDescending(p => p.DataInicio);
                    break;
            }

            // Para popular o filtro de criadores
            var criadores = projetos.Select(p => p.Criador).Distinct().ToList();

            ViewBag.Creators = criadores;
            ViewBag.CurrentFilter = search;
            ViewBag.CurrentCreator = creatorId;
            ViewBag.CurrentSort = sortOrder;

            return View(projetos.ToList());
        }

        // GET: Projeto/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projeto/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Projeto projeto)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Challenge();
            }

            var userId = _userManager.GetUserId(User);
            projeto.CriadorId = userId;

            ModelState.Remove(nameof(projeto.CriadorId));

            if (ModelState.IsValid)
            {
                await _projetoRepository.AddAsync(projeto);
                return RedirectToAction(nameof(Index));
            }
            return View(projeto);
        }

        // GET: Projeto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            // Buscar o projeto pelo ID
            var projeto = await _projetoRepository.GetByIdAsync(id.Value);
            if (projeto == null)
                return NotFound();

            // Verificar se o usuário logado é o criador do projeto
            var userId = _userManager.GetUserId(User);
            if (projeto.CriadorId != userId)
                return Forbid(); // Usuário não tem permissão para editar

            return View(projeto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Projeto projeto)
        {
            if (id != projeto.Id)
                return NotFound();

            var userId = _userManager.GetUserId(User);
            var projetoExistente = await _projetoRepository.GetByIdAsync(id);
            if (projetoExistente == null || projetoExistente.CriadorId != userId)
                return Forbid();

            // Garante que o CriadorId não seja alterado
            projeto.CriadorId = projetoExistente.CriadorId;
            ModelState.Remove(nameof(projeto.CriadorId)); // <-- Adicione esta linha

            if (ModelState.IsValid)
            {
                try
                {
                    await _projetoRepository.UpdateAsync(projeto);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProjetoExists(projeto.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(projeto);
        }



        private async Task<bool> ProjetoExists(int id)
        {
            var projeto = await _projetoRepository.GetByIdAsync(id);
            return projeto != null;
        }


        public async Task<IActionResult> Details(int id)
        {
            // Obter o ID do usuário logado
            var userId = _userManager.GetUserId(User);

            // Buscar o projeto pelo ID
            var projeto = await _projetoRepository.GetByIdAsync(id);

            if (projeto == null)
            {
                return NotFound();
            }

            // Verificar se o usuário é o criador do projeto
            if (projeto.CriadorId == userId)
            {
                ViewBag.RegraUsuario = RegraProjeto.Criador;
            }
            else
            {
                // Verificar a regra do usuário na tabela UsuarioProjeto
                var usuarioProjeto = await _context.UsuarioProjeto
                    .FirstOrDefaultAsync(up => up.ProjetoId == id && up.UsuarioId == userId);

                if (usuarioProjeto == null)
                {
                    return Forbid(); // Usuário não tem acesso ao projeto
                }

                ViewBag.RegraUsuario = usuarioProjeto.Regra;
            }

            return View(projeto);
        }

        // GET: Projeto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var projeto = await _context.Projetos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projeto == null)
                return NotFound();

            return View(projeto);
        }

        // POST: Projeto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projeto = await _context.Projetos.FindAsync(id);
            if (projeto != null)
            {
                _context.Projetos.Remove(projeto);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
