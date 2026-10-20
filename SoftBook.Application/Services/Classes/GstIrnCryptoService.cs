using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace SoftBook.Application.Services.Classes;

public static class GstIrnCryptoService
{
    public static byte[] GenerateSecureKey()
    {
        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.GenerateKey();
        return aes.Key;
    }

    public static string EncryptWithPublicKey(string plainTextBase64, string publicKeyBase64)
    {
        var keyBytes = Convert.FromBase64String(publicKeyBase64);
        var asymmetricKeyParameter = PublicKeyFactory.CreateKey(keyBytes);
        var rsaKeyParameters = (RsaKeyParameters)asymmetricKeyParameter;

        var rsaParameters = new RSAParameters
        {
            Modulus = rsaKeyParameters.Modulus.ToByteArrayUnsigned(),
            Exponent = rsaKeyParameters.Exponent.ToByteArrayUnsigned()
        };

        using var rsa = RSA.Create();
        rsa.ImportParameters(rsaParameters);
        var plaintext = Encoding.UTF8.GetBytes(plainTextBase64);
        var ciphertext = rsa.Encrypt(plaintext, RSAEncryptionPadding.Pkcs1);
        return Convert.ToBase64String(ciphertext);
    }

    public static string DecryptBySymmetricKey(string encryptedText, byte[] key)
    {
        var dataToDecrypt = Convert.FromBase64String(encryptedText);
        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Key = key;
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor();
        var deCipher = decryptor.TransformFinalBlock(dataToDecrypt, 0, dataToDecrypt.Length);
        return Convert.ToBase64String(deCipher);
    }

    public static string EncryptBySymmetricKey(string plainTextBase64, string sekBase64)
    {
        var dataToEncrypt = Convert.FromBase64String(plainTextBase64);
        var keyBytes = Convert.FromBase64String(sekBase64);

        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Key = keyBytes;
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        var cipher = encryptor.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
        return Convert.ToBase64String(cipher);
    }
}
