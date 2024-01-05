using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AbyssalDraw.UI.Load;
using AbyssalDraw.Logging;
namespace AbyssalDraw.Core {
    public static class CoreEngine {
        #region Loading
        public static void AddTasks() {
            LoadProgress.AddTask("Initialize Logger...");


            LoadProgress.NextTask();
        }

        public static void Load() {
            Logger.RootLogger = new GuiLogger();

        }
        #endregion
    }
}
