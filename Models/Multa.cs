using Biblioteca.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Biblioteca.Models
{
    public class Multa
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo 'InicioMulta' é obrigatório.")]
        [DataType(DataType.Date, ErrorMessage = "O campo 'InicioMulta' deve ser uma data válida.")]
        public DateTime InicioMulta { get; set; }

        public DateTime? TerminoMulta { get; set; }

        [Required(ErrorMessage = "O campo 'DiasAtrasados' é obrigatório.")]
        public int DiasAtrasados { get; set; }

        [Required(ErrorMessage = "O campo 'Valor' é obrigatório.")]
        public float Valor { get; set; }

        [Required(ErrorMessage = "O campo 'Status' é obrigatório.")]

        public StatusMulta StatusMulta { get; set; } // 0 = Pendente, 1 = Paga

        // Chave Estrangeira
        [Required(ErrorMessage = "O campo 'EmprestimoId' é obrigatório.")]
        public int EmprestimoId { get; set; }

        public Emprestimo Emprestimo { get; set; }


    }
}
