using BivConnection.CSStatusInformatieService;
using System;
using System.ServiceModel;
using System.Diagnostics;
using System.Security;

namespace BivConnection
{
    internal class StatusInformatieService : AbstractService
    {

        public StatusResultaat[] GetNieuweStatussenProces(string identifier, SecureString password)
        {
            Console.WriteLine("Creating request for identifier: " + identifier);

            var request = new getNieuweStatussenProcesRequest1
            {
                getNieuweStatussenProcesRequest = new getNieuweStatussenProcesRequest
                {
                    kenmerk = identifier,
                    autorisatieAdres = "http://geenausp.nl/"
                }
            };

            Console.WriteLine("Creating client");
            var client = GetClient<StatusinformatieService_V1_2>("/biv-wus20v12/StatusInformatieService", password);

            Console.WriteLine("Sending request");
            var timer = new Stopwatch();
            timer.Start();
            try
            {
                var response = client.getNieuweStatussenProces(request);
                Console.WriteLine("Succesfully sent request");
                Console.WriteLine("Number of statusses: " + response.getNieuweStatussenProcesResponse.getNieuweStatussenProcesReturn.Length);
                timer.Stop();
                Console.WriteLine($"Received response in {timer.Elapsed}");
                return response.getNieuweStatussenProcesResponse.getNieuweStatussenProcesReturn;
            }
            catch (FaultException<foutType> fault)
            {
                Console.WriteLine("Error sending request");
                Console.WriteLine("Code: " + fault.Detail.foutcode);
                Console.WriteLine("Message: " + fault.Detail.foutbeschrijving);
            }
            return null;
        }

        public StatusResultaat[] GetStatussenProces(string identifier, SecureString password)
        {
            Console.WriteLine("Creating request for identifier: " + identifier);

            var request = new getStatussenProcesRequest1
            {
                getStatussenProcesRequest = new getStatussenProcesRequest
                {
                    kenmerk = identifier,
                    autorisatieAdres = "http://geenausp.nl/"
                }
            };

            Console.WriteLine("Creating client");
            var client = GetClient<StatusinformatieService_V1_2>("/biv-wus20v12/StatusInformatieService", password);

            Console.WriteLine("Sending request");
            var timer = new Stopwatch();
            timer.Start();
            try
            {
                var response = client.getStatussenProces(request);
                Console.WriteLine("Succesfully sent request");
                Console.WriteLine("Number of statusses: " + response.getStatussenProcesResponse.getStatussenProcesReturn.Length);
                timer.Stop();
                Console.WriteLine($"Received response in {timer.Elapsed}");
                return response.getStatussenProcesResponse.getStatussenProcesReturn;
            }
            catch (FaultException<foutType> fault)
            {
                Console.WriteLine("Error sending request");
                Console.WriteLine("Code: " + fault.Detail.foutcode);
                Console.WriteLine("Message: " + fault.Detail.foutbeschrijving);
            }
            return null;
        }
    }
}
