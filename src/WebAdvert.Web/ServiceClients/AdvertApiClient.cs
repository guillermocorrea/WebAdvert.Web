using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using AutoMapper;
using AdvertApi.Models;

namespace WebAdvert.Web.ServiceClients
{
    public class AdvertApiClient : IAdvertApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;
        private readonly IMapper _mapper;
        public AdvertApiClient(IConfiguration configuration, HttpClient client, IMapper mapper)
        {
            _mapper = mapper;
            _client = client;
            _configuration = configuration;
            var baseUrl = _configuration.GetSection("AdvertApi").GetValue<string>("BaseUrl");
            _client.BaseAddress = new System.Uri(baseUrl);
            _client.DefaultRequestHeaders.Add("Content-type", "application/json");
        }

        public async Task<bool> Confirm(ConfirmAdvertDto model)
        {
            var confirmModel = _mapper.Map<ConfirmAdvertModel>(model);
            var jsonModel = JsonConvert.SerializeObject(confirmModel);
            var response = await _client.PutAsync($"{_client.BaseAddress}/{model.Id}", new StringContent(jsonModel)).ConfigureAwait(false);
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<AdvertResponseDto> Create(CreateAdvertDto model)
        {
            var advertApiModel = _mapper.Map<AdvertModel>(model);
            var jsonModel = JsonConvert.SerializeObject(advertApiModel);
            var response = await _client.PostAsync(_client.BaseAddress, new StringContent(jsonModel)).ConfigureAwait(false);
            var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var createResponse = JsonConvert.DeserializeObject<CreateAdvertResponse>(responseJson);
            var advertResponseDto = _mapper.Map<AdvertResponseDto>(createResponse);
            return advertResponseDto;
        }
    }
}