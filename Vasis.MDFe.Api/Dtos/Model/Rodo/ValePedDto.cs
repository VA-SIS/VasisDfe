// Vasis.MDFe.Api\Dtos\Model\Rodo\ValePedDto.cs
using System.Collections.Generic;
using Vasis.MDFe.Api.Dtos.Model.Rodo; // Para DispDto

namespace Vasis.MDFe.Api.Dtos.Model.Rodo
{
    public class ValePedDto
    {
        public List<DispDto> Disp { get; set; }
    }
}