using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessAndResourceManager
{
    class ProcessControlBlock
    {
        private string Pid;
        private List<ResourceControlBlock> Other_Resources;
        private ProcessStates StatusType;
        // private List<StatusList>* StatusList;
        private ProcessControlBlock Parent;
        private List<ProcessControlBlock> Children;

    }
}
