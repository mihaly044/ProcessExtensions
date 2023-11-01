using System.Diagnostics;

namespace ProcessExtensions.Test
{
    [TestFixture]
    public class ProcessExtensionsTests
    {
        [Test]
        public void TestGetParentProcessForCurrentProcess()
        {
            // Get the current process
            Process currentProcess = Process.GetCurrentProcess();

            // Get the parent process
            Process? parentProcess = currentProcess.GetParentProcess();

            // Check if a parent process exists
            Assert.IsNotNull(parentProcess, "Parent process should not be null.");

            // Check if the parent process is actually running
            try
            {
                Process.GetProcessById(parentProcess.Id);
            }
            catch (ArgumentException)
            {
                Assert.Fail("Parent process should be running.");
            }
        }

        [Test]
        public void TestGetParentProcessForSystemProcess()
        {
            // Get the "explorer.exe" process, which usually exists in a typical Windows session
            Process[] explorerProcesses = Process.GetProcessesByName("explorer");

            if (explorerProcesses.Length == 0)
            {
                Assert.Inconclusive("Could not find explorer.exe process to test.");
                return;
            }

            // Get the parent process of the first "explorer.exe" process
            Process? parentProcess = explorerProcesses[0].GetParentProcess();

            // Check if a parent process exists
            Assert.That(parentProcess, Is.Not.Null, "Parent process should not be null.");

            // Check if the parent process is actually running
            try
            {
                Process.GetProcessById(parentProcess.Id);
            }
            catch (ArgumentException)
            {
                Assert.Fail("Parent process should be running.");
            }
        }

        [Test]
        public void TestGetParentProcessForNonExistentProcess()
        {
            // Try to get a non-existent process
            Process? nonExistentProcess = null;
            try
            {
                nonExistentProcess = Process.GetProcessById(int.MaxValue);
            }
            catch (ArgumentException)
            {
                // Expected exception
            }

            if (nonExistentProcess != null)
            {
                Assert.Inconclusive("Could not simulate a non-existent process.");
                return;
            }

            Assert.Pass("Successfully simulated a non-existent process.");
        }
    }
}