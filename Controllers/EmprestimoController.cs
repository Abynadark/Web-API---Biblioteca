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
    public class EmprestimosController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;

        public EmprestimosController(IUnitOfWork uof, IMapper mapper)
        {
            _uof = uof;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmprestimoResponseDTO>>> GetEmprestimos()
        {
            var emprestimos = await _uof.emprestimoRepository.GetEmprestimos();
            var emprestimosResponseDto = _mapper.Map<IEnumerable<EmprestimoResponseDTO>>(emprestimos);
            return Ok(emprestimosResponseDto);
        }

        [HttpGet("pesquisar-por-cpf-do-usuario")]
        public async Task<ActionResult<IEnumerable<EmprestimoResponseDTO>>> GetEmprestimosPorUsuarioCPF(string cpf)
        {
            var emprestimos = await _uof.emprestimoRepository.GetEmprestimosPorUsuarioCPF(cpf);
            
            var emprestimosResponseDto = _mapper.Map<IEnumerable<EmprestimoResponseDTO>>(emprestimos);

            return Ok(emprestimosResponseDto);
        }


        [HttpGet("por-usuario-status-exemplar")]
        public async Task<ActionResult<IEnumerable<EmprestimoResponseDTO>>> ConsultarEmprestimos([FromQuery] string? cpf, [FromQuery] int? exemplarId, [FromQuery] StatusEmprestimo? statusEmprestimo, [FromQuery] string? isbn10)
        {
            try
            {
                var emprestimos = await _uof.emprestimoRepository.ConsultarEmprestimos(cpf, exemplarId, statusEmprestimo, isbn10);
                var emprestimosResponseDTO = _mapper.Map<IEnumerable<EmprestimoResponseDTO>>(emprestimos);
                return Ok(emprestimosResponseDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao obter empréstimos: {ex.Message}");
            }
        }


        [HttpGet("{id}", Name = "GetEmprestimoPorId")]
        public async Task<ActionResult<EmprestimoResponseDTO>> GetEmprestimoPorId(int id)
        {
            var emprestimo = await _uof.emprestimoRepository.GetEmprestimoPorId(id);
            if (emprestimo == null)
            {
                return NotFound($"Empréstimo com Id {id} não encontrado");
            }
            var emprestimoResponseDto = _mapper.Map<EmprestimoResponseDTO>(emprestimo);
            return Ok(emprestimoResponseDto);
        }

        [HttpPost]
        public async Task<ActionResult<EmprestimoResponseDTO>> CreateEmprestimo(EmprestimoRequestDTO emprestimoRequestDto)
        {

            // Buscar o usuário pelo CPF fornecido
            var usuario = await _uof.usuarioRepository.GetUsuarioPorCPF(emprestimoRequestDto.UsuarioCPF);

            // Verificar se o usuário existe
            if (usuario == null)
            {
                return BadRequest($"Usuário de CPF {emprestimoRequestDto.UsuarioCPF} encontrado.");
            }

            // Buscar o exemplar pelo id fornecido
            var exemplar = await _uof.exemplarRepository.GetExemplarPorId(emprestimoRequestDto.ExemplarId);

            // Verificar se o exemplar existe
            if (exemplar == null)
            {
                return BadRequest($"Exemplar de Id {emprestimoRequestDto.ExemplarId} não encontrado.");
            }

            // Verifica se o exemplar está disponível para o emprestimo 

            if (exemplar.StatusExemplar == Enum.StatusExemplar.Emprestado)
            {
                return BadRequest($"Exemplar de Id {emprestimoRequestDto.ExemplarId} consta como emprestado no sistema.");
            } 
            else if (exemplar.StatusExemplar == Enum.StatusExemplar.Danificado)
            {
                return BadRequest($"Exemplar de Id {emprestimoRequestDto.ExemplarId} consta como danificado no sistema.");
            }
            else if (exemplar.StatusExemplar == Enum.StatusExemplar.LivroDeConsulta) 
            {
                return BadRequest($"Exemplar de Id {emprestimoRequestDto.ExemplarId} consta como Livro de Consulta no sistema.");
            }

            // Mapear os dados do DTO para o objeto de empréstimo
            var emprestimo = _mapper.Map<Emprestimo>(emprestimoRequestDto);

            // Atribuir o ID do usuário ao empréstimo
            emprestimo.UsuarioId = usuario.Id;

            if (emprestimo == null)
            {
                return BadRequest("Dados Inválidos");
            }


            
            //verifica se existe emprestimos atrasados
            var emprestimosAtrasado = await _uof.emprestimoRepository.ConsultarEmprestimos(usuario.CPF, null, StatusEmprestimo.Atrasada, null );
            
            if (emprestimosAtrasado.Any())
            {
                return BadRequest($"O usuario de CPF {usuario.CPF} possiu emprestimo atrasado!");
            }

            var emprestimosRenovacaoAtrasada = await _uof.emprestimoRepository.ConsultarEmprestimos(usuario.CPF, null, StatusEmprestimo.RenovacaoAtrasada, null);
            
            if (emprestimosRenovacaoAtrasada.Any())
            {
                return BadRequest($"O usuario de CPF {usuario.CPF} possiu emprestimo com renovação atrasada!");
            }
            //

            
            //verifica se já possui emprestimo ativo de um exemplar com mesmo isbn10 

            var emprestimosNaoDevolvidosMesmoISBN10 = await _uof.emprestimoRepository.ConsultarEmprestimos(usuario.CPF, null, StatusEmprestimo.NaoDevolvido, exemplar.LivroISBN10);
            
            var emprestimosRenovadosMesmoISBN10 = await _uof.emprestimoRepository.ConsultarEmprestimos(usuario.CPF, null, StatusEmprestimo.Renovacao, exemplar.LivroISBN10);

            if (emprestimosNaoDevolvidosMesmoISBN10.Any() || emprestimosRenovadosMesmoISBN10.Any())
            {
                return BadRequest($"O usuario de CPF {usuario.CPF} já possui um empréstimo ativo de exemplar com ISBN-10 {exemplar.LivroISBN10}!");
               
            }
            //

            //faz a contagem de empréstimo ativo
            var emprestimosNaoDevolvidos = await _uof.emprestimoRepository.ConsultarEmprestimos(usuario.CPF, null, StatusEmprestimo.NaoDevolvido, null);

            var emprestimosRenovados = await _uof.emprestimoRepository.ConsultarEmprestimos(usuario.CPF, null, StatusEmprestimo.Renovacao, null);
            
            var quantidadeEmprestimosAtivos = emprestimosNaoDevolvidos.Count() + emprestimosRenovados.Count();

            //faz as limitações por status do usuario
            if(usuario.StatusUsuario == StatusUsuario.PodePegar3Livros && quantidadeEmprestimosAtivos >= 3)
            {
                return BadRequest($"O usuario de CPF {usuario.CPF} já possui três emprestimos ativos!");
            
            }else if (usuario.StatusUsuario == StatusUsuario.PodePegar2Livros2Meses && quantidadeEmprestimosAtivos >= 2)
            {
             
                return BadRequest($"O usuario de CPF {usuario.CPF} está limitado a dois empréstimos por vez e já possui dois emprestimos ativos!");
            
            }
            else if (usuario.StatusUsuario == StatusUsuario.PodePegar1Livro2Meses && quantidadeEmprestimosAtivos >= 1)
            {
                return BadRequest($"O usuario de CPF {usuario.CPF} está limitado a um empréstimo por vez e já possui um emprestimo ativo!");
            
            }else if(usuario.StatusUsuario == StatusUsuario.NaoPodePegarLivro2Meses && quantidadeEmprestimosAtivos >= 0)
            {
                
                return BadRequest($"O usuario de CPF {usuario.CPF} não pode fazer empréstimos POR ENQUANTO!");
            
            }else if (usuario.StatusUsuario == StatusUsuario.NaoPodePegarLivro)
            {
                return BadRequest($"O usuario de CPF {usuario.CPF} está BANIDO e por isso não pode fazer mais empréstimos!");
            }
            


            
            var novoEmprestimo = await _uof.emprestimoRepository.CreateEmprestimo(emprestimo);
            
           
            exemplar.StatusExemplar = StatusExemplar.Emprestado;
            await _uof.exemplarRepository.UpdateExemplar(exemplar);
            await _uof.CommitAsync();


            var exemplaresLivro = await _uof.exemplarRepository.GetExemplaresPorISBN10(exemplar.LivroISBN10);

            if (!exemplaresLivro.Any(e => e.StatusExemplar == StatusExemplar.Disponivel))//se der erro pode ser porque mudei all po any
            {
                exemplar.Livro.StatusLivro = StatusLivro.NaoDisponivel;
                await _uof.livroRepository.UpdateLivro(exemplar.Livro);
                await _uof.CommitAsync();
            }

            var novoEmprestimoResponseDto = _mapper.Map<EmprestimoResponseDTO>(novoEmprestimo);

          
            return CreatedAtRoute("GetEmprestimoPorId", new { id = novoEmprestimoResponseDto.Id }, novoEmprestimoResponseDto);
        }





        [HttpPut("{id}")]
        public async Task<ActionResult<EmprestimoResponseDTO>> UpdateEmprestimo(int id, EmprestimoRequestDTO emprestimoRequestDto)
        {

            var emprestimoExistente = await _uof.emprestimoRepository.GetEmprestimoPorId(id);
            if (emprestimoExistente == null)
            {
                return NotFound($"Empréstimo de Id {id} não encontrado.");
            }


            var usuarioExistente = await _uof.usuarioRepository.GetUsuarioPorCPF(emprestimoRequestDto.UsuarioCPF);
            if (usuarioExistente == null)
            {
                return NotFound($"Usuario de CPF {emprestimoRequestDto.UsuarioCPF} não encontrado.");
            }
           

            var exemplarExistente = await _uof.exemplarRepository.GetExemplarPorId(emprestimoRequestDto.ExemplarId);
            if (exemplarExistente == null)
            {
                return NotFound($"Exemplar de Id {emprestimoRequestDto.ExemplarId} não encontrado.");
            }
            if (exemplarExistente.StatusExemplar != StatusExemplar.Disponivel)
            {
                return NotFound($"Exemplar de Id {emprestimoRequestDto.ExemplarId} não disponível para empréstimo!");
            }
            Console.WriteLine($"exemplar existente  é: {exemplarExistente}");

            //não deixa atualizar um empréstimo se o usuario já tem um exemplar cadastrado de mesmo isbn10, a não ser que ele esteja retira um exemplar de mesmo isbn10 pelo outro
            var exemplarAntigo = await _uof.exemplarRepository.GetExemplarPorId(emprestimoExistente.ExemplarId);
            Console.WriteLine($"exemplar antigo é: {exemplarAntigo}");

            if (exemplarAntigo.LivroISBN10 != exemplarExistente.LivroISBN10)
            {
                var emprestimosNaoDevolvidosMesmoISBN10 = await _uof.emprestimoRepository.ConsultarEmprestimos(usuarioExistente.CPF, null, StatusEmprestimo.NaoDevolvido, exemplarExistente.LivroISBN10);

                var emprestimosRenovadosMesmoISBN10 = await _uof.emprestimoRepository.ConsultarEmprestimos(usuarioExistente.CPF, null, StatusEmprestimo.Renovacao, exemplarExistente.LivroISBN10);

                if (emprestimosNaoDevolvidosMesmoISBN10.Any() || emprestimosRenovadosMesmoISBN10.Any())
                {
                    return BadRequest($"O usuário de CPF {usuarioExistente.CPF} já possui um empréstimo ativo de exemplar com ISBN-10 {exemplarExistente.LivroISBN10}!");
                }

            }
            


            _mapper.Map(emprestimoRequestDto, emprestimoExistente);

            emprestimoExistente.UsuarioId = usuarioExistente.Id;


            var emprestimoAtualizado = await _uof.emprestimoRepository.UpdateEmprestimo(emprestimoExistente);
            await _uof.CommitAsync();

    
            exemplarAntigo.StatusExemplar = StatusExemplar.Disponivel;
            var exemplarAntigoAtualizado = await _uof.exemplarRepository.UpdateExemplar(exemplarAntigo);
            await _uof.CommitAsync();

            Console.WriteLine($"exemplar antigo atualizado é: {exemplarAntigoAtualizado}");

            exemplarExistente.StatusExemplar = StatusExemplar.Emprestado;
            var exemplarNovoAtualizado = await _uof.exemplarRepository.UpdateExemplar(exemplarExistente);
            await _uof.CommitAsync();

            Console.WriteLine($"exemplar novo atualizado é: {exemplarNovoAtualizado}");
            

            
            if (exemplarAntigo.LivroISBN10 != exemplarExistente.LivroISBN10)
            {
               

                var exemplaresDoLivroAntigoDisponiveis = await _uof.exemplarRepository.GetExemplaresISBN10eStatus(exemplarAntigo.LivroISBN10, StatusExemplar.Disponivel);
                Console.WriteLine($"atualizadoexemplaresDoLivroAntigoDisponiveis é: {exemplaresDoLivroAntigoDisponiveis}");
                if (exemplaresDoLivroAntigoDisponiveis.Any())
                {
                    var livroAntigo = await _uof.livroRepository.GetLivroPorISBN10(exemplarAntigo.LivroISBN10);
                    livroAntigo.StatusLivro = StatusLivro.Disponivel;
                    await _uof.livroRepository.UpdateLivro(livroAntigo);
                    await _uof.CommitAsync();
                }

                
                var exemplaresDoLivroNovoDisponiveis = await _uof.exemplarRepository.GetExemplaresISBN10eStatus(exemplarExistente.LivroISBN10, StatusExemplar.Disponivel);
                var livroNovo = await _uof.livroRepository.GetLivroPorISBN10(exemplarExistente.LivroISBN10);

              
                if (exemplaresDoLivroNovoDisponiveis.Any())
                {
                    
                    livroNovo.StatusLivro = StatusLivro.Disponivel;
                    await _uof.livroRepository.UpdateLivro(livroNovo);
                    await _uof.CommitAsync(); 
                    Console.WriteLine($"Status do novo livro (ISBN10: {livroNovo.ISBN10}) atualizado para Disponivel.");
                }
                else
                {
                    livroNovo.StatusLivro = StatusLivro.NaoDisponivel;
                    await _uof.livroRepository.UpdateLivro(livroNovo);
                    await _uof.CommitAsync(); 
                    Console.WriteLine($"Status do novo livro (ISBN10: {livroNovo.ISBN10}) atualizado para Disponivel.");
                }
            }

            var emprestimoAtualizadoResponseDto = _mapper.Map<EmprestimoResponseDTO>(emprestimoAtualizado);

            return Ok(emprestimoAtualizadoResponseDto);

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<EmprestimoResponseDTO>> DeleteEmprestimo(int id)
        {
           
            var emprestimo = await _uof.emprestimoRepository.GetEmprestimoPorId(id);
            if (emprestimo == null)
            {
                return NotFound($"Empréstimo de Id {id} não encontrado");
            }

           
            var emprestimoExcluido = await _uof.emprestimoRepository.DeleteEmprestimo(id);
            await _uof.CommitAsync();

          
            var exemplar = await _uof.exemplarRepository.GetExemplarPorId(emprestimo.ExemplarId);
            exemplar.StatusExemplar = StatusExemplar.Disponivel;
            await _uof.exemplarRepository.UpdateExemplar(exemplar);
            await _uof.CommitAsync();

           
            var livro = await _uof.livroRepository.GetLivroPorISBN10(exemplar.LivroISBN10);

            if (livro != null)
            {
               
                var exemplaresLivro = await _uof.exemplarRepository.GetExemplaresPorISBN10(livro.ISBN10);

                
                if (exemplaresLivro.Any(e => e.StatusExemplar == StatusExemplar.Disponivel))
                {
                   
                    livro.StatusLivro = StatusLivro.Disponivel;
                    await _uof.livroRepository.UpdateLivro(livro);
                    await _uof.CommitAsync();
                }

            }

           
            var emprestimoExcluidoResponseDto = _mapper.Map<EmprestimoResponseDTO>(emprestimoExcluido);

            return Ok(emprestimoExcluidoResponseDto);
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult<EmprestimoResponseDTO>> UpdateEmprestimoStatus(int id, [FromBody] StatusEmprestimo novoStatus)
        {
            var emprestimo = await _uof.emprestimoRepository.GetEmprestimoPorId(id);
            if (emprestimo == null)
            {
                return NotFound($"Empréstimo de Id {id} não encontrado.");
            }
            if(novoStatus == null)
            {
                return BadRequest($"Adicione um valor para status");
            }

            if(novoStatus != StatusEmprestimo.Devolvido)
            {
                return BadRequest("Status só pode ser modificado para 1(Devolvido)!");
            }

            var exemplar = await _uof.exemplarRepository.GetExemplarPorId(emprestimo.ExemplarId);
            if (exemplar == null)
            {
                return BadRequest($"Exemplar de Id {emprestimo.ExemplarId} associado ao emprestimo de Id{emprestimo.Id} não encontrado");
            }

            var livro = await _uof.livroRepository.GetLivroPorISBN10(exemplar.LivroISBN10);

        
            emprestimo.DataDevolucao = DateTime.Now;
            emprestimo.StatusEmprestimo = novoStatus;
            await _uof.emprestimoRepository.UpdateEmprestimoStatus(emprestimo);
            await _uof.CommitAsync();

            exemplar.StatusExemplar = StatusExemplar.Disponivel;
            await _uof.exemplarRepository.UpdateExemplar(exemplar);
            await _uof.CommitAsync();

            if(livro.StatusLivro != StatusLivro.Disponivel)
            {
                livro.StatusLivro = StatusLivro.Disponivel;
                await _uof.livroRepository.UpdateLivro(livro);
                await _uof.CommitAsync();
            }
            
          
            var emprestimoResponseDto = _mapper.Map<EmprestimoResponseDTO>(emprestimo);

            return Ok(emprestimoResponseDto);
        }




    }
}
            

