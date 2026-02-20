// Vasis.MDFe.Api\Dtos\Model\IdeDto.cs
using System;
using System.Collections.Generic;
using DFe.Classes.Entidades;
using DFe.Classes.Flags;
using Vasis.MDFe.Api.Dtos.Model.Enums; // Para seus enums DTOs

namespace Vasis.MDFe.Api.Dtos.Model
{
    public class IdeDto
    {
        public DFe.Classes.Entidades.Estado CUF { get; set; }
        public DFe.Classes.Flags.TipoAmbiente TpAmb { get; set; }
        public MDFeTipoEmitenteEnum TpEmit { get; set; }
        public MDFeTpTranspEnum? TpTransp { get; set; }
        public DFe.Classes.Flags.ModeloDocumento Mod { get; set; }
        public short Serie { get; set; }
        public long NMDF { get; set; }
        public int CMDF { get; set; } // Codigo numerico do MDF-e
        public byte CDV { get; set; }
        public MDFeModalEnum Modal { get; set; }
        public DateTimeOffset DhEmi { get; set; }
        public MDFeTipoEmissaoEnum TpEmis { get; set; }
        public MDFeIdentificacaoProcessoEmissaoEnum ProcEmi { get; set; }
        public string VerProc { get; set; }
        public DFe.Classes.Entidades.Estado UFIni { get; set; }
        public DFe.Classes.Entidades.Estado UFFim { get; set; }
        public List<MunCarregaDto> InfMunCarrega { get; set; } // Será criado abaixo
        public List<PercursoDto> InfPercurso { get; set; } // Será criado abaixo
        public DateTimeOffset? DhIniViagem { get; set; }
        public string IndCanalVerde { get; set; }
        public string IndCarregaPosterior { get; set; }
    }
}