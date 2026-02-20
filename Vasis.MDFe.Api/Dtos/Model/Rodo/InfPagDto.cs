// Vasis.MDFe.Api\Dtos\Model\Rodo\InfPagDto.cs
using System;
using System.Collections.Generic;
using Vasis.MDFe.Api.Dtos.Model.Enums; // Para MDFeIndPagEnum, MDFeIndAltoDesempEnum
using Vasis.MDFe.Api.Dtos.Model.Rodo; // Para CompDto, InfPrazoDto, InfBancDto

namespace Vasis.MDFe.Api.Dtos.Model.Rodo
{
    public class InfPagDto
    {
        public string XNome { get; set; }
        public string CPF { get; set; }
        public string CNPJ { get; set; }
        public string IdEstrangeiro { get; set; }
        public List<CompDto> Comp { get; set; }
        public decimal VContratoProxy { get; set; }
        public MDFeIndPagEnum IndPag { get; set; }
        public List<InfPrazoDto> InfPrazo { get; set; }
        public InfBancDto InfBanc { get; set; }
        public MDFeIndAltoDesempEnum IndAltoDesemp { get; set; }
    }
}