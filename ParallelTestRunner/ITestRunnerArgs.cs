﻿using System;
using System.Collections.Generic;

namespace ParallelTestRunner
{
    public interface ITestRunnerArgs
    {
        string Provider { get; }   // VSTest, Nunit, etc
        
        IList<string> AssemblyList { get; }
        
        int ThreadCount { get; }
        
        string Root { get; }
        
        string Output { get; }

        PLevel PLevel { get; }

        bool filterMode { get; }

        string GetExecutablePath();

        bool IsValid();
    }
}