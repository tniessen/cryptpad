using System.Windows.Input;

namespace cryptpad
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand Exit = new RoutedUICommand(
            Properties.Resources.CommandExit,
            "Exit",
            typeof(CustomCommands)
        );

        public static readonly RoutedUICommand WordWrap = new RoutedUICommand(
            Properties.Resources.CommandWordWrap,
            "WordWrap",
            typeof(CustomCommands)
        );

        public static readonly RoutedUICommand StatusBar = new RoutedUICommand(
            Properties.Resources.CommandStatusBar,
            "StatusBar",
            typeof(CustomCommands)
        );

        public static readonly RoutedUICommand AboutTheApp = new RoutedUICommand(
            Properties.Resources.CommandAboutTheApp,
            "AboutTheApp",
            typeof(CustomCommands)
        );
    }
}
