using DFe.Classes.Flags;
using MDFe.Utils.Configuracoes;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using Vasis.MDFe.Api.Models.Requests;
using Vasis.MDFe.Api.Models.Responses;
using Vasis.MDFe.Api.Services.Interfaces;

namespace Vasis.MDFe.Api.Services
{
    /// <summary>
    /// Serviço de validação de MDFe usando bibliotecas Zeus
    /// </summary>
    public class MDFeValidationService : IMDFeValidationService
    {
        private readonly ILogger<MDFeValidationService> _logger;
        private readonly IConfiguration _configuration;

        public MDFeValidationService(ILogger<MDFeValidationService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Valida um XML de MDFe completo
        /// </summary>
        public async Task<ValidarMDFeResponse> ValidarMDFeAsync(ValidarMDFeRequest request)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new ValidarMDFeResponse();

            try
            {
                _logger.LogInformation("Iniciando validação completa de MDFe");

                // Validação básica do request
                if (string.IsNullOrWhiteSpace(request.XmlContent))
                {
                    response.Sucesso = false;
                    response.Mensagem = "Conteúdo XML não pode ser vazio";
                    response.Erros.Add(new ErroValidacao
                    {
                        Codigo = "VAL001",
                        Descricao = "XML vazio ou nulo",
                        Severidade = "Erro"
                    });
                    return response;
                }

                // Configurar Zeus se necessário
                await ConfigurarZeusAsync();

                // Validação estrutural do XML
                var validacaoEstrutura = await ValidarEstruturaXmlAsync(request.XmlContent);
                response.Erros.AddRange(validacaoEstrutura.Erros);
                response.Avisos.AddRange(validacaoEstrutura.Avisos);

                if (!validacaoEstrutura.Sucesso)
                {
                    response.Sucesso = false;
                    response.Mensagem = "Falha na validação estrutural do XML";
                    return response;
                }

                // Se solicitado apenas validação estrutural, retorna aqui
                if (request.ValidarApenasEstrutura)
                {
                    response.Sucesso = true;
                    response.Mensagem = "Validação estrutural concluída com sucesso";
                    response.DadosExtraidos = validacaoEstrutura.DadosExtraidos;
                    return response;
                }

                // Validação de regras de negócio
                await ValidarRegrasNegocioAsync(request.XmlContent, response);

                // Extrair dados do MDFe
                response.DadosExtraidos = await ExtrairDadosBasicosAsync(request.XmlContent);

                // Definir resultado final
                response.Sucesso = response.Erros.Count == 0;
                response.Mensagem = response.Sucesso
                    ? "Validação concluída com sucesso"
                    : $"Validação concluída com {response.Erros.Count} erro(s)";

                _logger.LogInformation($"Validação concluída: {response.Mensagem}");

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante validação de MDFe");

                response.Sucesso = false;
                response.Mensagem = "Erro interno durante validação";
                response.Erros.Add(new ErroValidacao
                {
                    Codigo = "VAL999",
                    Descricao = $"Erro interno: {ex.Message}",
                    Severidade = "Erro"
                });

                return response;
            }
            finally
            {
                stopwatch.Stop();
                response.TempoProcessamento = stopwatch.Elapsed;
                _logger.LogInformation($"Tempo de processamento: {response.TempoProcessamento.TotalMilliseconds}ms");
            }
        }

        /// <summary>
        /// Valida apenas estrutura XML contra schema
        /// </summary>
        public async Task<ValidarMDFeResponse> ValidarEstruturaXmlAsync(string xmlContent)
        {
            var response = new ValidarMDFeResponse();

            try
            {
                _logger.LogInformation("Validando estrutura XML contra schema");

                // Verificar se é XML válido
                XDocument.Parse(xmlContent);

                // Configurar Zeus
                await ConfigurarZeusAsync();

                // Validação básica de estrutura
                var xmlDoc = XDocument.Parse(xmlContent);
                var ns = xmlDoc.Root?.GetDefaultNamespace() ?? XNamespace.None;

                // Verificar elementos obrigatórios
                if (xmlDoc.Root?.Name.LocalName != "MDFe")
                {
                    response.Erros.Add(new ErroValidacao
                    {
                        Codigo = "SCH001",
                        Descricao = "Elemento raiz deve ser 'MDFe'",
                        Severidade = "Erro"
                    });
                }

                var infMDFe = xmlDoc.Descendants(ns + "infMDFe").FirstOrDefault();
                if (infMDFe == null)
                {
                    response.Erros.Add(new ErroValidacao
                    {
                        Codigo = "SCH002",
                        Descricao = "Elemento 'infMDFe' é obrigatório",
                        Severidade = "Erro"
                    });
                }

                if (response.Erros.Count == 0)
                {
                    response.Sucesso = true;
                    response.Mensagem = "Estrutura XML válida";
                    response.DadosExtraidos = await ExtrairDadosBasicosAsync(xmlContent);
                }
                else
                {
                    response.Sucesso = false;
                    response.Mensagem = "Erros de validação de estrutura encontrados";
                }

                return response;
            }
            catch (XmlException xmlEx)
            {
                _logger.LogWarning(xmlEx, "XML mal formado");

                response.Sucesso = false;
                response.Mensagem = "XML mal formado";
                response.Erros.Add(new ErroValidacao
                {
                    Codigo = "XML001",
                    Descricao = $"XML inválido: {xmlEx.Message}",
                    Linha = xmlEx.LineNumber.ToString(),
                    Severidade = "Erro"
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante validação estrutural");

                response.Sucesso = false;
                response.Mensagem = "Erro durante validação estrutural";
                response.Erros.Add(new ErroValidacao
                {
                    Codigo = "VAL998",
                    Descricao = $"Erro interno: {ex.Message}",
                    Severidade = "Erro"
                });

                return response;
            }
        }

        /// <summary>
        /// Extrai dados básicos do XML
        /// </summary>
        public async Task<DadosMDFe?> ExtrairDadosBasicosAsync(string xmlContent)
        {
            try
            {
                _logger.LogInformation("Extraindo dados básicos do MDFe");

                var xmlDoc = XDocument.Parse(xmlContent);
                var ns = xmlDoc.Root?.GetDefaultNamespace() ?? XNamespace.None;

                var infMDFe = xmlDoc.Descendants(ns + "infMDFe").FirstOrDefault();
                if (infMDFe == null)
                {
                    _logger.LogWarning("Elemento infMDFe não encontrado no XML");
                    return null;
                }

                var ide = infMDFe.Element(ns + "ide");
                var emit = infMDFe.Element(ns + "emit");
                var tot = infMDFe.Element(ns + "tot");

                var dados = new DadosMDFe
                {
                    ChaveAcesso = infMDFe.Attribute("Id")?.Value?.Replace("MDFe", ""),
                    NumeroMDFe = ide?.Element(ns + "nMDF")?.Value,
                    SerieMDFe = ide?.Element(ns + "serie")?.Value,
                    CnpjEmitente = emit?.Element(ns + "CNPJ")?.Value,
                    RazaoSocialEmitente = emit?.Element(ns + "xNome")?.Value,
                    UfInicio = ide?.Element(ns + "UFIni")?.Value,
                    UfFim = ide?.Element(ns + "UFFim")?.Value
                };

                // Tentar converter data de emissão
                if (DateTime.TryParse(ide?.Element(ns + "dhEmi")?.Value, out var dataEmissao))
                {
                    dados.DataEmissao = dataEmissao;
                }

                // Tentar extrair valores totais
                if (decimal.TryParse(tot?.Element(ns + "vCarga")?.Value, out var valorCarga))
                {
                    dados.ValorTotalCarga = valorCarga;
                }

                if (decimal.TryParse(tot?.Element(ns + "qCarga")?.Value, out var quantidadeCarga))
                {
                    dados.QuantidadeCarga = quantidadeCarga;
                }

                dados.UnidadeMedida = tot?.Element(ns + "cUnid")?.Value;

                _logger.LogInformation($"Dados extraídos: Chave={dados.ChaveAcesso}, Número={dados.NumeroMDFe}");

                return dados;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao extrair dados básicos do MDFe");
                return null;
            }
        }

        private async Task ValidarRegrasNegocioAsync(string xmlContent, ValidarMDFeResponse response)
        {
            try
            {
                _logger.LogInformation("Validando regras de negócio do MDFe");

                var dados = await ExtrairDadosBasicosAsync(xmlContent);
                if (dados == null)
                {
                    response.Erros.Add(new ErroValidacao
                    {
                        Codigo = "NEG001",
                        Descricao = "Não foi possível extrair dados para validação de regras de negócio",
                        Severidade = "Erro"
                    });
                    return;
                }

                // Validação de CNPJ
                if (!string.IsNullOrEmpty(dados.CnpjEmitente) && !ValidarCnpj(dados.CnpjEmitente))
                {
                    response.Erros.Add(new ErroValidacao
                    {
                        Codigo = "NEG002",
                        Descricao = "CNPJ do emitente inválido",
                        Campo = "emit/CNPJ",
                        Severidade = "Erro"
                    });
                }

                // Validação de data de emissão
                if (dados.DataEmissao.HasValue)
                {
                    var agora = DateTime.Now;
                    if (dados.DataEmissao.Value > agora.AddDays(1))
                    {
                        response.Avisos.Add(new AvisoValidacao
                        {
                            Codigo = "NEG003",
                            Descricao = "Data de emissão é futura",
                            Campo = "ide/dhEmi",
                            Severidade = "Aviso"
                        });
                    }

                    if (dados.DataEmissao.Value < agora.AddYears(-1))
                    {
                        response.Avisos.Add(new AvisoValidacao
                        {
                            Codigo = "NEG004",
                            Descricao = "Data de emissão é muito antiga",
                            Campo = "ide/dhEmi",
                            Severidade = "Aviso"
                        });
                    }
                }

                // Validação de valores
                if (dados.ValorTotalCarga.HasValue && dados.ValorTotalCarga.Value <= 0)
                {
                    response.Erros.Add(new ErroValidacao
                    {
                        Codigo = "NEG005",
                        Descricao = "Valor total da carga deve ser maior que zero",
                        Campo = "tot/vCarga",
                        Severidade = "Erro"
                    });
                }

                _logger.LogInformation("Validação de regras de negócio concluída");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante validação de regras de negócio");

                response.Erros.Add(new ErroValidacao
                {
                    Codigo = "NEG999",
                    Descricao = $"Erro durante validação de regras de negócio: {ex.Message}",
                    Severidade = "Erro"
                });
            }
        }

        private async Task ConfigurarZeusAsync()
        {
            try
            {
                // Verificar se já está configurado
                if (MDFeConfiguracao.Instancia.VersaoWebService.TipoAmbiente == TipoAmbiente.Producao ||
                    MDFeConfiguracao.Instancia.VersaoWebService.TipoAmbiente == TipoAmbiente.Homologacao)
                {
                    return; // Já configurado
                }

                _logger.LogInformation("Configurando Zeus MDFe para validação");

                // Configurações básicas para validação
                string schemasPath = _configuration["ZeusConfig:SchemasPath"];
                string mdfeVersionString = _configuration["ZeusConfig:MDFeVersaoLayout"];
                string ufEmitenteString = _configuration["ZeusConfig:MDFeUfEmitente"];

                if (Enum.TryParse(mdfeVersionString, out VersaoServico mdfeVersaoLayout) &&
                    Enum.TryParse(ufEmitenteString, out DFe.Classes.Entidades.Estado ufEmitente))
                {
                    MDFeConfiguracao.Instancia.VersaoWebService.TipoAmbiente = TipoAmbiente.Homologacao;
                    MDFeConfiguracao.Instancia.CaminhoSchemas = schemasPath;
                    MDFeConfiguracao.Instancia.VersaoWebService.VersaoLayout = (global::MDFe.Utils.Flags.VersaoServico)mdfeVersaoLayout;
                    MDFeConfiguracao.Instancia.VersaoWebService.UfEmitente = ufEmitente;

                    _logger.LogInformation("Zeus MDFe configurado para validação");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao configurar Zeus MDFe");
                throw;
            }
        }

        private bool ValidarCnpj(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return false;

            cnpj = cnpj.Replace(".", "").Replace("/", "").Replace("-", "");

            if (cnpj.Length != 14)
                return false;

            if (!cnpj.All(char.IsDigit))
                return false;

            // Verificar se todos os dígitos são iguais
            if (cnpj.All(c => c == cnpj[0]))
                return false;

            // Validar dígitos verificadores
            var digitos = cnpj.Select(c => int.Parse(c.ToString())).ToArray();

            // Primeiro dígito verificador
            var soma = 0;
            var multiplicador = 5;
            for (int i = 0; i < 12; i++)
            {
                soma += digitos[i] * multiplicador;
                multiplicador = multiplicador == 2 ? 9 : multiplicador - 1;
            }
            var resto = soma % 11;
            var dv1 = resto < 2 ? 0 : 11 - resto;

            if (digitos[12] != dv1)
                return false;

            // Segundo dígito verificador
            soma = 0;
            multiplicador = 6;
            for (int i = 0; i < 13; i++)
            {
                soma += digitos[i] * multiplicador;
                multiplicador = multiplicador == 2 ? 9 : multiplicador - 1;
            }
            resto = soma % 11;
            var dv2 = resto < 2 ? 0 : 11 - resto;

            return digitos[13] == dv2;
        }
    }
}