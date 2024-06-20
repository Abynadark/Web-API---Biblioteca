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
    public class ExemplaresController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;

        public ExemplaresController(IUnitOfWork uof, IMapper mapper)
        {
            _uof = uof;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExemplarResponseDTO>>> GetExemplares()
        {
            var exemplares = await _uof.exemplarRepository.GetExemplares();

            var exemplaresResponseDto = _mapper.Map<IEnumerable<ExemplarResponseDTO>>(exemplares);

            return Ok(exemplaresResponseDto);
        }


        [HttpGet("{id}", Name = "GetExemplarPorId")]
        public async Task<ActionResult<ExemplarResponseDTO>> GetExemplarPorId(int id)
        {
            var exemplar = await _uof.exemplarRepository.GetExemplarPorId(id);
            if (exemplar == null)
            {
                return NotFound($"Exemplar com ID {id} não encontrado");
            }

            var exemplarResponseDto = _mapper.Map<ExemplarResponseDTO>(exemplar);

            return Ok(exemplarResponseDto);
        }

        [HttpGet("consultar-por-isbn10-e-status")]
        public async Task<ActionResult<IEnumerable<Exemplar>>> GetExemplaresISBN10eStatus([FromQuery] string ISBN10, [FromQuery] StatusExemplar? statusExemplar)
        {
            try
            {
                var exemplares = await _uof.exemplarRepository.GetExemplaresISBN10eStatus(ISBN10, statusExemplar);
                if (exemplares == null)
                {
                    return NotFound();
                }

                return Ok(exemplares);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter exemplares: {ex.Message}");
            }
        }

        [HttpGet("livro/{isbn10}")]
        public async Task<ActionResult<IEnumerable<ExemplarResponseDTO>>> GetExemplaresPorISBN10(string isbn10)
        {
            var exemplares = await _uof.exemplarRepository.GetExemplaresPorISBN10(isbn10);

            if (exemplares == null || !exemplares.Any())
            {
                return NotFound($"Nenhum exemplar encontrado para o livro com ISBN-10 {isbn10}");
            }

            var exemplaresResponseDto = _mapper.Map<IEnumerable<ExemplarResponseDTO>>(exemplares);

            return Ok(exemplaresResponseDto);
        }

        [HttpPost]

        public async Task<ActionResult<ExemplarResponseDTO>> CreateExemplar(ExemplarRequestDTO exemplarRequestDto)
        {
            var livro = await _uof.livroRepository.GetLivroPorISBN10(exemplarRequestDto.LivroISBN10);
            if (livro == null)
            {
                return NotFound($"Livro com ISBN-10 {exemplarRequestDto.LivroISBN10} não encontrado");
            }

            var jaexiste = await _uof.exemplarRepository.GetExemplarPorId(exemplarRequestDto.Id);
            if (jaexiste != null)
            {
                return BadRequest($"Exemplar de Id {exemplarRequestDto.Id} já cadastrado!");
            }

            var exemplar = _mapper.Map<Exemplar>(exemplarRequestDto);
            exemplar.Livro = livro;

            var novoExemplar = await _uof.exemplarRepository.CreateExemplar(exemplar);
            
            
            if (livro.StatusLivro == StatusLivro.NaoDisponivel)
            {
                livro.StatusLivro = StatusLivro.Disponivel;
                await _uof.livroRepository.UpdateLivro(livro); 
                
            }
            
            await _uof.CommitAsync();

            var exemplarNovoResponseDto = _mapper.Map<ExemplarResponseDTO>(novoExemplar);



            return CreatedAtRoute("GetExemplarPorId", new { id = exemplarNovoResponseDto.Id }, exemplarNovoResponseDto);
           
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ExemplarResponseDTO>> UpdateExemplar(int id, ExemplarRequestDTO exemplarRequestDto)
        {
            if (id != exemplarRequestDto.Id)
            {
                return BadRequest("O ID fornecido não corresponde ao ID do exemplar");
            }

            var exemplarExistente = await _uof.exemplarRepository.GetExemplarPorId(id);
            if (exemplarExistente == null)
            {
                return NotFound($"Exemplar de ID {id} não encontrado.");
            }

            var livroNovo = await _uof.livroRepository.GetLivroPorISBN10(exemplarRequestDto.LivroISBN10);
            if (livroNovo == null)
            {
                return NotFound($"Livro com ISBN-10 {exemplarRequestDto.LivroISBN10} não encontrado");
            }

            var livroAntigo = exemplarExistente.Livro;

            _mapper.Map(exemplarRequestDto, exemplarExistente);

            exemplarExistente.Livro = livroNovo;

            await _uof.exemplarRepository.UpdateExemplar(exemplarExistente);
            await _uof.CommitAsync(); 

           
            if (livroNovo != null && livroAntigo != null && livroNovo.ISBN10 != livroAntigo.ISBN10)
            {
              
                if (exemplarExistente.StatusExemplar == StatusExemplar.Disponivel)
                {
                    var exemplaresDoLivroAntigo = await _uof.exemplarRepository.GetExemplaresPorISBN10(livroAntigo.ISBN10);
                    if (exemplaresDoLivroAntigo != null && exemplaresDoLivroAntigo.All(e => e.StatusExemplar != StatusExemplar.Disponivel))
                    {
                        livroAntigo.StatusLivro = StatusLivro.NaoDisponivel;
                        await _uof.livroRepository.UpdateLivro(livroAntigo);
                        await _uof.CommitAsync(); // Commit das mudanças no livro antigo
                        Console.WriteLine($"Status do livro antigo (ISBN10: {livroAntigo.ISBN10}) atualizado para NaoDisponivel.");
                    }
                }

                if (livroNovo.StatusLivro != StatusLivro.Disponivel)
                {
                    livroNovo.StatusLivro = StatusLivro.Disponivel;
                    await _uof.livroRepository.UpdateLivro(livroNovo);
                    await _uof.CommitAsync(); 
                    Console.WriteLine($"Status do novo livro (ISBN10: {livroNovo.ISBN10}) atualizado para Disponivel.");
                }
            }
            

            var exemplarAtualizadoResponseDto = _mapper.Map<ExemplarResponseDTO>(exemplarExistente);
            return Ok(exemplarAtualizadoResponseDto);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<ExemplarResponseDTO>> DeleteExemplar(int id)
        {
            var exemplar = await _uof.exemplarRepository.GetExemplarPorId(id);
            if (exemplar == null)
            {
                return NotFound($"Exemplar de ID {id} não encontrado");
            }

            var exemplarExcluido = await _uof.exemplarRepository.DeleteExemplar(id);
            await _uof.CommitAsync();

            if(exemplar.StatusExemplar == 0) 
            {
              
                var livro = await _uof.livroRepository.GetLivroPorISBN10(exemplarExcluido.LivroISBN10);
                var listaExemplaresAssociadosLivroeDisponiveis = await _uof.exemplarRepository.GetExemplaresISBN10eStatus(exemplarExcluido.LivroISBN10, StatusExemplar.Disponivel);


                if (livro.StatusLivro == StatusLivro.Disponivel && livro != null && listaExemplaresAssociadosLivroeDisponiveis.Any() == false)
                {
                    livro.StatusLivro = StatusLivro.NaoDisponivel;
                    await _uof.livroRepository.UpdateLivro(livro);
                    await _uof.CommitAsync();
                }
                
            }

            var exemplarExcluidoResponseDto = _mapper.Map<ExemplarResponseDTO>(exemplarExcluido);

            return Ok(exemplarExcluidoResponseDto);
        }

        [HttpPatch("{id}/atualizar-status")]
        public async Task<ActionResult<ExemplarResponseDTO>> UpdateExemplarStatus(int id, [FromBody] StatusExemplar novoStatus)
        {
           
            var exemplar = await _uof.exemplarRepository.GetExemplarPorId(id);
            if (exemplar == null)
            {
                return NotFound($"Exemplar com ID {id} não encontrado.");
            }

            if(novoStatus == null)
            {
                return BadRequest($"O status não pode ser nulo");
            }
            
            if(novoStatus != StatusExemplar.Danificado)
            {
                return BadRequest($"O status de um exemplar só pode ser modificado para danificado(3)");
            }

        
            exemplar.StatusExemplar = novoStatus;
            await _uof.exemplarRepository.UpdateExemplar(exemplar);
            await _uof.CommitAsync();

         
            var exemplarResponseDto = _mapper.Map<ExemplarResponseDTO>(exemplar);

         
            return Ok(exemplarResponseDto);
        }

    }
}
