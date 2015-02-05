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

            while ((line = stream.ReadLine()) != null)
            {
                var tokens = line.Split(' ');

                try
                {
                    switch (tokens[0].ToLower())
                    {
                        case "init":
                            manager.Initialize();
                            break;
                        case "quit":
                            manager.Quit();
                            break;
                        case "cr":
                            manager.Create(Char.Parse(tokens[1]), Int32.Parse(tokens[2]));
                            break;
                        case "de":
                            manager.Destroy(Char.Parse(tokens[1]));
                            break;
                        case "req":
                            manager.Request(tokens[1], Int32.Parse(tokens[2]));
                            break;
                        case "rel":
                            manager.Release(tokens[1], Int32.Parse(tokens[2]));
                            break;
                        case "to":
                            manager.Timeout();
                            break;
                        default:
                            throw new Exception("invalid operation");
                    }

                    sb.AppendLine(manager.GetRunningProcess());
                }
                catch (Exception e)
                {
                    sb.AppendLine("error: " + e.Message);
                }
            }

            stream.Close();
            File.WriteAllText("C:\\Users\\Matthew\\Desktop\\87401675.txt", sb.ToString());
        }
    }
}
