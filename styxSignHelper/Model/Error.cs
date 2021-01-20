using System;
using System.Runtime.Serialization;

namespace styxSignHelper.Model
{
    [Serializable]
    [DataContract]
    public class Error
    {
        [DataMember(Name = "code")]
        public int Code { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        public Error()
        {
            Code = 0;
            Message = "";
        }

        public Error(int code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}
