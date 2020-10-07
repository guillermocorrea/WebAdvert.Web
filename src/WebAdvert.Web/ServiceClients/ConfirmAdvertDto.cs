using AdvertApi.Models;

namespace WebAdvert.Web.ServiceClients
{
    public class ConfirmAdvertDto
    {
        public string Id { get; set; }
        public string FilePath { get; set; }
        public AdvertStatus Status { get; set; }
    }
}