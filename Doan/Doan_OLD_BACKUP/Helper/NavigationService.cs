using System;

namespace Doan.Helper
{
    public static class NavigationService
    {
        public static event Action<string, object> NavigateRequested;

        public static void Navigate(string route, object parameter = null)
        {
            NavigateRequested?.Invoke(route, parameter);
        }
    }
}
