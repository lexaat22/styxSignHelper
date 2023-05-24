using System;
using System.Runtime.Serialization;

namespace styxSignHelper.Model
{
    [DataContract]
    [Serializable]
    public class SignMSGIn
    {
        [DataMember(Name = "obj")]
        public string Obj { get; set; }

        [DataMember(Name = "cert_sn")]
        public string CertSn { get; set; }
    }
}
