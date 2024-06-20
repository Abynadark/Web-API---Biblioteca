using AutoMapper;
using Biblioteca.DTOs.Request;
using Biblioteca.DTOs.Response;
using Biblioteca.Models;

namespace Biblioteca.Profiles
{
    public class RenovacaoProfile : Profile
    {
        public RenovacaoProfile()
        {
            

            CreateMap<RenovacaoRequestDTO, Renovacao>();

            CreateMap<Renovacao, RenovacaoResponseDTO>()
                .ForMember(dest => dest.DataRenovacao, opt => opt.MapFrom(src => src.DataRenovacao))
                .ForMember(dest => dest.NovaDataPrevista, opt => opt.MapFrom(src => src.NovaDataPrevista));


        }
    }
}
