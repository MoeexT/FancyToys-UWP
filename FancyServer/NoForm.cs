using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using FancyServer.Action;
using FancyServer.Logging;
using FancyServer.Nursery;


namespace FancyServer {

    public partial class NoForm: Form {

        private readonly ActionManager ActionManager;
        private readonly ProcessManager ProcessManager;
        private readonly ToolStripItemCollector NurseryCollector;

        public static NoForm CurrentInstance { get; private set; }

        public NoForm(ActionManager actionManager, ProcessManager processManager) {
            InitializeComponent();
            InitForm();
            NurseryCollector = new ToolStripItemCollector(NurseryMenu.DropDownItems);
            ActionManager = actionManager ?? throw new ArgumentNullException(nameof(actionManager));
            ProcessManager = processManager ?? throw new ArgumentNullException(nameof(processManager));
            ProcessManager.OnProcessAdd += AddNurseryItem;
            ProcessManager.OnProcessLaunched += UpdateNurseryItem;
            ProcessManager.OnProcessExited += UpdateNurseryItem;
            ProcessManager.OnProcessRemoved += RemoveNurseryItem;
            CurrentInstance = this;
        }

        private void InitForm() {
            NurserySeparatorItem.Paint += (s, e) => {
                if (!(s is ToolStripSeparator sep)) return;
                e.Graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, sep.Width, sep.Height);
                e.Graphics.DrawLine(new Pen(Color.Black), 25, sep.Height >> 1, sep.Width - 4, sep.Height >> 1);
            };
        }

        public void AddNurseryItem(ProcessInfo info) {
            Logger.Trace("AddNurseryItem" + info);

            ToolStripItem item = NurseryCollector[info.Id];

            if (item != null) {
                Logger.Warn($"Nursery item({info.Id}/{info.Alias}) has existed");
                return;
            }

            ToolStripMenuItem newItem = new ToolStripMenuItem() {
                Text = info.Alias,
                Tag = info.Id,
                AutoToolTip = true,
                ToolTipText = $"[{info.Id}]{info.Pcs.StartInfo.FileName}",
                CheckOnClick = false,
                BackColor = Color.White,
                CheckState = CheckState.Unchecked,
            };
            newItem.Click += (s, e) => {
                if (s is null) return;
                ToolStripMenuItem i = s as ToolStripMenuItem;

                if (i.CheckState == CheckState.Checked) {
                    i.CheckState = CheckState.Unchecked;
                    ProcessManager.Stop((int)i.Tag);
                } else {
                    i.CheckState = CheckState.Checked;
                    ProcessManager.Launch((int)i.Tag);
                }
            };

            NurseryCollector[info.Id] = newItem;
            Logger.Info($"Added {info.Alias}({info.Id})");
        }

        private void UpdateNurseryItem(ProcessInfo info) {
            Logger.Trace($"Process state changed: {info}");
            ToolStripMenuItem item = NurseryCollector[info.Id];

            if (item is null) {
                Logger.Warn($"Nursery item({info.Id}/{info.Alias}) doesn't exist.");
                return;
            }
            item.Text = info.Alias;
            item.CheckState = info.Pcs.HasExited ? CheckState.Unchecked : CheckState.Checked;
        }

        private void RemoveNurseryItem(ProcessInfo info) {
            if (NurseryCollector.Remove(info.Id)) {
                Logger.Info($"Remove Nursery item successfully: {info.Id}/{info.Alias}");
            } else {
                Logger.Warn($"Remove Nursery item failed: {info.Id}/{info.Alias}");
            }
        }

        private void ExitMenu_Click(object sender, EventArgs e) {
            MessageBox.Show("exit");
            ActionManager.Exit();
        }

        private void TheNotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e) {
            MessageBox.Show("Show");
            ActionManager.Show();
        }

        private void NurseryAddFileItem_Click(object sender, EventArgs e) { MessageBox.Show("Comming soon."); }
    }

    /// <summary>
    /// This class defined a Dictionary to redirect `ToolStripItemCollection.index` to `ProcessInfo.Id`.
    /// With the help of this class, U can set/get `ProcessInfo` with `ProcessInfo.Id` easily.
    /// </summary>
    internal class ToolStripItemCollector {
        private readonly Dictionary<int, int> Map;
        private readonly ToolStripItemCollection Collection;

        internal ToolStripItemCollector(ToolStripItemCollection collection) {
            Logger.Trace("Create ToolStripItemCollector");
            Collection = collection;
            foreach (ToolStripItem o in Collection) {
                Logger.Debug(o.Name);
            }
            Map = new Dictionary<int, int>();
        }

        public ToolStripMenuItem this[int i] {
            get => Get(i);
            set => Add(i, value);
        }

        public int Add(int index, ToolStripMenuItem item) {
            Logger.Trace($"Add {item.Name}({index})");
            Map[index] = Collection.Add(item);
            return Map[index];
        }

        public ToolStripMenuItem Get(int index) => Map.ContainsKey(index) ? (ToolStripMenuItem)Collection[Map[index]] : null;

        public bool Remove(int index) {
            Collection.RemoveAt(Map[index]);
            // if ((Collection.Count >> 3) > Map.Count) Shuffle();
            return Map.Remove(index);
        }

        private void Shuffle() {
            // TODO check if shuffle is necessary
        }
    }

}
