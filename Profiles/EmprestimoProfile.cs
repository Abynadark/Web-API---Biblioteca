using AutoMapper;
using Biblioteca.DTOs.Request;
using Biblioteca.DTOs.Response;
using Biblioteca.Models;

namespace Biblioteca.Profiles
{
    public class EmprestimoProfile : Profile
    {
        public EmprestimoProfile()
        {
            CreateMap<EmprestimoRequestDTO, Emprestimo>()
               .ForMember(dest => dest.Usuario, opt => opt.Ignore()) 
               .ForMember(dest => dest.Exemplar, opt => opt.Ignore()) 
               .ForMember(dest => dest.StatusEmprestimo, opt => opt.MapFrom(src => src.StatusEmprestimo))
               .ForMember(dest => dest.Renovacoes, opt => opt.Ignore()) 
               .ForMember(dest => dest.Multa, opt => opt.Ignore());

            CreateMap<Emprestimo, EmprestimoResponseDTO>()
                .ForMember(dest => dest.UsuarioCPF, opt => opt.MapFrom(src => src.Usuario.CPF))
                .ForMember(dest => dest.DataEmprestimo, opt => opt.MapFrom(src => src.DataEmprestimo))
                .ForMember(dest => dest.DataPrevistaInicial, opt => opt.MapFrom(src => src.DataPrevistaInicial))
                .ForMember(dest => dest.StatusEmprestimo, opt => opt.MapFrom(src => src.StatusEmprestimo))
                .ForMember(dest => dest.Multa, opt => opt.MapFrom(src => src.Multa)) 
                .ForMember(dest => dest.Renovacoes, opt => opt.MapFrom(src => src.Renovacoes)); 

        }
    }
}
