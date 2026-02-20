// Vasis.MDFe.Api\Dtos\Configuracao\EmpresaConfigDto.cs
using DFe.Classes.Entidades; // Para Estado

namespace Vasis.MDFe.Api.Dtos.Configuracao
{
    public class EmpresaConfigDto
    {
        public string Cnpj { get; set; }
        public string InscricaoEstadual { get; set; }
        public string Nome { get; set; }
        public string NomeFantasia { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public long CodigoIbgeMunicipio { get; set; }
        public string NomeMunicipio { get; set; }
        public string Cep { get; set; }
        public Estado SiglaUf { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public string RNTRC { get; set; }
    }
}