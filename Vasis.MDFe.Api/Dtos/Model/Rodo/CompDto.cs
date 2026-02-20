// Vasis.MDFe.Api\Dtos\Model\Rodo\CompDto.cs
using Vasis.MDFe.Api.Dtos.Model.Enums; // Para MDFeTpCompEnum

namespace Vasis.MDFe.Api.Dtos.Model.Rodo
{
    public class CompDto
    {
        public MDFeTpCompEnum TpComp { get; set; }
        public decimal VComp { get; set; }
        public string XComp { get; set; }
    }
}