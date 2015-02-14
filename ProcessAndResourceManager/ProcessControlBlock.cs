using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessAndResourceManager
{
    class ProcessControlBlock
    {
        public string Pid;
        public List<KeyValuePair<ResourceControlBlock, int>> OtherResources;
        public ProcessStates StatusType;
        public object StatusList;
        public ProcessControlBlock Parent;
        public List<ProcessControlBlock> Children;
        public Priorities Priority;

        public ProcessControlBlock(string pid, Priorities priority)
        {
            Pid = pid;
            Priority = priority;
            StatusType = ProcessStates.Ready;
            Children = new List<ProcessControlBlock>();
            OtherResources = new List<KeyValuePair<ResourceControlBlock, int>>();
        }
    }
}
