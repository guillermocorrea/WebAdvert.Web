namespace WebAdvert.Web.ServiceClients
{
    public class CreateAdvertDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string UserName { get; set; }
    }
}