namespace ASP_P26.Services.Jwt
{
    public interface IJwtService
    {
        String EncodeJwt(Object payload, Object? header = null, String? secret = null);
        (Object, Object) DecodeJwt(String jwt, String? secret = null);
    }
}
