﻿namespace RSAcli.Interfaces
{
    internal interface IKeyParams
    {
        int KeyBitLength { get; set; }
        string OutputFileNamePrefix { get; set; }
    }
}