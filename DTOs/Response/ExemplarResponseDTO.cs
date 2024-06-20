using Biblioteca.Enum;

namespace Biblioteca.DTOs.Response
{
    public class ExemplarResponseDTO
    {
        public int Id { get; set; }
        public StatusExemplar StatusExemplar { get; set; }
        public string LivroISBN10 { get; set; }
        public LivroResponseDTO Livro { get; set; } // tire essa linha caso não queira que mostre os detralhes do livro
    }
}
