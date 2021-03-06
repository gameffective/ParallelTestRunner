﻿using Autofac;
using System;
using System.Diagnostics.CodeAnalysis;
using ParallelTestRunner.Autofac;
using ParallelTestRunner.Impl;
using System.Reflection;
using ParallelTestRunner.Common;
using System.Threading;

namespace ParallelTestRunner
{
    public class Program
    {
        private static ITestRunnerArgsFactory argsFactory = new TestRunnerArgsFactoryImpl();

        public static int Main(string[] args)
        {
            Console.WriteLine("logMe the args are : " + args.ToString());
            Console.WriteLine("Parallel Test Execution Command Line Tool Version " + Assembly.GetCallingAssembly().GetName().Version.ToString());

            
            ITestRunnerArgs testArgs = argsFactory.ParseArgs(args);
            Console.WriteLine("logMe the testargs are : " + testArgs.ToString());
            if (!testArgs.IsValid())
            {


                PrintHelp();
                return 1;
            }

            Console.WriteLine("logMe:  Starting test execution, please wait...");

            IContainer container = AutofacContainer.RegisterTypes(testArgs);
            Console.WriteLine("logMe: IContainer container = AutofacContainer.RegisterTypes(testArgs); the contatier is : " + container.ToString()+ "   and the test args are : " + testArgs.ToString());
            container.Resolve<IStopwatch>().Start();
            Console.WriteLine("Starting container.Resolve<IStopwatch>().Start();");

            int resultCode = 2;
            using (container.BeginLifetimeScope())
            {
                ITestRunner testRunner = container.Resolve<ITestRunner>();
                Console.WriteLine("finish container.Resolve<IStopwatch>().Start();");

                Console.WriteLine("testArgs.filterCategory" + testArgs.filterCategory);
                Console.WriteLine("estArgs.filterMode" + testArgs.filterMode);

                testRunner.Parse(testArgs.filterMode,testArgs.filterCategory, testArgs.filterCategories);
                Console.WriteLine("finish  testRunner.Parse(testArgs.filterMode, testArgs.filterCategory);");
                testRunner.Execute();
                Console.WriteLine("finish  testRunner.Execute();");
                testRunner.WriteTrx();
                Console.WriteLine("finish testRunner.WriteTrx(); ");
                testRunner.Clean();
                Console.WriteLine("finish testRunner.Clean();");
                resultCode = testRunner.ResultCode;
                Console.WriteLine("finish  resultCode = testRunner.ResultCode;");
            }
            Console.WriteLine("finish  resukt code is");
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