﻿using BivConnection.CSAanleverService;
using System;
using System.Diagnostics;
using System.Security;
using System.ServiceModel;

namespace BivConnection 
{
    internal class AanleverService : AbstractService
    {

        public aanleverResponse Aanleveren(string fileName, String identifier, SecureString password, String messageType, String IdOntvanger)
        {
            Console.WriteLine("Creating request for fileName: " + fileName);
            var request = CreateRequest(fileName, identifier, messageType, IdOntvanger);

            Console.WriteLine("Creating client");
            var client = GetClient<AanleverService_V1_2>("/biv-wus20v12/AanleverService", password);

            Console.WriteLine("Sending request");
            var timer = new Stopwatch();
            timer.Start();

            try
            {
                var response = client.aanleveren(request);
                timer.Stop();
                Console.WriteLine($"Received response in {timer.Elapsed}");
                return response.aanleverResponse;               
            }
            catch (FaultException<foutType> fault)
            {
                Console.WriteLine("Error sending request");
                Console.WriteLine($"Code: {fault.Detail.foutcode}");
                Console.WriteLine($"Message: {fault.Detail.foutbeschrijving}");
                return null;
            }
            catch(Exception e)
            {
                Console.WriteLine("Error sending request");
                Console.WriteLine($"Message: {e.Message}");
                return null;
            }
        }

        private static aanleverenRequest CreateRequest(string contentsFilename, String identifier, String messageType, String IdOntvanger)
        {
            return new aanleverenRequest
            {
                aanleverRequest = new aanleverRequest
                {
                    aanleverkenmerk = Guid.NewGuid().ToString(),
                    berichtsoort = messageType,
                    identiteitOntvanger = !String.IsNullOrEmpty(IdOntvanger) ? new identiteitType() { nummer = IdOntvanger,type = "ID-ontvanger" } : null,
                    identiteitBelanghebbende = new identiteitType() { nummer = identifier, type = "kvk" },
                    rolBelanghebbende = "",
                    berichtInhoud = new berichtInhoudType()
                    {
                        bestandsnaam = contentsFilename,
                        mimeType = "application/xml",
                        inhoud = GetEmbeddedFile(ContentsPath + contentsFilename)
                    },
                    autorisatieAdres = "http://geenausp.nl/"
                }
            };
        }
    }
}
