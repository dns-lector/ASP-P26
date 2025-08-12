using Microsoft.AspNetCore.Authentication;
using System.Text;
using System.Text.Json;

namespace ASP_P26.Services.Jwt
{
    public class JwtServiceV1 : IJwtService
    {
        const String defaultSecret = "JwtServiceV1";

        public (object, object) DecodeJwt(string jwt, string? secret = null)
        {
            int lastDotIndex = jwt.LastIndexOf('.');
            if (lastDotIndex == -1)
            {
                throw new Exception("Invalid format: dot not found");
            }
            secret ??= defaultSecret;
            String signature = jwt[(lastDotIndex + 1)..];
            String openPart  = jwt[..lastDotIndex];
            String controlSign = Sign(openPart, secret);
            if(controlSign != signature)
            {
                throw new Exception("Invalid signature");
            }
            String[] parts = openPart.Split('.');
            if (parts.Length != 2)
            {
                throw new Exception("Invalid format: dot not found in openPart");
            }
            var header = JsonSerializer.Deserialize<JsonElement>(
                Encoding.UTF8.GetString(          // {"alg":"HS256","typ":"JWT"}
                    Base64UrlTextEncoder.Decode(  // byte[]
                        parts[0]                  // eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9
                    ))
            );
            var payload = JsonSerializer.Deserialize<JsonElement>(
                Encoding.UTF8.GetString(          // {"loggedInAs":"admin","iat":1422779638}
                    Base64UrlTextEncoder.Decode(  // byte[]
                        parts[1]                  // eyJsb2dnZWRJbkFzIjoiYWRtaW4iLCJpYXQiOjE0MjI3Nzk2Mzh9.gzSraSYS8EXBxLN
                    ))
            );
            return (header, payload);
        }

        public string EncodeJwt(object payload, object? header = null, string? secret = null)
        {
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

            String signature = Sign(openPart, secret);

            return openPart + "." + signature;
        }

        private String Sign(String openPart, string? secret = null)
        {
            secret ??= defaultSecret;
            return Base64UrlTextEncoder.Encode(
                System.Security.Cryptography.HMACSHA256.HashData(
                    Encoding.UTF8.GetBytes(secret),
                    Encoding.UTF8.GetBytes(openPart)));
        }
    }
}
