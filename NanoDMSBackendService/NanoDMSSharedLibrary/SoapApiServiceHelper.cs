using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NanoDMSSharedLibrary
{
    public  class SoapApiServiceHelper
    {
        private readonly HttpClient _httpClient;

        public SoapApiServiceHelper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> SendSoapRequestAsync(string url, string soapAction, string soapBody)
        {
            try
            {
                // Construct SOAP request
                var soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
                <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:web=""http://example.com/webservices"">
                    <soapenv:Header/>
                    <soapenv:Body>
                        {soapBody}
                    </soapenv:Body>
                </soapenv:Envelope>";

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml")
                };

                // Add SOAPAction header if required by the API
                request.Headers.Add("SOAPAction", soapAction);

                // Send request
                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception($"SOAP API Error: {response.StatusCode}, Message: {errorMessage}");
                }

                // Get response as string
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"SOAP Request Failed: {ex.Message}");
            }
        }

        //Example: Call a SOAP API to Get User Info//
        //    var soapService = new SoapApiService(new HttpClient());

        //    string soapAction = "http://example.com/webservices/GetUser";
        //    string soapBody = @"
        //<GetUser>
        //    <UserId>12345</UserId>
        //</GetUser>";

        //    string responseXml = await soapService.SendSoapRequestAsync("https://api.example.com/soap", soapAction, soapBody);
        //    Console.WriteLine(responseXml);


        //How to Parse SOAP XML Response//

        //XDocument doc = XDocument.Parse(responseXml);
        //string userName = doc.Descendants("UserName").FirstOrDefault()?.Value;
        //Console.WriteLine($"User Name: {userName}");



    }
}
