using System.Security.Cryptography;
using System.Text;

namespace FileEncrypter
{
    public class Encryption
    {
        /// <summary>
        /// Encrypt file
        /// </summary>
        /// <param name="inputFile">File to encrypt</param>
        /// <param name="password">Password to encrypt</param>
        public void FileEncrypt(string inputFile, string password)
        {
            byte[] salt = GenerateRandomSalt();

            var fsCrypt = new FileStream(inputFile + ".aes", FileMode.Create);

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            var AES = Aes.Create();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            AES.Padding = PaddingMode.PKCS7;

            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000, HashAlgorithmName.SHA512);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);

            AES.Mode = CipherMode.CFB;

            fsCrypt.Write(salt, 0, salt.Length);

            var cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write);
            var fsIn = new FileStream(inputFile, FileMode.Open);

            byte[] buffer = new byte[1048576];
            int read;

            try
            {
                while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                {
                    cs.Write(buffer, 0, read);
                }

                fsIn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                cs.Close();
                fsCrypt.Close();
            }
        }

        /// <summary>
        /// Generates random password (64 chars)
        /// </summary>
        /// <returns>Random password</returns>
        public string GenerateRandomPassword()
        {
            const string src = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!\"#$%&\\'()*+,-./:;<=>?@[\\\\]^_`{|}~";
            int length = 64;
            var sb = new StringBuilder();
            Random RNG = new Random();
            for (var i = 0; i < length; i++)
            {
                var c = src[RNG.Next(0, src.Length)];
                sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Generates random salt to hash
        /// </summary>
        /// <returns>Random salt</returns>
        private static byte[] GenerateRandomSalt()
        {
            byte[] data = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                for (int i = 0; i < 10; i++)
                {
                    rng.GetBytes(data);
                }
            }

            return data;
        }
    }
}
