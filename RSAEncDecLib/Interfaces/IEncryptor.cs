namespace RSAcli.Interfaces
{
    using System.Numerics;

    public interface IEncryptor
    {
        byte[] EncryptData(byte[] plainText);

        void ImportPublicKey(BigInteger encryptionExp, BigInteger modulus);
        void ImportPublicKey(byte[] encryptionExp, byte[] modulus);
    }
}