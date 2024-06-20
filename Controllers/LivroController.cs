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
    public class LivrosController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;

        public LivrosController(IUnitOfWork uof, IMapper mapper)
        {
            _uof = uof;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<LivroResponseDTO>>> GetLivros()
        {
            var livros = await _uof.livroRepository.GetLivros();

            var livrosResponseDto = _mapper.Map<IEnumerable<LivroResponseDTO>>(livros);

            return Ok(livrosResponseDto);
        }

        [HttpGet("{isbn10}", Name = "GetLivroPorISBN10")]
        public async Task<ActionResult<LivroResponseDTO>> GetLivroPorISBN10(string isbn10)
        {
            var livro = await _uof.livroRepository.GetLivroPorISBN10(isbn10);
            if (livro == null)
            {
                return NotFound($"Livro com ISBN-10 {isbn10} não encontrado");
            }

            var livroResponseDto = _mapper.Map<LivroResponseDTO>(livro);

            return Ok(livroResponseDto);
        }

        [HttpPost]
        public async Task<ActionResult<LivroResponseDTO>> CreateLivro(LivroRequestDTO livroRequestDto)
        {
            var livro = _mapper.Map<Livro>(livroRequestDto);
            
            if(livro is null)
            {
                return BadRequest("Dados Inválidos");
            }

            var jaexiste = await _uof.livroRepository.GetLivroPorISBN10(livroRequestDto.ISBN10);
            if (jaexiste != null)
            {
                return BadRequest($"Livro de ISBN-10 {livroRequestDto.ISBN10} já cadastrado!");
            }

            var novoLivro = await _uof.livroRepository.CreateLivro(livro);
            await _uof.CommitAsync();

            var livroNovoResponseDto = _mapper.Map<LivroResponseDTO>(novoLivro);

            return CreatedAtRoute("GetLivroPorISBN10", new { isbn10 = livroNovoResponseDto.ISBN10 }, livroNovoResponseDto);
        }

        [HttpPut("{isbn10}")]
        public async Task<ActionResult<LivroResponseDTO>> UpdateLivro(string isbn10, LivroRequestDTO livroRequestDto)
        {
            
            if (isbn10 != livroRequestDto.ISBN10)
            {
                return BadRequest("O ISBN-10 fornecido não corresponde ao ISBN-10 Livro");
            }

            var livroExistente = await _uof.livroRepository.GetLivroPorISBN10(isbn10);
            if (livroExistente == null)
            {
                return NotFound($"Livro de ISBN-10 {isbn10} não encontrado.");
            }

            _mapper.Map(livroRequestDto, livroExistente);

            try
            {
                // Atualizar o usuário no banco de dados
                var livroAtualizado = await _uof.livroRepository.UpdateLivro(livroExistente);
                await _uof.CommitAsync();

                var livroAtualizadoResponseDto = _mapper.Map<LivroResponseDTO>(livroAtualizado);

                return Ok(livroAtualizadoResponseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar usuário: {ex.Message}");

            }

        }

        [HttpDelete("{isbn10}")]
        public async Task<ActionResult<LivroResponseDTO>> DeleteLivro(string isbn10)
        {

            var livro = await _uof.livroRepository.GetLivroPorISBN10(isbn10);
            if (livro == null)
            {
                return NotFound($"Exemplar de ISBN-10 {isbn10} não encontrado");
            }

            var livroExcluido = await _uof.livroRepository.DeleteLivro(isbn10);
            await _uof.CommitAsync();

            var livroExcluidoReponseDto = _mapper.Map<LivroResponseDTO>(livroExcluido);

            return Ok(livroExcluidoReponseDto);

        }

        [HttpGet("consultar")]
        public async Task<ActionResult<IEnumerable<LivroResponseDTO>>> ConsultarLivros([FromQuery] string nome,[FromQuery] string autor, [FromQuery] string isbn10, [FromQuery] string isbn13, [FromQuery] string editora, [FromQuery] string edicao, [FromQuery] string idioma, [FromQuery] string genero, [FromQuery] DateTime? datapublicacao, [FromQuery] string classificacao, [FromQuery] int? qnt_Pagina, [FromQuery] float? precoMinimo, [FromQuery] float? precoMaximo, [FromQuery] StatusLivro? statusLivro)
        {

            var livros = await _uof.livroRepository.ConsultarLivros(nome, autor, isbn10, isbn13, editora, edicao, idioma, genero, datapublicacao, classificacao, qnt_Pagina, precoMinimo, precoMaximo, statusLivro);

            var livrosResponseDTO = _mapper.Map<IEnumerable<LivroResponseDTO>>(livros);
            return Ok(livrosResponseDTO);

        }

    }
}
