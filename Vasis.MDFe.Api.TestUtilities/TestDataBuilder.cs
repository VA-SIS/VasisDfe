using System.Text;

namespace Vasis.MDFe.Api.TestUtilities
{
    public static class TestDataBuilder
    {
        public static class Auth
        {
            public static object ValidLoginRequest => new
            {
                Username = "admin",
                Password = "senhaforte123"
            };

            public static object InvalidLoginRequest => new
            {
                Username = "admin",
                Password = "senhaerrada"
            };

            public static object EmptyLoginRequest => new
            {
                Username = "",
                Password = ""
            };
        }

        public static class Http
        {
            public static StringContent CreateJsonContent(object obj)
            {
                return new StringContent(
                    JsonConvert.SerializeObject(obj),
                    Encoding.UTF8,
                    "application/json");
            }
        }

        public static class Assertions
        {
            public static readonly string[] SuccessKeywords =
            {
                "SUCESSO NA POC DE INTEGRAÇÃO DO FONTE",
                "Zeus DFe.NET",
                "MDF-e",
                "certificado foi configurado"
            };

            public static readonly string[] ErrorKeywords =
            {
                "erro",
                "exception",
                "falha",
                "error"
            };
        }
    }
}