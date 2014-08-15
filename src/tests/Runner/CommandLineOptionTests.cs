// *****************************************************
// Copyright 2007, Charlie Poole
//
// Licensed under the Open Software License version 3.0
// *****************************************************

using System;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Env = NUnit.Env;

namespace NUnitLite.Runner.Tests
{
    [TestFixture]
    class CommandLineOptionTests
    {
        [Test]
        public void TestWaitOption()
        {
            var options = new CommandLineOptions("-wait");
            Assert.That(options.Error, Is.False);
            Assert.That(options.Wait, Is.True);
        }

        [Test]
        public void TestNoheaderOption()
        {
            var options = new CommandLineOptions("-noheader");
            Assert.That(options.Error, Is.False);
            Assert.That(options.NoHeader, Is.True);
        }

        [Test]
        public void TestHelpOption()
        {
            var options = new CommandLineOptions("-help");
            Assert.That(options.Error, Is.False);
            Assert.That(options.ShowHelp, Is.True);
        }

        [Test]
        public void TestFullOption()
        {
            var options = new CommandLineOptions("-full");
            Assert.That(options.Error, Is.False);
            Assert.That(options.Full, Is.True);
        }

        [Test]
        public void TestOptionStartingWithTwoHyphens()
        {
            var options = new CommandLineOptions("--full");
            Assert.That(options.Error, Is.False);
            Assert.That(options.Full, Is.True);
        }

#if !SILVERLIGHT && !NETCF
        [Test]
        public void TestExploreOptionWithNoFileName()
        {
            var options = new CommandLineOptions("-explore");
            Assert.That(options.Error, Is.False);
            Assert.That(options.Explore, Is.True);
            Assert.That(options.ExploreFile, Is.EqualTo(Path.GetFullPath("tests.xml")));
        }

        [Test]
        public void TestExploreOptionWithGoodFileName()
        {
            var options = new CommandLineOptions("-explore=MyFile.xml");
            Assert.That(options.Error, Is.False);
            Assert.That(options.Explore, Is.True);
            Assert.That(options.ExploreFile, Is.EqualTo(Path.GetFullPath("MyFile.xml")));
        }

        [Test]
        [Platform(Exclude = "Mono", Reason = "No Exception thrown on bad path under Mono. Test should be revised.")]
        public void TestExploreOptionWithBadFileName()
        {
            var options = new CommandLineOptions("-explore=MyFile*.xml");
            Assert.That(options.Error, Is.True);
            Assert.That(options.ErrorMessage, Is.EqualTo("Invalid option: -explore=MyFile*.xml" + Env.NewLine));
        }

        [Test]
        public void TestResultOptionWithNoFileName()
        {
            var options = new CommandLineOptions("-result");
            Assert.That(options.Error, Is.False);
            Assert.That(options.ResultFile, Is.EqualTo(Path.GetFullPath("TestResult.xml")));
        }

        [Test]
        public void TestResultOptionWithGoodFileName()
        {
            var options = new CommandLineOptions("-result=MyResult.xml");
            Assert.That(options.Error, Is.False);
            Assert.That(options.ResultFile, Is.EqualTo(Path.GetFullPath("MyResult.xml")));
        }

        [Test]
        [Platform(Exclude = "Mono", Reason = "No Exception thrown on bad path under Mono. Test should be revised.")]
        public void TestResultOptionWithBadFileName()
        {
            var options = new CommandLineOptions("-result=MyResult*.xml");
            Assert.That(options.Error, Is.True);
            Assert.That(options.ErrorMessage, Is.EqualTo("Invalid option: -result=MyResult*.xml" + Env.NewLine));
        }
#endif

        [Test]
        public void TestNUnit2FormatOption()
        {
            var options = new CommandLineOptions("-format=nunit2");
            Assert.That(options.Error, Is.False);
            Assert.That(options.ResultFormat, Is.EqualTo("nunit2"));
        }

        [Test]
        public void TestNUnit3FormatOption()
        {
            var options = new CommandLineOptions("-format=nunit3");
            Assert.That(options.Error, Is.False);
            Assert.That(options.ResultFormat, Is.EqualTo("nunit3"));
        }

        [Test]
        public void TestBadFormatOption()
        {
            var options = new CommandLineOptions("-format=xyz");
            Assert.That(options.Error, Is.True);
            Assert.That(options.ErrorMessage, Is.EqualTo("Invalid option: -format=xyz" + Env.NewLine));
        }

        [Test]
        public void TestMissingFormatOption()
        {
            var options = new CommandLineOptions("-format");
            Assert.That(options.Error, Is.True);
            Assert.That(options.ErrorMessage, Is.EqualTo("Invalid option: -format" + Env.NewLine));
        }

#if !SILVERLIGHT && !NETCF
        [Test]
        public void TestOutOptionWithGoodFileName()
        {
            var options = new CommandLineOptions("-out=myfile.txt");
            Assert.False(options.Error);
            Assert.That(options.OutFile, Is.EqualTo(Path.GetFullPath("myfile.txt")));
        }

        [Test]
        public void TestOutOptionWithNoFileName()
        {
            var options = new CommandLineOptions("-out=");
            Assert.True(options.Error);
            Assert.That(options.ErrorMessage, Is.EqualTo("Invalid option: -out=" + Env.NewLine));
        }

        [Test]
        [Platform(Exclude = "Mono", Reason = "No Exception thrown on bad path under Mono. Test should be revised.")]
        public void TestOutOptionWithBadFileName()
        {
            var options = new CommandLineOptions("-out=my*file.txt");
            Assert.True(options.Error);
            Assert.That(options.ErrorMessage, Is.EqualTo("Invalid option: -out=my*file.txt" + Env.NewLine));
        }
#endif

        [Test]
        public void TestLabelsOption()
        {
            var options = new CommandLineOptions("-labels");
            Assert.That(options.Error, Is.False);
            Assert.That(options.ShowLabels, Is.True);
        }

        [Test]
        public void TestSeedOption()
        {
            var options = new CommandLineOptions("-seed=123456789");
            Assert.False(options.Error);
            Assert.That(options.InitialSeed, Is.EqualTo(123456789));
        }

        [Test]
        public void InvalidOptionProducesError()
        {
            var options = new CommandLineOptions("-junk");
            Assert.That(options.Error);
            Assert.That(options.ErrorMessage, Is.EqualTo("Invalid option: -junk" + Env.NewLine));
        }

        [Test]
        public void MultipleInvalidOptionsAreListedInErrorMessage()
        {
            var options = new CommandLineOptions("-junk", "-trash", "something", "-garbage");
            Assert.That(options.Error);
            Assert.That(options.ErrorMessage, Is.EqualTo(
                "Invalid option: -junk" + Env.NewLine +
                "Invalid option: -trash" + Env.NewLine +
                "Invalid option: -garbage" + Env.NewLine));
        }

        [Test]
        public void SingleParameterIsSaved()
        {
            var options = new CommandLineOptions("myassembly.dll");
            Assert.That(options.Error, Is.False);
            Assert.That(options.Parameters.Count, Is.EqualTo(1));
            Assert.That(options.Parameters[0], Is.EqualTo("myassembly.dll"));
        }

        [Test]
        public void MultipleParametersAreSaved()
        {
            var options = new CommandLineOptions("assembly1.dll", "-wait", "assembly2.dll", "assembly3.dll");
            Assert.That(options.Error, Is.False);
            Assert.That(options.Parameters.Count, Is.EqualTo(3));
            Assert.That(options.Parameters[0], Is.EqualTo("assembly1.dll"));
            Assert.That(options.Parameters[1], Is.EqualTo("assembly2.dll"));
            Assert.That(options.Parameters[2], Is.EqualTo("assembly3.dll"));
        }

        [Test]
        public void TestOptionIsRecognized()
        {
            var options = new CommandLineOptions("-test:Some.Class.Name");
            Assert.That(options.Error, Is.False);
            Assert.That(options.Tests.Count, Is.EqualTo(1));
            Assert.That(options.Tests[0], Is.EqualTo("Some.Class.Name"));
        }

        [Test]
        public void MultipleTestOptionsAreRecognized()
        {
            var options = new CommandLineOptions("-test:Class1", "-test=Class2", "-test:Class3");
            Assert.That(options.Error, Is.False);
            Assert.That(options.Tests.Count, Is.EqualTo(3));
            Assert.That(options.Tests[0], Is.EqualTo("Class1"));
            Assert.That(options.Tests[1], Is.EqualTo("Class2"));
            Assert.That(options.Tests[2], Is.EqualTo("Class3"));
        }
#if !SILVERLIGHT
        [Test]
        public void TestIncludeOption()
        {
            var options = new CommandLineOptions("-include:1,2");
            Assert.That(options.Error, Is.False);
            Assert.That(options.Include == "1,2");
        }
        [Test]
        public void TestExcludeOption()
        {
            var options = new CommandLineOptions("-exclude:1,2");
            Assert.That(options.Error, Is.False);
            Assert.That(options.Exclude == "1,2");
        }
        [Test]
        public void TestIncludeExcludeOption()
        {
            var options = new CommandLineOptions("-include:3,4", "-exclude:1,2");
            Assert.That(options.Error, Is.False);
            Assert.That(options.Exclude == "1,2");
            Assert.That(options.Include == "3,4");
        }
#endif

        [Test]
        public void TestInternalTraceLevelOption([Values]InternalTraceLevel traceLevel)
        {
            var options = new CommandLineOptions(String.Format("-trace:{0}", traceLevel));
            Assert.That(options.Error, Is.False);
            Assert.That(options.InternalTraceLevel, Is.EqualTo(traceLevel));
        }

        [Test]
        public void TestInvalidInternalTraceLevelOption()
        {
            var options = new CommandLineOptions("-trace:bad");
            Assert.That(options.Error, Is.True);
            Assert.That(options.ErrorMessage, Is.StringContaining("-trace:bad"));
        }

        [Test]
        public void TestDefaultInternalTraceLevel()
        {
            var options = new CommandLineOptions();
            Assert.That(options.Error, Is.False);
            Assert.That(options.InternalTraceLevel, Is.EqualTo(InternalTraceLevel.Off));
        }
    }
}
