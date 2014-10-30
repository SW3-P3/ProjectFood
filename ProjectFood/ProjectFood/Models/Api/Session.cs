using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ProjectFood.Models.Api
{
    public class Session
    {
        public string Token { get; set; }
        public string Expires { get; set; }
        public string User { get; set; }
        public string Provider { get; set; }
        public Permissions Permissions { get; set; }
        public string Apikey { get; set; }
        public string Secret { get; set; }
        public string Signature { get; set; }
        public static string Sha256(string password)
        {
            var crypt = new SHA256Managed();
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));
            return crypto.Aggregate(hash, (current, bit) => current + bit.ToString("x2"));
        }
    }
}
