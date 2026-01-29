using DFe.Classes.Flags;    // Para ServicoMDFeStatusServico
using MDFe.Servicos.StatusServicoMDFe;
// Usings para os projetos de código fonte do Zeus DFe.NET
// Removendo aliases e usando full namespace where needed
using MDFe.Utils.Configuracoes;           // Para MDFeConfiguracao
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Vasis.MDFe.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MdfeServiceController : ControllerBase
    {
        private readonly ILogger<MdfeServiceController> _logger;
        private readonly IConfiguration _configuration; // Para ler os secrets e appsettings

        public MdfeServiceController(ILogger<MdfeServiceController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            // Lógica para garantir a existência do arquivo dummy de certificado durante o desenvolvimento
            string certPathFromSecrets = _configuration["Certificado:Path"];
            if (!string.IsNullOrEmpty(certPathFromSecrets))
            {
                string certDirectory = Path.GetDirectoryName(certPathFromSecrets);
                if (!Directory.Exists(certDirectory))
                {
                    Directory.CreateDirectory(certDirectory);
                    _logger.LogWarning($"Pasta de certificado '{certDirectory}' criada. Certificado será esperado aqui.");
                }
                if (!System.IO.File.Exists(certPathFromSecrets))
                {
                    System.IO.File.WriteAllBytes(certPathFromSecrets, new byte[] { /* bytes de um certificado dummy ou vazio */ });
                    _logger.LogWarning($"Arquivo dummy de certificado criado em: {certPathFromSecrets}. " +
                                       "Este arquivo NÃO É UM CERTIFICADO VÁLIDO e serve apenas para evitar erros de FileNotFound durante a POC. " +
                                       "Você precisará de um certificado PFX válido para operações reais de assinatura.");
                }
            }
        }

        /// <summary>
        /// Endpoint de "Health Check" para verificar a integração básica com o código fonte do Zeus DFe.NET (MDFe).
        /// Tenta instanciar as classes de configuração e serviço do Zeus para MDFe.
        /// NÃO REALIZA COMUNICAÇÃO REAL COM A SEFAZ. Apenas verifica o carregamento e instanciação da biblioteca.
        /// </summary>
        /// <returns>Um status indicando se a integração com o código fonte do Zeus foi bem-sucedida.</returns>
        [HttpGet("source-integration-health")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 500)]
        [Authorize]
        public IActionResult GetSourceIntegrationHealth()
        {
            _logger.LogInformation("Iniciando teste de integração com o código fonte do Zeus DFe.NET (MDF-e).");

            try
            {
                // #region Leitura das Configurações do appsettings.json e secrets.json
                string schemasPath = _configuration["ZeusConfig:SchemasPath"];
                string xmlSavePath = _configuration["ZeusConfig:XmlSavePath"];
                string mdfeVersionString = _configuration["ZeusConfig:MDFeVersaoLayout"];
                string ufEmitenteString = _configuration["ZeusConfig:MDFeUfEmitente"];

                string certificadoPath = _configuration["Certificado:Path"];
                string certificadoPassword = _configuration["Certificado:Password"];
                string empresaCnpj = _configuration["Empresa:Cnpj"];
                // #endregion

                // #region Validação e Conversão das Configurações
                if (string.IsNullOrEmpty(schemasPath) || !Directory.Exists(schemasPath))
                {
                    throw new InvalidOperationException($"Caminho de schemas inválido ou não configurado: '{schemasPath}'. Verifique 'ZeusConfig:SchemasPath' em appsettings.json e se a pasta existe.");
                }
                if (string.IsNullOrEmpty(xmlSavePath) || !Directory.Exists(xmlSavePath))
                {
                    throw new InvalidOperationException($"Caminho para salvar XMLs inválido ou não configurado: '{xmlSavePath}'. Verifique 'ZeusConfig:XmlSavePath' em appsettings.json e se a pasta existe.");
                }
                // Usando o nome totalmente qualificado para o enum VersaoServico do MDFe.Utils.Flags
                if (!Enum.TryParse(mdfeVersionString, out VersaoServico mdfeVersaoLayout))
                {
                    throw new InvalidOperationException($"Versão do layout MDF-e inválida: '{mdfeVersionString}'. Verifique 'ZeusConfig:MDFeVersaoLayout' em appsettings.json.");
                }
                // Usando o nome totalmente qualificado para o enum Estado do DFe.Classes.Entidades
                if (!Enum.TryParse(ufEmitenteString, out DFe.Classes.Entidades.Estado ufEmitente))
                {
                    throw new InvalidOperationException($"UF Emitente inválida: '{ufEmitenteString}'. Verifique 'ZeusConfig:MDFeUfEmitente' em appsettings.json.");
                }
                if (string.IsNullOrEmpty(certificadoPath) || string.IsNullOrEmpty(certificadoPassword) || string.IsNullOrEmpty(empresaCnpj))
                {
                    throw new InvalidOperationException("As configurações de certificado (Path, Password) e do CNPJ da Empresa não foram encontradas em User Secrets. Por favor, verifique se 'Certificado:Path', 'Certificado:Password' e 'Empresa:Cnpj' estão definidos no secrets.json.");
                }
                // #endregion

                // *** PASSO 1: Configurar o singleton MDFeConfiguracao ***
                // Usando o nome totalmente qualificado para o enum TipoAmbiente do DFe.Classes.Flags
                MDFeConfiguracao.Instancia.VersaoWebService.TipoAmbiente = DFe.Classes.Flags.TipoAmbiente.Homologacao;
                MDFeConfiguracao.Instancia.CaminhoSchemas = schemasPath;
                MDFeConfiguracao.Instancia.CaminhoSalvarXml = xmlSavePath;
                MDFeConfiguracao.Instancia.IsSalvarXml = true;
                MDFeConfiguracao.Instancia.VersaoWebService.VersaoLayout = (global::MDFe.Utils.Flags.VersaoServico)mdfeVersaoLayout;
                MDFeConfiguracao.Instancia.VersaoWebService.UfEmitente = ufEmitente;

                _logger.LogInformation($"MDFeConfiguracao.Instancia configurada: Ambiente={MDFeConfiguracao.Instancia.VersaoWebService.TipoAmbiente}, Layout={MDFeConfiguracao.Instancia.VersaoWebService.VersaoLayout}, UF={MDFeConfiguracao.Instancia.VersaoWebService.UfEmitente}.");

                // *** PASSO 2: Configurar o certificado digital a partir dos secrets ***
                // Usando o nome totalmente qualificado para ConfiguracaoCertificado e TipoCertificado do DFe.Utils
                MDFeConfiguracao.Instancia.ConfiguracaoCertificado = new DFe.Utils.ConfiguracaoCertificado
                {
                    TipoCertificado = DFe.Utils.TipoCertificado.A1Arquivo,
                    Arquivo = certificadoPath,
                    Senha = certificadoPassword
                };
                _logger.LogInformation($"ConfiguracaoCertificado atribuída. Caminho: {MDFeConfiguracao.Instancia.ConfiguracaoCertificado.Arquivo}.");

                // Testar se o certificado pode ser carregado (valida path/senha e funcionamento do DFe.Utils.Assinatura.CertificadoDigital)
                System.Security.Cryptography.X509Certificates.X509Certificate2 loadedCert = DFe.Utils.Assinatura.CertificadoDigital.ObterCertificado(MDFeConfiguracao.Instancia.ConfiguracaoCertificado);
                _logger.LogInformation($"Certificado digital carregado com sucesso (teste de validação de path/senha). Subject: {loadedCert.Subject}.");

                // *** PASSO 3: Instanciar um serviço específico do MDF-e (ex: ServicoMDFeStatusServico) ***
                var statusServicoMdfe = new ServicoMDFeStatusServico();

                _logger.LogInformation("ServicoMDFeStatusServico instanciado com sucesso do código fonte Zeus.");

                return Ok("SUCESSO NA POC DE INTEGRAÇÃO DO FONTE: As classes do Zeus DFe.NET (MDF-e) foram instanciadas e " +
                          "o certificado foi configurado a partir dos SECRETS/APPSETTINGS na sua Web API .NET 8.0. " +
                          "A base para o desenvolvimento do MDF-e está sólida.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FALHA CRÍTICA NA POC DE INTEGRAÇÃO DO FONTE Zeus DFe.NET (MDF-e).");

                string errorMessage = $"ERRO CRÍTICO DE INTEGRAÇÃO: {ex.Message}. \n";
                errorMessage += $"Tipo do Erro: {ex.GetType().FullName}. \n";

                if (ex is System.Reflection.ReflectionTypeLoadException rtle)
                {
                    errorMessage += "--- Loader Exceptions (detalhes de carregamento de tipo):\n";
                    foreach (var lex in rtle.LoaderExceptions)
                    {
                        errorMessage += $"  - {lex?.Message}\n";
                        if (lex?.InnerException != null) errorMessage += $"    Inner: {lex.InnerException.Message}\n";
                    }
                }
                else if (ex is System.IO.FileLoadException fle)
                {
                    errorMessage += $"Tentou carregar o arquivo: {fle.FileName}. \n";
                }
                else if (ex is System.IO.FileNotFoundException fnfe)
                {
                    errorMessage += $"Arquivo não encontrado: {fnfe.FileName}. \n";
                }
                else if (ex is System.Security.Cryptography.CryptographicException ce)
                {
                    errorMessage += $"Erro ao carregar o certificado. Verifique o caminho e a senha no secrets.json. Detalhes: {ce.Message}\n";
                }
                else if (ex.InnerException != null)
                {
                    errorMessage += $"Inner Exception: {ex.InnerException.Message}. \n";
                }
                errorMessage += $"StackTrace: {ex.StackTrace}. \n\n" +
                                $"Este erro indica um problema de configuração, compatibilidade de TargetFramework ou de dependências entre seu projeto .NET 8.0 e os projetos de código fonte do Zeus. \n" +
                                $"Verifique o '.csproj' de DFe.Classes, DFe.Utils, MDFe.Classes, MDFe.Utils, MDFe.Servicos, etc., para garantir que todos são '.NET Standard 2.0/2.1' ou '.NET 6/7/8'.";

                return StatusCode(500, errorMessage);
            }
        }
    }
}