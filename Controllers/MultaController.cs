using AutoMapper;
using Biblioteca.DTOs.Request;
using Biblioteca.DTOs.Response;
using Biblioteca.Enum;
using Biblioteca.Interfaces;
using Biblioteca.Models;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MultasController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;

        public MultasController(IUnitOfWork uof, IMapper mapper)
        {
            _uof = uof;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MultaResponseDTO>>> GetMultas()
        {
            var multas = await _uof.multaRepository.GetMultas();
            var multasResponseDTO = _mapper.Map<IEnumerable<MultaResponseDTO>>(multas);
            return Ok(multasResponseDTO);
        }

        [HttpGet("{id}", Name = "GetMultaPorId")]
        public async Task<ActionResult<MultaResponseDTO>> GetMultaPorId(int id)
        {
            var multa = await _uof.multaRepository.GetMultaPorId(id);
            if (multa == null)
            {
                return NotFound($"Multa com Id {id} não encontrada");
            }

            var multaResponseDTO = _mapper.Map<MultaResponseDTO>(multa);
            return Ok(multaResponseDTO);
        }

        [HttpGet("emprestimo/{idEmprestimo}", Name = "GetMultaPorIdEmprestimo")]
        public async Task<ActionResult<MultaResponseDTO>> GetMultaPorIdEmprestimo(int idEmprestimo)
        {
            var multa = await _uof.multaRepository.GetMultaPorIdEmprestimo(idEmprestimo);
            if (multa == null)
            {
                return NotFound($"Multa para o empréstimo com ID {idEmprestimo} não encontrada");
            }

            var multaResponseDTO = _mapper.Map<MultaResponseDTO>(multa);
            return Ok(multaResponseDTO);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<MultaResponseDTO>> UpdateMulta(int id, MultaRequestDTO multaRequestDto)
        {
            if(multaRequestDto is null)
            {
                return BadRequest("Dados Inválidos");
            }

            var multaExistente = await _uof.multaRepository.GetMultaPorId(id);
            if (multaExistente == null)
            {
                return NotFound($"Multa com Id {id} não encontrada");
            }

            _mapper.Map(multaRequestDto, multaExistente);

            var multaAtualizada = await _uof.multaRepository.UpdateMulta(multaExistente);
            await _uof.CommitAsync();

            var multaResponseDTO = _mapper.Map<MultaResponseDTO>(multaAtualizada);
            return Ok(multaResponseDTO);
        }



        [HttpGet("consulta")]
        public async Task<ActionResult<IEnumerable<MultaResponseDTO>>> ConsultarMultas([FromQuery] string? usuarioCPf, [FromQuery] int? emprestimoId, [FromQuery] int? dias, [FromQuery] StatusMulta statusMulta)
        {
            var multas = await _uof.multaRepository.ConsultarMultas(usuarioCPf, emprestimoId, dias, statusMulta);
            var multasResponseDTO = _mapper.Map<IEnumerable<MultaResponseDTO>>(multas);
            return Ok(multasResponseDTO);
        }


        [HttpPost("VerificarAtrasos")]
        public async Task<ActionResult> VerificarAtrasos()
        {

            DateTime dataAtual = DateTime.Now.Date;

            var emprestimos = await _uof.emprestimoRepository.GetEmprestimos();

            foreach (var emprestimo in emprestimos)
            {
                if (emprestimo.StatusEmprestimo != StatusEmprestimo.Devolvido)
                {
                    var exemplar = await _uof.exemplarRepository.GetExemplarPorId(emprestimo.ExemplarId);
                    if(exemplar == null)
                    {
                        return BadRequest($"Empréstimo de Id {emprestimo.Id} vinculado a um exemplar de Id {emprestimo.ExemplarId} não encontrado");
                    }

                    var livro = await _uof.livroRepository.GetLivroPorISBN10(exemplar.LivroISBN10);
                    if (livro == null)
                    {
                        return BadRequest($"Livro de ISBN-10 '{emprestimo.Exemplar.LivroISBN10}' é igual nulo");
                    }

                    var valorLivro = exemplar;
                    if(valorLivro == null)
                    {
                        return BadRequest($"Livro de ISBN-10 '{emprestimo.Exemplar.LivroISBN10}' possui valor do livro igual nulo");
                    }
                   

                    if (emprestimo.Renovacoes.Count == 0 && dataAtual > emprestimo.DataPrevistaInicial.Date) // não tem renovação
                    {

                        var diasAtraso = (dataAtual - emprestimo.DataPrevistaInicial.Date).Days;
                        var valorMulta = diasAtraso * 2.0f; // Convertido para float
                        if (valorMulta != null && valorMulta > livro.Valor * 2.0f) // Convertido para float
                        {
                            valorMulta = emprestimo.Exemplar.Livro.Valor * 2.0f; // Convertido para float
                        }


                        if (emprestimo.StatusEmprestimo == StatusEmprestimo.NaoDevolvido && emprestimo.Multa == null)//não tem renovação, mas não tem multa
                        {

                            var multa = new Multa
                            {
                                InicioMulta = emprestimo.DataPrevistaInicial.AddDays(1),
                                DiasAtrasados = diasAtraso,
                                Valor = valorMulta,
                                StatusMulta = StatusMulta.Pendente,
                                EmprestimoId = emprestimo.Id
                            };

                            await _uof.multaRepository.CreateMulta(multa);

                            emprestimo.StatusEmprestimo = StatusEmprestimo.Atrasada;

                            await _uof.emprestimoRepository.UpdateEmprestimo(emprestimo);

                            await _uof.CommitAsync();


                        }
                        else if (emprestimo.StatusEmprestimo == StatusEmprestimo.Atrasada) //não tem renovação  e já tem multa
                        {
                            var multa = await _uof.multaRepository.GetMultaPorIdEmprestimo(emprestimo.Id);
                            if(multa == null)
                            {
                                return BadRequest($"Multa de empréstimo com id {emprestimo.Id} não encontrada");
                            }



                            multa.InicioMulta = emprestimo.DataPrevistaInicial.AddDays(1);
                            multa.DiasAtrasados = diasAtraso;
                            multa.Valor = valorMulta;
                            multa.StatusMulta = StatusMulta.Pendente;

                            await _uof.multaRepository.UpdateMulta(multa);
                            await _uof.CommitAsync();

                        }

                    }
                    else if (emprestimo.Renovacoes.Count > 0) //tem renovação
                    {
                        var ultimaRenovacao = await _uof.renovacaoRepository.GetUltimaRenovacaoAsync(emprestimo.Id);

                        if(ultimaRenovacao == null)
                        {
                            return BadRequest($"Mesmo o sistema verificando que a há uma renovação no empréstimo de Id{emprestimo.Id}, não foi possível encontrar a última renovação.");
                        }

                        Console.WriteLine($"Data Atual: {dataAtual}");
                        Console.WriteLine($"Nova Data Prevista: {ultimaRenovacao.NovaDataPrevista}");

                        TimeSpan diferenca = dataAtual - ultimaRenovacao.NovaDataPrevista.Date;
                        int difereca = diferenca.Days;

                        if (difereca > 0)
                        {

                            var diasAtraso = difereca;
                            var valorMulta = diasAtraso * 2.0f;

                            if (valorMulta > livro.Valor * 2)
                            {
                                valorMulta = livro.Valor * 2;
                            }

                            if (emprestimo.Multa is null){

                                var multa = new Multa
                                {
                                    InicioMulta = ultimaRenovacao.NovaDataPrevista.AddDays(1),
                                    DiasAtrasados = diasAtraso,
                                    Valor = valorMulta,
                                    StatusMulta = StatusMulta.Pendente,
                                    EmprestimoId = emprestimo.Id
                                };

                                await _uof.multaRepository.CreateMulta(multa);

                                emprestimo.StatusEmprestimo = StatusEmprestimo.RenovacaoAtrasada;
                                await _uof.emprestimoRepository.UpdateEmprestimo(emprestimo);

                                await _uof.CommitAsync();


                            }
                            else if (emprestimo.StatusEmprestimo == StatusEmprestimo.RenovacaoAtrasada) //tem renovação  e já tem multa
                            {
                                var multa = await _uof.multaRepository.GetMultaPorIdEmprestimo(emprestimo.ExemplarId);

                                multa.InicioMulta = emprestimo.DataPrevistaInicial.AddDays(1);
                                multa.DiasAtrasados = diasAtraso;
                                multa.Valor = valorMulta;
                                multa.StatusMulta = StatusMulta.Pendente;

                                await _uof.multaRepository.UpdateMulta(multa);
                                await _uof.CommitAsync();

                            }

                        }

                    }
                }
            }

            return Ok("Verificação de atrasos concluída");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<MultaResponseDTO>> DeleteMulta(int id)
        {
            var multa = await _uof.multaRepository.GetMultaPorId(id);
            if (multa == null)
            {
                return NotFound($"Multa com Id {id} não encontrada");
            }
            var emprestimo = await _uof.emprestimoRepository.GetEmprestimoPorId(multa.EmprestimoId);

            var multaExcluida = await _uof.multaRepository.DeleteMulta(id);
            await _uof.CommitAsync();

            if(emprestimo.StatusEmprestimo == StatusEmprestimo.Atrasada)
            {
                emprestimo.StatusEmprestimo = StatusEmprestimo.NaoDevolvido;
                await _uof.emprestimoRepository.UpdateEmprestimo(emprestimo);
                await _uof.CommitAsync();

            }
            else if(emprestimo.StatusEmprestimo == StatusEmprestimo.RenovacaoAtrasada)
            {
                emprestimo.StatusEmprestimo = StatusEmprestimo.Renovacao;
                await _uof.emprestimoRepository.UpdateEmprestimo(emprestimo);
                await _uof.CommitAsync();

            }
            


            var multaResponseDTO = _mapper.Map<MultaResponseDTO>(multaExcluida);
            return Ok(multaResponseDTO);
        }

        [HttpPatch("atualizar-status-por-id-{id}")]
        public async Task<ActionResult<MultaResponseDTO>> UpdateMultaStatus(int id, [FromBody] StatusMulta statusMulta)
        {
            var multa = await _uof.multaRepository.GetMultaPorId(id);
            if (multa == null)
            {
                return NotFound($"Multa com ID {id} não encontrada.");
            }

            if (statusMulta == null)
            {
                return BadRequest("Adicione um valor para o status.");
            }

            if (statusMulta != StatusMulta.Paga)
            {
                return BadRequest("Você só pode mudar o valor de multa para 1(Paga)!");
            }

            multa.StatusMulta = statusMulta;
            await _uof.multaRepository.UpdateMulta(multa);
            await _uof.CommitAsync();

            
            var multaResponseDto = _mapper.Map<MultaResponseDTO>(multa);

            return Ok(multaResponseDto);
        }

    }

}
