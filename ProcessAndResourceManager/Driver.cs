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

                switch (tokens[0].ToLower())
                {
                    case "init":
                        try
                        {
                            manager.Initialize();
                        }
                        catch (Exception e)
                        {
                            sb.AppendLine(e.Message);
                        }
                        break;
                    case "quit":
                        try
                        {
                            manager.Quit();
                        }
                        catch (Exception e)
                        {
                            sb.AppendLine(e.Message);
                        }
                        break;
                    case "cr":
                        try
                        {
                            manager.Create(Char.Parse(tokens[1]), Int32.Parse(tokens[2]));
                        }
                        catch (Exception e)
                        {
                            sb.AppendLine(e.Message);
                        }
                        break;
                    case "de":
                        try
                        {
                            manager.Destroy(Char.Parse(tokens[1]));
                        }
                        catch (Exception e)
                        {
                            sb.AppendLine(e.Message);
                        }
                        break;
                    case "req":
                        try
                        {
                            manager.Request(Int32.Parse(tokens[1]), Int32.Parse(tokens[2]));
                        }
                        catch (Exception e)
                        {
                            sb.AppendLine(e.Message);
                        }
                        break;
                    case "rel":
                        try
                        {
                            manager.Release(Int32.Parse(tokens[1]), Int32.Parse(tokens[2]));
                        }
                        catch (Exception e)
                        {
                            sb.AppendLine(e.Message);
                        }
                        break;
                    case "to":
                        try
                        {
                            manager.Timeout();
                        }
                        catch (Exception e)
                        {
                            sb.AppendLine(e.Message);
                        }
                        break;
                    default:
                        sb.AppendLine("error: invalid operation");
                        break;
                }
            }

            stream.Close();
            File.WriteAllText("C:\\Users\\Matthew\\Desktop\\87401675.txt", sb.ToString());
        }
    }
}
