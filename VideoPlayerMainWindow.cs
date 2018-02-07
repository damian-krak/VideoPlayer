using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VideoPlayer40
{
    public partial class VideoPlayerMainWindow : Form
    {
        private readonly string url;

        public VideoPlayerMainWindow(string url)
        {
            this.url = url;
            InitializeComponent();
        }

        private void VideoPlayerMainWindow_Load(object sender, EventArgs e)
        {
            dsPlayer.OpenURL(url);
        }
    }
}
