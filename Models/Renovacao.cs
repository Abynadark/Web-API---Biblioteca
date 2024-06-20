using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Models
{
    public class Renovacao
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //[JsonIgnore]
        [Required(ErrorMessage = "O campo Data de Renovação é obrigatório.")] // isso futuramente irá deixar de ser obrigatório e passará a ser gerado automaticamente pelo sistema
        [DataType(DataType.Date)]
        public DateTime DataRenovacao { get; set; }

        //[JsonIgnore]
        [DataType(DataType.Date)]
        public DateTime NovaDataPrevista { get; set; }

        // Chave Estrangeira
        [Required(ErrorMessage = "O campo ID de Emprestimo é obrigatório.")]
        public int EmprestimoId { get; set; }
        public Emprestimo Emprestimo { get; set; }

    }
}
