using System.Security.Cryptography;
using System.Text;

public class PasswordEncryptionService
{
    private const int EncryptionLayers = 10; // Número de capas de encriptación

    public string EncryptPassword(string password)
    {
        string encryptedPassword = password;

        for (int i = 0; i < EncryptionLayers; i++)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(encryptedPassword));
                encryptedPassword = Convert.ToBase64String(hashedBytes);
            }
        }

        return encryptedPassword;
    }

    public bool VerifyPassword(string enteredPassword, string storedPasswordHash)
    {
        string encryptedEnteredPassword = EncryptPassword(enteredPassword);
        return encryptedEnteredPassword == storedPasswordHash;
    }
}
