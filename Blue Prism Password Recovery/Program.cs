using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Threading;

namespace Blue_Prism_Password_Recovery
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Witaj!\nZa chwilę nastąpi zmiana danych logowania do BluePrism.\nNie wyłączaj programu!");
            Thread.Sleep(3000);
            string serverAddress = GetNamedPipe();
            if (serverAddress != "" && serverAddress.Contains("np:"))
            {
                Console.WriteLine($"Znaleziono serwer: {serverAddress}");
                using (var connection = new SqlConnection($"Server={serverAddress}"))
                {
                    Console.WriteLine("Łączenie z bazą danych...");
                    Thread.Sleep(2000);
                    try {
                        connection.Open();
                        Console.WriteLine("Połączenie udane.");
                        Console.WriteLine("Przygotowywanie zapytania do bazy danych...");
                        string query = "USE BluePrism UPDATE TOP(1) [dbo].[BPAPassword] SET [hash]='208512264222772174181102151942010236531331277169151', [salt]='', [type]=0";
                        SqlCommand command = new SqlCommand(query, connection);
                        Console.WriteLine("Wysyłanie zapytania do bazy danych...");
                        Thread.Sleep(2000);
                        command.ExecuteNonQuery();
                        Console.WriteLine("Pomyślnie wysłano zapytanie.");
                        Console.WriteLine("Rozłączanie z bazą danych...");
                        connection.Close();
                        Console.WriteLine("Pomyślnie rozłączono z bazą danych.");
                        Console.WriteLine("Twoje nowe dane logowania:\nLogin: admin\nHasło: admin");
                        Console.WriteLine("Możesz je dowolnie zmienić w ustawieniach ;)");
                    }
                    catch (Exception ex) {
                        string errMes = ex.Message;
                        Console.WriteLine("Wystąpił błąd!\nJeśli ten komunikat będzie się powtarzać proszę o kontakt: jakub@kocmouch.xyz lub jakubkaminski@duck.com");
                        Console.WriteLine($"Kod błędu:{errMes}");
                    }
                    
                }
            } else {
                Console.WriteLine("Nie znaleziono serwera!\nSprawdź czy aplikacja Blue Prism jest uruchomiona\nJeśli nie, uruchom ją i ponownie ");
            }

            Console.WriteLine("Dziękuję za skorzystanie z programu!\nMade with <3 by Jakub 'Kocmouch' Kamiński\nW razie pytań proszę o kontakt:\njakub@kocmouch.xyz lub jakubkaminski@duck.com");
            Console.WriteLine("Wciśnięcie dowolnego przycisku zakończy działanie progamu");
            Console.ReadKey();
        }

        static string GetNamedPipe()
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/C sqllocaldb info BluePrismLocalDB",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            var output = new List<string>();
            process.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    output.Add(args.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();

            var lastLine = output[output.Count - 1];

            return lastLine.Remove(0, 20);
        }
    }
}
