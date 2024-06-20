using Biblioteca.Enum;
using Biblioteca.Interfaces;
using Biblioteca.Models;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Repositories
{
    public class ExemplarRepository : IExemplarRepository
    {
        private readonly Context _context;

        public ExemplarRepository(Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Exemplar>> GetExemplares()
        {
            return await _context.Exemplares.Include(e => e.Livro).ToListAsync();
        }

        public async Task<IEnumerable<Exemplar>> GetExemplaresISBN10eStatus(string? ISBN10, StatusExemplar? statusExemplar)
        {
            IQueryable<Exemplar> query = _context.Exemplares;

            if (!string.IsNullOrEmpty(ISBN10))
            {
                query = query.Where(e => e.LivroISBN10 == ISBN10);
            }

            if (statusExemplar.HasValue)
            {
                query = query.Where(e => e.StatusExemplar == statusExemplar);
            }

            return await query.ToListAsync();
        }

        public async Task<Exemplar> GetExemplarPorId(int id)
        {
            return await _context.Exemplares.Include(e => e.Livro).FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Exemplar>> GetExemplaresPorISBN10(string isbn10)
        {
            return await _context.Exemplares.Where(e => e.Livro.ISBN10 == isbn10).ToListAsync();
        }

        public async Task<Exemplar> CreateExemplar(Exemplar exemplar)
        {
            if (exemplar == null)
                throw new ArgumentNullException(nameof(exemplar));

            exemplar.StatusExemplar = StatusExemplar.Disponivel;
            _context.Exemplares.Add(exemplar);
            
            return exemplar;
        }

        public async Task<Exemplar> UpdateExemplar(Exemplar exemplar)
        {
            if (exemplar == null)
                throw new ArgumentNullException(nameof(exemplar));

            _context.Entry(exemplar).State = EntityState.Modified;
            
            return exemplar;
        }

        public async Task<Exemplar> DeleteExemplar(int id)
        {
            var exemplar = await _context.Exemplares.FindAsync(id);
            if (exemplar == null)
                throw new ArgumentNullException(nameof(exemplar));

            _context.Exemplares.Remove(exemplar);
          
            return exemplar;
        }
    }
}
