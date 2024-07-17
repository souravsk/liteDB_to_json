using System;
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
                string userName = Environment.UserName;
                string pythonInterpreterPath = Path.Combine(@"C:\Users", userName, "AppData", "Local", "Microsoft", "WindowsApps", "PythonSoftwareFoundation.Python.3.11_qbz5n2kfra8p0", "python.exe");
                string pythonScriptPath = Path.Combine(Environment.CurrentDirectory, "my_python_script.py");

                if (!File.Exists(pythonScriptPath))
                {
                    throw new FileNotFoundException($"The Python script file was not found: {pythonScriptPath}");
                }

                Process pythonProcess = new Process();
                pythonProcess.StartInfo.FileName = pythonInterpreterPath;
                pythonProcess.StartInfo.Arguments = pythonScriptPath;
                pythonProcess.StartInfo.UseShellExecute = false;
                pythonProcess.StartInfo.RedirectStandardOutput = true;
                pythonProcess.StartInfo.RedirectStandardError = true;
                pythonProcess.Start();

                await pythonProcess.WaitForExitAsync();

                string output = await pythonProcess.StandardOutput.ReadToEndAsync();
                string error = await pythonProcess.StandardError.ReadToEndAsync();

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