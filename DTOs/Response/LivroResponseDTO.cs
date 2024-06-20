using Biblioteca.Enum;

namespace Biblioteca.DTOs.Response
{
        public class LivroResponseDTO
        {
            public int Id { get; set; }
            public string Nome { get; set; }
            public string Autor { get; set; }
            public string ISBN10 { get; set; }
            public string? ISBN13 { get; set; }
            public string Editora { get; set; }
            public string Edicao { get; set; }
            public string Idioma { get; set; }
            public string Genero { get; set; }
            public DateTime DataPublicacao { get; set; }
            public string Classificacao { get; set; }
            public int Qnt_Pagina { get; set; }
            public float Valor { get; set; }
            public StatusLivro StatusLivro { get; set; }
   
        }
}
