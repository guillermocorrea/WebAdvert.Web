using AdvertApi.Models;
using AutoMapper;

namespace WebAdvert.Web.ServiceClients
{
    public class AdvertApiProfile : Profile
    {
        public AdvertApiProfile()
        {
            CreateMap<CreateAdvertDto, AdvertModel>().ReverseMap();
            CreateMap<CreateAdvertDto, CreateAdvertResponse>().ReverseMap();
            CreateMap<ConfirmAdvertDto, ConfirmAdvertModel>().ReverseMap();
        }
    }
}