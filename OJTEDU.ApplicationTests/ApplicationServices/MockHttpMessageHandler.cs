using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Application.UnitTests.ApplicationServices
{
    internal class MockHttpMessageHandler : DelegatingHandler
    {
        private readonly string _responseContent;
        private readonly HttpStatusCode _statusCode;

        public MockHttpMessageHandler(string responseContent, HttpStatusCode statusCode)
        {
            _responseContent = responseContent;
            _statusCode = statusCode;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = _statusCode,
                Content = new StringContent(_responseContent, Encoding.UTF8, "application/json")
            };
            return await Task.FromResult(responseMessage);
        }
    }
}
