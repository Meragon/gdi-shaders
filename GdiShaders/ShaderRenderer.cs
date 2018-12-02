namespace GdiShaders
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public class ShaderRenderer : UserControl
    {
        public GdiShader shader;
        public bool fixedStep; // In case you wan't to smooth shader frame transition (does not affect performance).
        public float fixedStepValue = 0.01f;

        private float time;
        private static bool stop;

        public ShaderRenderer()
        {
            Size = new Size(128, 128);

            GdiShader.iResolution = new vec3(Width, Height);
            GdiShader.iChannelResolution = new vec3[4]
        {
            new vec3(Width, Height),
            new vec3(Width, Height),
            new vec3(Width, Height),
            new vec3(Width, Height),
        };
            GdiShader.iChannel0 = new samplerXX();
        }

        public void Start()
        {
            stop = false;

            GdiShader.iTime = 0;

            lock (shaderLock)
            {
                shader.Start();
            }

            System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(UpdateShader);
            task.Start();
        }
        public static void Stop()
        {
            stop = true;
        }
        public void UpdateShader()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            while (stop == false)
            {
                if (shader == null) continue;

                if (fixedStep == false)
                    GdiShader.iTime = watch.ElapsedMilliseconds / 1000f; // Minimum shader step.
                else
                    GdiShader.iTime += fixedStepValue;

                if (GdiShader.iResolution.x != Width || GdiShader.iResolution.y != Height)
                    lock (shaderLock)
                        shader.Start();

                GdiShader.iResolution.x = Width;
                GdiShader.iResolution.y = Height;

                if (shader != null)
                {
                    lock (shaderLock)
                        shader.Step();
                    if (InvokeRequired)
                        Invoke((MethodInvoker)Refresh, null);
                    else
                        Refresh();
                }
            }

            OnStop(this, EventArgs.Empty);
        }

        public event EventHandler OnStop = delegate { };

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (shader != null)
                lock (shaderLock)
                    shader.Draw(e);
        }

        private static readonly object shaderLock = new object();

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;    // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
    }
}
