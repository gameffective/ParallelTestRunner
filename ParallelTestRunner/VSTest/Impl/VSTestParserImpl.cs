using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ParallelTestRunner.Common;

namespace ParallelTestRunner.VSTest.Impl
{
    public class VSTestParserImpl : IParser
    {
        public ITestRunnerArgs Args { get; set; }

        public TestAssembly Parse(Assembly assembly, FilterMode filterMode, string filterCategory)
        {
            IList<Type> types = new List<Type>();
            foreach (Type type in assembly.ExportedTypes)
            {
                try
                {
                    // if this class is marked is a class containing tests
                    if (type.CustomAttributes.Any(x => x.AttributeType.Name == "TestClassAttribute"))
                    {
                        // if we are in filter mode by attribute
                        if (filterMode == FilterMode.Attribute)
                        {
                            // only add classes that are marked to pass the filter fules
                            if (type.CustomAttributes.Any(x => x.AttributeType.Name == "TestClassForCIParellel"))
                            {
                                types.Add(type);
                            }
                        }
                        // if we are in filter mode by class
                        // if we are not in filter mode, add any class marked as containing tests
                        else
                        {
                            types.Add(type);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occur trying to read and analyze " + type.ToString() + " details: " + e.ToString());
                }
            }

            TestAssembly item = new TestAssembly();
            item.Name = assembly.Location;
            item.Fixtures = new List<TestFixture>();
            foreach (Type type in types)
            {
                if (Args.PLevel == PLevel.TestClass)
                {
                    TestFixture fixture = new TestFixture();
                    fixture.Name = type.FullName;
                    SetGroupAndExclusiveParams(type, fixture);

                    // added to allow executing only marked methods within specified testClass
                    fixture.TestsNames = new List<string>();

                    if (filterMode == FilterMode.Attribute)
                    {
                        foreach (MemberInfo memberInfo in type.GetMethods().Where(x => x.CustomAttributes.Any(y => y.AttributeType.Name == "TestMethodForCIParellel")))
                        {
                            fixture.TestsNames.Add(type.FullName + "." + memberInfo.Name);
                        }
                    }
                    else if (filterMode == FilterMode.Category)
                    {
                        foreach (MemberInfo memberInfo in type.GetMethods().Where(
                            x => x.CustomAttributes.Any(
                                y => y.AttributeType.Name == "TestCategoryAttribute" && 
                                y.ConstructorArguments.Any(z => z.Value.Equals(filterCategory)))))
                        {
                            fixture.TestsNames.Add(type.FullName + "." + memberInfo.Name);
                        }
                    }

                    if (fixture.TestsNames.Count > 0)
                    {
                        item.Fixtures.Add(fixture);
                    }
                }
                else if (Args.PLevel == PLevel.TestMethod)
                {
                    TestFixture testClassFixture = new TestFixture();
                    SetGroupAndExclusiveParams(type, testClassFixture);

                    string attributeName = null;
                    if (filterMode == FilterMode.Attribute)
                    {
                        attributeName = "TestMethodForCIParellel";
                    }
                    else
                    {
                        attributeName = "TestMethodAttribute";
                    }
                    foreach (MemberInfo memberInfo in type.GetMethods().Where(x => x.CustomAttributes.Any(y => y.AttributeType.Name == attributeName)))
                    {

                        if ((filterMode != FilterMode.Category) ||
                                ((filterMode == FilterMode.Category) && (memberInfo.CustomAttributes.Any(y => y.AttributeType.Name == "TestCategoryAttribute" && y.ConstructorArguments.Any(z => z.Value.Equals(filterCategory))))))
                        {
                            TestFixture fixture = new TestFixture();
                            fixture.Name = type.FullName + "." + memberInfo.Name;
                            SetGroupAndExclusiveParams(memberInfo, fixture);

                            if (string.IsNullOrEmpty(testClassFixture.Group) == false)
                            {
                                fixture.Group = testClassFixture.Group;
                            }

                            if (testClassFixture.Exclusive != null)
                            {
                                fixture.Exclusive = testClassFixture.Exclusive;
                            }

                            item.Fixtures.Add(fixture);
                        }
                    }
                }
            }

            return item;
        }

        private static void SetGroupAndExclusiveParams(MemberInfo memberInfo, TestFixture fixture)
        {
            CustomAttributeData groupAttr = memberInfo.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == "TestGroupAttribute");
            if (null != groupAttr)
            {
                if (groupAttr.ConstructorArguments.Count > 0)
                {
                    fixture.Group = (string)groupAttr.ConstructorArguments[0].Value;
                }

                if (groupAttr.ConstructorArguments.Count > 1)
                {
                    fixture.Exclusive = (bool)groupAttr.ConstructorArguments[1].Value;
                }

                CustomAttributeNamedArgument name = groupAttr.NamedArguments.FirstOrDefault(x => x.MemberName == "Name");
                if (name.MemberInfo != null)
                {
                    fixture.Group = (string)name.TypedValue.Value;
                }

                CustomAttributeNamedArgument exclusive = groupAttr.NamedArguments.FirstOrDefault(x => x.MemberName == "Exclusive");
                if (exclusive.MemberInfo != null)
                {
                    fixture.Exclusive = (bool)exclusive.TypedValue.Value;
                }
            }
        }
    }
}