using Biblioteca.Enum;
using Biblioteca.Interfaces;
using Biblioteca.Models;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Repositories
{
    public class MultaRepository : IMultaRepository
    {
        private readonly Context _context;

        public MultaRepository(Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Multa>> GetMultas()
        {
            return await _context.Multa.ToListAsync();
        }

        public async Task<Multa> GetMultaPorId(int id)
        {
            return await _context.Multa.FindAsync(id);
        }

        public async Task<Multa> GetMultaPorIdEmprestimo(int idEmprestimo)
        {
            return await _context.Multa
                .FirstOrDefaultAsync(m => m.EmprestimoId == idEmprestimo);
        }

        public async Task<Multa> CreateMulta(Multa multa)
        {
            if (multa is null)
                throw new ArgumentNullException(nameof(multa));

            multa.StatusMulta = Enum.StatusMulta.Pendente;

            _context.Multa.Add(multa);
            //await _context.SaveChangesAsync();
            return multa;
        }

        public async Task<Multa> UpdateMulta(Multa multa)
        {
            if (multa is null)
                throw new ArgumentNullException(nameof(multa));

            _context.Entry(multa).State = EntityState.Modified;
            //await _context.SaveChangesAsync();
            return multa;
        }

        public async Task<Multa> DeleteMulta(int id)
        {
            var multa = await _context.Multa.FindAsync(id);
            if (multa == null)
                throw new ArgumentNullException(nameof(multa));

            _context.Multa.Remove(multa);
            //await _context.SaveChangesAsync();
            return multa;
        }

        public async Task<IEnumerable<Multa>> ConsultarMultas(string? usuarioCPF, int? emprestimoId, int? dias, StatusMulta? statusMulta)
        {
            IQueryable<Multa> query = _context.Multa.Include(m => m.Emprestimo).ThenInclude(e => e.Usuario);

            if (!string.IsNullOrEmpty(usuarioCPF))
            {
                query = query.Where(m => m.Emprestimo.Usuario.CPF == usuarioCPF);
            }

            if (emprestimoId.HasValue)
            {
                query = query.Where(m => m.EmprestimoId == emprestimoId.Value);
            }

            if (dias.HasValue)
            {
                var dataInicio = DateTime.Now.AddDays(-dias.Value);
                query = query.Where(m => m.InicioMulta >= dataInicio);
            }

            if(statusMulta.HasValue){

                query = query.Where(m => m.StatusMulta == statusMulta);
            }

            return await query.ToListAsync();
        }
    }
}
