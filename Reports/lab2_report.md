# Lab 2 Report

### Course: Cryptography & Security

### Author: Todirascu Ilie, FAF-203

---

## Overview:

Symmetric Cryptography deals with the encryption of plain text when having only one encryption key which needs to remain
private. Based on the way the plain text is processed/encrypted there are 2 types of ciphers:

- Stream ciphers:
  - The encryption is done one byte at a time.
  - Stream ciphers use confusion to hide the plain text.
  - Make use of substitution techniques to modify the plain text.
  - The implementation is fairly complex.
  - The execution is fast.
- Block ciphers:
  - The encryption is done one block of plain text at a time.
  - Block ciphers use confusion and diffusion to hide the plain text.
  - Make use of transposition techniques to modify the plain text.
  - The implementation is simpler relative to the stream ciphers.
  - The execution is slow compared to the stream ciphers.

## Objectives:

1. Get familiar with the symmetric cryptography, stream and block ciphers.

2. Implement an example of a stream cipher.

3. Implement an example of a block cipher.

4. The implementation should, ideally follow the abstraction/contract/interface used in the previous laboratory work.

5. Please use packages/directories to logically split the files that you will have.

6. As in the previous task, please use a client class or test classes to showcase the execution of your programs.

## Implementation Description

### Feistel cipher example of Block Ciphers

The Feistel cipher developed by the German-born physicist and cryptographer Horst Feistel, works by splitting the plain text into 2 chunks and converting them to binary data and then those chunks go through
however many rounds of encryption. It can work with any encryption function, I just used another XOR and one round. First the left side goes through the encryption and the left chunk is put on the right and the right on the left. The encryption works by XOR ing the chunks with the binary number we get from the encryption function and that is also where we use our keys. The beauty of this algorithm is that
it takes advantage of the reversable nature of the XOR and thus the decryption is just the same round of encryption but the chunks are reversed as well as the keys.

```C#
  public string Encrypt(string key1, string key2)
        {
            key1 = BinaryConverter.StringToBinary(key1);
            key2 = BinaryConverter.StringToBinary(key2);
            key1 = key1.Length < Text.Length / 2 ? key1 + new string('0',Text.Length / 2 - key1.Length) : new(key1.Take(Text.Length / 2).ToArray());
            key2 = key2.Length < Text.Length / 2 ? key2 + new string('0',Text.Length / 2 - key2.Length)  : new(key2.Take(Text.Length / 2).ToArray());
            var l1 = XOR(L1, XOR(R1, key1));
            var r1 = XOR(R1, XOR(l1, key2));
            L1 = r1;
            R1 = l1;
            return l1 + r1;
        }
        public string Decrypt(string key1,string key2)
        {
            Encrypt(key2, key1);
            return BinaryConverter.ToString(L1 + R1);
        }
```

### RC4 cipher example of Stream Cipher

RC4 (also known as Rivest Cipher 4) is a form of stream cipher. It encrypts messages one byte at a time via an algorithm.

Plenty of stream ciphers exist, but RC4 is among the most popular. It's simple to apply, and it works quickly, even on very large pieces of data. TSL (transport layer security) and SSL (secure socket layer), both use RC4 encryption.

How it was implemented:

Key Scheduling Algorithm Phase:
KSA Phase Step 1: First, the entries of S are set equal to the values of 0 to 255 in ascending order.
KSA Phase Step 2 a: A temporary vector T is created.
KSA Phase Step 2 b: If the length of the key k is 256 bytes, then k is assigned to T. Otherwise, for a key with a given length, copy the elements of the key into vector T, repeating for as many times as neccessary to fill T.
KSA Phase Step 3: We use T to produce the initial permutation of S

Pseudo random generation algorithm (Stream Generation):
Once the vector S is initialized from above in the Key Scheduling Algorithm Phase, the input key is no longer used.
PRGA Phase Step 1. Continously increment i from 0 to 255, starting it back at 0 once we go beyond 255 (this is done with mod (%) division
PRGA Phase Step 2. Lookup the i'th element of S and add it to j, keeping the result within the range of 0 to 255 using mod (%) division
PRGA Phase Step 3. Swap the values of S[i] and S[j]
PRGA Phase Step 4. Use the result of the sum of S[i] and S[j], mod (%) by 256 to get the index of S that handls the value of the stream value K.
PRGA Phase Step 5. Use bitwise exclusive OR (^) with the next byte in the data to produce the next byte of the resulting ciphertext (when encrypting) or plaintext (when decrypting)

And surely, because it is an symetric encryption algorithm, encryption and decryption is the same and was implemented as one method

```C#
public static byte[] EncDec(byte[] data, byte[] key)
        {

            int[] S = new int[256];
            int[] T = new int[256];
            for (int z = 0; z < 256; z++)
            {
                S[z] = z;
                T[z] = key[z % key.Length];
            }
            int i = 0;
            int j = 0;
            for (; i < 256; i++)
            {
                j = (j + S[i] + T[i]) % 256;
                Swap(ref S, i, j);
            }
            i = j = 0;
            byte[] result = new byte[data.Length];
            for (int iteration = 0; iteration < data.Length; iteration++)
            {
                i = (i + 1) % 256;
                j = (j + S[i]) % 256;
                Swap(ref S, i, j);
                int K = S[(S[i] + S[j]) % 256];
                result[iteration] = Convert.ToByte(data[iteration] ^ K);
            }

            return result;
        }
```
