namespace RSAEncDecLib.Interfaces
{
    using System.Numerics;

    public interface IDecryptor
    {
        byte[] DecryptData(byte[] cipherText);

        void ImportPrivateKey(BigInteger decryptionExp, BigInteger modulus);
        void ImportPrivateKey(byte[] decryptionExp, byte[] modulus);
    }
}