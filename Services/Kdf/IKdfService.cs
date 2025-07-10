namespace ASP_P26.Services.Kdf
{
    public interface IKdfService
    {
        string Dk(string password, string salt);
    }
}
