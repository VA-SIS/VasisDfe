// Vasis.MDFe.Api\Dtos\Configuracao\ConfiguracaoDto.cs
using Vasis.MDFe.Api.Dtos.Configuracao; // Para os outros DTOs de configuração

namespace Vasis.MDFe.Api.Dtos.Configuracao
{
    public class ConfiguracaoDto
    {
        public EmpresaConfigDto Empresa { get; set; }
        public CertificadoDigitalConfigDto CertificadoDigital { get; set; }
        public SistemaDFeConfigDto ConfigWebService { get; set; }
        public string DiretorioSalvarXml { get; set; }
        public bool IsSalvarXml { get; set; }
    }
}