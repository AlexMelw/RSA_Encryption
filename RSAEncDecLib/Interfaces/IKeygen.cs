namespace RSAEncDecLib.Interfaces
{
    using System.Numerics;

    public interface IKeygen
    {
        void GenerateKyes(int keySizeBits,
            out BigInteger modulus,
            out BigInteger encryptionExponent,
            out BigInteger decryptionExponent);

        void GenerateKyes(int keySizeBits,
            out byte[] modulus,
            out byte[] encryptionExponent,
            out byte[] decryptionExponent);
    }
}