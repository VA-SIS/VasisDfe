// Vasis.MDFe.Api\Dtos\Model\Rodo\DispDto.cs
using System.Collections.Generic; // Para List, se necessário
using Vasis.MDFe.Api.Dtos.Model.Enums; // <<-- ESTE USING É CRUCIAL para encontrar o enum

namespace Vasis.MDFe.Api.Dtos.Model.Rodo
{
    public class DispDto
    {
        public string CNPJForn { get; set; }
        public string CNPJPg { get; set; }
        public string CPFPg { get; set; }
        public string NCompra { get; set; }
        public decimal VValePed { get; set; }
        public MDFeTpValePedEnum? TpValePed { get; set; } // Usando o enum DTO
        public bool TpValePedSpecified { get; set; }
    }
}