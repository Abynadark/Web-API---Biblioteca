using Biblioteca.Enum;
using System;
using System.Collections.Generic;

namespace Biblioteca.DTOs.Response
{
    public class EmprestimoResponseDTO
    {
        public int Id { get; set; }
        public DateTime DataEmprestimo { get; set; }
        public DateTime DataPrevistaInicial { get; set; }
        public DateTime? DataDevolucao { get; set; }
        public StatusEmprestimo StatusEmprestimo { get; set; }
        public string UsuarioCPF { get; set; }
        public int ExemplarId { get; set; }

        public MultaResponseDTO Multa { get; set; } // Alteração aqui

        public ICollection<RenovacaoResponseDTO> Renovacoes { get; set; }
    }
}
