
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
            this.ExitMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.TheMenuStrip.SuspendLayout();
            this.SuspendLayout();

            // 
            // TheNotifyIcon
            // 
            this.TheNotifyIcon.ContextMenuStrip = this.TheMenuStrip;
            this.TheNotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("TheNotifyIcon.Icon")));
            this.TheNotifyIcon.Text = "FancyToys";
            this.TheNotifyIcon.Visible = true;
            this.TheNotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TheNotifyIcon_MouseDoubleClick);

            // 
            // TheMenuStrip
            // 
            this.TheMenuStrip.BackColor = System.Drawing.Color.White;
            this.TheMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.TheMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.NurseryMenu, this.ExitMenu });
            this.TheMenuStrip.Name = "TheMenuStrip";
            this.TheMenuStrip.Size = new System.Drawing.Size(136, 52);

            // 
            // NurseryMenu
            // 
            this.NurseryMenu.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.NurseryMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.NurseryAddFileItem, this.NurserySeparatorItem });
            this.NurseryMenu.Name = "NurseryMenu";
            this.NurseryMenu.Size = new System.Drawing.Size(135, 24);
            this.NurseryMenu.Text = "Nursery";

            // 
            // NurseryAddFileItem
            // 
            this.NurseryAddFileItem.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.NurseryAddFileItem.Name = "NurseryAddFileItem";
            this.NurseryAddFileItem.Size = new System.Drawing.Size(118, 24);
            this.NurseryAddFileItem.Text = "Open";
            this.NurseryAddFileItem.Click += new System.EventHandler(this.NurseryAddFileItem_Click);

            // 
            // NurserySeparatorItem
            // 
            this.NurserySeparatorItem.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.NurserySeparatorItem.ForeColor = System.Drawing.Color.Black;
            this.NurserySeparatorItem.Name = "NurserySeparatorItem";
            this.NurserySeparatorItem.Size = new System.Drawing.Size(115, 6);

            // 
            // ExitMenu
            // 
            this.ExitMenu.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ExitMenu.Name = "ExitMenu";
            this.ExitMenu.Size = new System.Drawing.Size(135, 24);
            this.ExitMenu.Text = "Exit";
            this.ExitMenu.Click += new System.EventHandler(this.ExitMenu_Click);

            // 
            // NoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "NoForm";
            this.ShowInTaskbar = false;
            this.Text = "FancyServer";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.TheMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.ToolStripMenuItem ExitMenu;

        private System.Windows.Forms.ToolStripSeparator NurserySeparatorItem;

        private System.Windows.Forms.ToolStripMenuItem NurseryAddFileItem;

        private System.Windows.Forms.ToolStripMenuItem NurseryMenu;

        #endregion

        private System.Windows.Forms.NotifyIcon TheNotifyIcon;
        private System.Windows.Forms.ContextMenuStrip TheMenuStrip;
        // private System.Windows.Forms.ToolStripMenuItem nurseryToolStripMenuItem;
    }
}

