using System.Net.Http.Json;
using System.Text.Json;

namespace Vasis.MDFe.Web.Client.Services
{
    public class EventoService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public EventoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
        }

        public async Task<string> EncerrarMDFeAsync(object eventoData)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/evento/encerrar", eventoData, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Erro HTTP {response.StatusCode}: {errorContent}");
                }
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao encerrar MDFe: {ex.Message}", ex);
            }
        }

        public async Task<string> CancelarMDFeAsync(object eventoData)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/evento/cancelar", eventoData, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Erro HTTP {response.StatusCode}: {errorContent}");
                }
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cancelar MDFe: {ex.Message}", ex);
            }
        }

        public async Task<string> IncluirCondutorAsync(object eventoData)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/evento/incluir-condutor", eventoData, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Erro HTTP {response.StatusCode}: {errorContent}");
                }
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao incluir condutor: {ex.Message}", ex);
            }
        }

        public async Task<string> IncluirDFeAsync(object eventoData)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/evento/incluir-dfe", eventoData, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Erro HTTP {response.StatusCode}: {errorContent}");
                }
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao incluir DF-e: {ex.Message}", ex);
            }
        }
    }
}