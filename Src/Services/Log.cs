using System;
using System.IO;

namespace Imperium_Incursions_Waitlist
{
    enum Type  { Debug, Info, Warn, Error }

    public static class Log
    {
        /// <summary>
        /// Log a low level debug event. 
        /// Logging only occurs if log level = Debug
        /// </summary>
        public static void Debug(string message)
        {
            if (true)
                LogOutput(Type.Debug, message);
        }

        /// <summary>
        /// Designates informational messages that highlight the progress of the application at coarse-grained level.
        /// </summary>
        public static void Info(string message) => LogOutput(Type.Info, message);

        /// <summary>
        /// Designates potentially harmful situations,
        /// Logging always occurs.
        /// </summary>
        public static void Warn(string message) => LogOutput(Type.Warn, message);

        /// <summary>
        /// Designates an error,
        /// Logging always occurs.
        /// </summary>
        public static void Error(string message) => LogOutput(Type.Error, message);



        /// <summary>
        /// Writes a log message to the console. 
        /// </summary>
        /// <param name="t">t.Info || t.Debug || t.Warn || t.Error</param>
        /// <param name="m">Message to log</param>
        private static void LogOutput(Type t, string m)
        {
            switch (t)
            {
                case Type.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;

                case Type.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;

                case Type.Warn:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;

                case Type.Error:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
            }

            string output = string.Format("[{0}] {1}: {2} ",
                                DateTime.UtcNow.ToString("dd MMM H:mm:ss"),
                                t.ToString(),
                                m
                            );

            // Print to console
            Console.WriteLine(output);

            // Save to file
            LogToFile(t, m);
        }

        /// <summary>
        /// Appends the log line to the current text file
        /// </summary>
        /// <param name="t">t.Info || t.Debug || t.Warn || t.Error</param>
        /// <param name="m">Message to append to file.</param>
        private static void LogToFile(Type t, string m)
        {
            string todaysFile = String.Format("{0}_log.txt", DateTime.UtcNow.ToString("dd MMM"));

            // Create the logs directory
            if (!Directory.Exists("./Logs"))
                Directory.CreateDirectory("./Logs");

            // Appends log to the log file.
            using (StreamWriter sw = File.AppendText("./Logs/" + todaysFile))
            {
                sw.WriteLine(
                    string.Format("[{0}] {1}: {2} ",
                        DateTime.UtcNow.ToString("dd MMM H:mm:ss"),
                        t.ToString(),
                        m
                    )
                );
            }      
        }
        

        /// <summary>
        /// Purges and logs filed over 7 days ago.
        /// </summary>
        public static void PurgeOldLogs()
        {
            // No directory means no logs to delete
            if (!Directory.Exists("./Logs"))
                return;

            string[] files = Directory.GetFiles("./Logs");
            foreach(string s in files)
            {
                FileInfo fi = new FileInfo(s);
                if (fi.LastAccessTimeUtc < DateTime.UtcNow.AddDays(-7))
                    fi.Delete();
            }
        }
    }
}
