// See https://aka.ms/new-console-template for more information
using PwCracker;

Console.Write("Enter hash to decrypt: ");
string hash = Console.ReadLine() ?? string.Empty;
string decrypted = PasswordCracker.Decrypt(hash);
Console.WriteLine($"Decrypted password: {decrypted}");
Console.WriteLine("Press any key to exit...");
Console.ReadKey();

