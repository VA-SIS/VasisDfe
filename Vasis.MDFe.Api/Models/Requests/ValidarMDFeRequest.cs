using System.ComponentModel.DataAnnotations;

namespace Vasis.MDFe.Api.Models.Requests
{
    /// <summary>
    /// Request para validação de XML MDFe
    /// </summary>
    public class ValidarMDFeRequest
    {
        /// <summary>
        /// Conteúdo XML do MDFe a ser validado
        /// </summary>
        [Required(ErrorMessage = "O conteúdo XML é obrigatório")]
        [MinLength(100, ErrorMessage = "XML deve ter pelo menos 100 caracteres")]
        public string XmlContent { get; set; } = string.Empty;

        /// <summary>
        /// Indica se deve validar apenas estrutura ou também regras de negócio
        /// </summary>
        public bool ValidarApenasEstrutura { get; set; } = false;

        /// <summary>
        /// Versão do schema para validação (opcional - usa configuração padrão se não informado)
        /// </summary>
        public string? VersaoSchema { get; set; }
    }
}