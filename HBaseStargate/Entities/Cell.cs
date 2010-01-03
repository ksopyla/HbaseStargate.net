using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace HBaseStargate.Entities
{
    [DataContract]
    public class Cell
    { 
        [DataMember(Name = "timestamp",Order=1,EmitDefaultValue=false,IsRequired=false)]
        public long Timestamp{ get; set;}

        [DataMember(Name = "column",Order=2)]
        public string Column{ get; set;}

        [DataMember(Name = "$", Order = 3)]
        public string Value { get; set; }

    }
}
