using AutoMapper;
using Biblioteca.Models;
using Biblioteca.DTOs;
using Biblioteca.DTOs.Request;
using Biblioteca.DTOs.Response;

namespace Biblioteca.Profiles
{
    public class MappingsProfile : Profile
    {
        public MappingsProfile()
        {
            CreateMap<Livro, LivroResponseDTO>();
            CreateMap<LivroRequestDTO, Livro>();



        }
    }
}
