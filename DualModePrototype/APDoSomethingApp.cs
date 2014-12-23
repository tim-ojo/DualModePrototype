using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;

namespace DualModePrototype
{
    public class APDoSomethingApp : APService
    {
        private bool _continue = false;
        //private StreamWriter file;

        public APDoSomethingApp()
        {
            this.ServiceName = "MyWidgetService";
            // perform any setup needed for this instance of the service
        }

        /// <summary>
        /// The Main() method is where we detect if we are a in interactive mode or not. 
        /// If we are not in interactive mode we register the service and allow Service Control Manager (SCM)
        ///     to call the OnStart method either automatically or manually.
        /// If we are in interactive mode the OnStart() method gets called right away with the args from Main().
        /// </summary>
        /// <param name="args"></param>    
        static void Main(string[] args)
        {
            // Create an instance of the class
            APDoSomethingApp app = new APDoSomethingApp();

            // if (args.Length > 0)
            if (Environment.UserInteractive)
            {
                app.OnStart(args);  
                

                // Trying out shelling out a command line app
                //int number = 0;
                //Int32.TryParse(args[0], out number);
                //app.SendNumber(number);
            }
            else
            {
                ServiceBase.Run(app);
            }
        }
        
        /// <summary>
        /// The OnStart() method is where we kick of execution of the work to be done. 
        /// If the process is long running, the body of work must be done in a separate thread or process.
        /// 
        /// Notes:
        /// For long running processes there will be a main thread listening. Once an event occurs it will 
        /// spin up a new thread or program to handle the event. This should happen regardless of whether we are 
        /// running as a command line program or as a service. When running as a service, OnStop should be used 
        /// to stop the main thread from listening for the event and to release any resources. However, for console 
        /// apps OnStop doesn't get called, the only way to stop a the console app would be to close the console 
        /// window which would kill the thread but will not call OnStop to cleanup any resources. Therefore resource 
        /// management should really be done in the main body of the application.
        /// 
        /// For short running processes OnStart will contain the main body of work. OnStop will be empty. If running as 
        /// a service, the application should call super.MarkServiceAsStopped() after processing is completed to mark 
        /// the service as stopped. This is not necessary when running interactively.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            //this.DoParentStuff();
            Console.WriteLine("Application running...");
            EventLog.WriteEntry("OnStart executed");
            
            if (args.Length > 0)
            {
                // Short running
                int number = 0;
                Int32.TryParse(args[0], out number);
                SendNumber(number);
                EventLog.WriteEntry("Task completed");
            }
            else
            {
                // Long running
                // Should spin up new thread or process
                APDoSomethingApp forkedApp = new APDoSomethingApp();
                Thread listeningThread = new Thread(new ThreadStart(forkedApp.ContinuousSend));
                listeningThread.Start();
            }
            
        }

        /*
        // Trying out shelling out a command line app
        protected override void OnStart (string[] args)
        {
            EventLog.WriteEntry("Begin OnStart executed");

            ProcessStartInfo startInfo = new ProcessStartInfo(@"C:\Stuff\DualModePrototype\DualModePrototype.exe", "01234");
            Process.Start(startInfo);

            EventLog.WriteEntry("End OnStart executed");
        }
        */

        /// <summary>
        /// An example of a long running process
        /// </summary>
        private void ContinuousSend()
        {
            _continue = true;
            Random random = new Random();

            while (_continue)
            {
                int genNumber = random.Next(0, 99);

                // Event occurs when a number divisible by 7 is generated
                if ((genNumber % 7) == 0)
                {
                    SendNumber(genNumber);
                }

                Thread.Sleep(500);
            }
        }

        private void SendNumber(int number)
        {
            string path = @"C:\logs\APServiceExample.txt";
            using (StreamWriter file = File.AppendText(path))
            {
                file.WriteLine(String.Format("{0} >> Hello. Your lucky number for the day is {1}! This is {2}",
                    DateTime.Now.ToLongTimeString(), number, this.ServiceName));
            }
        }

        /// <summary>
        /// Called by the Service Control Manager when the windows service is stopped
        /// </summary>
        protected override void OnStop()
        {
            EventLog.WriteEntry("OnStop executed");
            _continue = false;
        }

    }
}
