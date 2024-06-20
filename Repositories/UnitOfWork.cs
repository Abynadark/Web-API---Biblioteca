using Biblioteca.Interfaces;
using Biblioteca.Models;

namespace Biblioteca.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private IEmprestimoRepository? _emprestimoRepo;
        private IExemplarRepository? _exemplarRepo;
        private ILivroRepository? _livroRepo;
        private IMultaRepository? _multaRepo;
        private IRenovacaoRepository? _renovacaoRepo;
        private IUsuarioRepository? _usuarioRepo;
 

        private Context _context;
        

        public UnitOfWork(Context context)
        {
            _context = context;
        }

        // Repositórios individuais

        public IEmprestimoRepository emprestimoRepository
        {
            get
            {
                //if (_emprestimoRepo == null)
                //{
                //    _emprestimoRepo = new EmprestimoRepository(_context);
                //}
                //return _emprestimoRepo;
                return _emprestimoRepo = _emprestimoRepo ?? new EmprestimoRepository(_context);
            }
        }


        public IExemplarRepository exemplarRepository
        {
            get
            {
                return _exemplarRepo = _exemplarRepo ?? new ExemplarRepository(_context);
            }
        }



        public ILivroRepository livroRepository
        {
            get
            {
                return _livroRepo = _livroRepo ?? new LivroRepository(_context);
            }
        }


        public IMultaRepository multaRepository
        {
            get
            {
                return _multaRepo = _multaRepo ?? new MultaRepository(_context);
            }
        }


        public IRenovacaoRepository renovacaoRepository
        {
            get
            {
                return _renovacaoRepo = _renovacaoRepo ?? new RenovacaoRepository(_context);
            }
        } 
        

        public IUsuarioRepository usuarioRepository
        {
            get
            {
                if (_usuarioRepo == null)
                {
                    _usuarioRepo = new UsuarioRepository(_context);
                }
                return _usuarioRepo;
            }
        }

 

        // Método para salvar as alterações no banco de dados de forma assíncrona

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }


        // Método para liberar os recursos do contexto quando não for mais necessário

        public void Dispose()
        {
            _context.Dispose();
        }

        //É necessário ter metodos Dispose() assincronos ?
    }
}
