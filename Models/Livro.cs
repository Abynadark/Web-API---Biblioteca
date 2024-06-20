using Biblioteca.Enum;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations; 
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca.Models
{
    public class Livro
    {
        public Livro()
        {
            Exemplares = new Collection<Exemplar>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo Autor é obrigatório.")]
        public string Autor { get; set; }

        [Required(ErrorMessage = "O campo ISBN-10 é obrigatório.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "ISBN-10 deve conter exatamente 10 dígitos e ser composto apenas por dígitos numéricos")]
        public string ISBN10 { get; set; }

        [RegularExpression(@"^\d{13}$|^$", ErrorMessage = "ISBN-13 deve conter exatamente 13 dígitos numéricos ou ser nulo.")]
        public string? ISBN13 { get; set; }

        [Required(ErrorMessage = "O campo Editora é obrigatório.")]
        public string Editora { get; set; }

        [Required(ErrorMessage = "O campo Edição é obrigatório.")]
        public string Edicao { get; set; }

        [Required(ErrorMessage = "O campo Idioma é obrigatório.")]
        public string Idioma { get; set; }

        [Required(ErrorMessage = "O campo Gênero é obrigatório.")]
        public string Genero { get; set; }

        [Required(ErrorMessage = "O campo Data de Publicação é obrigatório.")]
        [DataType(DataType.Date)]
        public DateTime DataPublicacao { get; set; }

        [Required(ErrorMessage = "O campo Classificação é obrigatório.")]
        public string Classificacao { get; set; }

        [Required(ErrorMessage = "O campo Quantidade de Páginas é obrigatório.")]
        public int Qnt_Pagina { get; set; }

        [Required(ErrorMessage = "O campo Valor é obrigatório.")]
        public float Valor { get; set; }

        public StatusLivro StatusLivro { get; set; } // 0 = Não há exemplares disponível para empréstimo, 1 = Há exemplares disponível para empréstimo


        public ICollection<Exemplar> Exemplares { get; set; }
    }
}
