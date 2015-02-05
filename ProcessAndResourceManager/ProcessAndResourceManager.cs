using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessAndResourceManager
{
    class ProcessAndResourceManager
    {
        private static ProcessAndResourceManager instance;
        private List<ProcessControlBlock>[] ReadyList = new List<ProcessControlBlock>[3];

        private ProcessAndResourceManager() {}
        public static ProcessAndResourceManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ProcessAndResourceManager();
                }
                return instance;
            }
        }


        /// <summary>
        /// Initializes the Process and Resource manager state back to the original
        /// state.
        /// </summary>
        public void Initialize() { throw new NotImplementedException(); }

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
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public char Create(char name, int priority) { throw new NotImplementedException(); }

        /// <summary>
        /// Destroys the process <name> and all of its descendants.
        /// </summary>
        /// <param name="name">The process name.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public char Destroy(char name) { throw new NotImplementedException(); }


        /// <summary>
        /// Requests the specified resource for # of <units> units. <resource> can only be 1,2,3,4
        /// <units> can only be at max equal to the value of <resource>
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="units">The units.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public char Request(int resource, int units) { throw new NotImplementedException(); }


        /// <summary>
        /// Releases the specified resource for # of <units> units from <resource>. <resource> can only be 1,2,3,4
        /// <units> can only be at max equal to the value of <resource>
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="units">The units.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public char Release(int resource, int units) { throw new NotImplementedException(); }


        /// <summary>
        /// Timesouts the manager and shifts the current running process to the back of it's queue,
        /// and sets the current running process to the next in line.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public char Timeout() { throw new NotImplementedException(); }



    }
}
