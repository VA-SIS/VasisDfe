// Vasis.MDFe.Api\Dtos\Model\InfModalDto.cs
using Vasis.MDFe.Api.Dtos.Model; // Para ModalBaseDto
using Vasis.MDFe.Api.Dtos.Model.Enums; // Para MDFeVersaoModalEnum

namespace Vasis.MDFe.Api.Dtos.Model
{
    public class InfModalDto
    {
        public MDFeVersaoModalEnum VersaoModal { get; set; }
        public ModalBaseDto Modal { get; set; }
    }
}