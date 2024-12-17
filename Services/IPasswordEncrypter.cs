namespace Movies.Services
{
    public interface IPasswordEncrypter
    {
        string HashPassword(string password, string salt);
        string GenerateSalt();
    }
}
