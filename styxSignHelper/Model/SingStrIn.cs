using System;
using System.Runtime.Serialization;

namespace styxSignHelper.Model
{
    [DataContract]
    [Serializable]
    public class SingStrIn
    {
        [DataMember(Name = "cert_sn")]
        public string CertSn { get; set; }

        [DataMember(Name = "str_to_sign")]
        public string StrToSign { get; set; }
    }
}
