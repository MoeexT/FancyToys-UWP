﻿using System;
using System.Drawing;
using System.Windows.Forms;

using FancyLibrary.Logger;


namespace FancyServer {

    public partial class NoForm: Form {
        public NoForm() {
            InitializeComponent();
            InitForm();
            ServerManager.InitPipe();
        }

        private void InitForm() {
            NurserySeparatorItem.Paint += (s, e) => {
                if (s is not ToolStripSeparator sep) return;
                e.Graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, sep.Width, sep.Height);
                e.Graphics.DrawLine(new Pen(Color.Black), 25, sep.Height >> 1, sep.Width - 4, sep.Height >> 1);
            };
        }

        public bool AddNurseryItem(string pathName) {
            /* 
             *  NurseryMenu.DropDownItems ---------> ToolStripItemCollection<ToolStripItem>
             *                                                                     ↑
             * ToolStripItem ---> ToolStripSeparator  -----------------------------|
             *          ↓                                                          |
             * ToolStripDropDownItem ----------------------------------------------|
             */
            bool hasThisProcess = false;

            foreach (ToolStripItem item in NurseryMenu.DropDownItems) {
                if (item.ToolTipText != null && item.ToolTipText.Equals(pathName)) { hasThisProcess = true; }
            }

            if (!hasThisProcess) {
                ToolStripMenuItem newItem = Actioner.GetItem(pathName);
                // BeginInvoke(new CrossThreadDelegate(() =>
                // { // TODO
                //     NurseryMenu.DropDownItems.Add(newItem);
                // }));
                LogClerk.Info($"Added {pathName}");
            }
            return !hasThisProcess;
        }

        public bool SetNurseryItemCheckState(string pathName, CheckState checkState) {
            foreach (ToolStripItem item in NurseryMenu.DropDownItems) {
                if (item.ToolTipText != null && item.ToolTipText.Equals(pathName)) {
                    // BeginInvoke(new CrossThreadDelegate(() =>
                    // {  // TODO
                    //     ((ToolStripMenuItem) item).CheckState = checkState;

                    // }));
                    LogClerk.Info($"Set {item.Text} {checkState}");
                    return true;
                }
            }
            return false;
        }

        public bool UpdateNurseryItem(string pathName, string processName) {
            foreach (ToolStripItem item in NurseryMenu.DropDownItems) {
                if (item.ToolTipText != null && item.ToolTipText.Equals(pathName)) {
                    // BeginInvoke(new CrossThreadDelegate(() =>
                    // {  // TODO
                    //     item.Text = processName;
                    // }));
                    LogClerk.Info($"Updated {pathName}");
                    return true;
                }
            }
            LogClerk.Warn($"Menu item not exit while updating it: {pathName}");
            return false;
        }

        public bool RemoveNurseryItem(string pathName) {
            foreach (ToolStripItem item in NurseryMenu.DropDownItems) {
                if (item.ToolTipText != null && item.ToolTipText.Equals(pathName)) {
                    // BeginInvoke(new CrossThreadDelegate(() =>
                    // {
                    //     NurseryMenu.DropDownItems.Remove(item);
                    // }));
                    LogClerk.Info($"Removed {pathName}");
                    return true;
                }
            }
            LogClerk.Warn($"Menu item not exit while removing it: {pathName}");
            return false;
        }

        private void ExitMenu_Click(object sender, EventArgs e) { }

        private void NurseryAddFileItem_Click(object sender, EventArgs e) { MessageBox.Show("Comming soon."); }
    }

}