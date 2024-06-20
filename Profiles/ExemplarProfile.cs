using AutoMapper;
using Biblioteca.DTOs.Request;
using Biblioteca.DTOs.Response;
using Biblioteca.Models;

namespace Biblioteca.Profiles
{
    public class ExemplarProfile : Profile
    {
        public ExemplarProfile()
        {

            CreateMap<Exemplar, ExemplarResponseDTO>()
                .ForMember(dest => dest.LivroISBN10, opt => opt.MapFrom(src => src.Livro.ISBN10))
                .ForMember(dest => dest.Livro, opt => opt.MapFrom(src => src.Livro)); 

            CreateMap<ExemplarRequestDTO, Exemplar>()
                .ForMember(dest => dest.Livro, opt => opt.Ignore());

            CreateMap<Livro, LivroResponseDTO>(); 
        }
    }
}
