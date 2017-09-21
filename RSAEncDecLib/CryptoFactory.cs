﻿namespace RSAEncDecLib
{
    using RSAcli;
    using RSAcli.Interfaces;

    public static class CryptoFactory
    {
        public static IEncryptor CreateEncryptor()
        {
            return new RSAEngine();
        }

        public static IDecryptor CreateDecryptor()
        {
            return new RSAEngine();
        }

        public static IKeygen CreateKeygen()
        {
            return new RSAEngine();
        }
    }
}