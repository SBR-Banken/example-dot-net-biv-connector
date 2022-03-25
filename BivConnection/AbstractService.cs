using System;
using System.IO;
using System.Net.Security;
using System.Reflection;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;

namespace BivConnection
{
    internal abstract class AbstractService
    {
        // Not included in example, replace with your own certificate.
        private const string ClientCertPath = "BivConnection.Cert.Client.p12";

        // Not included in example, replace with the certificate of the BIV server you connect with.
        private const string ServerCertPath = "BivConnection.Cert.Server.crt";

        // Preconfigured for BTT, for production use btp-frcportaal.nl
        private const string EndPointUrl = "https://btt-frcportaal.nl/";

        protected const string ContentsPath = "BivConnection.Docs.";

        private static X509Certificate2 GetEmbeddedCertificate(string filename, SecureString password = null)
        {
            var bytes = GetEmbeddedFile(filename);
            return password != null ? new X509Certificate2(bytes, password, X509KeyStorageFlags.DefaultKeySet) : new X509Certificate2(bytes);
        }

        protected static byte[] GetEmbeddedFile(string path)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(path))
            {
                using (var streamReader = new MemoryStream())
                {
                    stream.CopyTo(streamReader);
                    return streamReader.ToArray();
                }
            }
        }

        protected static T GetClient<T>(string endpoint, SecureString password)
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
            var encodingBindingElement = new MtomMessageEncodingBindingElement(MessageVersion.Soap11WSAddressing10, Encoding.UTF8);

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

            var endPointUri = new Uri(EndPointUrl);

            var address = new EndpointAddress(new Uri(endPointUri, endpoint));
            var factory = new ChannelFactory<T>(cbinding, address);
            var certClient = GetEmbeddedCertificate(ClientCertPath, password);
            var certService = GetEmbeddedCertificate(ServerCertPath);

            //Explicit prevent check on chain of trust
            factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
            factory.Credentials.ClientCertificate.Certificate = certClient;
            factory.Credentials.ServiceCertificate.DefaultCertificate = certService;
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                {
                    return certificate.Equals(factory.Credentials.ServiceCertificate.DefaultCertificate);
                };
            
            return factory.CreateChannel();
        }
    }
}
