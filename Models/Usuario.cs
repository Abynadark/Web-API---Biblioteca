using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Biblioteca.Enum;
using System.Text.Json.Serialization;
using System.Collections.ObjectModel;

namespace Biblioteca.Models
{
    public class Usuario
    {
        public Usuario()
        {
            Emprestimos = new Collection<Emprestimo>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Define o Id como auto incrementado
        public int Id { get; set; }

   
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [RegularExpression(@"\d{11}", ErrorMessage = "O CPF deve conter exatamente 11 dígitos.")]
        public string CPF { get; set; }

        [DataType(DataType.Date)]
        public DateTime DataNasc { get; set; }

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O email deve ser válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O celular é obrigatório.")]
        //[RegularExpression(@"\(?\d{2}\)?[\s-]?\d{4,5}-?\d{4}", ErrorMessage = "O número de celular deve ser válido.")]
        [RegularExpression(@"^\([1-9]{2}\) [9]{1}[0-9]{4}-[0-9]{4}$", ErrorMessage = "O celular deve estar no formato (XX) 9XXXX-XXXX.")]
        public string Celular { get; set; }

        [Required(ErrorMessage = "O endereço é obrigatório.")]
        public string Endereco { get; set; }

        //[JsonIgnore]
        public StatusUsuario StatusUsuario { get; set; }

        [JsonIgnore]
        public ICollection<Emprestimo> Emprestimos { get; set; }
    }
}
