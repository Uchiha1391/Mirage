using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using NUnit.Framework;
using UnityEditor.Compilation;

namespace Mirage.Tests.Weaver
{
    public class AssertionMethodAttribute : Attribute { }

    public abstract class TestsBuildFromTestName : Tests
    {
        [SetUp]
        public virtual void TestSetup()
        {
            string className = TestContext.CurrentContext.Test.ClassName.Split('.').Last();

            BuildAndWeaveTestAssembly(className, TestContext.CurrentContext.Test.Name);
        }

        [AssertionMethod]
        protected void IsSuccess()
        {
            Assert.That(compileMessages, Is.Empty, $"Failed because there are Diagnostics messages: \n  {string.Join("\n  ", compileMessages.Select(x => x.message))}\n");
        }

        /// <summary>
        /// Like <see cref="IsSuccess"/> but doesn't fail if there are warnings
        /// </summary>
        [AssertionMethod]
        protected void NoErrors()
        {
            WeaverMessages[] errors = compileMessages.Where(x => x.type == CompilerMessageType.Error).ToArray();
            Assert.That(errors, Is.Empty, $"Failed because there are Error messages: \n  {string.Join("\n  ", errors.Select(d => d.message))}\n");
        }

        [AssertionMethod]
        protected void HasErrorCount(int count)
        {
            string[] errorMessages = compileMessages
                .Where(d => d.type == CompilerMessageType.Error)
                .Select(d => d.message).ToArray();

            Assert.That(errorMessages.Length, Is.EqualTo(count), $"Error messages: \n  {string.Join("\n  ", errorMessages)}\n");
        }

        [AssertionMethod]
        protected void HasError(string messsage, string atType)
        {
            string fullMessage = $"{messsage} (at {atType})";
            string[] errorMessages = compileMessages
                .Where(d => d.type == CompilerMessageType.Error)
                .Select(d => d.message).ToArray();

            Assert.That(errorMessages, Contains.Item(fullMessage),
                $"Could not find error message in list\n" +
                $"  Message: \n    {fullMessage}\n" +
                $"  Errors: \n    {string.Join("\n    ", errorMessages)}\n"
                );
        }

        [AssertionMethod]
        protected void HasWarning(string messsage, string atType)
        {
            string fullMessage = $"{messsage} (at {atType})";
            string[] warningMessages = compileMessages
                .Where(d => d.type == CompilerMessageType.Warning)
                .Select(d => d.message).ToArray();

            Assert.That(warningMessages, Contains.Item(fullMessage),
                $"Could not find warning message in list\n" +
                $"  Message: \n    {fullMessage}\n" +
                $"  Warnings: \n    {string.Join("\n    ", warningMessages)}\n"
                );
        }
    }


    [TestFixture]
    public abstract class Tests
    {

        protected List<WeaverMessages> compileMessages = new List<WeaverMessages>();

        protected AssemblyDefinition assembly;

        protected Assembler assembler;

        protected void BuildAndWeaveTestAssembly(string className, string testName)
        {
            Console.WriteLine($"[WeaverTests] Test {className}.{testName}");

            compileMessages.Clear();
            assembler = new Assembler();

            string testSourceDirectory = className + "~";
            assembler.OutputFile = Path.Combine(testSourceDirectory, testName + ".dll");
            assembler.AddSourceFile(Path.Combine(testSourceDirectory, testName + ".cs"));
            assembly = assembler.Build(compileMessages);

            Assert.That(assembler.CompilerErrors, Is.False, "Failed to compile C# code");
            foreach (WeaverMessages error in compileMessages)
            {
                // ensure all errors have a location
                Assert.That(error.message, Does.Match(@"\(at .*\)$"));
            }
        }

        [TearDown]
        public void TestCleanup()
        {
            assembler.DeleteOutput();
        }
    }
}
