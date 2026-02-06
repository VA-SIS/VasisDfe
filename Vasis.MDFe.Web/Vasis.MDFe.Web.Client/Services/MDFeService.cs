using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Vasis.MDFe.Web.Client.Services
{
    public class MDFeService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public MDFeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
        }

        public async Task<string> CriarMDFeAsync(object mdfeData)
        {
            try
            {
                var json = JsonSerializer.Serialize(mdfeData, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/mdfe/criar", content);

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
                throw new Exception($"Erro ao criar MDFe: {ex.Message}", ex);
            }
        }

        public async Task<string> ValidarMDFeAsync(string xml)
        {
            try
            {
                var requestData = new { xml = xml };
                var response = await _httpClient.PostAsJsonAsync("api/mdfe/validar", requestData, _jsonOptions);

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
                throw new Exception($"Erro ao validar MDFe: {ex.Message}", ex);
            }
        }

        public async Task<string> AssinarMDFeAsync(string xml)
        {
            try
            {
                var requestData = new { xml = xml };
                var response = await _httpClient.PostAsJsonAsync("api/mdfe/assinar", requestData, _jsonOptions);

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
                throw new Exception($"Erro ao assinar MDFe: {ex.Message}", ex);
            }
        }

        public async Task<string> TransmitirMDFeAsync(string xml)
        {
            try
            {
                var requestData = new { xml = xml };
                var response = await _httpClient.PostAsJsonAsync("api/mdfe/transmitir", requestData, _jsonOptions);

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
                throw new Exception($"Erro ao transmitir MDFe: {ex.Message}", ex);
            }
        }

        public async Task<string> ConsultarMDFeAsync(string chave)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/mdfe/consultar/{chave}");

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
                throw new Exception($"Erro ao consultar MDFe: {ex.Message}", ex);
            }
        }

        public async Task<bool> TestarConexaoAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/health");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}