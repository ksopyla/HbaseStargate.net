using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HBaseStargate.Entities
{
    [DataContract()]
    public class Row
    {

        [DataMember(Name = "key",Order=1)]
        public string Key { get; set; }

        [DataMember(Name="Cell",Order=2)]
        public CellList Cells { get; set; }
        
    }
}
