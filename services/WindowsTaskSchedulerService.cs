using System.Diagnostics;

namespace Alphatag_Game.Services
{
    public class WindowsTaskSchedulerService
    {
        private readonly string _executablePath;

        public WindowsTaskSchedulerService(string executablePath)
        {
            _executablePath = executablePath;
        }

        public void CreateScheduledTask()
        {
            // Create a new task
            var task = Task.Factory.StartNew(() =>
            {
                // Task logic here
                var process = new Process();
                process.StartInfo.FileName = _executablePath;
                process.Start();
            });

            task.ContinueWith(t =>
            {
                Console.WriteLine("Windows Scheduled Task created successfully.");
            });
        }
    }
}