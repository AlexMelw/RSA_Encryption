# RSA Encryption Utility (CLI)

Implemented in C# (.NET Framework 4.6.1) adhering to the Ron Rivest, Adi Shamir, and Leonard Adleman encryption algorithm.

### Documentation

To get some help concerning the use of CLI utility, type:

`cmd> RSAcli --help` or just `cmd> RSAcli help`

**The output:**

```
RSA CLI Utility 1.3.0.37697
Copyright (C) 2017. Developed by BARBARII Veaceslav

  keygen     Generates RSA public/private key-pair of the specified bit-length.

  enc        Enforces file encryption with the specified key RSA public.

  dec        Enforces file decryption with the specified key RSA private key.

  help       Display more information on a specific command.

  version    Display version information.
```
  
For more information related to each command verb, type:

`cmd> RSAcli help verb`, wherein verb can be either `keygen`, `enc` or `dec`.

**The output:**

```
RSA CLI Utility 1.3.0.37697
Copyright (C) 2017. Developed by BARBARII Veaceslav
USAGE:
RSA Key-Pair Generation:
  RSAcli keygen --prefix "Your Key Name Prefix"

  -s, --size      (Default: 2048) Recommended values: 1024, 2048, 4096. Specifies bit-length (integer value) of the RSA public/private keys.

  -p, --prefix    The Prefix of Output Filenames. Example of generated filenames: {Prefix}-{4096}bits.public {Prefix}-{4096}bits.private

  --help          Display this help screen.

  --version       Display version information.
```
