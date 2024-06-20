using AutoMapper;
using Biblioteca.DTOs.Request;
using Biblioteca.DTOs.Response;
using Biblioteca.Models;

namespace Biblioteca.Profiles
{
    public class UsuarioProfile : Profile
    {
        public UsuarioProfile() {
            CreateMap<Usuario, UsuarioResponseDTO>();
            CreateMap<UsuarioRequestDTO, Usuario>();
        }
       
    }
}
