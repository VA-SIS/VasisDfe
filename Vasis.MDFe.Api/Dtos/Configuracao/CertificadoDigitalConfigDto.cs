// Vasis.MDFe.Api\Dtos\Configuracao\CertificadoDigitalConfigDto.cs
namespace Vasis.MDFe.Api.Dtos.Configuracao
{
    public class CertificadoDigitalConfigDto
    {
        public string NumeroDeSerie { get; set; }
        public string CaminhoArquivo { get; set; }
        public string Senha { get; set; }
        public bool ManterEmCache { get; set; }
    }
}