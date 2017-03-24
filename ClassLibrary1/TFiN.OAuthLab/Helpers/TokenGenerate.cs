using System;
using System.Text;
using System.Web.UI;

namespace TFiN.OAuthLab.Helpers
{
    public static class TokenGenerate
    {
        public static string GenerateToken(Guid client, Guid user, string type)
        {
            string encryptionKey = "samplekey";
            string clientId = client.ToString();
            string userId = user.ToString();
            string plainText = GeneratePlainToken(clientId, userId, type);

            if (type.Equals("BearerToken"))
            {
                return BearerTokenHelper.Encrypt(plainText, encryptionKey);
            }

            return sha256(plainText);
        }

        static string sha256(string plainText)
        {
            System.Security.Cryptography.SHA256Managed crypt = new System.Security.Cryptography.SHA256Managed();
            System.Text.StringBuilder hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(plainText), 0,
                Encoding.UTF8.GetByteCount(plainText));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        private static string GeneratePlainToken(string clientId, string userId, string type)
        {
            const char delimiter = ',';

            return clientId + delimiter + userId + delimiter + type + delimiter + DateTime.Now; ;
        }
    }
}