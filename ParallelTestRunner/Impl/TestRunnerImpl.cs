﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ParallelTestRunner.Common;
using ParallelTestRunner.Common.Trx;

namespace ParallelTestRunner.Impl
{
    public class TestRunnerImpl : ITestRunner
    {
        public ITestRunnerArgs Args { get; set; }
        
        public IParser Parser { get; set; }
        
        public ICollector Collector { get; set; }
        
        public ICleaner Cleaner { get; set; }
        
        public IRunDataBuilder RunDataBuilder { get; set; }
        
        public IRunDataListBuilder RunDataListBuilder { get; set; }
        
        public IExecutorLauncher ExecutorLauncher { get; set; }
        
        public ITrxWriter TrxWriter { get; set; }
        
        public IBreaker Breaker { get; set; }
        
        public IWindowsFileHelper WindowsFileHelper { get; set; }

        public void Parse()
        {
            if (Breaker.IsBreakReceived())
            {
                return;
            }

            foreach (string assemblyPath in Args.AssemblyList)
            {
                Assembly assembly = WindowsFileHelper.GetAssembly(assemblyPath);
                TestAssembly testAssembly = Parser.Parse(assembly);
                IList<RunData> items = RunDataBuilder.Create(testAssembly, Args);
                RunDataListBuilder.Add(items);
            }
        }

        public void Execute()
        {
            IRunDataEnumerator enumerator = RunDataListBuilder.GetEnumerator();
            while (enumerator.HasItems())
            {
                if (Breaker.IsBreakReceived())
                {
                    return;
                }

                ExecutorLauncher.WaitForAnyThreadToComplete();
                while (ExecutorLauncher.HasFreeThreads())
                {
                    if (Breaker.IsBreakReceived())
                    {
                        return;
                    }

                    if (!enumerator.HasItems())
                    {
                        break;
                    }

                    RunData data = enumerator.PeekNext();
                    if (null == data)
                    {
                        // if peeked all items till the end, but none of them was valid to launch a thread (due to concurrent groups)
                        break;
                    }

                    if (ExecutorLauncher.LaunchExecutor(data))
                    {
                        // if launching succeded (group was valid)
                        enumerator.MoveNext();
                    }
                }
            }

            ExecutorLauncher.WaitForAll();
        }

        public void WriteTrx()
        {
            if (Breaker.IsBreakReceived())
            {
                return;
            }

            IList<RunData> items = RunDataListBuilder.GetFull();
            IList<ResultFile> results = Collector.Collect(items);

            using (Stream stream = TrxWriter.OpenResultFile(Args))
            {
                // Console.WriteLine("Results File: " + Args.Root + "\\" + Args.Output);
                TrxWriter.WriteFile(results, stream);
            }
        }

        public void Clean()
        {
            if (Breaker.IsBreakReceived())
            {
                return;
            }

            IList<RunData> items = RunDataListBuilder.GetFull();
            Cleaner.Clean(items);
        }
    }
}