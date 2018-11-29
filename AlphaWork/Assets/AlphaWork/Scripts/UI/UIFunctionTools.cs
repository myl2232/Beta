using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphaWork
{
    public static class UIFunctionTools
    {
        public static void OpenUIFromId(int ui_id)
        {
            GameEntry.UI.OpenUIForm(ui_id, null);
        }
    }
}
