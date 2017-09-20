namespace RSAcli
{
    using System;
    using System.Net;
    using System.Numerics;
    using EasySharp.NHelpers.Utils.Cryptography;
    using Maurer;
    using RSAEncDecLib.AlgorithmHelpers;

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

        public RSAEngine() { }

        #endregion

        public static void GenerateKyes(int keySizeBits, out BigInteger n, out BigInteger e, out BigInteger d)
        {
            BigInteger p = MaurerAlgorithm.Instance.ProvablePrime(keySizeBits);
            BigInteger q = MaurerAlgorithm.Instance.ProvablePrime(keySizeBits);

            //BigInteger p = 2;
            //BigInteger q = 7;

            n = ComputeModulus(p, q);

            BigInteger totient = ComputeTotient(p, q);
            e = GenerateEncryptionExponent(n, totient);
            d = GenerateDecryptionExponent(e, totient);
        }

        private static BigInteger GenerateEncryptionExponent(BigInteger modulus, BigInteger totient)
        {
            BigInteger e;
            do
            {
                e = BigIntegerHelper.NextBigInteger(2, totient);
            } while (BigInteger.GreatestCommonDivisor(e, totient) != 1);

            return e;
        }

        private static BigInteger GenerateDecryptionExponent(BigInteger encryptionExp, BigInteger totient)
        {
            int k = 2;
            BigInteger d = BigInteger.Multiply(k, totient) - 1;

            return d;
        }

        private static BigInteger ComputeTotient(BigInteger p, BigInteger q)
        {
            return (p - 1) * (q - 1);
        }

        private static BigInteger ComputeModulus(BigInteger p, BigInteger q)
        {
            return BigInteger.Multiply(p, q);
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