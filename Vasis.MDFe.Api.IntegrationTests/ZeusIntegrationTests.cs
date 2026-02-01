using Xunit; // Para [Fact], [Theory], etc.
using System.Net.Http; // Para HttpClient
using System.Net.Http.Headers; // Para AuthenticationHeaderValue
using System.Text; // Para Encoding
using System.Text.Json; // Para JsonSerializer e JsonSerializerOptions
using System.Threading.Tasks; // Para async/await
using System.Net; // Para HttpStatusCode
using System; // Para Guid

// Namespace do seu projeto de testes de integração.
// É fundamental que este namespace corresponda ao do seu projeto.
namespace Vasis.MDFe.Api.IntegrationTests
{
    // Esta classe de teste herda da nossa IntegrationTestBase, que já configura o HttpClient e o IConfiguration.
    public class ZeusIntegrationTests : IntegrationTestBase
    {
        // O _client, _factory e _configuration já são injetados e configurados pela IntegrationTestBase.
        // Não precisamos declará-los novamente aqui.

        // Construtor: Essencial para que o xUnit injete o TestWebApplicationFactory e a base seja inicializada.
        public ZeusIntegrationTests(TestWebApplicationFactory factory) : base(factory)
        {
            // Qualquer setup adicional específico para os testes do Zeus, se necessário.
            // Por exemplo, setup de um banco de dados de teste, se você estiver usando um.
        }

        [Fact]
        public async Task GetHealthCheck_ReturnsOk()
        {
            // Este é um exemplo de teste para um endpoint público (sem autenticação).
            // Adapte o endpoint "/health" para o seu endpoint de Health Check ou outro público.
            var response = await _client.GetAsync("/health");

            response.EnsureSuccessStatusCode(); // Verifica se o status é 2xx (Ok, Created, etc.)
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Healthy", content); // Exemplo de verificação de conteúdo
        }


        [Fact]
        public async Task GetMDFE_ReturnsOk_WithValidAuthToken()
        {
            // 1. Geramos um token JWT válido usando as configurações do appsettings.Testing.json.
            //    Você pode ajustar o userId e a role conforme o que sua API espera para acessar este endpoint.
            var token = TestJwtTokenGenerator.GenerateToken(_configuration, userId: "admin-zeus", role: "Admin");

            // 2. Anexamos o token ao cabeçalho de autorização do HttpClient.
            //    Este cabeçalho será enviado em todas as requisições subsequentes feitas com _client neste teste.
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 3. Fazemos a requisição HTTP para o endpoint protegido.
            //    Substitua "/api/mdfe/some-valid-id" pelo caminho real do seu endpoint e um ID existente no seu ambiente de teste.
            var response = await _client.GetAsync("/api/mdfe/some-valid-id");

            // 4. Verificamos a resposta.
            response.EnsureSuccessStatusCode(); // Espera um status de sucesso (200 OK, por exemplo)
            var content = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(content), "O conteúdo da resposta do MDF-e não deveria ser vazio.");
            // Adicione mais asserções para validar o conteúdo da resposta do MDF-e.
        }

        [Fact]
        public async Task CreateMDFE_ReturnsCreated_WithValidDataAndAuthToken()
        {
            // 1. Geramos um token JWT válido para um usuário autorizado a criar MDF-e.
            var token = TestJwtTokenGenerator.GenerateToken(_configuration, userId: "editor-zeus", role: "Editor");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 2. Preparamos os dados do MDF-e para a requisição.
            //    ATENÇÃO: Você precisa preencher este objeto 'mdfeData' com a *estrutura EXATA* do seu DTO de criação de MDF-e.
            //    As propriedades devem seguir o padrão de nomenclatura (ex: camelCase ou PascalCase) que sua API espera.
            var mdfeData = new
            {
                // Estrutura de exemplo BEM SIMPLIFICADA e GENÉRICA.
                // Adapte para o DTO de ENTRADA do seu endpoint POST /api/mdfe.
                ChaveAcesso = "99999999999999999999999999999999999999999999", // Chave de acesso de exemplo
                Versao = "3.00",
                Modal = 1, // Exemplo: 1 para Rodoviário
                // Ide - Identificação do MDFe
                Ide = new
                {
                    cUF = 43, // Código da UF do emitente
                    tpAmb = 2, // Tipo de ambiente: 1-Produção, 2-Homologação
                    tpEmit = 1, // Tipo de emitente: 1-Prestador de serviço de transporte, 2-Transportador de Carga Própria
                    tpTransp = 1, // Tipo de transportador: 1-ETC, 2-TAC Independente, 3-CTC
                    mod = 58, // Modelo do documento fiscal eletrônico (58 para MDF-e)
                    serie = 1,
                    nMDF = 123456, // Número do MDFe
                    cMDF = "12345678", // Código numérico que compõe a Chave de Acesso
                    modal = 1, // Modalidade do transporte: 1-Rodoviário
                    dhEmi = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz"), // Data e hora de emissão
                    procEmi = 0, // Processo de emissão: 0-Emissão de MDF-e com aplicativo do contribuinte
                    verProc = "1.00", // Versão do processo de emissão
                    UFIni = "RS", // UF de início da prestação
                    UFFim = "SP", // UF de fim da prestação
                    InfMunCarrega = new[] { // Informações dos Municípios de Carregamento
                        new {
                            cMunCarrega = "4314404", // Código IBGE de Porto Alegre
                            xMunCarrega = "Porto Alegre"
                        }
                    },
                    InfMunDescarga = new[] { // Informações dos Municípios de Descarga
                        new {
                            cMunDescarga = "3550308", // Código IBGE de São Paulo
                            xMunDescarga = "São Paulo"
                        }
                    }
                },
                // Emit - Emitente do MDF-e
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

            // 3. Serializamos os dados para JSON e criamos um StringContent.
            //    PropertyNamingPolicy = JsonNamingPolicy.CamelCase é comum para APIs RESTful em .NET.
            //    Se sua API usa PascalCase ou outro, ajuste ou remova esta opção.
            var jsonContent = JsonSerializer.Serialize(mdfeData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // 4. Fazemos a requisição POST para o endpoint de criação.
            var response = await _client.PostAsync("/api/mdfe", content);

            // 5. Verificamos a resposta.
            Assert.Equal(HttpStatusCode.Created, response.StatusCode); // Espera um status 201 Created para criação bem-sucedida
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(responseContent), "O conteúdo da resposta de criação não deveria ser vazio.");
            // Adicione asserções para validar o corpo da resposta (ex: que retornou o ID do MDF-e criado).
        }

        [Fact]
        public async Task GetMDFEById_ReturnsNotFound_ForNonExistentId_WithValidAuthToken()
        {
            // Geramos um token. Para este teste, a role pode não importar tanto, mas é bom ter.
            var token = TestJwtTokenGenerator.GenerateToken(_configuration, userId: "viewer-zeus", role: "Viewer");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Tentamos buscar um MDF-e com um ID que sabemos que não existe.
            var nonExistentId = Guid.NewGuid(); // Um GUID aleatório garante que não existe.
            var response = await _client.GetAsync($"/api/mdfe/{nonExistentId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode); // Espera um status 404 Not Found
        }

        [Fact]
        public async Task GetMDFE_ReturnsUnauthorized_WithoutAuthToken()
        {
            // Garante que nenhuma autorização está sendo enviada.
            _client.DefaultRequestHeaders.Authorization = null;

            // Tenta acessar um endpoint protegido sem token.
            var response = await _client.GetAsync("/api/mdfe/any-id");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode); // Espera 401 Unauthorized
        }

        // ... Você pode adicionar outros testes aqui, seguindo o mesmo padrão ...
    }
}