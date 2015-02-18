using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            Scheduler(process);
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
                throw new Exception(String.Format("duplicate process name: {0}", name));
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

            Scheduler(process);
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

            // Deletes children
            KillTree(p);

            // Removes process from parent
            p.Parent.Children.Remove(p);
            p.Parent = null;

            Scheduler(null);
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
            ResourceControlBlock r;

            try
            {
                r = Resources.First(x => x.Rid.ToLower() == resource.ToLower());
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("non-existent resource: {0}", resource));
            }

            // Max units is less than requested units, throw exception
            if (units > r.MaxUnits)
            {
                throw new Exception(String.Format("request too many units: {0}/{1}", units, resource));
            }

            // If there are enough available units, access resource
            if (units <= r.CurUnits)
            {
                // Test and see if process already allocates a part of this resource
                var ResourceBeingUsed = new KeyValuePair<ResourceControlBlock, int>();

                try
                {
                    ResourceBeingUsed = RunningProcess.OtherResources.First(x => x.Key.Rid == resource);
                }
                catch (Exception e)
                {
                }

                if (ResourceBeingUsed.Key != null)
                {
                    var newPair = new KeyValuePair<ResourceControlBlock, int>(ResourceBeingUsed.Key, ResourceBeingUsed.Value + units);
                    RunningProcess.OtherResources.Remove(
                        RunningProcess.OtherResources.First(x => x.Key == ResourceBeingUsed.Key));
                    RunningProcess.OtherResources.Add(newPair);
                    r.CurUnits -= units;
                }
                else
                {
                    r.CurUnits -= units;
                    RunningProcess.OtherResources.Add(new KeyValuePair<ResourceControlBlock, int>(r, units));
                }

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

            Scheduler(RunningProcess);
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

            ReleaseResource(RunningProcess, r, units);

            Scheduler(RunningProcess);
        }


        /// <summary>
        /// Timesouts the manager and shifts the current running process to the back of it's queue,
        /// and sets the current running process to the next in line.
        /// </summary>
        public void Timeout()
        {
            ProcessControlBlock process = null;

            for (var i = 2; i > 0; i--)
            {
                if (ReadyList[i].Count > 0)
                {
                    process = ReadyList[i][0];
                    break;
                }
            }

            ReadyList[(int) process.Priority].Remove(process);
            process.StatusType = ProcessStates.Ready;
            ReadyList[(int) process.Priority].Add(process);

            Scheduler(process);
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

        /// <summary>
        /// Releases resources from all children nodes and from process.
        /// </summary>
        /// <param name="process">The process.</param>
        private void KillTree(ProcessControlBlock process)
        {
            if (process == null) return;

            foreach (var proc in process.Children)
            {
                KillTree(proc);
            }

            // remove all children
            process.Children = null;

            // if process is blocked by resource, remove process from waiting list
            if (process.StatusType == ProcessStates.Blocked)
            {
                var resource = (ResourceControlBlock) process.StatusList;
                resource.WaitingList.Remove(resource.WaitingList.First(x => x.Key.Equals(process)));
                process.StatusList = null;
            }
            // else remove process from ready list
            else
            {
                ReadyList[(int) process.Priority].Remove(process);
            }

            // Release resources
            foreach (var resourceRequest in process.OtherResources.ToList())
            {
                var units = resourceRequest.Value;
                var resource = resourceRequest.Key;

                ReleaseResource(process, resource, units);
            }

            // remove pointer to parent
            //process.Parent = null;
        }

        /// <summary>
        /// Helper method used to release the resource. This function is the core of the "Release"
        /// function, but is used because "Kill Tree" also recursively releases resources on delete.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="resource">The resource.</param>
        /// <param name="units">The units.</param>
        /// <exception cref="Exception">
        /// </exception>
        private void ReleaseResource(ProcessControlBlock process, ResourceControlBlock resource, int units)
        {
            // Validate that the process has units from the resource
            if (!process.OtherResources.Where(x => x.Key == resource).Select(y => y).Any())
            {
                throw new Exception(String.Format("not holding resource: {0}", resource.Rid));
            }

            // Is the release attempting to release too many units
            if (resource.CurUnits + units > resource.MaxUnits)
            {
                throw new Exception(String.Format("release too many units: {0}/{1}:{2}", units, resource.Rid, resource.MaxUnits - resource.CurUnits));
            }


            // Reduce the allocated units from resource units pair
            var resourceUnits = process.OtherResources.First(x => x.Key == resource);
            var newPair = new KeyValuePair<ResourceControlBlock, int>(resourceUnits.Key,
                resourceUnits.Value - units);

            // process is no longer using resource
            if (newPair.Value == 0)
            {
                process.OtherResources.Remove(process.OtherResources.First(x => x.Key.Equals(resource)));
            }
            else
            {
                process.OtherResources.Remove(
                    RunningProcess.OtherResources.First(x => x.Key == resourceUnits.Key));
                RunningProcess.OtherResources.Add(newPair);
            }


            resource.CurUnits += units;


            // loop through all waiting processes to get access to resource
            while (resource.WaitingList.Count != 0 && resource.WaitingList[0].Value <= resource.CurUnits)
            {
                // Get process from waiting list
                process = resource.WaitingList[0].Key;

                // Get requested amount of resource
                var u = resource.WaitingList.First(x => x.Key.Equals(process)).Value;

                // Remove units from resource
                resource.CurUnits -= u;

                // Remove process from waiting list
                resource.WaitingList.RemoveAt(0);

                // Add resource to process list
                process.StatusType = ProcessStates.Ready;
                process.StatusList = ReadyList;
                process.OtherResources.Add(new KeyValuePair<ResourceControlBlock, int>(resource, u));

                // Insert process into ready list
                ReadyList[(int)process.Priority].Add(process);
            }
        }


        /// <summary>
        /// Helper function that adjusts the ready list based upon recent changes
        /// </summary>
        private void Scheduler(ProcessControlBlock process)
        {
            ProcessControlBlock p = null;

            for (var i = 2; i >= 0; i--)
            {
                if (ReadyList[i].Count > 0)
                {
                    p = ReadyList[i][0];
                    break;
                }
            }

            if (p == null)
            {
                throw new Exception("No processes running: EXTREME FAILURE!");
                
            }

            if (process == null ||
                process.Priority < p.Priority ||
                process.StatusType != ProcessStates.Running)
            {
                p.StatusType = ProcessStates.Running;
                RunningProcess = p;
            }
        }
    }
}
