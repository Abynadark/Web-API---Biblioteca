using Biblioteca.Enum;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca.Models
{
    public class Emprestimo
    {
       
        public Emprestimo()
        {
            Renovacoes = new Collection<Renovacao>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "A Data de Emprestimo é obrigatório.")]
        [DataType(DataType.Date)]
        public DateTime DataEmprestimo { get; set; }

        [DataType(DataType.Date)]
        public DateTime DataPrevistaInicial { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataDevolucao { get; set; }

        public StatusEmprestimo StatusEmprestimo { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public int ExemplarId { get; set; }
        public Exemplar Exemplar { get; set; }

        public ICollection<Renovacao> Renovacoes { get; set; }

        public Multa? Multa { get; set; } // Relacionamento um-para-um com Multa
    }
}
