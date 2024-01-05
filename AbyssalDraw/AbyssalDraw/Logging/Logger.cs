using System;
using System.Collections.Generic;

namespace AbyssalDraw.Logging {
    public abstract class Logger {
        private static Logger rootLogger = null;
        public static MessageLevel LogLevel { get; set; } = MessageLevel.Info;

        public enum MessageLevel {
            Verbose,
            Info,
            Warning,
            Error,
            Fatal
        }

        private readonly static Dictionary<MessageLevel, string> Prefixes =
            new Dictionary<MessageLevel, string> {
                { MessageLevel.Verbose, "[VERB]\t"},
                { MessageLevel.Info,    "[INFO]\t"},
                { MessageLevel.Warning, "[WARN]\t"},
                { MessageLevel.Error,   "[ERR ]\t"},
                { MessageLevel.Fatal,   "[FATL]\t"}
            };
        private readonly static Dictionary<MessageLevel, ConsoleColor> Colours =
            new Dictionary<MessageLevel, ConsoleColor> {
                { MessageLevel.Verbose, ConsoleColor.Blue},
                { MessageLevel.Info,    ConsoleColor.White},
                { MessageLevel.Warning, ConsoleColor.Yellow},
                { MessageLevel.Error,   ConsoleColor.Red},
                { MessageLevel.Fatal,   ConsoleColor.DarkRed}
            };

        protected static System.Drawing.Color ColourConvertFromConsole(ConsoleColor c) {
            switch (c) {
                case ConsoleColor.DarkYellow:
                    return System.Drawing.Color.FromArgb(255, 128, 128, 0);
                default:
                    return System.Drawing.Color.FromName(c.ToString());
            }
        }

        public static Logger RootLogger { 
            get => rootLogger;
            set {
                if (rootLogger == null)
                    rootLogger = value;
            }
        }

        protected string GetPrefix(MessageLevel msg_level) => Prefixes[msg_level];
        protected ConsoleColor GetColour(MessageLevel msg_level) => Colours[msg_level];
        public abstract void Log(string text, MessageLevel msg_level = MessageLevel.Info);

        public virtual void LogVerbose(string text) => Log(text, MessageLevel.Verbose);
        public virtual void LogInfo(string text) => Log(text, MessageLevel.Info);
        public virtual void LogWarning(string text) => Log(text, MessageLevel.Warning);
        public virtual void LogError(string text) => Log(text, MessageLevel.Error);
        public virtual void LogFatal(string text) => Log(text, MessageLevel.Fatal);
    }
}
