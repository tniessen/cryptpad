[![Build status](https://ci.appveyor.com/api/projects/status/rdkgt36mc8ksjp7d?svg=true)](https://ci.appveyor.com/project/tniessen/cryptpad)

## Building and requirements

To build the program, simply open the Visual Studio 2015 solution and click Build. The resulting
binary `cryptpad.exe` requires the .NET Framework 4.

## Encryption

cryptpad uses AES encryption, supporting key lengths of up to 256 bits, and can generate keys using
either SHA-256 (default), SHA-512, RIPEMD-160 (128 bit keys only) or MD5 (128 bit keys only). Only
two block cipher modes of operation are supported, CBC (recommended) and ECB.

## File format

Encrypted text files begin with a 128 bit IV, followed by any number of encrypted blocks. Therefore,
the file size is always a multiple of the block size, which is 128 bits (16 bytes).

### Manual encryption and decryption using OpenSSL

You can, in fact, manually encrypt and decrypt text files exactly as cryptpad does, using OpenSSL.
The following example uses bash and assumes AES/CBC/PKCS7 with a 256 bit key generated using
SHA-256. However, only minimal changes are required to adapt the commands to other configurations.

First, you need to create the key:

    key=`echo -n "$password" | sha256sum | head -c 64`

To encrypt content, generate a random initialization vector and its hexadecimal representation,
and use it in combination with the computed key in order to produce the output file:

    dd if=/dev/random bs=16 count=1 status=none > file.stxt
    iv=`xxd -c 32 -p file.stxt`

    echo 'Hello world!' | openssl enc -aes-256-cbc -e -iv $iv -K $key >> file.stxt

You can now open `file.stxt` in cryptpad.

To decrypt the file, we need to read the IV before reading the actual contents:

    iv=`dd if=file.stxt bs=16 count=1 status=none | xxd -c 32 -p`

    dd if=file.stxt bs=16 skip=1 status=none | openssl enc -aes-256-cbc -d -iv $iv -K $key


