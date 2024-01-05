using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AbyssalDraw.UI.Load;

namespace AbyssalDraw.UI {
    public partial class LoadScreen : Form {
        public LoadScreen() {
            InitializeComponent();
            LoadProgress.Initialize(pb_progress, l_status);
            bw_loader.RunWorkerAsync();
        }

        private void bw_loader_DoWork(object sender, DoWorkEventArgs e) {
            Core.CoreEngine.Load();
        }
    }
}
