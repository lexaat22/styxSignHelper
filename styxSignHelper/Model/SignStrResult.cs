using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace styxSignHelper.Model
{
    [Serializable]
    [DataContract]
    public class SignStrResult
    {
        [DataMember(Name = "str_to_sign")]
        public string StrToSign { get; set; }

        [DataMember(Name = "cert_sn")]
        public string CertSn { get; set; }

        [DataMember(Name = "signature")]
        public string Signature { get; set; }

    }
}
