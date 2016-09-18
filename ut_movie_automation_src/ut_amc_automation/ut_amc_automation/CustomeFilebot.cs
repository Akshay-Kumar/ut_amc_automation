using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using SourceCodeDAL;
using System.Text.RegularExpressions;

namespace ut_amc_automation
{
    class CustomeFilebot
    {
        public Movie movieMetadata = null;
        SourceDataAccessLayer dal = new SourceDataAccessLayer();
        private string arguments = string.Empty;
        private string command = string.Empty;
        private ErrorLoggerListener logger = null;
        private static bool INSERT_FLAG = false;
        public void StartProcess(string[] args)
        {
            if (args != null)
            {
                if (args.Length > 0)
                {
                    try
                    {
                        movieMetadata = new Movie();
                        string workingDir = Path.GetDirectoryName(
                        Assembly.GetExecutingAssembly().Location);
                        string logFile = Path.Combine(workingDir, "amc-cmd.log");
                        string batchFile = Path.Combine(workingDir, args[0]);
                        logger = new ErrorLoggerListener(logFile);
                        for (int i = 1; i < args.Length; i++)
                        {
                            arguments += "\"" + NVL(args[i], "") + "\"";
                            arguments += " ";
                        }

                        command = batchFile + " " + arguments.Trim();
                        ProcessStartInfo startInfo = new ProcessStartInfo("cmd", "/c " + command);
                        //startInfo.Arguments = arguments.Trim();
                        startInfo.UseShellExecute = false;
                        startInfo.RedirectStandardOutput = true;
                        startInfo.RedirectStandardError = true;
                        startInfo.WorkingDirectory = workingDir;
                        //startInfo.CreateNoWindow = true;
                        Process process = new Process();
                        process.StartInfo = startInfo;
                        process.OutputDataReceived += CaptureOutput;
                        process.ErrorDataReceived += CaptureError;
                        process.Start();
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();
                        process.WaitForExit();
                        if (INSERT_FLAG)
                        {
                            movieMetadata.InsertDate = DateTime.Now;
                            if (dal.InsertOrUpdateMovieData(movieMetadata))
                            {
                                ShowOutput(string.Format("{0} added successfully to the library.", movieMetadata.Movie_title), ConsoleColor.Green);
                            }
                            else
                            {
                                ShowOutput(string.Format("Error adding {0} to the library.", movieMetadata.Movie_title), ConsoleColor.Red);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.ErrorLog.ErrorRoutine(e);
                    }
                }
                else
                {
                    ShowOutput("No arguments passed.", ConsoleColor.Red);
                }
            }
            else
            {
                ShowOutput("No arguments passed.", ConsoleColor.Red);
            }
        }
        private void CaptureOutput(object sender, DataReceivedEventArgs e)
        {
            ShowOutput(e.Data, ConsoleColor.Green);
        }

        private void CaptureError(object sender, DataReceivedEventArgs e)
        {
            ShowOutput(e.Data, ConsoleColor.Red);
        }

        private void ShowOutput(string data, ConsoleColor color)
        {
            if (data != null)
            {
                try
                {
                    string consoleOutput = string.Format("{0}", data);
                    ConsoleColor oldColor = Console.ForegroundColor;
                    Console.ForegroundColor = color;
                    Console.WriteLine(consoleOutput);
                    Console.ForegroundColor = oldColor;
                    logger.Log_Error(consoleOutput);
                    ExtractMovieData(consoleOutput);
                }
                catch (Exception e)
                {
                    Logger.ErrorLog.ErrorRoutine(e);
                }
            }
            if (data != null && data.Contains("Done"))
                INSERT_FLAG = true;
        }

        private string NVL(string input, string replacement)
        {
            if (string.IsNullOrEmpty(input))
                return replacement;
            return input;
        }

        private void ExtractMovieData(string rawData)
        {
            try
            {
                if (!string.IsNullOrEmpty(rawData))
                {
                    if (rawData.Contains("ut_title"))
                    {
                        movieMetadata.Movie_title = ExtractData(rawData, "ut_title");
                        //ProcessMonitor.Instance().Monitor("Notification : ", string.Format("{0} : {1}", "ut_title", movieMetadata.Movie_title), Monitor.ComConfig.Notification.Information);
                    }
                    if (rawData.Contains("ut_title"))
                    {
                        int release_yr = 1900;
                        string resultString = Regex.Match(movieMetadata.Movie_title, @"\d{4}").Value;
                        release_yr = Int32.Parse(resultString);
                        movieMetadata.Release_year = release_yr;
                        //ProcessMonitor.Instance().Monitor("Notification : ", string.Format("{0} : {1}", "Release_year", movieMetadata.Release_year), Monitor.ComConfig.Notification.Information);
                    }
                    //[COPY] Rename
                    if (rawData.Contains("[COPY] Rename"))
                    {
                        string path = Path.GetFullPath(ExtractAddData(rawData, "[COPY] Rename"));
                        string directory = Path.GetDirectoryName(path);
                        movieMetadata.Movie_file_name.Add(path);
                        movieMetadata.Movie_directory = directory;
                        //ProcessMonitor.Instance().Monitor("Notification : ", string.Format("{0} : {1}", "directory", movieMetadata.Movie_directory), Monitor.ComConfig.Notification.Information);
                        //ProcessMonitor.Instance().Monitor("Notification : ", string.Format("{0} : {1}", "Movie_file_name", path), Monitor.ComConfig.Notification.Information);
                    }
                    //Fetching
                    if (rawData.Contains("Fetching") && (rawData.Contains(".jpg") || rawData.Contains(".png")))
                    {
                        string format = rawData.Contains(".jpg") ? ".jpg" : (rawData.Contains(".png") ? ".png" : "");
                        string artwork = ExtractArtWork(rawData, "Fetching", format);
                        movieMetadata.Movie_file_name.Add(artwork);
                        //ProcessMonitor.Instance().Monitor("Notification : ", string.Format("{0} : {1}", "Movie_file_art_work", artwork), Monitor.ComConfig.Notification.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorLog.ErrorRoutine(ex);
            }
        }
        private string ExtractData(string data,string search)
        {
            int start = -1;
            try
            {
                start = data.IndexOf(search);
                start = start + search.Length + 3;
            }
            catch (Exception ex)
            {
                Logger.ErrorLog.ErrorRoutine(ex);
            }
            return data.Substring(start);
        }
        private string ExtractAddData(string data, string search)
        {
            int start = -1;
            try
            {
                start = data.IndexOf(search);
                start = start + search.Length + 1;
                start = data.IndexOf("to", start);
                start = start + "to".Length + 1;
            }
            catch (Exception ex)
            {
                Logger.ErrorLog.ErrorRoutine(ex);
            }
            return data.Substring(start + 1,data.Substring(start + 1).Trim().Length-1).Trim();
        }
        private string ExtractArtWork(string data, string search, string format)
        {
            int start = -1, end = -1, length =-1;
            try
            {
                start = data.IndexOf(search);
                start = start + search.Length + 1;
                end = data.IndexOf(format, start) + format.Length;
                length = end - start;
            }
            catch (Exception ex)
            {
                Logger.ErrorLog.ErrorRoutine(ex);
            }
            return data.Substring(start, length).Trim();
        }
    }
}
