// Vasis.MDFe.Api\Dtos\Model\Rodo\InfAnttDto.cs
using System.Collections.Generic;
using Vasis.MDFe.Api.Dtos.Model; // Para ValePedDto
using Vasis.MDFe.Api.Dtos.Model.Rodo; // Para InfCiotDto, InfContratanteDto, InfPagDto

namespace Vasis.MDFe.Api.Dtos.Model.Rodo
{
    public class InfAnttDto
    {
        public string RNTRC { get; set; }
        public List<InfCiotDto> InfCIOT { get; set; }
        public ValePedDto ValePed { get; set; } // ValePedDto é de Dtos/Model/
        public List<InfContratanteDto> InfContratante { get; set; }
        public List<InfPagDto> InfPag { get; set; }
    }
}