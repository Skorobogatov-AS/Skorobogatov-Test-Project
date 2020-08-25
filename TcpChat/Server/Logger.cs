using System;
using System.IO;

namespace Server
{
    /// <summary>
    /// Обработчик логов.
    /// </summary>
    public class Logger
    {
        // В последний момент я захотел отображать логи ещё и в окне сервера.
        // Поэтому вместо статического метода тут будет синглтон с Action.

        /// <summary>
        /// <see cref="Logger"/>
        /// </summary>
        private static volatile Logger _instance;

        /// <summary>
        /// Делегат для передачи сообщения логгера.
        /// </summary>
        private Action<string> ReservedAction { get; }

        /// <summary>
        /// Oбъект для lock.
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// <inheritdoc cref="Logger"/>
        /// </summary>
        /// <param name="reservedAction"><see cref="ReservedAction"/></param>
        private Logger(Action<string> reservedAction = null)
        {
            ReservedAction = reservedAction;
        }

        /// <summary>
        /// Получает объект логгера с установленным обработчиком для передачи сообщений.
        /// </summary>
        /// <param name="reservedAction"></param>
        /// <returns></returns>
        public static Logger GetInstance(Action<string> reservedAction = null)
        {
            if (_instance != null) 
                return _instance;
            
            lock (SyncRoot)
            {
                if (_instance == null)
                    _instance = new Logger(reservedAction);
            }

            return _instance;
        }

        /// <summary>
        /// Записать в журнал.
        /// </summary>
        /// <param name="logMessage"> Сообщение. </param>
        public async void LogMessage(string logMessage)
        {
            var timeStampString = $@"{DateTime.Now.ToLongTimeString()}: {logMessage}";
            Console.WriteLine(timeStampString);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), $"{DateTime.Now.ToShortDateString()} - logfile.txt");

            using (var streamWriter = new StreamWriter(filePath, true, System.Text.Encoding.UTF8))
            {
                await streamWriter.WriteLineAsync(timeStampString);
            }

            ReservedAction?.Invoke(timeStampString);
        }
    }
}