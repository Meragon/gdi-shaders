namespace GdiShaders
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    using GdiShaders.Core;

    public class ShaderRenderer : UserControl
    {
        public GdiShader shader;
        public bool fixedStep; // In case you wan't to smooth shader frame transition (does not affect performance).
        public float fixedStepValue = 0.01f;
        public float scale = 1f;

        private static readonly object shaderLock = new object();
        private static bool stop;
        private bool mouseButtonPressed;
        private float mouseX;
        private float mouseY;
        private float mouseStartX;
        private float mouseStartY;

        public ShaderRenderer()
        {
            Size = new Size(128, 128);

            var w = GetWidth();
            var h = GetHeight();

            GdiShader.iResolution = new vec3(w, h);
            GdiShader.iChannelResolution = new[]
            {
                new vec3(w, h),
                new vec3(w, h),
                new vec3(w, h),
                new vec3(w, h),
            };
        }

        public event EventHandler OnStop;

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;    // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        public static void Stop()
        {
            stop = true;
        }
        public void Start()
        {
            stop = false;

            GdiShader.iTime = 0;

            lock (shaderLock)
                shader.Start();

            new System.Threading.Tasks.Task(UpdateShader).Start();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            mouseButtonPressed = true;
            mouseX = e.X * scale;
            mouseY = e.Y * scale;
            mouseStartX = mouseX;
            mouseStartY = mouseY;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (mouseButtonPressed)
            {
                mouseX = e.X * scale;
                mouseY = e.Y * scale;
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            mouseButtonPressed = false;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var graphics = e.Graphics;

            // Looks like it's slightly faster to use low quality settings.
            graphics.InterpolationMode = InterpolationMode.Low;
            graphics.CompositingQuality = CompositingQuality.HighSpeed;
            graphics.SmoothingMode = SmoothingMode.HighSpeed;
            graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
            graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;

            if (shader != null)
                lock (shaderLock)
                    graphics.DrawImage(shader.bmp, 0, 0, Width, Height);
        }

        private int GetHeight()
        {
            return (int)(Height * scale);
        }
        private int GetWidth()
        {
            return (int)(Width * scale);
        }
        private void UpdateShader()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            while (stop == false)
            {
                if (shader == null) continue;

                if (fixedStep == false)
                    GdiShader.iTime = watch.ElapsedMilliseconds / 1000f; // Minimum shader step.
                else
                    GdiShader.iTime += fixedStepValue;

                var w = GetWidth();
                var h = GetHeight();

                if (GdiShader.iResolution.x != w || 
                    GdiShader.iResolution.y != h)
                {
                    GdiShader.iResolution = new vec3(w, h);

                    lock (shaderLock)
                        shader.Start();
                }

                if (shader != null)
                {
                    lock (shaderLock)
                    {
                        if (mouseButtonPressed)
                            GdiShader.iMouse = new vec4(mouseX, mouseY, mouseStartX, mouseStartY);

                        shader.Step();
                    }

                    if (InvokeRequired)
                        Invoke((MethodInvoker)Refresh, null);
                    else
                        Refresh();
                }
            }

            OnStop?.Invoke(this, EventArgs.Empty);
        }
    }
}
