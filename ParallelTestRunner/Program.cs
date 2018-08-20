﻿using Autofac;
using System;
using System.Diagnostics.CodeAnalysis;
using ParallelTestRunner.Autofac;
using ParallelTestRunner.Impl;
using System.Reflection;
using ParallelTestRunner.Common;

namespace ParallelTestRunner
{
    public class Program
    {
        private static ITestRunnerArgsFactory argsFactory = new TestRunnerArgsFactoryImpl();

        public static int Main(string[] args)
        {
            Console.WriteLine("Parallel Test Execution Command Line Tool Version " + Assembly.GetCallingAssembly().GetName().Version.ToString());
            Console.WriteLine();

            ITestRunnerArgs testArgs = argsFactory.ParseArgs(args);
            if (!testArgs.IsValid())
            {
                PrintHelp();
                return 1;
            }

            Console.WriteLine("Starting test execution, please wait...");

            IContainer container = AutofacContainer.RegisterTypes(testArgs);

            container.Resolve<IStopwatch>().Start();
            int resultCode = 2;
            using (container.BeginLifetimeScope())
            {
                ITestRunner testRunner = container.Resolve<ITestRunner>();
                testRunner.Parse(testArgs.filterMode, testArgs.filterCategory);
                testRunner.Execute();
                try 
                {
                    testRunner.WriteTrx();
                }
                catch (Exception)
                {

                    //throw;
                }
                
                testRunner.Clean();
                resultCode = testRunner.ResultCode;
            }

            return resultCode;
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Usage: ParallelTestRunner.exe [assemblyPaths] [Options]");
            Console.WriteLine();
            Console.WriteLine("Description: Runs tests from specified assembly files.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine();
            Console.WriteLine("[assemblyPaths]");
            Console.WriteLine("\tRun tests from the specified files. Separate multiple test file names");
            Console.WriteLine("\tby spaces.");
            Console.WriteLine("\tExamples: mytestproject.dll");
            Console.WriteLine("\t\t  mytestproject.dll myothertestproject.dll");
            Console.WriteLine();
            Console.WriteLine("provider:");
            Console.WriteLine("\tSpecifies provider name");
            Console.WriteLine("\tExamples: provider:VSTEST_2012");
            Console.WriteLine("\t\t  provider:VSTEST_2013");
            Console.WriteLine();
            Console.WriteLine("threadcount:");
            Console.WriteLine("\tinteger - specifies number of concurrent threads, defaut is 4");
            Console.WriteLine("\tExamples: threadcount:4");
            Console.WriteLine();
            Console.WriteLine("root:");
            Console.WriteLine("\tspecifies path to the folder where temp files are created and");
            Console.WriteLine("\twhere result file is placed");
            Console.WriteLine("\tExamples: root:D:\\rootfolder");
            Console.WriteLine();
            Console.WriteLine("out:");
            Console.WriteLine("\tspecifies result file name, default is Result.trx");
            Console.WriteLine("\tExamples: out:Result.trx");
            Console.WriteLine();
            Console.WriteLine("plevel:");
            Console.WriteLine("\tspecifies what should be run parallel");
            Console.WriteLine("\tExamples: plevel:testclass");
            Console.WriteLine("\t\t  plevel:testmethod");

            Console.WriteLine();
            Console.WriteLine("filtermode:");
            Console.WriteLine("\tHow tests should be filtered: None, Attribute, Category");
            Console.WriteLine("\tExamples: filtermode:Attribute");
            Console.WriteLine("\t\t  filtermode:Category");

            Console.WriteLine();
            Console.WriteLine("filtercategory:");
            Console.WriteLine("\tIf filter is set to category, which category to filter by");
            Console.WriteLine("\tExample: filtercategory:Sanity");


        }
    }
}