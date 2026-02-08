namespace Vasis.MDFe.Configuration
{
    /// <summary>
    /// Representa as configurações da empresa emitente do MDF-e.
    /// Contém os dados cadastrais básicos necessários para a identificação fiscal e preenchimento dos documentos.
    /// </summary>
    public class ConfiguracaoEmpresa
    {
        /// <summary>
        /// Obtém ou define o Cadastro Nacional de Pessoas Jurídicas (CNPJ) da empresa emitente.
        /// Formato: Apenas números (14 dígitos). Obrigatório.
        /// </summary>
        public string CNPJ { get; set; }

        /// <summary>
        /// Obtém ou define a Inscrição Estadual (IE) da empresa emitente.
        /// Formato: Apenas números. Pode ser "ISENTO" ou nulo/vazio se não aplicável ou a empresa for isenta. Opcional.
        /// </summary>
        public string InscricaoEstadual { get; set; }

        /// <summary>
        /// Obtém ou define a Razão Social da empresa. Obrigatório.
        /// </summary>
        public string RazaoSocial { get; set; }

        /// <summary>
        /// Obtém ou define o Nome Fantasia da empresa (opcional).
        /// </summary>
        public string NomeFantasia { get; set; }

        /// <summary>
        /// Obtém ou define o Logradouro (rua, avenida, etc.) do endereço da empresa. Obrigatório.
        /// </summary>
        public string EnderecoLogradouro { get; set; }

        /// <summary>
        /// Obtém ou define o Número do endereço da empresa. Obrigatório.
        /// </summary>
        public string EnderecoNumero { get; set; }

        /// <summary>
        /// Obtém ou define o Complemento do endereço da empresa (ex: apto, sala). Opcional.
        /// </summary>
        public string EnderecoComplemento { get; set; }

        /// <summary>
        /// Obtém ou define o Bairro do endereço da empresa. Obrigatório.
        /// </summary>
        public string EnderecoBairro { get; set; }

        /// <summary>
        /// Obtém ou define o Código do Município (IBGE) do endereço da empresa.
        /// Formato: Apenas números. Obrigatório.
        /// </summary>
        public int EnderecoCodigoMunicipio { get; set; }

        /// <summary>
        /// Obtém ou define o Nome do Município do endereço da empresa. Obrigatório.
        /// </summary>
        public string EnderecoNomeMunicipio { get; set; }

        /// <summary>
        /// Obtém ou define a sigla da Unidade da Federação (UF) do endereço da empresa. Obrigatório.
        /// </summary>
        public string EnderecoUF { get; set; }

        /// <summary>
        /// Obtém ou define o Código de Endereçamento Postal (CEP) do endereço da empresa.
        /// Formato: Apenas números (8 dígitos). Obrigatório.
        /// </summary>
        public string EnderecoCEP { get; set; }

        /// <summary>
        /// Obtém ou define o número de Telefone da empresa (opcional).
        /// Formato: Apenas números, incluindo DDD.
        /// </summary>
        public string Telefone { get; set; }

        /// <summary>
        /// Obtém ou define o Endereço de Email da empresa (opcional).
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Obtém ou define o Registro Nacional de Transportadores Rodoviários de Cargas (RNTRC).
        /// Relevante para empresas que realizam transporte rodoviário de cargas. Opcional.
        /// </summary>
        public string RNTRC { get; set; }

        /// <summary>
        /// Valida se as configurações essenciais da empresa estão preenchidas.
        /// Considera campos que são geralmente obrigatórios para emissão de documentos fiscais.
        /// </summary>
        /// <returns><c>true</c> se as configurações essenciais são válidas; caso contrário, <c>false</c>.</returns>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(CNPJ) &&
                   !string.IsNullOrWhiteSpace(RazaoSocial) &&
                   !string.IsNullOrWhiteSpace(EnderecoLogradouro) &&
                   !string.IsNullOrWhiteSpace(EnderecoNumero) &&
                   !string.IsNullOrWhiteSpace(EnderecoBairro) &&
                   EnderecoCodigoMunicipio > 0 && // Código IBGE deve ser um número positivo
                   !string.IsNullOrWhiteSpace(EnderecoNomeMunicipio) &&
                   !string.IsNullOrWhiteSpace(EnderecoUF) &&
                   !string.IsNullOrWhiteSpace(EnderecoCEP);
        }
    }
}