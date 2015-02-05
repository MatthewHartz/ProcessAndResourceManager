using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessAndResourceManager
{
    class ResourceControlBlock
    {
        public string Rid;
        public int MaxUnits;
        public int CurUnits;
        public List<KeyValuePair<ProcessControlBlock, int>> WaitingList;

        public ResourceControlBlock(string rid, int units)
        {
            Rid = rid;
            MaxUnits = CurUnits = units;
            WaitingList = new List<KeyValuePair<ProcessControlBlock, int>>();
        }
    }
}
