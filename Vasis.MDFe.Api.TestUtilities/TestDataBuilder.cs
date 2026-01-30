using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

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
                var json = JsonConvert.SerializeObject(obj);
                var content = new StringContent(json, Encoding.UTF8);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return content;
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