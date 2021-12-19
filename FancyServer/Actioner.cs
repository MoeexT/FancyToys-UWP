using System.Drawing;
using System.IO;
using System.Windows.Forms;


namespace FancyServer {

    public class Actioner {
        public static ToolStripMenuItem GetItem(string pathName, string processName = null) {
            if (processName == null || processName.Equals(string.Empty)) {
                processName = Path.GetFileNameWithoutExtension(pathName);
            }

            ToolStripMenuItem item = new ToolStripMenuItem() {
                Text = processName,
                // Tag = pathName,
                AutoToolTip = true,
                ToolTipText = pathName,
                CheckOnClick = false,
                BackColor = Color.White,
                CheckState = CheckState.Unchecked
            };

            item.Click += (s, e) => {
                ToolStripMenuItem i = s as ToolStripMenuItem;

                // 这里有不懂的地方(bug)：`i.CheckState`理应为Unchecked，却总是为Checked 现在没了
                // if (i?.CheckState == CheckState.Checked) {
                //     NoformToOperation.StopProcess(i.ToolTipText);
                // } else {
                //     NoformToOperation.StartProcess(i?.ToolTipText);
                // }
            };
            return item;
        }
    }

}