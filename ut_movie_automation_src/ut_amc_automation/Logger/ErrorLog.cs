using System;
using System.IO;
using System.Xml;
using System.Net;
using System.Drawing;

namespace Logger
{
	/// <summary>
	/// Logger is used for creating a customized error log files or an error can be registered as
	/// a log entry in the Windows Event Log on the administrator's machine.
	/// </summary>
	public class ErrorLog
    {
        #region Declaration
        private const double MAX_SIZE = 1000;
        protected static string strLogFilePath	= string.Empty;
		private static StreamWriter sw = null;
        #endregion
        /// <summary>
		/// Setting LogFile path. If the logfile path is null then it will update error info into LogFile.txt under
		/// application directory.
		/// </summary>
		public static string LogFilePath
		{
			set
			{
                FileInfo fileInf0 = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "\\" +DEFAULT_LOG_FILE.DefaultLogFolder + "\\" + value);
                if (!fileInf0.Exists)
                {
                    if (CheckDirectory(fileInf0.FullName))
                    {
                        fileInf0.Create().Close();
                        strLogFilePath = fileInf0.FullName;
                    }
                }
			}
			get
			{
				return strLogFilePath;
			}
		}
        /// <summary>
        /// Write error log entry for window event if the bLogType is true. Otherwise, write the log entry to
        /// customized text-based text file
        /// </summary>
        /// <param name="bLogType"></param>
        /// <param name="objException"></param>
        /// <returns>false if the problem persists</returns>
        public static bool ErrorRoutine(Exception objException)
        {
            try
            {
                //Check whether logging is enabled or not
                bool bLoggingEnabled = false;
                bLoggingEnabled = CheckLoggingEnabled();

                //Write to Custom text-based event log
                if (bLoggingEnabled)
                {
                    if (true != CustomErrorRoutine(objException))
                        return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// Write error log entry for window event if the bLogType is true. Otherwise, write the log entry to
        /// customized text-based text file
        /// </summary>
        /// <param name="bLogType"></param>
        /// <param name="objException"></param>
        /// <returns>false if the problem persists</returns>
        public static bool ErrorRoutine(string error)
        {
            try
            {
                //Check whether logging is enabled or not
                bool bLoggingEnabled = false;
                bLoggingEnabled = CheckLoggingEnabled();
                //Write to Custom text-based event log
                if (bLoggingEnabled)
                {
                    if (true != CustomErrorRoutine(error))
                        return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// Check Logginstatus config file is exist. If exist read the value set the loggig status
        /// </summary>
        private static bool CheckLoggingEnabled()
        {
            string strLoggingStatusConfig = string.Empty;

            strLoggingStatusConfig = GetLoggingStatusConfigFileName();

            //If it's empty then enable the logging status 
            if (strLoggingStatusConfig.Equals(string.Empty))
            {
                return true;
            }
            else
            {

                //Read the value from xml and set the logging status
                bool bTemp = GetValueFromXml(strLoggingStatusConfig);
                return bTemp;
            }
        }
        /// <summary>
        /// Check the Logginstatus config under debug or release folder. If not exist, check under 
        /// project folder. If not exist again, return empty string
        /// </summary>
        /// <returns>empty string if file not exists</returns>
        private static string GetLoggingStatusConfigFileName()
        {
            string strCheckinBaseDirecotry = AppDomain.CurrentDomain.BaseDirectory + "LoggingStatus.Config";

            if (File.Exists(strCheckinBaseDirecotry))
                return strCheckinBaseDirecotry;
            else
            {
                string strCheckinApplicationDirecotry = GetApplicationPath() + "LoggingStatus.Config";

                if (File.Exists(strCheckinApplicationDirecotry))
                    return strCheckinApplicationDirecotry;
                else
                    return string.Empty;
            }
        }
        /// <summary>
        /// public static void ToggleLogging(bool status)
        /// </summary>
        /// <param name="status"></param>
        public static void EnableOrDisableLogging(bool status)
        {
            string xmlPath = GetLoggingStatusConfigFileName();
            string strGetValue = string.Empty;
            //Open a FileStream on the Xml file
            FileStream docIn = new FileStream(xmlPath, FileMode.Open, FileAccess.ReadWrite, FileShare.Write);
            XmlDocument contactDoc = new XmlDocument();
            //Load the Xml Document
            contactDoc.Load(docIn);
            //Get a node
            XmlNodeList UserList = contactDoc.GetElementsByTagName("LoggingEnabled");
            if (status == true)
                strGetValue = "1";
            else if (status == false)
                strGetValue = "0";
            UserList.Item(0).InnerText = strGetValue;
            docIn.Close();
        }

        /// <summary>
        /// Read the xml file and getthe logging status
        /// </summary>
        /// <param name="strXmlPath"></param>
        /// <returns></returns>
        private static bool GetValueFromXml(string strXmlPath)
        {
            try
            {
                //Open a FileStream on the Xml file
                FileStream docIn = new FileStream(strXmlPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                XmlDocument contactDoc = new XmlDocument();
                //Load the Xml Document
                contactDoc.Load(docIn);

                //Get a node
                XmlNodeList UserList = contactDoc.GetElementsByTagName("LoggingEnabled");

                //get the value
                string strGetValue = UserList.Item(0).InnerText.ToString();

                if (strGetValue.Equals("0"))
                    return false;
                else if (strGetValue.Equals("1"))
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// If the LogFile path is empty then, it will write the log entry to Logger.log under application directory.
        /// If the Logger.log is not availble it will create it
        /// If the Log File path is not empty but the file is not availble it will create it.
        /// </summary>
        /// <param name="objException"></param>
        /// <returns>false if the problem persists</returns>
        private static bool CustomErrorRoutine(Exception objException)
        {
            string strPathName = string.Empty;
            if (strLogFilePath.Equals(string.Empty))
            {
                //Get Default log file path "Logger.log"
                strPathName = GetLogFilePath();
            }
            else
            {
                //If the log file path is not empty but the file is not available it will create it
                if (false == File.Exists(strLogFilePath))
                {
                    if (false == CheckDirectory(strLogFilePath))
                        return false;

                    FileStream fs = new FileStream(strLogFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fs.Close();
                }
                strPathName = strLogFilePath;
            }

            bool bReturn = true;
            // write the error log to that text file
            if (true != WriteErrorLog(strPathName, objException))
            {
                bReturn = false;
            }
            return bReturn;
        }
        /// <summary>
        /// If the LogFile path is empty then, it will write the log entry to Logger.log under application directory.
        /// If the Logger.log is not availble it will create it
        /// If the Log File path is not empty but the file is not availble it will create it.
        /// </summary>
        /// <param name="objException"></param>
        /// <returns>false if the problem persists</returns>
        private static bool CustomErrorRoutine(string logMessage)
        {
            string strPathName = string.Empty;
            if (strLogFilePath.Equals(string.Empty))
            {
                //Get Default log file path "Logger.log"
                strPathName = GetLogFilePath();
            }
            else
            {
                //If the log file path is not empty but the file is not available it will create it
                if (false == File.Exists(strLogFilePath))
                {
                    if (false == CheckDirectory(strLogFilePath))
                        return false;

                    FileStream fs = new FileStream(strLogFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fs.Close();
                }
                strPathName = strLogFilePath;
            }

            bool bReturn = true;
            // write the error log to that text file
            if (true != WriteErrorLog(strPathName, logMessage))
            {
                bReturn = false;
            }
            return bReturn;
        }
        /// <summary>
        /// Write Source,method,date,time,computer,error and stack trace information to the text file
        /// </summary>
        /// <param name="strPathName"></param>
        /// <param name="objException"></param>
        /// <returns>false if the problem persists</returns>
        private static bool WriteErrorLog(string strPathName, Exception objException)
        {
            bool bReturn = false;
            try
            {
                sw = new StreamWriter(strPathName, true);
                sw.WriteLine(ERROR_LABEL.SOURCE + "     \t  :" + objException.Source.ToString().Trim());
                sw.WriteLine(ERROR_LABEL.METHOD + "     \t  :" + objException.TargetSite.Name.ToString().Trim());
                sw.WriteLine(ERROR_LABEL.DATE + "       \t  :" + string.Format("{0:MM/dd/yyyy}", DateTime.Now));
                sw.WriteLine(ERROR_LABEL.TIME + "       \t  :" + string.Format("{0:hh-mm-ss-ffff tt}", DateTime.Now));
                sw.WriteLine(ERROR_LABEL.COMPUTER + "   \t  :" + Dns.GetHostName().ToString().Trim());
                sw.WriteLine(ERROR_LABEL.ERROR + "      \t  :" + objException.Message.ToString().Trim());
                sw.WriteLine(ERROR_LABEL.STACK_TRACE + "\t  :" + objException.StackTrace.ToString().Trim());
                sw.WriteLine("---------------------------------------------------------------------------------------------------");
                sw.Flush();
                sw.Close();
                bReturn = true;
            }
            catch (Exception)
            {
                bReturn = false;
            }
            return bReturn;
        }
        /// <summary>
        /// Write Source,method,date,time,computer,error and stack trace information to the text file
        /// </summary>
        /// <param name="strPathName"></param>
        /// <param name="objException"></param>
        /// <returns>false if the problem persists</returns>
        private static bool WriteErrorLog(string strPathName, string errorMsg)
        {
            bool bReturn = false;
            try
            {
                sw = new StreamWriter(strPathName, true);
                sw.WriteLine(string.Format("{0:MM/dd/yyyy} {1:hh-mm-ss-ffff tt} : {2}", DateTime.Now,DateTime.Now,errorMsg));
                sw.WriteLine("---------------------------------------------------------------------------------------------------");
                sw.Flush();
                sw.Close();
                bReturn = true;
            }
            catch (Exception)
            {
                bReturn = false;
            }
            return bReturn;
        }
        /// <summary>
        /// Check the log file in applcation directory. If it is not available, creae it
        /// </summary>
        /// <returns>Log file path</returns>
        public static string GetLogFilePath()
        {
            try
            {
                // get the base directory
                string baseDir = AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.RelativeSearchPath;

                // search the file below the current directory
                string retFilePath = baseDir + "" + DEFAULT_LOG_FILE.DefaultLogFolder + "\\" + DEFAULT_LOG_FILE.DefaultLogFile;
                
                // if exists, return the path
                if (File.Exists(retFilePath) == true)
                {
                    // check if log file needs to be rolled
                    loggerRoll(retFilePath);
                    return retFilePath;
                }
                //create a text file
                else
                {
                    if (false == CheckDirectory(retFilePath))
                        return string.Empty;

                    FileStream fs = new FileStream(retFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fs.Close();
                }

                return retFilePath;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// Create a directory if not exists
        /// </summary>
        /// <param name="strLogPath"></param>
        /// <returns></returns>
        private static bool CheckDirectory(string strLogPath)
        {
            try
            {
                int nFindSlashPos = strLogPath.Trim().LastIndexOf("\\");
                string strDirectoryname = strLogPath.Trim().Substring(0, nFindSlashPos);

                if (false == Directory.Exists(strDirectoryname))
                    Directory.CreateDirectory(strDirectoryname);

                return true;
            }
            catch (Exception)
            {
                return false;

            }
        }

        private static string GetApplicationPath()
        {
            try
            {
                string strBaseDirectory = AppDomain.CurrentDomain.BaseDirectory.ToString();
                int nFirstSlashPos = strBaseDirectory.LastIndexOf("\\");
                string strTemp = string.Empty;

                if (0 < nFirstSlashPos)
                    strTemp = strBaseDirectory.Substring(0, nFirstSlashPos);

                int nSecondSlashPos = strTemp.LastIndexOf("\\");
                string strTempAppPath = string.Empty;
                if (0 < nSecondSlashPos)
                    strTempAppPath = strTemp.Substring(0, nSecondSlashPos);

                string strAppPath = strTempAppPath.Replace("bin", "");
                return strAppPath;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private static void loggerRoll(string filePath)
        {
            FileInfo fInfo = new FileInfo(filePath);
            double len = fInfo.Length;
            double size = len / 1024;
            if (size >= MAX_SIZE)
            {
                // get the base directory
                string baseDir = AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.RelativeSearchPath;

                // search the file below the current directory
                string retFilePath = baseDir + "" + DEFAULT_LOG_FILE.DefaultLogFolder + "\\" + string.Format("{0:MM-dd-yyyy_hh-mm-ss}.log", DateTime.Now);
                fInfo.MoveTo(retFilePath);
            }
        }
    }

    public class ERROR_LABEL
    {
        public const string SOURCE = "Source";
        public const string METHOD = "Method";
        public const string DATE = "Date";
        public const string TIME = "Time";
        public const string COMPUTER = "Computer";
        public const string ERROR = "Error";
        public const string STACK_TRACE = "Stack Trace";
    }
    public class DEFAULT_LOG_FILE
    {
        private static string defaultLogFile = "ErrorLogger.log";
        private static string defaultLogFolder = "log";

        public static string DefaultLogFolder
        {
            get { return DEFAULT_LOG_FILE.defaultLogFolder; }
            set { DEFAULT_LOG_FILE.defaultLogFolder = value; }
        }
        public static string DefaultLogFile
        {
            get { return DEFAULT_LOG_FILE.defaultLogFile; }
            set { DEFAULT_LOG_FILE.defaultLogFile = value; }
        }
    }
}
