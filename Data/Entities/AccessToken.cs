using System.Text.Json.Serialization;

namespace ASP_P26.Data.Entities
{
    public class AccessToken   // організований за стандартом JWT
    {
        public String  Jti { get; set; } = null!;   // TokenId
        public Guid    Sub { get; set; }   // UserAccessId
        public String? Iat { get; set; }
        public String? Exp { get; set; }
        public String? Nbf { get; set; }
        public String? Aud { get; set; }   // Role / RoleId
        public String? Iss { get; set; }   // Видавник токена

        // -----------
        [JsonIgnore]
        public UserAccess UserAccess { get; set; } = null!;
    }
}
