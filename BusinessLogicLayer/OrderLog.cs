using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
  public class OrderLog : ILogging
    {
        private static string exceptionStr = "<N:LogFile; C:ErrorLog;";
        public static void OrderLogFile(string message, string moduleName, System.Web.HttpServerUtility server)
        {
            try
            {
                ReadConfig readConfig = new ReadConfig(server);

                string filePath = readConfig.Order_LOG_FOLDER_PATH;

                // add module name directory to the file path
                filePath += @"\" + moduleName + @"\";

                // To create a directory  if not exist and if it is exist then
                // to create a error log file if error log file is already exist then it
                // will append the data and write to that file, it will create a log file  
                // according to the system date, the file name will be datewise e.g. "16062011"  
                StreamWriter sw = new StreamWriter(
                    (Directory.Exists(filePath) ? filePath
                    : System.IO.Directory.CreateDirectory(filePath).FullName)
                    + DateTime.Now.ToString("ddMMyyyy") + ".txt",
                     true);
                sw.WriteLine(message);
                sw.WriteLine("-----------------------------------------------------------------------------------------------------");
                sw.Flush();
                sw.Close();
            }
            catch (IOException ex)
            {
                throw new MyException(exceptionStr + " M:ErrorLogFile>" + Environment.NewLine, "File IO Error!" + ex.Message);
            }
            catch (MyException mex)
            {
                throw new MyException(exceptionStr + " M:ErrorLogFile>" + Environment.NewLine + mex.EXCEPTION_PATH, "Can't Create Transaction Log File!" + mex.EXCEPTION_MSG);
            }
            catch (Exception ex)
            {
                throw new MyException(exceptionStr + " M:ErrorLogFile>" + Environment.NewLine, "Can't Create Transaction Log File!" + ex.Message);
            }
        }

    }
}
