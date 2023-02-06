
namespace LS_ERP.Forms
{
    partial class AppSplashScreen
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppSplashScreen));
            this.labelStatus = new DevExpress.XtraEditors.LabelControl();
            this.peImage = new DevExpress.XtraEditors.PictureEdit();
            this.marqueeProgressBarControl1 = new DevExpress.XtraEditors.MarqueeProgressBarControl();
            this.lblCopyright = new DevExpress.XtraEditors.LabelControl();
            this.labelCommand = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.peImage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.marqueeProgressBarControl1.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelStatus
            // 
            this.labelStatus.IndentBetweenImageAndText = 1;
            this.labelStatus.Location = new System.Drawing.Point(21, 159);
            this.labelStatus.Margin = new System.Windows.Forms.Padding(3, 3, 3, 1);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(50, 13);
            this.labelStatus.TabIndex = 7;
            this.labelStatus.Text = "Starting...";
            // 
            // peImage
            // 
            this.peImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.peImage.EditValue = ((object)(resources.GetObject("peImage.EditValue")));
            this.peImage.Location = new System.Drawing.Point(1, 1);
            this.peImage.Name = "peImage";
            this.peImage.Properties.AllowFocused = false;
            this.peImage.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.peImage.Properties.Appearance.Options.UseBackColor = true;
            this.peImage.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.peImage.Properties.ShowMenu = false;
            this.peImage.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
            this.peImage.Properties.SvgImageColorizationMode = DevExpress.Utils.SvgImageColorizationMode.None;
            this.peImage.Size = new System.Drawing.Size(448, 318);
            this.peImage.TabIndex = 1;
            // 
            // marqueeProgressBarControl1
            // 
            this.marqueeProgressBarControl1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.marqueeProgressBarControl1.EditValue = "0";
            this.marqueeProgressBarControl1.Location = new System.Drawing.Point(53, 242);
            this.marqueeProgressBarControl1.Name = "marqueeProgressBarControl1";
            this.marqueeProgressBarControl1.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.marqueeProgressBarControl1.Properties.Appearance.BackColor2 = System.Drawing.Color.Transparent;
            this.marqueeProgressBarControl1.Properties.Appearance.BorderColor = System.Drawing.Color.Transparent;
            this.marqueeProgressBarControl1.Properties.Appearance.ForeColor = System.Drawing.Color.Transparent;
            this.marqueeProgressBarControl1.Properties.AppearanceDisabled.BorderColor = System.Drawing.Color.Transparent;
            this.marqueeProgressBarControl1.Properties.AppearanceReadOnly.BorderColor = System.Drawing.Color.Transparent;
            this.marqueeProgressBarControl1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.marqueeProgressBarControl1.Properties.EndColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.marqueeProgressBarControl1.Properties.LookAndFeel.SkinName = "Visual Studio 2013 Blue";
            this.marqueeProgressBarControl1.Properties.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
            this.marqueeProgressBarControl1.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
            this.marqueeProgressBarControl1.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
            this.marqueeProgressBarControl1.Properties.StartColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.marqueeProgressBarControl1.Size = new System.Drawing.Size(350, 16);
            this.marqueeProgressBarControl1.TabIndex = 12;
            // 
            // lblCopyright
            // 
            this.lblCopyright.Appearance.BackColor = System.Drawing.Color.Black;
            this.lblCopyright.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.lblCopyright.Appearance.ForeColor = System.Drawing.Color.White;
            this.lblCopyright.Appearance.Options.UseBackColor = true;
            this.lblCopyright.Appearance.Options.UseFont = true;
            this.lblCopyright.Appearance.Options.UseForeColor = true;
            this.lblCopyright.LineColor = System.Drawing.Color.Transparent;
            this.lblCopyright.Location = new System.Drawing.Point(53, 264);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Padding = new System.Windows.Forms.Padding(2);
            this.lblCopyright.Size = new System.Drawing.Size(59, 18);
            this.lblCopyright.TabIndex = 13;
            this.lblCopyright.Text = "Copyright";
            // 
            // labelCommand
            // 
            this.labelCommand.Appearance.BackColor = System.Drawing.Color.Black;
            this.labelCommand.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.labelCommand.Appearance.ForeColor = System.Drawing.Color.White;
            this.labelCommand.Appearance.Options.UseBackColor = true;
            this.labelCommand.Appearance.Options.UseFont = true;
            this.labelCommand.Appearance.Options.UseForeColor = true;
            this.labelCommand.LineColor = System.Drawing.Color.Transparent;
            this.labelCommand.Location = new System.Drawing.Point(53, 218);
            this.labelCommand.Name = "labelCommand";
            this.labelCommand.Padding = new System.Windows.Forms.Padding(2);
            this.labelCommand.Size = new System.Drawing.Size(59, 18);
            this.labelCommand.TabIndex = 14;
            this.labelCommand.Text = "Copyright";
            // 
            // AppSplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 320);
            this.Controls.Add(this.labelCommand);
            this.Controls.Add(this.lblCopyright);
            this.Controls.Add(this.marqueeProgressBarControl1);
            this.Controls.Add(this.peImage);
            this.Controls.Add(this.labelStatus);
            this.Name = "AppSplashScreen";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "AppSplashScreen";
            ((System.ComponentModel.ISupportInitialize)(this.peImage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.marqueeProgressBarControl1.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraEditors.LabelControl labelStatus;
        private DevExpress.XtraEditors.PictureEdit peImage;
        private DevExpress.XtraEditors.MarqueeProgressBarControl marqueeProgressBarControl1;
        private DevExpress.XtraEditors.LabelControl lblCopyright;
        private DevExpress.XtraEditors.LabelControl labelCommand;
    }
}
