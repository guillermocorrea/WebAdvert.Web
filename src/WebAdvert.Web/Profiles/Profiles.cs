using AutoMapper;
using WebAdvert.Web.Models.AdvertManagement;
using WebAdvert.Web.ServiceClients;

namespace WebAdvert.Web.Profiles
{
    public class Profiles : Profile
    {
        public Profiles()
        {
            CreateMap<CreateAdvertViewModel, CreateAdvertDto>().ReverseMap();
        }
    }
}