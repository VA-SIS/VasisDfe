using System.Text.Json.Serialization; // Necessário para JsonPropertyName, embora não usado diretamente aqui, é boa prática para DTOs.
using System.ComponentModel.DataAnnotations; // Usado para validação de DTOs, se necessário.
                                             // Pode ser removido se não for usar validação no DTO, mas é útil.

namespace Vasis.MDFe.Common.Models
{
    /// <summary>
    /// DTO que representa o status completo da configuração do MDF-e.
    /// Usado para ser compartilhado entre o backend (API) e o frontend (Blazor).
    /// </summary>
    public class MdfeConfigStatusDto
    {
        /// <summary>
        /// Indica se a configuração geral do MDF-e é válida.
        /// </summary>
        public bool MdfeConfigIsValid { get; set; }

        /// <summary>
        /// Detalhes da configuração do certificado digital.
        /// </summary>
        // [Required] // Descomente se quiser exigir essa propriedade para validação de modelo
        public CertificadoDigitalConfigDto CertificadoDigital { get; set; } = new CertificadoDigitalConfigDto();

        /// <summary>
        /// Detalhes da empresa emitente.
        /// </summary>
        // [Required] // Descomente se quiser exigir essa propriedade para validação de modelo
        public EmpresaEmitenteConfigDto EmpresaEmitente { get; set; } = new EmpresaEmitenteConfigDto();

        /// <summary>
        /// Detalhes da configuração do sistema DFe.
        /// </summary>
        // [Required] // Descomente se quiser exigir essa propriedade para validação de modelo
        public SistemaDFeConfigDto SistemaDFe { get; set; } = new SistemaDFeConfigDto();
    }

    /// <summary>
    /// DTO para informações do certificado digital.
    /// </summary>
    public class CertificadoDigitalConfigDto
    {
        public bool UsaWindowsStore { get; set; }
        public string? NomeArquivoCertificado { get; set; }
        public string? CaminhoCompletoArquivo { get; set; }
        public string? Thumbprint { get; set; }
        public string? StoreLocation { get; set; } // Representa StoreLocation.CurrentUser/LocalMachine
        public string? StoreName { get; set; }     // Representa StoreName.My
        public bool SenhaPresente { get; set; }
    }

    /// <summary>
    /// DTO para informações da empresa emitente.
    /// </summary>
    public class EmpresaEmitenteConfigDto
    {
        public string? CNPJ { get; set; }
        public string? InscricaoEstadual { get; set; }
        public string? RazaoSocial { get; set; }
        public string? NomeFantasia { get; set; }
        public string? EnderecoLogradouro { get; set; }
        public string? EnderecoNumero { get; set; }
        public string? EnderecoComplemento { get; set; }
        public string? EnderecoBairro { get; set; }
        public int EnderecoCodigoMunicipio { get; set; }
        public string? EnderecoNomeMunicipio { get; set; }
        public string? EnderecoUF { get; set; } // Representa o enum Estado
        public string? EnderecoCEP { get; set; }
        public string? Telefone { get; set; }
        public string? Email { get; set; }
        public string? RNTRC { get; set; }
    }

    /// <summary>
    /// DTO para informações do sistema DFe.
    /// </summary>
    public class SistemaDFeConfigDto
    {
        public string? TipoAmbiente { get; set; } // Representa o enum TipoAmbiente
        public string? VersaoLayoutMDFe { get; set; } // Representa o enum VersaoServico
        public string? PastaSchemas { get; set; }
        public string? CaminhoCompletoSchemas { get; set; }
        public string? PastaSalvarXml { get; set; }
        public string? CaminhoCompletoSalvarXml { get; set; }
        public bool IsSalvarXml { get; set; }
        public int TimeOutServicoMs { get; set; }
        public string? UFEmitente { get; set; } // Representa o enum Estado
    }
}