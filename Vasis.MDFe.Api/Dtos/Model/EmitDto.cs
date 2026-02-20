// Vasis.MDFe.Api\Dtos\Model\EmitDto.cs
using Vasis.MDFe.Api.Dtos.Model; // Para EnderEmitDto

namespace Vasis.MDFe.Api.Dtos.Model
{
    public class EmitDto
    {
        public string CNPJ { get; set; }
        public string CPF { get; set; }
        public string IE { get; set; }
        public string XNome { get; set; }
        public string XFant { get; set; }
        public EnderEmitDto EnderEmit { get; set; }
    }
}