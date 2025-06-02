using DiarioDeEspecime.Models;
using DiarioDeEspecime.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DiarioDeEspecime.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        // GET: /Usuario
        public IActionResult Index()
        {
            var usuarios = _usuarioRepository.GetAll();
            return View(usuarios);
        }

        // GET: /Usuario/Details/5
        public IActionResult Details(string id)
        {
            var usuario = _usuarioRepository.GetById(id);
            if (usuario == null)
                return NotFound();
            return View(usuario);
        }

        // GET: /Usuario/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Usuario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.Celular = usuario.Celular?.Replace(" ", string.Empty);

                try
                {
                    _usuarioRepository.Add(usuario);
                    _usuarioRepository.Save();

                    // Log the user in after creation
                    var signInManager = HttpContext.RequestServices.GetService<SignInManager<Usuario>>();
                    if (signInManager != null)
                    {
                        await signInManager.SignInAsync(usuario, isPersistent: false);
                    }

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(usuario);
        }

        // GET: /Usuario/Edit/5
        public IActionResult Edit(string id)
        {
            var usuario = _usuarioRepository.GetById(id);
            if (usuario == null)
                return NotFound();
            return View(usuario);
        }

        // POST: /Usuario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Usuario usuario)
        {
            if (id != usuario.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _usuarioRepository.Update(usuario);
                    _usuarioRepository.Save();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(usuario);
        }

        // GET: /Usuario/Delete/5
        public IActionResult Delete(string id)
        {
            var usuario = _usuarioRepository.GetById(id);
            if (usuario == null)
                return NotFound();
            return View(usuario);
        }

        // POST: /Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            try
            {
                _usuarioRepository.Delete(id);
                _usuarioRepository.Save();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
    }
}
