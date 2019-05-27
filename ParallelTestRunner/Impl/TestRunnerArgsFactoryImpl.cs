using System;
using System.Collections.Generic;

namespace ParallelTestRunner.Impl
{
    public class TestRunnerArgsFactoryImpl : ITestRunnerArgsFactory
    {
        public ITestRunnerArgs ParseArgs(string[] args)
        {
            TestRunnerArgsImpl data = new TestRunnerArgsImpl();
            Console.WriteLine("logMe  1");
            data.AssemblyList = new List<string>();
            Console.WriteLine("logMe  2");
            for (int i = 0; i < args.Length; i++)
            {
                Console.WriteLine("logMe  3");
                string item = args[i].Replace("\"", string.Empty);
                if (item.StartsWith("provider:", StringComparison.InvariantCultureIgnoreCase))
                {
                    data.Provider = item.Remove(0, 9);
                }
                else if (item.StartsWith("threadcount:", StringComparison.InvariantCultureIgnoreCase))
                {
                    int number;
                    int.TryParse(item.Remove(0, 12), out number);
                    data.ThreadCount = number;
                }
                else if (item.StartsWith("root:", StringComparison.InvariantCultureIgnoreCase))
                {
                    data.Root = item.Remove(0, 5);
                    if (data.Root.EndsWith("\\") || data.Root.EndsWith("/"))
                    {
                        data.Root = data.Root.Substring(0, data.Root.Length - 1);
                    }
                }
                else if (item.StartsWith("out:", StringComparison.InvariantCultureIgnoreCase))
                {
                    data.Output = item.Remove(0, 4);
                }
                else if (item.StartsWith("plevel:", StringComparison.InvariantCultureIgnoreCase))
                {
                    PLevel result;
                    if (Enum.TryParse<PLevel>(item.Remove(0, 7), true, out result))
                    {
                        data.PLevel = result;
                    }
                }
                else if (item.StartsWith("filtermode:", StringComparison.InvariantCultureIgnoreCase))
                {
                    FilterMode result;                    
                    if (Enum.TryParse(item.Remove(0, 11), out result))
                    {
                        data.filterMode = result;
                    } else
                    {
                        Console.WriteLine("An error occur trying to parse FilterMode argument.. using default value of 'Attribute'");
                    }
                }
                else if (item.StartsWith("filtercategory:", StringComparison.InvariantCultureIgnoreCase))
                {
                    System.Diagnostics.Debug.WriteLine(item.ToString());
                    data.filterCategory = item.Remove(0, 15);
                    Console.WriteLine("logMe   else if (item.StartsWith filtercategory:  the data is   : " + data.ToString());
                    Console.WriteLine("logMe   the irem is: " + item.ToString());

                }
                else
                {
                    string[] pathList = item.Split(' ');
                    for (int j = 0; j < pathList.Length; j++)
                    {
                        data.AssemblyList.Add(pathList[j]);
                    }
                }
            }
            Console.WriteLine("logMe the data is   : " + data.ToString());

            return data;
        }
    }
}