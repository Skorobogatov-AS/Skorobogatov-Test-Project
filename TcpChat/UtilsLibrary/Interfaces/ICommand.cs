using System.Threading.Tasks;
using System.Windows.Input;

namespace UtilsLibrary.Interfaces
{
    /// <summary>
    /// Интерфейс асинхронной команды.
    /// </summary>
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}