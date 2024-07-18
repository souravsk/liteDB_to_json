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
                string pythonInterpreterPath = @"C:\Users\Admin\AppData\Local\Programs\Python\Python312\python.exe";
                string pythonScriptPath = Path.Combine(Environment.CurrentDirectory, "my_python_script.py");

                if (!File.Exists(pythonScriptPath))
                {
                    throw new FileNotFoundException($"Python script not found: {pythonScriptPath}");
                }

                var startInfo = new ProcessStartInfo
                {
                    FileName = pythonInterpreterPath,
                    Arguments = pythonScriptPath,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };

                using (var process = Process.Start(startInfo))
                {
                    if (process == null)
                    {
                        throw new Exception("Failed to start Python process.");
                    }

                    await Task.WhenAll(
                        process.StandardOutput.ReadToEndAsync().ContinueWith(t => Console.WriteLine($"Python output: {t.Result}")),
                        process.StandardError.ReadToEndAsync().ContinueWith(t => Console.WriteLine($"Python error: {t.Result}"))
                    );

                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error running Python script: {ex.Message}");
                throw;
            }
        }
    }
}
