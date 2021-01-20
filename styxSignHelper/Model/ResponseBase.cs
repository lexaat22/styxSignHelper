using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace styxSignHelper.Model
{
    [Serializable]
    [DataContract]
    public class ResponseBase
    {
        [DataMember(Name = "error")]
        public Error error;

        public ResponseBase()
        {
            error = new Error();
        }
        public ResponseBase(long id)
        {
            error = new Error();
        }
    }
}
