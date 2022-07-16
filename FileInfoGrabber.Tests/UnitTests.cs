using System;
using System.IO;

using Xunit;

namespace FileInfoGrabber.Tests
{
    public class UnitTests
    {
        [Fact]
        public void TestNullArgs()
        {
            Grabber grabber = new Grabber();
            Assert.True(grabber.Grab(null));

            Assert.True(File.Exists(Common.DefaultFileName));
        }

        [Fact]
        public void TestEmptyArgs()
        {
            Grabber grabber = new Grabber();

            string argLine = "";
            Assert.True(grabber.Grab(argLine.Split(' ')));

            Assert.True(File.Exists(Common.DefaultFileName));
        }

        [Fact]
        public void TestCustomExtensions()
        {
            Grabber grabber = new Grabber();

            string argLine = "-e .dll .exe";
            Assert.True(grabber.Grab(argLine.Split(' ')));

            Assert.True(File.Exists(Common.DefaultFileName));

            var lines = File.ReadAllLines(Common.DefaultFileName);
            Assert.Equal("Extensions:	.dll | .exe", lines[1]);
        }

        [Fact]
        public void TestCustomInputDirectory()
        {
            Grabber grabber = new Grabber();

            string dir = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory)[0];

            string argLine = $"-i {dir}";
            Assert.True(grabber.Grab(argLine.Split(' ')));

            Assert.True(File.Exists(Common.DefaultFileName));

            string text = File.ReadAllText(Common.DefaultFileName);
            Assert.Contains(dir, text);
        }

        [Fact]
        public void TestCustomOutputDirectory()
        {
            Grabber grabber = new Grabber();

            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Custom");
            Directory.CreateDirectory(dir);

            string argLine = $"-o {dir}";
            Assert.True(grabber.Grab(argLine.Split(' ')));

            Assert.True(File.Exists(Path.Combine(dir, Common.DefaultFileName)));
        }

        [Fact]
        public void TestCustomOutputFile()
        {
            Grabber grabber = new Grabber();

            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Custom");
            Directory.CreateDirectory(dir);
            string filePath = Path.Combine(dir, "custom.txt");

            string argLine = $"-o {filePath}";
            Assert.True(grabber.Grab(argLine.Split(' ')));

            Assert.True(File.Exists(filePath));
        }
    }
}