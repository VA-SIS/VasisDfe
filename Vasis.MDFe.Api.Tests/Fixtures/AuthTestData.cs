// C:\Zeus\1935\DFe.NET-2026\Vasis.MDFe.Api.Tests\Fixtures\AuthTestData.cs

namespace Vasis.MDFe.Api.Tests.Fixtures
{
    public static class AuthTestData
    {
        public static readonly object ValidCredentials = new
        {
            Username = "admin",
            Password = "senhaforte123"
        };

        public static readonly object InvalidCredentials = new
        {
            Username = "admin",
            Password = "senhaerrada"
        };

        public static readonly object EmptyCredentials = new
        {
            Username = "",
            Password = ""
        };

        public const string InvalidJwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
    }
}