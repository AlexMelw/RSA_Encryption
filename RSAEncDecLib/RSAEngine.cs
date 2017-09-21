namespace RSAcli
{
    using System;
    using System.Net;
    using EasySharp.NHelpers.Utils.Cryptography;
    using Maurer;
    using RSAEncDecLib.AlgorithmHelpers;
    using ServiceWire.ZeroKnowledge;
    using BigInteger = System.Numerics.BigInteger;

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
            //BigInteger p = 2;
            //BigInteger q = 7;

            //BigInteger p = RsaHelper.GetRandomPrimeWithinRange(2000000, 4000000);
            //BigInteger q = RsaHelper.GetRandomPrimeWithinRange(2000000, 4000000);

            BigInteger p = MaurerAlgorithm.Instance.ProvablePrime(keySizeBits / 2);
            BigInteger q = MaurerAlgorithm.Instance.ProvablePrime(keySizeBits / 2);

            n = ComputeModulus(p, q);
            BigInteger totient = ComputeTotient(p, q);

            e = GenerateEncryptionExponent(n, totient);
            d = GenerateDecryptionExponent(e, totient);
        }

        private static BigInteger GenerateEncryptionExponent(BigInteger modulus, BigInteger totient)
        {
            BigInteger e;
            do {
                e = BigIntegerHelper.NextBigInteger(2, totient);
            } while (BigInteger.GreatestCommonDivisor(e, totient) != 1);

            return e;
        }

        private static BigInteger GenerateDecryptionExponent(BigInteger encryptionExp, BigInteger totient)
        {
            BigInteger[] result = new BigInteger[3];

            result = Extended_GCD(totient, encryptionExp);

            if (result[2] < 0)
            {
                result[2] = result[2] + totient;
            }

            return result[2];
        }

        private static BigInteger[] Extended_GCD(BigInteger A, BigInteger B)
        {
            BigInteger[] result = new BigInteger[3];

            bool reverse = false;

            if (A < B) //if A less than B, switch them
            {
                Swap(ref A, ref B);
                reverse = true;
            }

            //log("Extended GCD");

            BigInteger r = B;
            BigInteger q = 0;

            BigInteger x0 = 1;
            BigInteger y0 = 0;

            BigInteger x1 = 0;
            BigInteger y1 = 1;

            BigInteger x = 0;
            BigInteger y = 0;

            //log(A + "\t" + " " + "\t" + x0 + "\t" + y0);

            //log(B + "\t" + " " + "\t" + x1 + "\t" + y1);

            while (A % B != 0)
            {
                r = A % B;
                q = A / B;

                x = x0 - q * x1;
                y = y0 - q * y1;

                x0 = x1;
                y0 = y1;

                x1 = x;
                y1 = y;

                A = B;
                B = r;
                //log(B + "\t" + r + "\t" + x + "\t" + y);
            }

            result[0] = r;

            if (reverse)
            {
                result[1] = y;
                result[2] = x;
            }
            else
            {
                result[1] = x;
                result[2] = y;
            }

            return result;

            void Swap(ref BigInteger X, ref BigInteger Y)
            {
                var temp = X;
                X = Y;
                Y = temp;
            }
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