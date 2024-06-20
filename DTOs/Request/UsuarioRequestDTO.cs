using Biblioteca.Enum;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace Biblioteca.DTOs.Request
{
    public class UsuarioRequestDTO
    {
        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [StringLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo CPF é obrigatório.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve conter exatamente 11 dígitos")]
        public string CPF { get; set; }


        [DataType(DataType.Date)]
        public DateTime DataNasc { get; set; }

        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O email fornecido não é válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo Celular é obrigatório.")]
        [RegularExpression(@"^\([1-9]{2}\) [9]{1}[0-9]{4}-[0-9]{4}$", ErrorMessage = "O celular deve estar no formato (XX) 9XXXX-XXXX.")]
        public string Celular { get; set; }

        [Required(ErrorMessage = "O campo Endereço é obrigatório.")]
        public string Endereco { get; set; }

        [JsonIgnore]
        public StatusExemplar StatusUsuario { get; set; } // ajuda a pessoa ter controle do status automaticamente

    }

}


