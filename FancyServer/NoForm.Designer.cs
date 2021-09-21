
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NoForm));
            this.TheNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.TheMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.nurseryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.TheMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nurseryToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.TheMenuStrip.Name = "TheMenuStrip";
            this.TheMenuStrip.Size = new System.Drawing.Size(136, 52);
            // 
            // nurseryToolStripMenuItem
            // 
            this.nurseryToolStripMenuItem.Name = "nurseryToolStripMenuItem";
            this.nurseryToolStripMenuItem.Size = new System.Drawing.Size(135, 24);
            this.nurseryToolStripMenuItem.Text = "Nursery";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(135, 24);
            this.exitToolStripMenuItem.Text = "Exit";
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

        #endregion

        private System.Windows.Forms.NotifyIcon TheNotifyIcon;
        private System.Windows.Forms.ContextMenuStrip TheMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem nurseryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    }
}

