using System;
using System.Runtime.Serialization;

namespace styxSignHelper.Model
{
    [DataContract]
    [Serializable]
    public class SignMSGOut
    {
        [DataMember(Name = "result")]
        public int Result { get; set; } = 0;

        [DataMember(Name = "errorCode")]
        public int ErrorCode { get; set; } = 0;

        [DataMember(Name = "status")]
        public string Status { get; set; } = "success";

        [DataMember(Name = "signedMsg")]
        public string SignedMsg { get; set; }
    }
}
