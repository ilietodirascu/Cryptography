# Lab 4 Report

### Course: Cryptography & Security

### Author: Ilie Todirascu, FAF-203

---

## Overview:

&ensp;&ensp;&ensp; Hashing is a technique used to compute a new representation of an existing value, message or any piece of text. The new representation is also commonly called a digest of the initial text, and it is a one way function meaning that it should be impossible to retrieve the initial content from the digest.

&ensp;&ensp;&ensp; Such a technique has the following usages:

- Offering confidentiality when storing passwords,
- Checking for integrity for some downloaded files or content,
- Creation of digital signatures, which provides integrity and non-repudiation.

&ensp;&ensp;&ensp; In order to create digital signatures, the initial message or text needs to be hashed to get the digest. After that, the digest is to be encrypted using a public key encryption cipher. Having this, the obtained digital signature can be decrypted with the public key and the hash can be compared with an additional hash computed from the received message to check the integrity of it.

## Objectives:

1. Get familiar with the hashing techniques/algorithms.
2. Use an appropriate hashing algorithms to store passwords in a local DB.
   1. You can use already implemented algortihms from libraries provided for your language.
   2. The DB choise is up to you, but it can be something simple, like an in memory one.
3. Use an asymmetric cipher to implement a digital signature process for a user message.
   1. Take the user input message.
   2. Preprocess the message, if needed.
   3. Get a digest of it via hashing.
   4. Encrypt it with the chosen cipher.
   5. Perform a digital signature check by comparing the hash of the message with the decrypted one.

## Implementation Description:

### Hashing

`Database` deals with addding accounts to the datastore and verifying the password hash.

```C#
  public void AddUser(string email,string password)
        {
            if (Users.Any(x => x.Login.Equals(email))) return;
            Users.Add(new User(email, password));
        }
   public void Login(string login, string password)
        {
            if (!Users.Any(x => x.Login.Equals(login)))
            {
                Console.WriteLine("Wrong email");
                return;
            }
            using SHA256 mySHA256 = SHA256.Create();
            var passwordHash = mySHA256.ComputeHash(Encoding.UTF32.GetBytes(password));
            var user = Users.FirstOrDefault(u => u.Login.Equals(login));
            if (StructuralComparisons.StructuralEqualityComparer.Equals(passwordHash, user.Password))
            {
                Console.WriteLine("Successful login");
                return;
            }
            Console.WriteLine("Wrong password");
        }
```

`User` class deals with hashing the initial string and keeping the public key

```C#
public class User
    {
        public string Login { get; set; }
        public byte[] Password { get; set; }
        public List<byte[]> Messages { get; set; } = new();
        public KeyValuePair<int, int> PublicKey;

        public User(string login, string password)
        {
            Login = login;
            using SHA256 mySHA256 = SHA256.Create();
            var passwordHash = mySHA256.ComputeHash(Encoding.UTF32.GetBytes(password));
            PublicKey = Utility.GetCoPrimePair();
            Password = passwordHash;
        }
    }
```

`Login` verifies the password hash against the hash in the datastore and returns the result.

### Digital Signatures

`AddMessage and VerifySignature` deals with signing and verifying messages.

```C#
  public void AddMessage(User user,string text)
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {
                RSA rsa = new();
                var initialMsgHash = mySHA256.ComputeHash(Encoding.UTF32.GetBytes(text));
                var encryptedHash = rsa.Encrypt(initialMsgHash, user.PublicKey.Key, user.PublicKey.Value);
                user.Messages.Add(encryptedHash);
            }
        }
        public void VerifySignature(User user,string text)
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {
                RSA rsa = new();
                var initialMsgHash = mySHA256.ComputeHash(Encoding.UTF32.GetBytes(text));
                var encryptedHash = rsa.Encrypt(initialMsgHash, user.PublicKey.Key, user.PublicKey.Value);
                var decryptedHash = rsa.Decrypt(encryptedHash, user.PublicKey.Key, user.PublicKey.Value);
                var userMsg = user.Messages.FirstOrDefault(x =>
                BigInteger.Compare(new BigInteger(rsa.Decrypt(x,user.PublicKey.Key,user.PublicKey.Value)), new BigInteger(decryptedHash)) == 0);
                if (userMsg is null)
                {
                    Console.WriteLine("no such message");
                    return;
                }
                Console.WriteLine("Signature passed");
            }
        }
```

`AddMessage` takes in the message and the instance of user to whom it belongs hashes the text and encrypts it with RSA
`VerifySignature` requires to send a text that was already encrypted to encrypt it again, decrypt and check if the decryption for the message in the db is the same.

## Conclusions:

In this lab I got familiar with hashing techniques, and used RSA to store passwords to a in-memory datastore as well as refactored the folder structure.
Also I used RSA and SHA-256 to create a digital signatures and verify the integrity of a message.
