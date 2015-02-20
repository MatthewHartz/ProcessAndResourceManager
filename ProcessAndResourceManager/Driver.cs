using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessAndResourceManager
{
    class Driver
    {
        static void Main(string[] args)
        {
            var manager = ProcessAndResourceManager.Instance;
            var sb = new StringBuilder(); // Will contain file output
            var stream = new StreamReader("C:\\Users\\Matthew\\Desktop\\input.txt");           
            String line;

            sb.Append(manager.GetRunningProcess());

            while ((line = stream.ReadLine()) != null)
            {
                var tokens = line.Split(' ');

                try
                {
                    switch (tokens[0].ToLower())
                    {
                        case "init":
                            manager.Initialize();
                            sb.AppendLine().AppendLine();
                            sb.Append(manager.GetRunningProcess());
                            break;
                        case "quit":
                            manager.Quit();
                            break;
                        case "cr":
                            manager.Create(Char.Parse(tokens[1]), Int32.Parse(tokens[2]));
                            sb.Append(" " + manager.GetRunningProcess());
                            break;
                        case "de":
                            manager.Destroy(Char.Parse(tokens[1]));
                            sb.Append(" " + manager.GetRunningProcess());
                            break;
                        case "req":
                            manager.Request(tokens[1], Int32.Parse(tokens[2]));
                            sb.Append(" " + manager.GetRunningProcess());
                            break;
                        case "rel":
                            manager.Release(tokens[1], Int32.Parse(tokens[2]));
                            sb.Append(" " + manager.GetRunningProcess());
                            break;
                        case "to":
                            manager.Timeout();
                            sb.Append(" " + manager.GetRunningProcess());
                            break;
                        case "":
                            break;
                        case "#":
                            break;
                        default:
                            throw new Exception("invalid operation");
                    }
                    
                }
                catch (Exception e)
                {
                    sb.Append(String.Format(" error({0})", e.Message));
                }
            }

            stream.Close();
            File.WriteAllText("C:\\Users\\Matthew\\Desktop\\87401675.txt", sb.ToString());
        }
    }
}
