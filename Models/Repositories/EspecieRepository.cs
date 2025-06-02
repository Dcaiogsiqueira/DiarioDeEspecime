using DiarioDeEspecime.Models.Repositories.Interfaces;
using DiarioDeEspecime.Data;
using Microsoft.EntityFrameworkCore;

namespace DiarioDeEspecime.Models.Repositories
{
    public class EspecieRepository : IEspecieRepository
    {
        private readonly ApplicationDbContext _context;

        public EspecieRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(Especie especie)
        {
            _context.Especies.Add(especie);
            _context.SaveChanges();
        }

        public IEnumerable<Especie> GetAll()
        {
            return _context.Especies
                .AsNoTracking()
                .ToList();
        }

        public Especie GetById(int id)
        {
            return _context.Especies
                .AsNoTracking()
                .FirstOrDefault(e => e.Id == id);
        }

        public void Update(Especie especie)
        {
            _context.Especies.Update(especie);
            _context.SaveChanges();
        }

        public void Remove(int id)
        {
            var especie = _context.Especies.Find(id);
            if (especie != null)
            {
                _context.Especies.Remove(especie);
                _context.SaveChanges();
            }
        }
    }
}