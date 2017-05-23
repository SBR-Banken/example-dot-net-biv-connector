using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;
using BivConnection.CSAanleverService;

namespace BivConnection
{
    class Program
    {
        private const string ContentsFilename = "frc-rpt-ihz-aangifte-2016.xbrl";
        private const string ContentsPath = "BivConnection.Docs." + ContentsFilename;

        // Not included in example.
        private const string ClientCertPath = "BivConnection.Cert.Client.p12";
        private const string ClientCertPassword = "";

        // Not included in example
        private const string ServerCertPath = "BivConnection.Cert.Server.crt";

        private const string EndPointAddress = "https://bta-frcportaal.nl/biv-wus20v12/AanleverService";

        static void Main(string[] args)
        {
            Console.WriteLine("Creating request");
            var request = CreateRequest();

            Console.WriteLine("Creating client");
            var client = GetClient();

            Console.WriteLine("Sending request");
            var timer = new Stopwatch();
            timer.Start();
            var response = client.aanleveren(request);
            timer.Stop();
            Console.WriteLine($"Received response in {timer.Elapsed}");

            Console.ReadKey();
        }

        private static aanleverenRequest CreateRequest()
        {
            return new aanleverenRequest
            {
                aanleverRequest = new aanleverRequest
                {
                    aanleverkenmerk = Guid.NewGuid().ToString(),
                    berichtsoort = "Testaanlevering",
                    identiteitBelanghebbende = new identiteitType()
                    {
                        nummer = "12345678",
                        type = "kvk"
                    },
                    rolBelanghebbende = "",
                    berichtInhoud = new berichtInhoudType()
                    {
                        bestandsnaam = ContentsFilename,
                        mimeType = "application/xml",
                        inhoud = GetEmbeddedFile(ContentsPath)
                    },
                    autorisatieAdres = "http://localhost:8080/ode/processes/CSPService-OK",
                }
            };
        }

        private static AanleverService_V1_2 GetClient()
        {
            //Create binding element for security
            var asymmetricSecurityBindingElement = (AsymmetricSecurityBindingElement)SecurityBindingElement.CreateMutualCertificateBindingElement(MessageSecurityVersion.
                WSSecurity10WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10
            );
            asymmetricSecurityBindingElement.MessageSecurityVersion = MessageSecurityVersion.WSSecurity10WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10;
            asymmetricSecurityBindingElement.EnableUnsecuredResponse = true;
            asymmetricSecurityBindingElement.MessageProtectionOrder = MessageProtectionOrder.EncryptBeforeSign;
            asymmetricSecurityBindingElement.IncludeTimestamp = true;
            asymmetricSecurityBindingElement.DefaultAlgorithmSuite = SecurityAlgorithmSuite.TripleDesRsa15;

            //Explicit accept secured answers from endpoint
            asymmetricSecurityBindingElement.AllowSerializedSigningTokenOnReply = true;

            //Create binding element for encoding
            var encodingBindingElement = new TextMessageEncodingBindingElement(MessageVersion.Soap11WSAddressing10, Encoding.UTF8);            
            
            //Create binding element for transport
            var httpsTransportBindingElement = new HttpsTransportBindingElement
            {
                RequireClientCertificate = true,
                AuthenticationScheme = System.Net.AuthenticationSchemes.Anonymous
            };

            var cbinding = new CustomBinding();
            cbinding.Elements.Add(asymmetricSecurityBindingElement);
            cbinding.Elements.Add(encodingBindingElement);
            cbinding.Elements.Add(httpsTransportBindingElement);

            var address = new EndpointAddress(EndPointAddress);
            var factory = new ChannelFactory<AanleverService_V1_2>(cbinding, address);
            var certClient = GetEmbeddedCertificate(ClientCertPath, ClientCertPassword);
            var certService = GetEmbeddedCertificate(ServerCertPath);

            //Explicit prevent check on chain of trust
            factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
            factory.Credentials.ClientCertificate.Certificate = certClient;
            factory.Credentials.ServiceCertificate.DefaultCertificate = certService;

            return factory.CreateChannel();
        }

        private static X509Certificate2 GetEmbeddedCertificate(string filename, string password = null)
        {
            var bytes = GetEmbeddedFile(filename);

            // http://paulstovell.com/blog/x509certificate2 Tip #5
            var tmpFileName = Path.Combine(Path.GetTempPath(), $"BivConnection-{Guid.NewGuid()}");
            try
            {
                File.WriteAllBytes(tmpFileName, bytes);
                return string.IsNullOrWhiteSpace(password) ? new X509Certificate2(tmpFileName) : new X509Certificate2(tmpFileName, password);
            }
            finally
            {
                File.Delete(tmpFileName);
            }
        }

        private static byte[] GetEmbeddedFile(string path)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(path))
            {
                using (var reader = new StreamReader(stream))
                {
                    var result = reader.ReadToEnd();
                    return Encoding.ASCII.GetBytes(result);
                }
            }
        }
    }
}
