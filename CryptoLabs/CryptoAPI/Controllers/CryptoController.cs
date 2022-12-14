using CryptoLabs.Ciphers.Classical;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CryptoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CryptoController : ControllerBase
    {
        private static CaesarCipher _caesarCipher = new();
        private static VigenereCipher _vigenereCipher = new();
        [HttpGet("encryptCaesar")]
        [Authorize(Roles = "Admin")]
        public string EncryptCaesar(string plaintext, int key)
        {
            return _caesarCipher.Encrypt(plaintext,key);
        }

        [HttpGet("decryptCaesar")]
        [Authorize(Roles = "Admin")]
        public string DecryptCaesar(string cipherText, int key)
        {
            return _caesarCipher.Decrypt(cipherText, key);
        }
        [HttpGet("encryptVigenere")]
        public string EncryptVigenere(string plaintext, string key)
        {
            return _vigenereCipher.Encrypt(plaintext, key);
        }
        [HttpGet("decryptVigenere")]
        [Authorize]
        public string DecryptVigenere(string cipherText, string key)
        {
            return _vigenereCipher.Decrypt(cipherText, key);
        }
    }
}
