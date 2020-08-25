using System.Threading.Tasks;
using System.Windows.Input;

namespace CommonUtils.Interfaces
{
    /// <summary>
    /// Интерфейс асинхронной команды.
    /// </summary>
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}