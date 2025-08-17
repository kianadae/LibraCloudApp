using Microsoft.AspNetCore.Mvc;

namespace LibraCloud.Controllers
{
    public class CryptoTestController : Controller
    {
        public IActionResult Index()
        {
            string key = Environment.GetEnvironmentVariable("ENCRYPTION_KEY");

            if (string.IsNullOrEmpty(key))
                return Content("❌ Key not found.");

            string sampleText = "P@ssword123";
            string encrypted = CryptoHelper.Encrypt(sampleText, key);
            string decrypted = CryptoHelper.Decrypt(encrypted, key);

            return Content($"✅ Encrypted: {encrypted}\nDecrypted: {decrypted}");
        }
    }
}
