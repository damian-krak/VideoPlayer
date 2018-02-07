using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DirectShowLib;
using DirectShowLib.Utils;
using EVRPresenter;
using MediaFoundation.EVR;
using VideoPlayer40.LAVInterfaces;

namespace VideoPlayer40
{

    public class DSPlayer : UserControl
    {
        private enum PlayState
        {
            Stopped,
            Paused,
            Running,
            Init
        }

        private const int WMGraphNotify = 0x0400 + 13;
        private const int VolumeFull = 0;
        private const int VolumeSilence = -10000;

        private IGraphBuilder graphBuilder = null;
        private IMediaControl mediaControl = null;
        private IMediaEventEx mediaEventEx = null;

        private IBasicAudio basicAudio = null;
        private IBasicVideo basicVideo = null;

        private bool isAudioOnly = false;
        private bool isFullScreen = false;
        private int currentVolume = VolumeFull;
        private PlayState currentState = PlayState.Stopped;

        private Panel panel;

        public DSPlayer()
        {
            InitializeComponent();
        }

        private void Play(string url)
        {
            BuildAndRunGraph(url);
        }

        public void Stop()
        {
            CloseClip();
        }


        private void FillWindow()
        {
            if (pDisplay == null) return;

            IMFVideoDisplayControl mfVideoDisplayControl = pDisplay;
            Rectangle r = panel.ClientRectangle;
            MediaFoundation.Misc.MFRect rc = new MediaFoundation.Misc.MFRect(r.Left, r.Top, r.Right, r.Bottom);

            mfVideoDisplayControl.SetVideoPosition(null, rc);
        }

        IMFVideoDisplayControl pDisplay;

        private void InitializeEVR(IBaseFilter pEVR, int dwStreams, out IMFVideoDisplayControl ppDisplay)
        {
            IMFVideoRenderer pRenderer;

            IEVRFilterConfig pConfig;
            IMFVideoPresenter pPresenter;

            pPresenter = new EVRCustomPresenter();

            pRenderer = (IMFVideoRenderer)pEVR;

            pRenderer.InitializeRenderer(null, pPresenter);

            object o;
            MediaFoundation.IMFGetService pGetService;
            pGetService = (MediaFoundation.IMFGetService)pEVR;
            pGetService.GetService(MediaFoundation.MFServices.MR_VIDEO_RENDER_SERVICE, typeof(IMFVideoDisplayControl).GUID, out o);

            try
            {
                pDisplay = (IMFVideoDisplayControl)o;
            }
            catch
            {
                Marshal.ReleaseComObject(o);
                throw;
            }

            pDisplay.SetVideoWindow(panel.Handle);

            if (dwStreams > 1)
            {
                pConfig = (IEVRFilterConfig)pEVR;
                pConfig.SetNumberOfStreams(dwStreams);
            }

            Rectangle r = ClientRectangle;
            MediaFoundation.Misc.MFRect rc = new MediaFoundation.Misc.MFRect(r.Left, r.Top, r.Right, r.Bottom);

            pDisplay.SetVideoPosition(null, rc);

            ppDisplay = pDisplay;
        }

        private bool IsVideoH264(IBaseFilter lavSourceFilter)
        {
            IPin videoPin;
            var hr = lavSourceFilter.FindPin("Video", out videoPin);
            DsError.ThrowExceptionForHR(hr);
            var mediaType = new AMMediaType[1];
            IEnumMediaTypes mediaTypes;
            videoPin.EnumMediaTypes(out mediaTypes);
            mediaTypes.Next(1, mediaType, (IntPtr)0);
            return mediaType[0].subType == MediaSubType.H264;
        }

        private bool IsAudioPinPresent(IBaseFilter lavSourceFilter)
        {
            IPin audioPin = null;
            var hr = lavSourceFilter.FindPin("Audio", out audioPin);
            return audioPin != null;
        }

        private void BuildAndRunGraph(string url)
        {
            graphBuilder = (IGraphBuilder)new FilterGraph();

            var sourceBaseFilter = (IBaseFilter)new LAVSplitterSource();

            ILAVSplitterSourceSettings sourceSettings = (ILAVSplitterSourceSettings)sourceBaseFilter;
            var hr = sourceSettings.SetNetworkStreamAnalysisDuration(5000);
            DsError.ThrowExceptionForHR(hr);
            
            hr = graphBuilder.AddFilter(sourceBaseFilter, "Lav splitter source");
            DsError.ThrowExceptionForHR(hr);

            hr = ((IFileSourceFilter)sourceBaseFilter).Load(url, new AMMediaType());
            DsError.ThrowExceptionForHR(hr);

            IBaseFilter decoderBaseFilter;
            string decoderOutputPinName;
            if (IsVideoH264(sourceBaseFilter))
            {
                //microsoft decoder
                decoderBaseFilter = FilterGraphTools.AddFilterFromClsid(graphBuilder,
                new Guid("{212690FB-83E5-4526-8FD7-74478B7939CD}"), "decoder");
                FilterGraphTools.ConnectFilters(graphBuilder, sourceBaseFilter, "Video", decoderBaseFilter, "Video Input",
                    useIntelligentConnect: true);

                decoderOutputPinName = "Video Output 1";
            }
            else
            {
                //lav
                decoderBaseFilter = FilterGraphTools.AddFilterFromClsid(graphBuilder,
                new Guid("{EE30215D-164F-4A92-A4EB-9D4C13390F9F}"), "decoder");
                FilterGraphTools.ConnectFilters(graphBuilder, sourceBaseFilter, "Video", decoderBaseFilter, "Input",
                    useIntelligentConnect: true);
                decoderOutputPinName = "Output";
            }
            
            IBaseFilter pEVR = (IBaseFilter)new EnhancedVideoRenderer();
            hr = graphBuilder.AddFilter(pEVR, "Enhanced Video Renderer");
            DsError.ThrowExceptionForHR(hr);

            FilterGraphTools.ConnectFilters(graphBuilder, decoderBaseFilter, decoderOutputPinName, pEVR, "EVR Input0",
                useIntelligentConnect: true);

            IMFVideoDisplayControl m_pDisplay;
            InitializeEVR(pEVR, 1, out m_pDisplay);

            //render audio from audio splitter
            if (IsAudioPinPresent(sourceBaseFilter))
            {
                var audioDecoderBaseFilter = FilterGraphTools.AddFilterFromClsid(graphBuilder,
               new Guid("{E8E73B6B-4CB3-44A4-BE99-4F7BCB96E491}"), "audio decoder");
                FilterGraphTools.ConnectFilters(graphBuilder, sourceBaseFilter, "Audio", audioDecoderBaseFilter, "Input",
                    useIntelligentConnect: true);
                FilterGraphTools.RenderPin(graphBuilder, audioDecoderBaseFilter, "Output");
            }

            
            mediaControl = (IMediaControl)graphBuilder;
            mediaEventEx = (IMediaEventEx)graphBuilder;


            basicVideo = graphBuilder as IBasicVideo;
            basicAudio = graphBuilder as IBasicAudio;

            // Have the graph signal event via window callbacks for performance
            hr = mediaEventEx.SetNotifyWindow(Handle, WMGraphNotify, IntPtr.Zero);
            DsError.ThrowExceptionForHR(hr);

            isFullScreen = false;
            
            Focus();

            hr = mediaControl.Run();
            DsError.ThrowExceptionForHR(hr);

            currentState = PlayState.Running;
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WMGraphNotify:
                    {
                        HandleGraphEvent();
                        break;
                    }
            }

            base.WndProc(ref m);
        }

        private void HandleGraphEvent()
        {
            EventCode evCode;
            IntPtr evParam1, evParam2;

            if (mediaEventEx == null)
                return;

            while (mediaEventEx.GetEvent(out evCode, out evParam1, out evParam2, 0) == 0)
            {
                mediaEventEx.FreeEventParams(evCode, evParam1, evParam2);

                if (evCode == EventCode.Complete)
                {
                    mediaControl.Stop();
                }
            }
        }

        private void CloseClip()
        {
            if (mediaControl != null)
                mediaControl.Stop();

            currentState = PlayState.Stopped;
            isFullScreen = false;

            CloseInterfaces();

            currentState = PlayState.Init;
        }

        private void CloseInterfaces()
        {
            try
            {
                lock (this)
                {
                    if (mediaEventEx != null)
                    {
                        var hr = mediaEventEx.SetNotifyWindow(IntPtr.Zero, 0, IntPtr.Zero);
                        DsError.ThrowExceptionForHR(hr);
                    }
                    if (mediaEventEx != null)
                        mediaEventEx = null;
                    if (mediaControl != null)
                        mediaControl = null;
                    if (basicAudio != null)
                        basicAudio = null;
                    if (basicVideo != null)
                        basicVideo = null;
                   
                    if (graphBuilder != null)
                        Marshal.ReleaseComObject(graphBuilder); graphBuilder = null;

                    GC.Collect();
                }
            }
            catch
            {
            }
        }

        private Form fullScreenForm;
        private void ToggleFullScreen()
        {
            if (!isFullScreen)
            {
                if (fullScreenForm == null)
                {
                    fullScreenForm = new Form();
                    fullScreenForm.ControlBox = false;
                    fullScreenForm.FormBorderStyle = FormBorderStyle.None;
                }

                fullScreenForm.WindowState = FormWindowState.Maximized;

                fullScreenForm.Controls.Add(panel);
                fullScreenForm.Show();
                isFullScreen = true;
            }
            else
            {
                Controls.Add(fullScreenForm.Controls[0]);
                fullScreenForm.Hide();

                isFullScreen = false;
            }

            FillWindow();
        }


        private bool IsMuted()
        {
            int volume;
            if (basicAudio == null) return true;

            int hr = basicAudio.get_Volume(out volume);
            if (hr < 0)
            {
                //error - no sound
                return true;
            }

            return volume == VolumeSilence;
        }

        private int ToggleMute()
        {
            var hr = 0;

            if ((graphBuilder == null) || (basicAudio == null))
                return 0;

            hr = basicAudio.get_Volume(out currentVolume);
            if (hr == -1) //E_NOTIMPL
            {
                return 0;
            }
            else if (hr < 0)
            {
                return hr;
            }

            if (currentVolume == VolumeFull)
                currentVolume = VolumeSilence;
            else
                currentVolume = VolumeFull;

            hr = basicAudio.put_Volume(currentVolume);

            return hr;
        }

        private void MoveVideoWindow()
        {
            FillWindow();
        }

        private void InitializeComponent()
        {
            this.panel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.BackColor = System.Drawing.Color.Black;
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Margin = new System.Windows.Forms.Padding(0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(150, 150);
            this.panel.TabIndex = 0;
            this.panel.DoubleClick += new System.EventHandler(this.panel_DoubleClick);
            // 
            // DSPlayer
            // 
            this.Controls.Add(this.panel);
            this.Name = "DSPlayer";
            this.DoubleClick += new System.EventHandler(this.DSPlayer_DoubleClick);
            this.Move += new System.EventHandler(this.DSPlayer_Move);
            this.Resize += new System.EventHandler(this.DSPlayer_Resize);
            this.ResumeLayout(false);

        }

        private void DSPlayer_Resize(object sender, EventArgs e)
        {
            if (!isAudioOnly)
                MoveVideoWindow();
        }

        private void DSPlayer_Move(object sender, EventArgs e)
        {
            if (!isAudioOnly)
                MoveVideoWindow();
        }


        public bool FullScreen
        {
            get { return isFullScreen; }
            set
            {
                if (value == true && !isFullScreen || (value == false && isFullScreen))
                    ToggleFullScreen();
            }
        }


        public bool Mute
        {
            get { return IsMuted(); }
            set
            {
                if (value == true && !IsMuted() || (value == false && IsMuted()))
                    ToggleMute();
            }
        }

        public int OpenURL(string url)
        {
            Play(url);
            return 0;
        }

        private void DSPlayer_DoubleClick(object sender, System.EventArgs e)
        {

        }

        private void panel_DoubleClick(object sender, System.EventArgs e)
        {
            ToggleFullScreen();
        }
    }
}
