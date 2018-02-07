namespace VideoPlayer40
{
    partial class VideoPlayerMainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VideoPlayerMainWindow));
            this.dsPlayer = new VideoPlayer40.DSPlayer();
            this.SuspendLayout();
            // 
            // dsPlayer
            // 
            this.dsPlayer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dsPlayer.FullScreen = false;
            this.dsPlayer.Location = new System.Drawing.Point(0, 0);
            this.dsPlayer.Margin = new System.Windows.Forms.Padding(0);
            this.dsPlayer.Mute = true;
            this.dsPlayer.Name = "dsPlayer";
            this.dsPlayer.Size = new System.Drawing.Size(304, 201);
            this.dsPlayer.TabIndex = 0;
            // 
            // VideoPlayerMainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 201);
            this.Controls.Add(this.dsPlayer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "VideoPlayerMainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Video player";
            this.Load += new System.EventHandler(this.VideoPlayerMainWindow_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private DSPlayer dsPlayer;
    }
}