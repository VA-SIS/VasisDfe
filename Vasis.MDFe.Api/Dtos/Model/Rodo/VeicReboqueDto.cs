// Vasis.MDFe.Api\Dtos\Model\Rodo\VeicReboqueDto.cs
using DFe.Classes.Entidades; // Para Estado (do Zeus)
using Vasis.MDFe.Api.Dtos.Model.Enums; // <<-- ESTE USING É CRUCIAL para encontrar o enum

namespace Vasis.MDFe.Api.Dtos.Model.Rodo
{
    public class VeicReboqueDto
    {
        public string Placa { get; set; }
        public string RENAVAM { get; set; }
        public Estado UF { get; set; }
        public int Tara { get; set; }
        public int CapM3 { get; set; }
        public int CapKG { get; set; }
        public MDFeTpRodEnum TpRod { get; set; } // Usando enum DTO
        public MDFeTpCarEnum TpCar { get; set; } // Usando enum DTO
        public MDFeTpPropEnum? TpProp { get; set; } // Usando o enum DTO
        public bool TpPropSpecified { get; set; }
    }
}