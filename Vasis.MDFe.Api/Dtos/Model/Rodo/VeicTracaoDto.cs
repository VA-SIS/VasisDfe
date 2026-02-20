// Vasis.MDFe.Api\Dtos\Model\Rodo\VeicTracaoDto.cs
using System.Collections.Generic;
using DFe.Classes.Entidades; // Para Estado
using Vasis.MDFe.Api.Dtos.Model.Enums; // Para MDFeTpRodEnum, MDFeTpCarEnum
using Vasis.MDFe.Api.Dtos.Model.Rodo; // Para CondutorDto

namespace Vasis.MDFe.Api.Dtos.Model.Rodo
{
    public class VeicTracaoDto
    {
        public string Placa { get; set; }
        public string RENAVAM { get; set; }
        public Estado UF { get; set; }
        public int Tara { get; set; }
        public int CapM3 { get; set; }
        public int CapKG { get; set; }
        public MDFeTpRodEnum TpRod { get; set; }
        public MDFeTpCarEnum TpCar { get; set; }
        public List<CondutorDto> Condutor { get; set; }
    }
}