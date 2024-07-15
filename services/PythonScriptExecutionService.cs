using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Alphatag_Game.Services
{
    public class PythonScriptExecutionService
    {
        public async Task RunPythonScriptAsync()
        {
            try
            {
                // Get the path to the Python interpreter
                string pythonInterpreterPath = "/usr/bin/python3"; // Update this with the correct path to your Python interpreter
                string pythonScriptPath = Path.Combine(Environment.CurrentDirectory, "my_python_script.py");

                // Check if the Python script file exists
                if (!File.Exists(pythonScriptPath))
                {
                    throw new FileNotFoundException($"The Python script file was not found: {pythonScriptPath}");
                }

                // Start the Python process
                Process pythonProcess = new Process();
                pythonProcess.StartInfo.FileName = pythonInterpreterPath;
                pythonProcess.StartInfo.Arguments = pythonScriptPath;
                pythonProcess.StartInfo.UseShellExecute = false;
                pythonProcess.StartInfo.RedirectStandardOutput = true;
                pythonProcess.StartInfo.RedirectStandardError = true;
                pythonProcess.Start();

                // Wait for the Python script to finish
                await pythonProcess.WaitForExitAsync();

                // Get the output from the Python script
                string output = await pythonProcess.StandardOutput.ReadToEndAsync();
                string error = await pythonProcess.StandardError.ReadToEndAsync();

                // Log the output and error
                Console.WriteLine("Python script output:");
                Console.WriteLine(output);
                Console.WriteLine("Python script error:");
                Console.WriteLine(error);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error running Python script: {ex.Message}");
            }
        }
    }
}