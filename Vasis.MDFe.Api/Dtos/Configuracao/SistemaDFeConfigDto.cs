// Vasis.MDFe.Api\Dtos\Configuracao\SistemaDFeConfigDto.cs
using DFe.Classes.Entidades; // Para Estado (Zeus)
using DFe.Classes.Flags;     // Para TipoAmbiente (Zeus)
using Vasis.MDFe.Api.Dtos.Model.Enums; // Adicionado: Para o seu enum DTO VersaoServico

namespace Vasis.MDFe.Api.Dtos.Configuracao
{
    public class SistemaDFeConfigDto
    {
        public Estado UfEmitente { get; set; }
        public TipoAmbiente Ambiente { get; set; }
        public short Serie { get; set; }
        public long Numeracao { get; set; }
        public MDFeVersaoServicoEnum VersaoLayout { get; set; } // Alterado para o seu enum DTO
        public string CaminhoSchemas { get; set; }
        public int TimeOut { get; set; }
    }
}