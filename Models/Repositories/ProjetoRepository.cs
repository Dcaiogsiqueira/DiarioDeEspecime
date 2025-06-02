using DiarioDeEspecime.Data;
using DiarioDeEspecime.Models.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DiarioDeEspecime.Models.Repositories
{
    public class ProjetoRepository : IProjetoRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjetoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Projeto>> GetAllAsync()
        {
            return await _context.Projetos
                .Include(p => p.Criador) // Inclui o criador do projeto
                .ToListAsync();
        }

        public async Task<Projeto> GetByIdAsync(int id)
        {
            return await _context.Projetos
                .Include(p => p.Criador) // Inclui o criador do projeto
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Projeto projeto)
        {
            await _context.Projetos.AddAsync(projeto);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Projeto projeto)
        {
            var existingProjeto = await _context.Projetos.FindAsync(projeto.Id);
            if (existingProjeto != null)
            {
                _context.Entry(existingProjeto).CurrentValues.SetValues(projeto);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var projeto = await _context.Projetos.FindAsync(id);
            if (projeto != null)
            {
                _context.Projetos.Remove(projeto);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Projeto>> GetByCreatorAsync(string userId)
        {
            return await _context.Projetos
                .Include(p => p.Criador)
                .Where(p => p.CriadorId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Projeto>> GetByParticipantAsync(string userId)
        {
            return await _context.UsuarioProjeto
                .Where(up => up.UsuarioId == userId)
                .Include(up => up.Projeto) // Inclui o projeto completo
                .ThenInclude(p => p.Criador) // Inclui o criador do projeto
                .Select(up => up.Projeto) // Projeta apenas os projetos
                .ToListAsync();
        }
    }
}
