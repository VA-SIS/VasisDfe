using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Vasis.MDFe.Common.Models; // DTOs compartilhados

namespace Vasis.MDFe.WebMudBlazor.Client.Services
{
    /// <summary>
    /// Cliente da API para comunicação com o MdfeConfigController da Vasis.MDFe.Api.
    /// </summary>
    public class MdfeConfigApiClient
    {
        private readonly HttpClient _httpClient;

        // O HttpClient é injetado automaticamente pelo ASP.NET Core DI
        // com o BaseAddress configurado no Program.cs.
        public MdfeConfigApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Obtém o status das configurações do MDF-e da API.
        /// </summary>
        /// <returns>Um DTO com o status da configuração do MDF-e, ou null em caso de erro.</returns>
        public async Task<MdfeConfigStatusDto?> GetMdfeConfigurationStatus()
        {
            try
            {
                // Chama o endpoint "api/MdfeConfig/status" da API
                // O BaseAddress já está configurado, então usamos apenas o caminho relativo.
                // Note: O Blazor WebAssembly roda no navegador, então erros de rede/CORS
                // serão visíveis no console do navegador (F12).
                return await _httpClient.GetFromJsonAsync<MdfeConfigStatusDto>("api/MdfeConfig/status");
            }
            catch (HttpRequestException ex)
            {
                // Erros de rede, API indisponível, CORS bloqueado, etc.
                Console.WriteLine($"Erro HTTP ao chamar a API de status do MDF-e: {ex.Message}");
                // No Blazor WASM, não temos um logger de servidor para usar aqui diretamente.
                // A mensagem será visível no console do navegador (F12).
                return null;
            }
            catch (Exception ex)
            {
                // Outros erros inesperados (ex: desserialização do JSON, DTO inválido)
                Console.WriteLine($"Erro inesperado ao obter status da configuração do MDF-e: {ex.Message}");
                return null;
            }
        }
    }
}