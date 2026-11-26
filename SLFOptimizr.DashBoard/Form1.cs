using System.Diagnostics;
using TheArtOfDevHtmlRenderer.Adapters;

namespace SLFOptimizr.DashBoard
{
    public partial class Optimizr : Form
    {

        private readonly Stopwatch _fpsStopwatch = Stopwatch.StartNew();
        private readonly System.Windows.Forms.Timer _fpsTimer = new System.Windows.Forms.Timer();
        private long _frameCount = 0;

        public Optimizr()
        {
            InitializeComponent();

            _fpsTimer.Interval = 500;
            _fpsTimer.Tick += FpsTimer_Tick;
            _fpsTimer.Start();
        }

        public void OnMapUpdated()
        {
            Interlocked.Increment(ref _frameCount);
        }

        private void FpsTimer_Tick(object sender, EventArgs e)
        {
            double fps = GetFps();
            lblFps.Text = $"FPS: {fps:F1}";
        }

        private double GetFps()
        {
            double secondsElapsed = _fpsStopwatch.Elapsed.TotalSeconds;
            if (secondsElapsed <= 0) return 0;

            long count = Interlocked.Exchange(ref _frameCount, 0);
            _fpsStopwatch.Restart();
            return count / secondsElapsed;
        }
    }
}
