using System.Threading.Tasks;

namespace WebAdvert.Web.ServiceClients
{
    public interface IAdvertApiClient
    {
        Task<AdvertResponseDto> Create(CreateAdvertDto model);
        Task<bool> Confirm(ConfirmAdvertDto model);
    }
}