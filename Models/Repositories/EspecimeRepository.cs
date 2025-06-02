using DiarioDeEspecime.Models.Repositories.Interfaces;
using DiarioDeEspecime.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DiarioDeEspecime.Models.Repositories
{
    public class EspecimeRepository : IEspecimeRepository
    {
        private readonly ApplicationDbContext _context;

        public EspecimeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(Especime especime)
        {
            _context.Especimes.Add(especime);
            _context.SaveChanges();
        }

        public IEnumerable<Especime> GetAll()
        {
            return _context.Especimes
                .AsNoTracking()
                .Include(e => e.Especie)
                .ToList();
        }

        public Especime GetById(int id)
        {
            return _context.Especimes
                .AsNoTracking()
                .Include(e => e.Especie)
                .FirstOrDefault(e => e.Id == id);
        }

        public void Update(Especime especime)
        {
            _context.Especimes.Update(especime);
            _context.SaveChanges();
        }

        public void Remove(int id)
        {
            var especime = _context.Especimes.Find(id);
            if (especime != null)
            {
                _context.Especimes.Remove(especime);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Especime> GetPaged(int page, int pageSize)
        {
            return _context.Especimes
                .AsNoTracking()
                .Include(e => e.Especie)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
    }
}
