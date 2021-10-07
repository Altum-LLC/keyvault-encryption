using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azure_manager.Models
{
    public class Crypto : ICrypto
    {
        private readonly SecretManager _secretManager;

        public Crypto(IConfiguration configuration, ITokenAcquisition tokenAcquisition, SecretManager secretManager)
        {
            Configuration = configuration;
            this.tokenAcquisition = tokenAcquisition;
            _secretManager = secretManager;
        }

        public IConfiguration Configuration { get; }
        readonly ITokenAcquisition tokenAcquisition;

        public async Task<EncryptData> EncryptAsync(string plainText, bool managedIdentity)
        {
            // Environment variable with the Key Vault endpoint.
            string keyVaultUrl = Configuration.GetSection("KeyVault").GetValue<string>("VaultUri");
            var keyId = await _secretManager.GetSecretAsync("KeyVault:KeyId");

            CryptographyClient cryptoClient;

            if (!managedIdentity)
            {
                // Then we create the CryptographyClient which can perform cryptographic operations with the key we just created using the same credential created above.
                cryptoClient = new CryptographyClient(new Uri(keyId), new TokenAcquisitionTokenCredential(tokenAcquisition));
            }
            else
            {
                cryptoClient = new CryptographyClient(new Uri(keyId), new DefaultAzureCredential());
            }

            // Next we'll encrypt some arbitrary plain text with the key using the CryptographyClient. Note that RSA encryption
            // algorithms have no chaining so they can only encrypt a single block of plaintext securely. For RSAOAEP this can be
            // calculated as (keysize / 8) - 42, or in our case (2048 / 8) - 42 = 214 bytes.
            byte[] plaintext = Encoding.UTF8.GetBytes(plainText);

            // First encrypt the data using RSAOAEP with the created key.
            EncryptResult encryptResult = await cryptoClient.EncryptAsync(EncryptionAlgorithm.RsaOaep, plaintext);
            return new EncryptData {
                Algorithm = encryptResult.Algorithm.ToString(),
                KeyId = encryptResult.KeyId,
                EncryptedText = Convert.ToBase64String(encryptResult.Ciphertext),
                PlainText = plainText,
                Result = new ResponseData { 
                    Success = $"Encryption successfully completed. KeyID: {encryptResult.KeyId}, Algorithm: {encryptResult.Algorithm}, PlainText: {plainText}, EncryptedText: {Convert.ToBase64String(encryptResult.Ciphertext)}"
                }
            };
        }

        public async Task<DecryptData> DecryptAsync(string encryptedText, bool managedIdentity)
        {
            // Environment variable with the Key Vault endpoint.
            string keyVaultUrl = Configuration.GetSection("KeyVault").GetValue<string>("BaseUrl");
            var keyId = await _secretManager.GetSecretAsync("KeyVault:KeyId");

            CryptographyClient cryptoClient;

            if (!managedIdentity)
            {
                cryptoClient = new CryptographyClient(new Uri(keyId), new TokenAcquisitionTokenCredential(tokenAcquisition));
            }
            else
            {
                cryptoClient = new CryptographyClient(new Uri(keyId), new DefaultAzureCredential());
            }

            // Next we'll encrypt some arbitrary plain text with the key using the CryptographyClient. Note that RSA encryption
            // algorithms have no chaining so they can only encrypt a single block of plaintext securely. For RSAOAEP this can be
            // calculated as (keysize / 8) - 42, or in our case (2048 / 8) - 42 = 214 bytes.
            byte[] encryptedtext = Convert.FromBase64String(encryptedText);

            // First encrypt the data using RSAOAEP with the created key.
            DecryptResult decryptResult = await cryptoClient.DecryptAsync(EncryptionAlgorithm.RsaOaep, encryptedtext);
            return new DecryptData
            {
                EncryptedText = encryptedText,
                DecryptedText = Convert.ToBase64String(decryptResult.Plaintext),
                Result = new ResponseData
                {
                    Success = $"Decryption successfully completed. KeyID: {decryptResult.KeyId}, Algorithm: {decryptResult.Algorithm}, PlainText: {Encoding.UTF8.GetString(decryptResult.Plaintext, 0, decryptResult.Plaintext.Length)}, EncryptedText: {encryptedText}"
                }
            };
        }
    }
}
