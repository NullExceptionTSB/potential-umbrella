using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbyssalDraw.UI.Load {
    public static class LoadProgress {
        public static int CurrentTask { get; private set; } = 0;
        private static List<string> Tasks { get; set; } = new List<string> { "Loading..." };

        private static ProgressBar p_bar;
        private static Label l_task;

        public static void NextTask() => p_bar.Invoke(new Action(() => { procNextTask(); }));
        public static void AddTask(string TaskName) => p_bar.Invoke(new Action(() => { procAddTask(TaskName); }));

        private static void procNextTask() {
            l_task.Text = Tasks[++CurrentTask];
            p_bar.Value = CurrentTask;
        }
        private static void procAddTask(string TaskName) {
            Tasks.Add(TaskName);
            p_bar.Maximum++;
        }

        public static void Initialize(ProgressBar p_bar, Label l_task) {
            LoadProgress.p_bar = p_bar;
            LoadProgress.l_task = l_task;
            p_bar.Value = 0;
            p_bar.Minimum = 0;
            p_bar.Maximum = 1;
        }
    }
}
