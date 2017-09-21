namespace RSAcli
{
    using System;
    using System.Numerics;
    using Interfaces;
    using Maurer;
    using RSAEncDecLib.AlgorithmHelpers;

    public class RSAEngine : IEncryptor, IDecryptor, IKeygen
    {
        private byte[] _encryptionExp;
        private byte[] _modulus;
        private byte[] _decryptionExp;

        private static BigInteger GenerateEncryptionExponent(BigInteger totient)
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
            (BigInteger x, BigInteger y) = Extended_GCD(totient, encryptionExp);

            y = NormalizeY(totient, y);

            return y;
        }

        private static BigInteger NormalizeY(BigInteger totient, BigInteger y)
        {
            if (y < 0)
            {
                y = y + totient;
            }
            return y;
        }

        private static (BigInteger, BigInteger) Extended_GCD(BigInteger A, BigInteger B)
        {
            bool reverse = false;

            if (A < B) //if A less than B, switch them
            {
                Swap(ref A, ref B);
                reverse = true;
            }

            BigInteger r = B;
            BigInteger q = 0;

            BigInteger x0 = 1;
            BigInteger y0 = 0;

            BigInteger x1 = 0;
            BigInteger y1 = 1;

            BigInteger x = 0;
            BigInteger y = 0;

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
            }

            var resultR = r;

            //if (reverse)
            //{
            //    resultX = y;
            //    resultY = x;
            //}
            //else
            //{
            //    resultX = x;
            //    resultY = y;
            //}

            return reverse ? (y, x) : (x, y);

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

        public void GenerateKyes(int keySizeBits, out BigInteger modulus,
            out BigInteger encryptionExponent,
            out BigInteger decryptionExponent)
        {
            BigInteger p = MaurerAlgorithm.Instance.ProvablePrime(keySizeBits / 2);
            BigInteger q = MaurerAlgorithm.Instance.ProvablePrime(keySizeBits / 2);

            modulus = ComputeModulus(p, q);
            BigInteger totient = ComputeTotient(p, q);

            encryptionExponent = GenerateEncryptionExponent(totient);
            decryptionExponent = GenerateDecryptionExponent(encryptionExponent, totient);
        }

        public void GenerateKyes(int keySizeBits, out byte[] modulus,
            out byte[] encryptionExponent, out byte[] decryptionExponent)
        {
            GenerateKyes(keySizeBits, out BigInteger modulusbBigInteger,
                out BigInteger encryptionExponentbBigInteger, out BigInteger decryptionExponentbBigInteger);

            modulus = modulusbBigInteger.ToByteArray();
            encryptionExponent = encryptionExponentbBigInteger.ToByteArray();
            decryptionExponent = decryptionExponentbBigInteger.ToByteArray();
        }
    }
}