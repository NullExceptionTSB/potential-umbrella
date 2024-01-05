using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbyssalDraw.UI.Draw;

namespace AbyssalDraw.Logging {
    public sealed class GuiLogger : Logger {
        private LogView log_view;

        public override void Log(string text, MessageLevel msg_level = MessageLevel.Info) {
            if (msg_level < LogLevel) return;

            log_view.Log.SelectionStart = log_view.Log.TextLength;
            log_view.Log.SelectionLength = 0;

            log_view.Log.SelectionColor = ColourConvertFromConsole(GetColour(msg_level));
            log_view.Log.AppendText(GetPrefix(msg_level) + text + "\r\n");
            log_view.Log.SelectionColor = log_view.Log.ForeColor;
        }

        public void ShowLogView() => log_view.Show();

        public GuiLogger() {
            log_view = new LogView();
        }
    }
}
