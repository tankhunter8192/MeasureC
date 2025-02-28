using Gpib.Web.Data.DBClasses;
namespace Gpib.Web.Data
{
    public class Logger
    {
        public void Log(Gpib.Web.Data.DBClasses.LogSeverity serverity, string source, string message)
        {
            GlobalStaticVariables.DbContext.LogMessages.Add(new LogMessage
            {
                Source = source,
                CreationDateTime = DateTime.UtcNow,
                Message = message,
                Severity = serverity
            });
            GlobalStaticVariables.DbContext.SaveChanges();
        }
        public void LogInfo(string source, string message)
        {
            Log(Gpib.Web.Data.DBClasses.LogSeverity.Info, source, message);
        }
        public void LogWarning(string source, string message)
        {
            Log(Gpib.Web.Data.DBClasses.LogSeverity.Warning, source, message);
        }
        public void LogError(string source, string message)
        {
            Log(Gpib.Web.Data.DBClasses.LogSeverity.Error, source, message);
        }

    }
}
