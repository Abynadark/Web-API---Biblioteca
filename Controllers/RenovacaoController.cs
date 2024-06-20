using AutoMapper;
using Biblioteca.DTOs.Request;
using Biblioteca.DTOs.Response;
using Biblioteca.Interfaces;
using Biblioteca.Models;
using Microsoft.AspNetCore.Mvc;
using Biblioteca.Enum;

namespace Biblioteca.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RenovacoesController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;

        public RenovacoesController(IUnitOfWork uof, IMapper mapper)
        {
            _uof = uof;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RenovacaoResponseDTO>>> GetRenovacoes()
        {
            var renovacoes = await _uof.renovacaoRepository.GetRenovacoes();
            
            var renovacoesResponseDto = _mapper.Map<IEnumerable<RenovacaoResponseDTO>>(renovacoes);
            
            return Ok(renovacoesResponseDto);
        }

        [HttpGet("{id}", Name = "GetRenovacaoPorId")]
        public async Task<ActionResult<RenovacaoResponseDTO>> GetRenovacaoPorId(int id)
        {
            var renovacao = await _uof.renovacaoRepository.GetRenovacaoPorId(id);
            if (renovacao == null)
            {
                return NotFound($"Renovação com ID {id} não encontrada");
            }
            var renovacaoResponseDto = _mapper.Map<RenovacaoResponseDTO>(renovacao);
            
            return Ok(renovacaoResponseDto);
        }
        
        [HttpGet("consultar-por-cpf-do-usuario-e-id-do-emprestimo")]
        public async Task<ActionResult<IEnumerable<RenovacaoResponseDTO>>> GetRenovacoesPorCpfEIdEmprestimo([FromQuery] string? cpf, [FromQuery] int? idEmprestimo)
        {
            var renovacoes = await _uof.renovacaoRepository.GetConsultarRenovacoes(cpf, idEmprestimo);
            var renovacoesResponseDto = _mapper.Map<IEnumerable<RenovacaoResponseDTO>>(renovacoes);
            return Ok(renovacoesResponseDto);
        }

        [HttpPost]
        public async Task<ActionResult<RenovacaoResponseDTO>> CreateRenovacao(RenovacaoRequestDTO renovacaoRequestDto)
        {
            if (renovacaoRequestDto is null)
            {
                BadRequest("Dados Inválidos");
            }

            var emprestimo = await _uof.emprestimoRepository.GetEmprestimoPorId(renovacaoRequestDto.EmprestimoId);
            
            if (emprestimo == null)
            {
                return NotFound($"Empréstimo de ID {renovacaoRequestDto.EmprestimoId} não encontrado");
            }
            //posso retirar, mas vou deixar para ter um fedeeback mais específico
            //Verificações no empréstimo específico para ver se ele possui algum emprestimo atrasado ou com renovação atrasada
            if (emprestimo.StatusEmprestimo == StatusEmprestimo.Devolvido)
            {
                return BadRequest($"Emprestimo de Id {renovacaoRequestDto.EmprestimoId} foi encerrado!");
            } 
            if (emprestimo.StatusEmprestimo == StatusEmprestimo.Atrasada || emprestimo.StatusEmprestimo == StatusEmprestimo.RenovacaoAtrasada)
            {
                return BadRequest($"Emprestimo de Id {renovacaoRequestDto.EmprestimoId} não pode ser renovado pois está atrasado!");
            }
            
            //Verificações de modo geral para ver se possui algum emprestimo atrasado ou com renovação atrasada
            var emprestimosAtrasado = await _uof.emprestimoRepository.ConsultarEmprestimos(emprestimo.Usuario.CPF, null, StatusEmprestimo.Atrasada, null);

            if (emprestimosAtrasado.Any())
            {
                return BadRequest($"O usuario de CPF {emprestimo.Usuario.CPF} possiu emprestimo atrasado!");
            }

            var emprestimosRenovacaoAtrasada = await _uof.emprestimoRepository.ConsultarEmprestimos(emprestimo.Usuario.CPF, null, StatusEmprestimo.RenovacaoAtrasada, null);

            if (emprestimosRenovacaoAtrasada.Any())
            {
                return BadRequest($"O usuario de CPF {emprestimo.Usuario.CPF} possiu emprestimo com renovação atrasada!");
            }

            var renovacoesEmprestimo = await _uof.renovacaoRepository.GetConsultarRenovacoes(null, emprestimo.Id);

            if (renovacoesEmprestimo != null && renovacoesEmprestimo.Count() >= 3)
            {
                return BadRequest($"Emprestimo de Id {emprestimo.Id} já atingiu o limite de renovações!");
            }


            var renovacao = _mapper.Map<Renovacao>(renovacaoRequestDto);
            renovacao.Emprestimo = emprestimo;

            var novaRenovacao = await _uof.renovacaoRepository.CreateRenovacao(renovacao);
            await _uof.CommitAsync();

            //atualiza status de empréstimo para renovado, caso ainda não tenha esse status
            if (emprestimo.StatusEmprestimo != StatusEmprestimo.Renovacao)
            {
                emprestimo.StatusEmprestimo = StatusEmprestimo.Renovacao;
                var emprestimoAtualizado = await _uof.emprestimoRepository.UpdateEmprestimo(emprestimo);
                await _uof.CommitAsync();
            }
          

            var renovacaoResponseDto = _mapper.Map<RenovacaoResponseDTO>(novaRenovacao);
            
            return CreatedAtRoute("GetRenovacaoPorId", new { id = renovacaoResponseDto.Id }, renovacaoResponseDto);
        }
        
        //[HttpPut("{id}")]
        //public async Task<ActionResult<RenovacaoResponseDTO>> UpdateRenovacao(int id, RenovacaoRequestDTO renovacaoRequestDto)
        //{
           

        //    var renovacaoExistente = await _uof.renovacaoRepository.GetRenovacaoPorId(id);
        //    if (renovacaoExistente == null)
        //    {
        //        return NotFound($"Renovação de ID {id} não encontrada.");
        //    }

        //    var emprestimo = await _uof.emprestimoRepository.GetEmprestimoPorId(renovacaoRequestDto.EmprestimoId);
        //    if (emprestimo == null)
        //    {
        //        return NotFound($"Empréstimo com ID {renovacaoRequestDto.EmprestimoId} não encontrado");
        //    }

        //    var renovacoesEmprestimo = await _uof.renovacaoRepository.GetConsultarRenovacoes(null, emprestimo.Id);

        //    if (renovacoesEmprestimo != null && renovacoesEmprestimo.Count() >= 3)
        //    {
        //        return BadRequest($"Emprestimo de Id {emprestimo.Id} já atingiu o limite de renovações!");
        //    }

        //    _mapper.Map(renovacaoRequestDto, renovacaoExistente);
        //    renovacaoExistente.Emprestimo = emprestimo;

        //    try
        //    {
        //        var renovacaoAtualizada = await _uof.renovacaoRepository.UpdateRenovacao(renovacaoExistente);
        //        await _uof.CommitAsync();

        //        emprestimo.StatusEmprestimo = StatusEmprestimo.Renovacao;
        //        await _uof.emprestimoRepository.UpdateEmprestimo(emprestimo);
        //        await _uof.CommitAsync();

        //        var renovacaoAtualizadaResponseDto = _mapper.Map<RenovacaoResponseDTO>(renovacaoAtualizada);

        //        return Ok(renovacaoAtualizadaResponseDto);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Erro ao atualizar renovação: {ex.Message}");
        //    }
        //}


        [HttpDelete("{id}")]
        public async Task<ActionResult<RenovacaoResponseDTO>> DeleteRenovacao(int id)
        {
            var renovacao = await _uof.renovacaoRepository.GetRenovacaoPorId(id);
            
            if (renovacao == null)
            {
                return NotFound($"Renovação com ID {id} não encontrada");
            }

            var renovacaoExcluida = await _uof.renovacaoRepository.DeleteRenovacao(id);
            await _uof.CommitAsync();

            var emprestimo = await _uof.emprestimoRepository.GetEmprestimoPorId(renovacao.EmprestimoId);

            if (emprestimo.Renovacoes.Count() > 0)
            {
                emprestimo.StatusEmprestimo = StatusEmprestimo.Renovacao;
                await _uof.emprestimoRepository.UpdateEmprestimo(emprestimo);
                await _uof.CommitAsync();

            }
            else
            {
                emprestimo.StatusEmprestimo = StatusEmprestimo.NaoDevolvido;
                await _uof.emprestimoRepository.UpdateEmprestimo(emprestimo);
                await _uof.CommitAsync();
            }

            var renovacaoExcluidaResponseDto = _mapper.Map<RenovacaoResponseDTO>(renovacaoExcluida);
            
            return Ok(renovacaoExcluidaResponseDto);
        }
    }
}
