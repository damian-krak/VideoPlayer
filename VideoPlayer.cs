using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using CommandLine;

namespace VideoPlayer40
{
    internal class VideoPlayer
    {
        [MTAThread]
        public static void Main(string[] args)
        {
            Application.ThreadException += OnThreadException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            var options = new Options();
            Parser.Default.ParseArguments(args, options);

            Application.Run(new VideoPlayerMainWindow(options.Url)
            {
                Location = new Point(options.X, options.Y),
                ClientSize = new Size(options.Width, options.Height),
                FormBorderStyle = options.Borderless ? FormBorderStyle.None : FormBorderStyle.FixedSingle
            });
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogErrorAndExit(e.ExceptionObject as Exception);
        }

        private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            LogErrorAndExit(e.Exception);
        }

        private static void LogErrorAndExit(Exception exception)
        {
            Console.Error.WriteLine("Error: {0}", exception);
            Environment.Exit(-1);
        }


        class Options
        {
            [Option]
            public string Url { get; set; }
         
            [Option(DefaultValue = 0)]
            public int X { get; set; }
            
            [Option(DefaultValue = 0)]
            public int Y { get; set; }

            [Option(DefaultValue = 320)]
            public int Width { get; set; }
            
            [Option(DefaultValue = 240)]
            public int Height { get; set; }

            [Option(DefaultValue = false)]
            public bool Borderless { get; set; }
        }
    }
}
