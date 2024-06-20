using Biblioteca.DTOs.Response;
using Biblioteca.DTOs.Request;
using Biblioteca.Interfaces;
using Biblioteca.Models;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Biblioteca.Enum;

namespace Biblioteca.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;

        public UsuariosController(IUnitOfWork uof, IMapper mapper)
        {
            _uof = uof;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioResponseDTO>>> GetUsuarios()
        {
            var usuarios = await _uof.usuarioRepository.GetUsuarios();

            var usuariosResponseDto = _mapper.Map<IEnumerable<UsuarioResponseDTO>>(usuarios);

            return Ok(usuariosResponseDto);
        }

        [HttpGet("pesquisar-por-cpf-do-usuario", Name = "ObterUsuario")]
        public async Task<ActionResult<UsuarioResponseDTO>> GetPorCPF(string cpf)
        {
            var usuario = await _uof.usuarioRepository.GetUsuarioPorCPF(cpf);
           
            if (usuario is null)
            {
                return NotFound($"Usuário com CPF {cpf} não encontrado");
            }

            var usuarioResponseDto = _mapper.Map<UsuarioResponseDTO>(usuario);

            return Ok(usuarioResponseDto);
        }

        [HttpGet("consultar-com-filtro")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios(string? nome = null, string? cpf = null, DateTime? dataNasc = null, string? email = null, string? celular = null, string? endereco = null, StatusUsuario? statusUsuario = null)
        {
            try
            {
                var usuarios = await _uof.usuarioRepository.GetUsuarios(nome, cpf, dataNasc, email, celular, endereco, statusUsuario);
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar usuários: {ex.Message}");
            }
        }

      

        [HttpPost]
        public async Task<ActionResult<UsuarioResponseDTO>> CreateUsuario(UsuarioRequestDTO usuarioRequestDto)
        {

            if (usuarioRequestDto is null)
            {
                return BadRequest("Dados Inválidos");
            }

            var usuarioVerificacao = await _uof.usuarioRepository.GetUsuarioPorCPF(usuarioRequestDto.CPF);


            if (usuarioVerificacao != null)
            {
                return BadRequest($"Usuário de CPF {usuarioRequestDto.CPF} já cadastrado!");
            }

            var emailExiste = await _uof.usuarioRepository.GetUsuarios(null, null, null, usuarioRequestDto.Email, null, null, null);
            if (emailExiste.Any())
            {
                return BadRequest($"Email '{usuarioRequestDto.Email}' já cadastrado!");
            }


            var usuario = _mapper.Map<Usuario>(usuarioRequestDto);

            var usuarioCriado = await _uof.usuarioRepository.CreateUsuario(usuario);
            await _uof.CommitAsync();

            var usuarioCriadoResponseDto = _mapper.Map<UsuarioResponseDTO>(usuarioCriado);

            return new CreatedAtRouteResult("ObterUsuario", new { cpf = usuarioCriadoResponseDto.CPF }, usuarioCriadoResponseDto);
        }

        [HttpPut("atualizar-por-cpf-do-usuario")]
        public async Task<ActionResult<UsuarioResponseDTO>> UpdateUsuario(string cpf, UsuarioRequestDTO usuarioRequestDto)
        {
            if (cpf != usuarioRequestDto.CPF)
            {
                return BadRequest("O CPF fornecido na rota não corresponde ao CPF fornecido no corpo da requisição.");
            }

          
            var usuarioExistente = await _uof.usuarioRepository.GetUsuarioPorCPF(cpf);
            if (usuarioExistente == null)
            {
                return NotFound($"Usuário com CPF {cpf} não encontrado.");
            }

         
            _mapper.Map(usuarioRequestDto, usuarioExistente);

            try
            {
               
                var usuarioAtualizado = await _uof.usuarioRepository.UpdateUsuario(usuarioExistente);
                await _uof.CommitAsync();
                var usuarioAtualizadoResponseDto = _mapper.Map<UsuarioResponseDTO>(usuarioAtualizado);
                return Ok(usuarioAtualizadoResponseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar usuário: {ex.Message}");

            }

        }

        [HttpDelete("deletar-por-cpf-do-usuario")]
        public async Task<ActionResult<UsuarioResponseDTO>> DeleteUsuario(string cpf)
        {
            var usuario = await _uof.usuarioRepository.GetUsuarioPorCPF(cpf);
            if (usuario is null)
            {
                return NotFound($"Usuário de CPF {cpf} não encontrado");
            }

            var usuarioExcluido = await _uof.usuarioRepository.DeleteUsuario(cpf);
            await _uof.CommitAsync();

            var usuarioExcluidoResponseDto = _mapper.Map<UsuarioResponseDTO>(usuarioExcluido);

            return Ok(usuarioExcluidoResponseDto);

        }

        [HttpPatch("atualizar-status-por-cpf")]
        public async Task<ActionResult<UsuarioResponseDTO>> UpdateUsuarioStatus(string cpf, [FromBody] StatusUsuario novoStatus)
        {
            var usuario = await _uof.usuarioRepository.GetUsuarioPorCPF(cpf);
            if (usuario == null)
            {
                return NotFound($"Usuário com CPF {cpf} não encontrado.");
            }

            if(novoStatus != StatusUsuario.NaoPodePegarLivro && novoStatus != StatusUsuario.NaoPodePegarLivro2Meses && novoStatus != StatusUsuario.PodePegar1Livro2Meses && novoStatus != StatusUsuario.PodePegar2Livros2Meses && novoStatus != StatusUsuario.PodePegar3Livros)
            {
                return BadRequest($"O status do usuário só pode ser mudado para \r\n0 (Pode ter 3 empréstimos ativos)\r\n1 (Pode ter 2 empréstimos ativo por vez)\r\n2 (Pode ter 1 empréstimo ativo por vez)\r\n3 (Não Pode ter empréstimo ativo por vez)\r\n4 (Não Pode ter empréstimo ativo nunca mais)\r\n");
            }
           
            usuario.StatusUsuario = novoStatus;
            await _uof.usuarioRepository.UpdateUsuario(usuario);
            await _uof.CommitAsync();

            
            var usuarioResponseDto = _mapper.Map<UsuarioResponseDTO>(usuario);

            return Ok(usuarioResponseDto);
        }

    }
}
