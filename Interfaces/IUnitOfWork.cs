namespace Biblioteca.Interfaces
{
    public interface IUnitOfWork
    {
        IEmprestimoRepository emprestimoRepository { get; }
        IExemplarRepository exemplarRepository { get; }
        ILivroRepository livroRepository { get; }
        IMultaRepository multaRepository { get; }
        IRenovacaoRepository renovacaoRepository { get; }
        IUsuarioRepository usuarioRepository { get; }
        
        Task CommitAsync();

    }
}