using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HBaseStargate.Entities
{
    [DataContract()]
    public class CellSet
    {
        [DataMember(Name = "Row")]
        public Row[] Rows { get; set; }
    }
}
