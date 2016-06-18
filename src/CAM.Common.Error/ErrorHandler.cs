using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAM.Common.Config;
using CAM.Common.DiskOperation.File;
using CAM.Common.DataProtocol;

namespace CAM.Common.Error
{
    public class ErrorHandler
    {

        private const string _ErrorConfigName = "ErrorConfig";

        [Serializable]
        private class ErrorConfig
        {
            public bool Log { get; set; }
            public string LogFile { get; set; }
            public string LogFormat { get; set; }

            public ErrorConfig()
            {
                Log = true;
                LogFile = "/CAM_WEB_LOG/ErrorLog.txt";

                StringBuilder sbFormat = new StringBuilder("");
                sbFormat.Append("\r\n");
                sbFormat.Append("---------Error:[{1}]----------\r\n");
                sbFormat.Append("Message：{0}\r\n");
                sbFormat.Append("Source：{2}\r\n");
                sbFormat.Append("StackTrace：{3}\r\n");
                sbFormat.Append("TargetSite：{4}\r\n");
                sbFormat.Append("DataPackager：{5}\r\n");
                sbFormat.Append("\r\n");

                LogFormat = sbFormat.ToString();
            }
        }

        private static ErrorConfig _ec = null;


        public static void ThrowException(string ErrorMessage)
        {
            Exception ex = new Exception(ErrorMessage);
            ThrowException(ex);
        }

        public static void ThrowException(Exception ex)
        {
            initErrorConfig();
            logError(ex);
            throw ex;
        }

        public static void NotThrowException(string ErrorMessage)
        {
            Exception ex = new Exception(ErrorMessage);
            NotThrowException(ex);
        }
        public static void NotThrowException(Exception ex)
        {
            initErrorConfig();
            logError(ex);
        }

        private static void initErrorConfig()
        {
            if (_ec == null)
            {
                IConfig<ErrorConfig> ic = ConfigFactory.createConfig<ErrorConfig>(_ErrorConfigName);
                _ec = ic.ConfigObject;
            }
        }

        private static void logError(Exception ex)
        {
            if (_ec.Log)
            {
                string logContent = string.Format(_ec.LogFormat,
                                        ex.Message,
                                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        string.IsNullOrWhiteSpace(ex.Source) ? "" : ex.Source,
                                        string.IsNullOrWhiteSpace(ex.StackTrace) ? "" : ex.StackTrace,
                                        ex.TargetSite == null ? "" : string.IsNullOrWhiteSpace(ex.TargetSite.Name) ? "" : ex.TargetSite.Name,
                                        DataPackager.packError(ex));

                IFileIO<string> ic = FileFactory.createStringFileIO();
                ic.appendFile(_ec.LogFile, logContent);
            }
        }

    }
}
