using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbyssalDraw.UI.Draw {
    public partial class LogView : Form {
        public RichTextBox Log { get => rtb_log; }
        public LogView() {
            InitializeComponent();
        }
    }
}
