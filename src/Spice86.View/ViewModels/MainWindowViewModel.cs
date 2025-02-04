﻿using Spice86.View.Views;

namespace Spice86.View.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly MainWindow _mainWindow;

        public MainWindowViewModel(MainWindow window)
        {
            _mainWindow = window;
        }

        public string Greeting => "Welcome to Avalonia!";

        internal static MainWindowViewModel Create(MainWindow mainWindow)
        {
            return new MainWindowViewModel(mainWindow);
        }
    }
}