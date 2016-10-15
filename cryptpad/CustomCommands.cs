using System.Windows.Input;

namespace cryptpad
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand Exit = new RoutedUICommand(
            "Exit",
            "Exit",
            typeof(CustomCommands)
        );

        public static readonly RoutedUICommand WordWrap = new RoutedUICommand(
            "Word wrap",
            "WordWrap",
            typeof(CustomCommands)
        );
    }
}
