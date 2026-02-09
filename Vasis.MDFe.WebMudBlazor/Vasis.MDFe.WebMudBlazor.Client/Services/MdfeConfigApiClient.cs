// Vasis.MDFe.WebMudBlazor.Client/Services/MdfeConfigApiClient.cs

using System.Net.Http;
using System.Net.Http.Json;
using Vasis.MDFe.Common.Models; // Referência aos DTOs compartilhados

namespace Vasis.MDFe.WebMudBlazor.Client.Services
{
    public class MdfeConfigApiClient
    {
        private readonly HttpClient _httpClient;

        public MdfeConfigApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<MdfeConfigStatusDto?> GetMdfeConfigurationStatus()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<MdfeConfigStatusDto>("api/MdfeConfig/status");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Erro ao chamar a API: {ex.Message}");
                return null;
            }
        }
    }
}