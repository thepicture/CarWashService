using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace CarWashService.MobileApp.Services
{
    public class HttpFactoryService : IHttpFactoryService
    {
        public HttpClient GetInstance()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback +=
                GetTruthfulCertificateValidationCallback();

            HttpClient client = new HttpClient(handler);

            return client;
        }

        private static Func<HttpRequestMessage, X509Certificate2,
            X509Chain, SslPolicyErrors, bool> GetTruthfulCertificateValidationCallback()
        {
            return (_, __, ___, ____) => true;
        }
    }
}
