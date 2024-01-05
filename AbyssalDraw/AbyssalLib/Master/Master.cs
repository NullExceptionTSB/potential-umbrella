using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbyssalLib.Master {
    public static class Master {
        private static BindingContext Context { get; set; } = null;

        public static bool Bind(BindingContext Context) {
            if (Context != null) return false;
            Master.Context = Context;
            return true;
        }
    }
}
