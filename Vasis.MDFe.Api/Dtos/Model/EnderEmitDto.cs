// Vasis.MDFe.Api\Dtos\Model\EnderEmitDto.cs
using DFe.Classes.Entidades; // Para Estado

namespace Vasis.MDFe.Api.Dtos.Model
{
    public class EnderEmitDto
    {
        public string XLgr { get; set; }
        public string Nro { get; set; }
        public string XCpl { get; set; }
        public string XBairro { get; set; }
        public long CMun { get; set; }
        public string XMun { get; set; }
        public long CEP { get; set; }
        public Estado UF { get; set; }
        public string Fone { get; set; }
        public string Email { get; set; }
    }
}