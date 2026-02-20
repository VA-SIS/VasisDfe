// Vasis.MDFe.Api\Dtos\Model\ModalBaseDto.cs
using System.Text.Json.Serialization;
using Vasis.MDFe.Api.Dtos.Model.Rodo; // Adicionar este using para referenciar RodoDto

namespace Vasis.MDFe.Api.Dtos.Model
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(RodoDto), "rodo")]
    // [JsonDerivedType(typeof(AereoDto), "aereo")] // Adicionar quando criar AereoDto
    public abstract class ModalBaseDto
    {
        // ... propriedades comuns, se houver
    }
}