using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace styxSignHelper
{
    public class MyBehaviorAttribute : Attribute, IContractBehavior, IContractBehaviorAttribute
    {
        string headers = "X-Requested-With, X-PINGOTHER, Content-Type, Accept, Origin, Connection, Sec-Fetch-Site, Sec-Fetch-Mode, Sec-Fetch-Dest, Accept-Encoding, Accept-Language";
        public Type TargetContract => typeof(MyBehaviorAttribute);

        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
        {
            var requiredHeaders = new Dictionary<string, string>();

            requiredHeaders.Add("Access-Control-Allow-Origin", "*");
            requiredHeaders.Add("Access-Control-Allow-Methods", "GET,HEAD,POST,DEBUG,OPTIONS");
            requiredHeaders.Add("Access-Control-Request-Headers", headers);
            requiredHeaders.Add("Access-Control-Allow-Headers", headers);
            requiredHeaders.Add("Access-Control-Max-Age", "86400");

            dispatchRuntime.MessageInspectors.Add(new CustomHeaderMessageInspector(requiredHeaders));
        }

        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
        }
    }
}
