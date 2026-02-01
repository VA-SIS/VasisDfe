using Xunit;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net;
using System;
using System.Collections.Generic; // Para List<T> se necessário

namespace Vasis.MDFe.Api.IntegrationTests
{
    public class ZeusIntegrationTests : IntegrationTestBase
    {
        public ZeusIntegrationTests(TestWebApplicationFactory factory) : base(factory)
        {
            // Qualquer setup específico para ZeusIntegrationTests, se necessário.
        }

        [Fact]
        public async Task GetHealthCheck_ReturnsOk()
        {
            // IMPORTANTE: Substitua "/health" pelo caminho real do seu endpoint de health check da API.
            var response = await _client.GetAsync("/health");

            response.EnsureSuccessStatusCode(); // Espera 2xx
            var content = await response.Content.ReadAsStringAsync();
            // IMPORTANTE: Ajuste a asserção com base no que seu endpoint de health check realmente retorna.
            Assert.Contains("Healthy", content); // Exemplo: verifica se a string "Healthy" está presente
        }

        [Fact]
        public async Task GetMDFE_ReturnsOk_WithValidAuthToken()
        {
            // Gera um token JWT válido. Ajuste userId e role com base na autorização da sua API.
            var token = TestJwtTokenGenerator.GenerateToken(_configuration, userId: "zeus-admin", role: "Admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // IMPORTANTE: Substitua "/api/mdfe/some-valid-id" pelo caminho real do seu endpoint
            // e certifique-se de que 'some-valid-id' representa um ID que *deveria* existir no seu ambiente de teste.
            var response = await _client.GetAsync("/api/mdfe/some-valid-id");

            response.EnsureSuccessStatusCode(); // Espera 2xx
            var content = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(content), "O conteúdo da resposta do MDF-e não deveria ser vazio.");
            // IMPORTANTE: Adicione asserções mais específicas para validar a estrutura e o conteúdo da resposta do MDF-e.
            // Exemplo: Assert.Contains("ExpectedMDFEProperty", content);
        }

        [Fact]
        public async Task CreateMDFE_ReturnsCreated_WithValidDataAndAuthToken()
        {
            var token = TestJwtTokenGenerator.GenerateToken(_configuration, userId: "editor-zeus", role: "Editor");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // IMPORTANTE: Este objeto 'mdfeData' *DEVE* corresponder à estrutura EXATA
            // do seu DTO (Data Transfer Object) de criação de MDF-e que sua API espera.
            // Os nomes das propriedades devem corresponder ao que o deserializador JSON da sua API espera (camelCase ou PascalCase).
            // O exemplo abaixo é genérico e precisa ser totalmente ADAPTADO ao seu modelo.
            var mdfeData = new
            {
                ChaveAcesso = "99999999999999999999999999999999999999999999", // Chave de acesso de exemplo
                Versao = "3.00",
                Modal = 1, // Exemplo: 1 para Rodoviário
                Ide = new
                {
                    cUF = 43, // Código da UF do emitente (RS)
                    tpAmb = 2, // Tipo de ambiente: 1-Produção, 2-Homologação
                    tpEmit = 1, // Tipo de emitente: 1-Prestador de serviço de transporte, 2-Transportador de Carga Própria
                    tpTransp = 1, // Tipo de transportador: 1-ETC, 2-TAC Independente, 3-CTC
                    mod = 58, // Modelo do documento fiscal eletrônico (58 para MDF-e)
                    serie = 1,
                    nMDF = 123456, // Número do MDFe
                    cMDF = "12345678", // Código numérico que compõe a Chave de Acesso
                    modal = 1, // Modalidade do transporte: 1-Rodoviário (1-Rodoviário, 2-Aéreo, 3-Aquaviário, 4-Ferroviário)
                    dhEmi = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz"), // Data e hora de emissão em formato ISO 8601
                    procEmi = 0, // Processo de emissão: 0-Emissão de MDF-e com aplicativo do contribuinte
                    verProc = "1.00", // Versão do processo de emissão
                    UFIni = "RS", // UF de início da prestação
                    UFFim = "SP", // UF de fim da prestação
                    InfMunCarrega = new List<object> { // Informações dos Municípios de Carregamento
                        new {
                            cMunCarrega = "4314404", // Código IBGE de Porto Alegre
                            xMunCarrega = "Porto Alegre"
                        }
                    },
                    InfMunDescarga = new List<object> { // Informações dos Municípios de Descarga
                        new {
                            cMunDescarga = "3550308", // Código IBGE de São Paulo
                            xMunDescarga = "São Paulo"
                        }
                    }
                },
                Emit = new
                {
                    CNPJ = "00000000000000", // CNPJ do emitente
                    IE = "123456789", // Inscrição Estadual
                    XNome = "Minha Empresa Teste", // Razão Social
                    XFant = "Fantasia Teste", // Nome Fantasia
                    xEnder = new
                    {
                        XLgr = "Rua Teste",
                        Nro = "100",
                        XBairro = "Centro",
                        CMun = "4314404", // Código IBGE de Porto Alegre
                        XMun = "Porto Alegre",
                        CEP = "90000000",
                        UF = "RS",
                        Fone = "5133333333"
                    }
                }
                // Adicione outras seções do DTO do MDF-e conforme a sua API espera (ex: InfDoc, Rodoviario, etc.)
            };

            // Serializa os dados para JSON, usando camelCase para as propriedades.
            // Se sua API espera PascalCase ou outro padrão, ajuste a opção de JsonSerializerOptions.
            var jsonContent = JsonSerializer.Serialize(mdfeData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // IMPORTANTE: Substitua "/api/mdfe" pelo endpoint real de criação da sua API.
            var response = await _client.PostAsync("/api/mdfe", content);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode); // Espera 201 Created para criação bem-sucedida
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(responseContent), "O conteúdo da resposta de criação não deveria ser vazio.");
            // IMPORTANTE: Adicione asserções para validar o corpo da resposta (ex: que retornou o ID do MDF-e criado).
        }

        [Fact]
        public async Task GetMDFEById_ReturnsNotFound_ForNonExistentId_WithValidAuthToken()
        {
            var token = TestJwtTokenGenerator.GenerateToken(_configuration, userId: "viewer-zeus", role: "Viewer");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Um GUID aleatório garante que não existe um MDF-e com este ID.
            var nonExistentId = Guid.NewGuid();
            var response = await _client.GetAsync($"/api/mdfe/{nonExistentId}"); // Endpoint de busca por ID

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode); // Espera 404 Not Found
        }

        [Fact]
        public async Task GetMDFE_ReturnsUnauthorized_WithoutAuthToken()
        {
            // Garante que não há token de autorização.
            _client.DefaultRequestHeaders.Authorization = null;
            // Tenta acessar um endpoint protegido sem token.
            var response = await _client.GetAsync("/api/mdfe/any-id");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode); // Espera 401 Unauthorized
        }
    }
}