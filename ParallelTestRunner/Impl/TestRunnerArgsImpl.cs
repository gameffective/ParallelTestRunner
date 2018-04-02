﻿using ParallelTestRunner.Common;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace ParallelTestRunner.Impl
{
    public class TestRunnerArgsImpl : ITestRunnerArgs
    {
        public TestRunnerArgsImpl()
        {
            Output = "Result.trx";
            ThreadCount = 4;
            PLevel = PLevel.TestClass;
            filterMode = FilterMode.Attribute;
            filterCategory = null;
        }

        public string Provider { get; set; }
        
        public IList<string> AssemblyList { get; set; }
        
        public int ThreadCount { get; set; }
        
        public string Root { get; set; }
        
        public string Output { get; set; }

        public PLevel PLevel { get; set; }

        public FilterMode filterMode { get; set; }

        public string filterCategory { get; set; }

        public string GetExecutablePath()
        {
            return ConfigurationManager.AppSettings.Get(Provider);
        }

        public bool IsValid()
        {
            if (AssemblyList == null ||
                AssemblyList.Count == 0)
            {
                Console.WriteLine("at least one DLL must be specified: c:\\work\\testassembly.dll");
                return false;
            }

            if (string.IsNullOrEmpty(Provider))
            {
                Console.WriteLine("Provider is required (see config file): provider:VSTEST_2012");
                return false;
            }

            if (ThreadCount <= 0)
            {
                Console.WriteLine("threadcount must be integer greater than 0: threadcount:4");
                return false;
            }

            if (string.IsNullOrEmpty(Root))
            {
                Console.WriteLine("root path is required: root:d:\\work");
                return false;
            }

            if (PLevel == PLevel.None)
            {
                Console.WriteLine("Level which will be considered parallel is required: plevel:testclass");
                return false;
            }

            return true;
        }
    }
}