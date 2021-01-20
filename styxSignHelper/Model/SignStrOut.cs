using System;
using System.Runtime.Serialization;

namespace styxSignHelper.Model
{
    [Serializable]
    [DataContract]
    public class SignStrOut : ResponseBase
    {
        [DataMember(Name = "result")]
        public SignStrResult Result;

        public SignStrOut() 
        {
            Result = new SignStrResult();
        }
    }
}
