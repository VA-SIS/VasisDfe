using System.Net.Http.Json;
using System.Text.Json;

namespace Vasis.MDFe.Web.Client.Services
{
    public class ConfiguracaoService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ConfiguracaoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
        }

        public async Task<object?> ObterConfiguracoesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/configuracao");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(content, _jsonOptions);
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
                throw new Exception($"Erro ao obter configurações: {ex.Message}", ex);
            }
        }

        public async Task<bool> SalvarConfiguracoesAsync(object configuracoes)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/configuracao", configuracoes, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return true;
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
                throw new Exception($"Erro ao salvar configurações: {ex.Message}", ex);
            }
        }

        public async Task<object?> ObterConfiguracacoesCertificadoAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/configuracao/certificado");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(content, _jsonOptions);
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
                throw new Exception($"Erro ao obter configurações do certificado: {ex.Message}", ex);
            }
        }

        public async Task<object?> ObterConfiguracoesWebServiceAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/configuracao/webservice");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(content, _jsonOptions);
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
                throw new Exception($"Erro ao obter configurações do web service: {ex.Message}", ex);
            }
        }
    }
}