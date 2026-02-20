// Vasis.MDFe.Api\Dtos\Model\MdfeDto.cs
using Vasis.MDFe.Api.Dtos.Model; // Para InfMDFeDto

namespace Vasis.MDFe.Api.Dtos.Model
{
    public class MdfeDto
    {
        public InfMDFeDto InfMDFe { get; set; }
        // ... outras propriedades que você possa ter no JSON de entrada.
    }
}