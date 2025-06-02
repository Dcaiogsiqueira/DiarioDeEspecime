using DiarioDeEspecime.Data;
using DiarioDeEspecime.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DiarioDeEspecime.Models.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(UserManager<Usuario> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IEnumerable<Usuario> GetAll()
        {
            return _userManager.Users
                .Include(u => u.UsuariosProjeto)
                .Include(u => u.ProjetosCriados)
                .AsNoTracking()
                .ToList();
        }

        public Usuario GetById(string id)
        {
            return _userManager.Users
                .Include(u => u.UsuariosProjeto)
                .Include(u => u.ProjetosCriados)
                .FirstOrDefault(u => u.Id == id);
        }

        public void Add(Usuario usuario)
        {
            // Use UserManager to create the user
            var result = _userManager.CreateAsync(usuario, usuario.Senha).Result;
            if (!result.Succeeded)
            {
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
            }
        }

        public void Update(Usuario usuario)
        {
            // Use UserManager to update the user
            var result = _userManager.UpdateAsync(usuario).Result;
            if (!result.Succeeded)
            {
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
            }
        }

        public void Delete(string id)
        {
            var usuario = GetById(id);
            if (usuario != null)
            {
                var result = _userManager.DeleteAsync(usuario).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
                }
            }
        }

        public void Save()
        {
            // Save changes to the database
            _context.SaveChanges();
        }
    }
}
