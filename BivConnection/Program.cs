using System;
using System.Security;
using BivConnection.CSStatusInformatieService;

namespace BivConnection
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var connector = new Program();

            Console.WriteLine("Please enter your certificate password.");
            var password = GetPassword();

            // do a annual report with assurance delivery
            connector.XbrlFileProcessor("frc-rpt-nt-inrichtingsjaarrekening-2021.xbrl", "30267975", password, "Jaarrekening_AV","00000000123456780000", new string [] {"nba-controleverklaring-goedkeurend-NL.xbrl", "signature.xml"});

            // do a valuation report with double signature delivery
            connector.XbrlFileProcessor("frc-rpt-vt-taxatierapport.xbrl", "12345678", password, "Taxatie_DH", "00000000123456780000", new string [] {"signature-about-document.xml", "signature-about-signature.xml"});
            Console.WriteLine($"\nPress any key to stop application...");            
            Console.ReadKey();
        }

        private void XbrlFileProcessor(string fileName, string identifier, SecureString password, string messageType, string IdOntvanger, string[] attachmentFileNames)
        {
            Console.WriteLine("\n===");
            var aanleverService = new AanleverService();
            var response = aanleverService.Aanleveren(fileName, identifier, password, messageType, IdOntvanger, attachmentFileNames);
            if (response == null) return;

            var kenmerk = response.kenmerk;
            Console.WriteLine("Succesfully sent request");
            Console.WriteLine($"Identifier: {response.kenmerk}");
            Console.WriteLine($"Statuscode: {response.statuscode}");
            Console.WriteLine($"Statusmessage: {response.statusdetails}");


            var statusInformatieService = new StatusInformatieService();
            var statusses = statusInformatieService.GetNieuweStatussenProces(kenmerk, password);
            Console.WriteLine($"All new statusses for identifier: {kenmerk}");
            if (statusses != null)
            {
                PrintStatusResults(statusses);
            }
            
            statusses = statusInformatieService.GetStatussenProces(kenmerk, password);
            Console.WriteLine($"All statusses for identifier: {kenmerk}");
            if (statusses != null)
            {
                PrintStatusResults(statusses);
            }
        }

        private static void PrintStatusResults(StatusResultaat[] statusses)
        {
            foreach (var status in statusses)
            {
                Console.WriteLine(" ---");
                Console.WriteLine($" Status details: {status.statusdetails}");
                Console.WriteLine($" Timestamp status: {status.tijdstempelStatus}");
                Console.WriteLine($" Statuscode: {status.statuscode}");
                Console.WriteLine($" Status description: {status.statusomschrijving}");
                if (status.statusFoutcode == null) continue;
                Console.WriteLine($" Error code: {status.statusFoutcode.foutcode}");
                Console.WriteLine($" Error description: {status.statusFoutcode.foutbeschrijving}");
            }
        }
        
        private static SecureString GetPassword()
        {
            var pwd = new SecureString();
            while (true)
            {
                var i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length <= 0) continue;
                    pwd.RemoveAt(pwd.Length - 1);
                    Console.Write("\b \b");
                }
                else if (i.KeyChar != '\u0000' ) // KeyChar == '\u0000' if the key pressed does not correspond to a printable character, e.g. F1, Pause-Break, etc
                {
                    pwd.AppendChar(i.KeyChar);
                    Console.Write("*");
                }
            }
            return pwd;
        }
    }
}
