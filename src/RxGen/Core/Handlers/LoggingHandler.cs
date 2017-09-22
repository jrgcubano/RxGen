using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RxGen.Core.Handlers
{
    public class LoggingHandler : DelegatingHandler
    {
        public LoggingHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            System.Console.WriteLine("Request:");
            System.Console.WriteLine(request.ToString());
            if (request.Content != null)
            {
                System.Console.WriteLine(await request.Content.ReadAsStringAsync());
            }

            System.Console.WriteLine();
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            System.Console.WriteLine("Response:");
            System.Console.WriteLine(response.ToString());
            if (response.Content != null)
            {
                System.Console.WriteLine(await response.Content.ReadAsStringAsync());
            }

            System.Console.WriteLine();
            return response;
        }
    }
}