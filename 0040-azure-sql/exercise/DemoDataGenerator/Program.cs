using System;

const string VowelsUppercase = "AEIOU";
const string VowelsLowercase = "aeiou";
const string Consonants = "bcdfghjklmnpqrstvwxyz";

var NumberOfRecords = 1_000_000;
if (args.Length == 1 && int.TryParse(args[0], out var noOfRows)) NumberOfRecords = noOfRows;
var random = new Random();

Console.WriteLine("id,first_name,last_name,email,gender,ip_address");
for (var i = 0; i < NumberOfRecords; i++)
{
    Console.Write(i);
    Console.Write(',');
    string firstName, lastName;
    Console.Write(firstName = BuildName());
    Console.Write(',');
    Console.Write(lastName = BuildName());
    Console.Write(',');
    Console.Write($"{firstName}@{lastName}.com");
    Console.Write(',');
    Console.Write(random.Next(2) switch
    {
        0 => "male",
        1 => "female",
        _ => "unknown"
    });
    Console.Write(',');
    Console.WriteLine($"{random.Next(256)}.{random.Next(256)}.{random.Next(256)}.{random.Next(256)}");
}

string BuildName()
{
    return string.Create(random.Next(4, 8), random, (buf, rand) =>
    {
        buf[0] = VowelsUppercase[rand.Next(0, VowelsUppercase.Length)];
        buf = buf[1..];
        for (var i = 0; i < buf.Length; i++)
        {
            buf[i] = i % 2 == 0
                ? Consonants[rand.Next(0, Consonants.Length)]
                : VowelsLowercase[rand.Next(0, VowelsLowercase.Length)];
        }
    });
}