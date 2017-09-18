namespace RSAcli
{
    using System;
    using System.Numerics;

    public class RSAEngine
    {
        private readonly int _keySize;
        private byte[] _encryptionExp;
        private byte[] _modulus;
        private byte[] _decryptionExp;

        #region CONSTRUCTORS

        public RSAEngine(int keySize)
        {
            _keySize = keySize;
        }

        #endregion

        public static void GenerateKyes(out byte[] privateKey, out byte[] publicKey)
        {
            throw new NotImplementedException();
        }

        public static void GenerateKyes(out BigInteger privateKey, out BigInteger publicKey)
        {
            throw new NotImplementedException();
        }

        public byte[] EncryptData(byte[] plainText)
        {
            BigInteger m = new BigInteger(plainText);
            BigInteger e = new BigInteger(_encryptionExp);
            BigInteger n = new BigInteger(_modulus);

            byte[] encryptedData = BigInteger.ModPow(m, e, n).ToByteArray();
            return encryptedData;
        }

        public byte[] DecryptData(byte[] cipherText)
        {
            BigInteger m = new BigInteger(cipherText);
            BigInteger d = new BigInteger(_decryptionExp);
            BigInteger n = new BigInteger(_modulus);

            byte[] decryptedData = BigInteger.ModPow(m, d, n).ToByteArray();
            return decryptedData;
        }

        public void ImportPrivateKey(byte[] decryptionExp, byte[] modulus)
        {
            _decryptionExp = decryptionExp;
            _modulus = modulus;
        }

        public void ImportPrivateKey(BigInteger decryptionExp, BigInteger modulus)
        {
            ImportPrivateKey(decryptionExp.ToByteArray(), modulus.ToByteArray());
        }

        public void ImportPublicKey(byte[] encryptionExp, byte[] modulus)
        {
            _encryptionExp = encryptionExp;
            _modulus = modulus;
        }

        public void ImportPublicKey(BigInteger encryptionExp, BigInteger modulus)
        {
            ImportPublicKey(encryptionExp.ToByteArray(), modulus.ToByteArray());
        }
    }
}