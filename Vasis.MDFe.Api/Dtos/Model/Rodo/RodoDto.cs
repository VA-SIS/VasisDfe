// Vasis.MDFe.Api\Dtos\Model\Rodo\RodoDto.cs
using System.Collections.Generic;
using Vasis.MDFe.Api.Dtos.Model; // Para ModalBaseDto, ValePedDto (se for direto em Model)
using Vasis.MDFe.Api.Dtos.Model.Enums; // Para seus enums DTOs
using Vasis.MDFe.Api.Dtos.Model.Rodo; // Para InfAnttDto, VeicTracaoDto, VeicReboqueDto, LacreDto

namespace Vasis.MDFe.Api.Dtos.Model.Rodo
{
    public class RodoDto : ModalBaseDto
    {
        public string RNTRC { get; set; }
        public InfAnttDto InfANTT { get; set; }
        public string CIOT { get; set; } // Se for direto aqui
        public VeicTracaoDto VeicTracao { get; set; }
        public List<VeicReboqueDto> VeicReboque { get; set; }
        public List<LacreDto> LacRodo { get; set; } // LacreDto é de Dtos/Model/
        public ValePedDto ValePed { get; set; } // ValePedDto é de Dtos/Model/
        public string CodAgPorto { get; set; }
    }
}