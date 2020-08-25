using System;
using System.Threading.Tasks;

namespace CommonUtils.Commands
{
    /// <summary>
    /// Команда с асинхронным выполнением.
    /// </summary>
    public class AsyncCommand : AsyncCommandBase
    {
        /// <summary>
        /// Метод без входных параметров для выполняемой команды.
        /// </summary>
        private readonly Func<Task> _noParamCommand;

        /// <summary>
        /// Метод с одним параметром для выполняемой команды.
        /// </summary>
        private readonly Func<object, Task> _singleParamCommand;

        /// <summary>
        /// Метод, определяющий возможность выполнения команды. 
        /// </summary>
        private readonly Func<bool> _canExecute;

        /// <summary>
        /// Команда с асинхронным выполнением.
        /// </summary>
        /// <param name="noParamCommand"> Метод выполняемой команды. </param>
        /// <param name="canExecute"> Метод, определяющий возможность выполнения команды. </param>
        public AsyncCommand(Func<Task> noParamCommand, Func<bool> canExecute = null)
        {
            _noParamCommand = noParamCommand;
            _canExecute = canExecute ?? (() => true);
        }

        /// <summary>
        /// Команда с асинхронным выполнением.
        /// </summary>
        /// <param name="singleParamCommand"> Метод выполняемой команды. </param>
        /// <param name="canExecute"> Метод, определяющий возможность выполнения команды. </param>
        public AsyncCommand(Func<object, Task> singleParamCommand, Func<bool> canExecute = null)
        {
            _singleParamCommand = singleParamCommand;
            _canExecute = canExecute ?? (() => true);
        }

        public override bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public override Task ExecuteAsync(object parameter)
        {
            return parameter == null 
                ? _noParamCommand() 
                : _singleParamCommand(parameter);
        }
    }
}
