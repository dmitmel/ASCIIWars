using System;
namespace ASCIIWars.ConsoleGraphics {
    public static class MyConsole {
        public const string ANSI_RESET = "\x1B[0m";
        public const string ANSI_BOLD = "\x1B[1m";
        public const string ANSI_ITALIC = "\x1B[3m";
        public const string ANSI_UNDERLINE = "\x1B[4m";

        public static void MoveCursorUp(int steps) {
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - steps);
        }

        public static void MoveCursorDown(int steps) {
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop + steps);
        }

        public static void MoveCursorLeft(int steps) {
            Console.SetCursorPosition(Console.CursorLeft - steps, Console.CursorTop);
        }

        public static void MoveCursorRight(int steps) {
            Console.SetCursorPosition(Console.CursorLeft + steps, Console.CursorTop);
        }

        public static void MoveCursorToBeginingOfLine() {
            Console.SetCursorPosition(0, Console.CursorTop);
        }

        public static void ClearLine() {
            MoveCursorToBeginingOfLine();
            // Отнимание 1 тут очень важно - теперь, курсор не будет перескакивать на линию вперёд
            Console.Write(new string(' ', Console.BufferWidth - 1));
            MoveCursorToBeginingOfLine();
        }

        public static void ClearLinesBeforeThis(int count) {
            for (int line = 0; line < count; line++) {
                MoveCursorUp(1);
                ClearLine();
            }
            MoveCursorDown(count);
        }

        public static void ClearLinesAfterThis(int count) {
            for (int line = 0; line < count; line++) {
                MoveCursorDown(1);
                ClearLine();
            }
            MoveCursorUp(count);
        }

        static ConsoleStyle _style = ConsoleStyle.NoStyle;
        public static ConsoleStyle Style {
            get {
                return _style;
            }
            set {
                _style = value;
                switch (value) {
                    case ConsoleStyle.NoStyle:
                        Console.Write(ANSI_RESET);
                        break;
                    case ConsoleStyle.Bold:
                        Console.Write(ANSI_BOLD);
                        break;
                    case ConsoleStyle.Italic:
                        Console.Write(ANSI_ITALIC);
                        break;
                    case ConsoleStyle.Underline:
                        Console.Write(ANSI_UNDERLINE);
                        break;
                }
            }
        }

        public static void ResetStyle() {
            Style = ConsoleStyle.NoStyle;
        }
    }

    public enum ConsoleStyle {
        NoStyle, Bold, Italic, Underline
    }
}
