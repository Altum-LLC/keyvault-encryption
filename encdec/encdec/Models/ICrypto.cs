using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace azure_manager.Models
{
    public interface ICrypto
    {
        Task<EncryptData> EncryptAsync(string plainText, bool managedIdentity);
        Task<DecryptData> DecryptAsync(string encryptedText, bool managedIdentity);
    }
}
