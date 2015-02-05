using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessAndResourceManager
{
    class ProcessAndResourceManager
    {
        private static ProcessAndResourceManager _instance;
        private List<ProcessControlBlock>[] ReadyList;
        private List<ResourceControlBlock> Resources; 
        private ProcessControlBlock RunningProcess;

        private ProcessAndResourceManager() { }
        public static ProcessAndResourceManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ProcessAndResourceManager();
                    _instance.Initialize();
                }
                return _instance;
            }
        }


        /// <summary>
        /// Initializes the Process and Resource manager state back to the original
        /// state.
        /// </summary>
        public void Initialize()
        {
            // Initialize the ready list
            ReadyList = new List<ProcessControlBlock>[3]
            {
                new List<ProcessControlBlock>(), 
                new List<ProcessControlBlock>(), 
                new List<ProcessControlBlock>()
            };

            // Create new Process
            var process = new ProcessControlBlock("init", Priorities.Init);

            // Initialize process
            process.StatusList = ReadyList;

            // Add process to ready list
            ReadyList[0].Add(process);

            // Create resources
            Resources = new List<ResourceControlBlock>()
            {
                new ResourceControlBlock("R1", 1),
                new ResourceControlBlock("R2", 2),
                new ResourceControlBlock("R3", 3),
                new ResourceControlBlock("R4", 4),
            };

            Scheduler();
        }

        /// <summary>
        /// Terminates the execution of the system
        /// </summary>
        public void Quit() { throw new NotImplementedException(); }

        /// <summary>
        /// Creates a new new process <name> at the priority level <priority>;
        /// <name> is a single character; <priority> can be a 1 or 2 (0 is reserved for Init process).
        /// </summary>
        /// <param name="name">The process name.</param>
        /// <param name="priority">The process priority.</param>
        public void Create(char name, int priority)
        {
            // Error if priority is not 1 or 2
            if (priority != 1 && priority != 2)
            {
                throw new Exception("Priority must be 1 or 2");
            }

            // Recursively search children tree of the init process to see if process id already exists
            if (FindProcessById(name.ToString(), ReadyList[0][0]) != null)
            {
                throw new Exception("process name already exists");
            }


            // Create new Process
            var process = new ProcessControlBlock(name.ToString(), (Priorities)priority);

            // Initialize process
            process.StatusList = ReadyList;
            process.Parent = RunningProcess;

            // Update parent process
            RunningProcess.Children.Add(process);

            // Add process to ready list
            ReadyList[priority].Add(process);

            Scheduler();
        }

        /// <summary>
        /// Destroys the process <name> and all of its descendants.
        /// </summary>
        /// <param name="name">The process name.</param>
        public void Destroy(char name)
        {
            // Search for process starting at the init process
            var p = FindProcessById(name.ToString(), ReadyList[0][0]);

            if (p == null)
            {
                throw new Exception("process does not exist");
            }

            KillTree(p);

            Scheduler();
        }


        /// <summary>
        /// Requests the specified resource for # of <units> units. <resource> can only be 1,2,3,4
        /// <units> can only be at max equal to the value of <resource>
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="units">The units.</param>
        public void Request(string resource, int units)
        {
            // Get the resource
            var r = Resources.First(x => x.Rid.ToLower() == resource.ToLower());

            // Can't find resource, throw exception
            if (r == null)
            {
                throw new Exception("could not located resource");
            }

            // Max units is less than requested units, throw exception
            if (units > r.MaxUnits)
            {
                throw new Exception("requested units too large for resource");
            }

            // If there are enough available units, access resource
            if (units <= r.CurUnits)
            {
                r.CurUnits -= units;
                RunningProcess.OtherResources.Add(r);
            }
            // Otherwise, block process, add resource to status list, remove process from ready queue,
            // and insert process to waitlist of resource
            else
            {
                RunningProcess.StatusType = ProcessStates.Blocked;
                RunningProcess.StatusList = r;
                ReadyList[(int)RunningProcess.Priority].RemoveAt(0);
                //r.WaitingList.Add(RunningProcess, units);
                r.WaitingList.Add(new KeyValuePair<ProcessControlBlock, int>(RunningProcess, units));
            }

            Scheduler();
        }


        /// <summary>
        /// Releases the specified resource for # of <units> units from <resource>. <resource> can only be 1,2,3,4
        /// <units> can only be at max equal to the value of <resource>
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="units">The units.</param>
        public void Release(string resource, int units)
        {
            // Get the resource
            var r = Resources.First(x => x.Rid.ToLower() == resource.ToLower());

            // Can't find resource, throw exception
            if (r == null)
            {
                throw new Exception("could not located resource");
            }

            // Release the resource from the current running process
            RunningProcess.OtherResources.Remove(r);
            r.CurUnits += units;

            // loop through all waiting processes to get access to resource
            while (r.WaitingList.Count != 0 && r.WaitingList[0].Value <= r.CurUnits)
            {
                // Get process from waiting list
                var process = r.WaitingList[0].Key;

                // Remove process from waiting list
                r.WaitingList.RemoveAt(0);

                process.StatusType = ProcessStates.Ready;
                process.StatusList = ReadyList;
                process.OtherResources.Add(r);

                // Insert process into ready list
                ReadyList[(int)process.Priority].Add(process);
            }

            Scheduler();
        }


        /// <summary>
        /// Timesouts the manager and shifts the current running process to the back of it's queue,
        /// and sets the current running process to the next in line.
        /// </summary>
        public void Timeout()
        {
            for (var i = 2; i > 0; i--)
            {
                if (ReadyList[i].Count > 0)
                {
                    var process = ReadyList[i][0];
                    ReadyList[i].RemoveAt(0);
                    ReadyList[i].Add(process);

                    break;
                }
            }

            Scheduler();
        }


        /// <summary>
        /// Gets the running process.
        /// </summary>
        /// <returns></returns>
        public string GetRunningProcess()
        {
            return RunningProcess.Pid;
        }

        /// <summary>
        /// Recursive function that searches for the process by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="process">The current process.</param>
        /// <returns></returns>
        private static ProcessControlBlock FindProcessById(string id, ProcessControlBlock process)
        {
            if ((process == null) || process.Pid == id) return process;

            // recursively navigate through tree to find process, otherwise return null
            foreach (var proc in process.Children)
            {
                var p = FindProcessById(id, proc);
                if (p != null) return p;
            }

            return null;
        }

        private void KillTree(ProcessControlBlock process)
        {
            foreach (var proc in process.Children)
            {
                KillTree(proc);
            }
        }


        /// <summary>
        /// Helper function that adjusts the ready list based upon recent changes
        /// </summary>
        private void Scheduler()
        {
            for (var i = 2; i >= 0; i--)
            {
                if (ReadyList[i].Count > 0)
                {
                    RunningProcess = ReadyList[i][0];
                    return;
                }
            }

            throw new Exception("No processes running: EXTREME FAILURE!");
        }
    }
}
