
namespace FancyServer
{
    partial class NoForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NoForm));
            this.TheNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.TheMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.NurseryMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.NurseryAddFileItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NurserySeparatorItem = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TheMenuStrip.SuspendLayout();
            this.SuspendLayout();

            // 
            // TheNotifyIcon
            // 
            this.TheNotifyIcon.ContextMenuStrip = this.TheMenuStrip;
            this.TheNotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("TheNotifyIcon.Icon")));
            this.TheNotifyIcon.Text = "FancyToys";
            this.TheNotifyIcon.Visible = true;

            // 
            // TheMenuStrip
            // 
            this.TheMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.TheMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.NurseryMenu, this.exitToolStripMenuItem });
            this.TheMenuStrip.Name = "TheMenuStrip";
            this.TheMenuStrip.Size = new System.Drawing.Size(153, 74);
            // 
            // NurseryMenu
            // 
            this.NurseryMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.NurseryAddFileItem, this.NurserySeparatorItem });
            this.NurseryMenu.Name = "NurseryMenu";
            this.NurseryMenu.Size = new System.Drawing.Size(152, 24);
            this.NurseryMenu.Text = "Nursery";

            // 
            // NurseryAddFileItem
            // 
            this.NurseryAddFileItem.Name = "NurseryAddFileItem";
            this.NurseryAddFileItem.Size = new System.Drawing.Size(118, 24);
            this.NurseryAddFileItem.Text = "Open";
            this.NurseryAddFileItem.Click += new System.EventHandler(this.NurseryAddFileItem_Click);

            // 
            // NurserySeparatorItem
            // 
            this.NurserySeparatorItem.Name = "NurserySeparatorItem";
            this.NurserySeparatorItem.Size = new System.Drawing.Size(115, 6);

            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 24);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitMenu_Click);

            // 
            // NoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "NoForm";
            this.ShowInTaskbar = false;
            this.Text = "FancyServer";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.TheMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.ToolStripSeparator NurserySeparatorItem;

        private System.Windows.Forms.ToolStripMenuItem NurseryAddFileItem;

        private System.Windows.Forms.ToolStripMenuItem NurseryMenu;

        #endregion

        private System.Windows.Forms.NotifyIcon TheNotifyIcon;
        private System.Windows.Forms.ContextMenuStrip TheMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem nurseryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    }
}

