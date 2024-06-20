using AutoMapper;
using Biblioteca.DTOs.Request;
using Biblioteca.DTOs.Response;
using Biblioteca.Models;

namespace Biblioteca.Profiles
{
    public class MultaProfile : Profile
    {
        public MultaProfile(){

           
            CreateMap<MultaRequestDTO, Multa>();
            CreateMap<Multa, MultaResponseDTO>()
               .ForMember(dest => dest.Valor, opt => opt.MapFrom(src => src.Valor))
               .ForMember(dest => dest.DiasAtrasados, opt => opt.MapFrom(src => src.DiasAtrasados))
               .ForMember(dest => dest.StatusMulta, opt => opt.MapFrom(src => src.StatusMulta));

        }
    }
}
