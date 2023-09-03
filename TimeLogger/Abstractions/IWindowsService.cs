using System.Windows;

namespace TimeLogger.Abstractions
{
    /// <summary>
    /// Сервис работы с окнами приложения
    /// </summary>
    public interface IWindowsService
    {
        void Show<TWindow>() where TWindow : Window;
        void Hide<TWindow>() where TWindow : Window;
    }
}
