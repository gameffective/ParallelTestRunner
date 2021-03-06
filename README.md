# ParallelTestRunner
This is an updated version of [ParallelTestRunner](https://github.com/sscobici/ParallelTestRunner).

What is ParallelTestRunner?
Parallel test runner for Visual Studio tests. It allows to control how the tests are executed in parallel by providing different options and TestGroupAttribute (see Additional Info).<br><br>
For simple Visual Studio parallel tests run you can use this [suggestion](http://stackoverflow.com/questions/3917060/how-to-run-unit-tests-mstest-in-parallel/17820520#17820520).

# Description
Allows parallel run of Visual Studio tests from the command line. Primary usage is to speed up slow tests (for ex Selenium UI tests) during Continuous Integration process. It is possible for example to write [Selenium](http://www.seleniumhq.org/) UI tests using Visual Studio testing framework and scale them by using ParallelTestRunner and [Selenium Grid](http://www.seleniumhq.org/projects/grid/). Basically this tool runs several Visual Studio VSTest.Console.exe processes and executes one [TestClass] or [TestMethod] in each of them. The tool generates result.trx file by merging all test results.

# Usage
```
ParallelTestRunner.exe [options] [assembly]...

Options:
  provider:        specifies which version of VSTest.Console.exe to use: VSTEST_2012, VSTEST_2013, ...,  VSTEST_2017
  threadcount:     specifies the number of parallel processes, default is 4
  root:            the working directory where the temporary files will be generated
  out:             resulting trx file, can be absolute path or relative to the working directory
  plevel:          specifies what should be run in parallel: TestClass, TestMethod. Default is TestClass
  filtermode:      should only run tests marked by attribute (TestClassForCIParellel / TestMethodForCIParellel) default is true
  
assembly           the list of assemblies which contain visual studio tests

Examples:
  ParallelTestRunner.exe provider:VSTEST_2013 root:TestResults ./UITests/SeleniumUI.Tests.dll
  ParallelTestRunner.exe provider:VSTEST_2013 root:TestResults threadcount:10 out:result.trx plevel:TestMethod ./UITests/SeleniumIntegration.Tests.dll
```

# Download
See [releases](https://github.com/sscobici/ParallelTestRunner/releases).
Build was created with the help of [AppVeyor](https://ci.appveyor.com/project/sscobici/paralleltestrunner) Continuous Integration tool

# Changelog
See [Changelog](https://github.com/sscobici/ParallelTestRunner/blob/master/CHANGELOG)

# Issues
Feel free to open an [issue](https://github.com/sscobici/ParallelTestRunner/issues) if the tool needs to be enhanced or you have found a bug 

# Additional Information
By default all TestClasses are executed in parallel. TestMethods inside each TestClass are executed consecutively unless you specify plevel:TestMethod option. There is a possibility to group several TestClasses or TestMethods in order to execute them consecutively.

Create the following class in your test project and apply it to test class or method:
```
    
    /// <summary>
    /// Used to mark a class to be executed as part of a parallel test execution
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class TestClassForCIParellel : Attribute
    {
        public TestClassForCIParellel()
        {
        }
    }

    /// <summary>
    /// Used to mark a method to be executed as part of a parallel test execution
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class TestMethodForCIParellel : Attribute
    {
        public TestMethodForCIParellel()
        {
        }
    }
    
    public class TestGroupAttribute : Attribute
    {
        public TestGroupAttribute()
        {
        }

        public TestGroupAttribute(string name)
        {
            Name = name;
        }

        public TestGroupAttribute(string name, bool exclusive)
            : this(name)
        {
            Exclusive = exclusive;
        }

        public string Name { get; set; }
        
        public bool Exclusive { get; set; }
    }
```

In the below example two groups are defined to be executed in parallel. ClassA and ClassB tests will be executed consecutively.

```
[TestGroup("FirstGroup")]
ClassA { ... }

[TestGroup("FirstGroup")]
ClassB { ... }

[TestGroup("SecondGroup")]
ClassC { ... }
```

Specify attribute parameter Exclusive = true if there is a need to run some tests exclusively. This will ensure that no other tests are run in parallel at that time.

```
[TestGroup("ExclusiveGroup", Exclusive = true)]
ClassExclusive { ... }
```

# Requirements
.Net Framework 4.5 or higher
