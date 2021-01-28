using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Web;

namespace styxSignHelper
{
    public class CustomHeaderMessageInspector : IDispatchMessageInspector
    {
        Dictionary<string, string> requiredHeaders;
        private static IDictionary<string, string> _headersToInject = new Dictionary<string, string>
      {
        { "Access-Control-Allow-Origin", "*" },
        { "Access-Control-Request-Methods", "POST,GET,HEAD,OPTIONS" },
        { "Access-Control-Allow-Methods", "POST,GET,HEAD,OPTIONS" },
        { "Access-Control-Allow-Headers", "X-Requested-With, X-PINGOTHER, Content-Type, Accept, Origin, Connection, Sec-Fetch-Site, Sec-Fetch-Mode, Sec-Fetch-Dest, Accept-Encoding, Accept-Language" }
      };
        public CustomHeaderMessageInspector(Dictionary<string, string> headers)
        {
            requiredHeaders = headers ?? new Dictionary<string, string>();
        }
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            //string displayText = $"Server has received the following message:\n{request}\n";
            //Console.WriteLine(displayText);
            //return null;

            var httpRequest = (HttpRequestMessageProperty)request
    .Properties[HttpRequestMessageProperty.Name];
            return new
            {
                origin = httpRequest.Headers["Origin"],
                handlePreflight = httpRequest.Method.Equals("OPTIONS",
                StringComparison.InvariantCultureIgnoreCase)
            };


        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            var httpHeader = reply.Properties["httpResponse"] as HttpResponseMessageProperty;
            foreach (var item in _headersToInject)
                httpHeader.Headers.Add(item.Key, item.Value);

            //if (!reply.Properties.ContainsKey("httpResponse"))
            //    reply.Properties.Add("httpResponse", new HttpResponseMessageProperty());

            //var httpHeader = reply.Properties["httpResponse"] as HttpResponseMessageProperty;
            //foreach (var item in requiredHeaders)
            //{
            //    httpHeader.Headers.Add(item.Key, item.Value);
            //}

            //string displayText = $"Server has replied the following message:\n{reply}\n";
            //Console.WriteLine(displayText);

        }
    }
}
