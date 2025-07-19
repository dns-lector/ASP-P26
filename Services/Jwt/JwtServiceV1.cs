using Microsoft.AspNetCore.Authentication;
using System.Text;
using System.Text.Json;

namespace ASP_P26.Services.Jwt
{
    public class JwtServiceV1 : IJwtService
    {
        public (object, object) DecodeJwt(string jwt, string? secret = null)
        {
            throw new NotImplementedException();
        }

        public string EncodeJwt(object payload, object? header = null, string? secret = null)
        {
            secret ??= "JwtServiceV1";
            header ??= new
            {
                alg = "HS256",
                typ = "JWT"
            };
            String openPart = Base64UrlTextEncoder.Encode(
                Encoding.UTF8.GetBytes(
                    JsonSerializer.Serialize(header)))
                + "." + Base64UrlTextEncoder.Encode(
                Encoding.UTF8.GetBytes(
                    JsonSerializer.Serialize(payload)));

            String signature = Base64UrlTextEncoder.Encode(
                System.Security.Cryptography.HMACSHA256.HashData(
                    Encoding.UTF8.GetBytes(secret),
                    Encoding.UTF8.GetBytes(openPart)));

            return openPart + "." + signature;
        }
    }
}
