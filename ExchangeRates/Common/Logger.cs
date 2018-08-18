using log4net;

namespace ExchangeRates.Common
{
    public static class Logger
    {

        private readonly static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string logUser;

        static Logger()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        public static void Debug(string Message)
        {
            if (log.IsDebugEnabled)
                log.Debug(TryAugmentMessage(Message));
        }

        public static void Info(string Message)
        {
            if (log.IsInfoEnabled)
                log.Info(TryAugmentMessage(Message));
        }

        public static void Warn(string Message)
        {
            if (log.IsWarnEnabled)
                log.Warn(TryAugmentMessage(Message));
        }

        public static void Error(string Message)
        {
            log.Error(TryAugmentMessage(Message));
        }

        public static void Fatal(string Message)
        {
            log.Fatal(TryAugmentMessage(Message));
        }

        public static string TryAugmentMessage(string Message)
        {
            string result = Message;
            return result;
        }
    }
}

