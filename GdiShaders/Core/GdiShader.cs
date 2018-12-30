namespace GdiShaders.Core
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;

    public abstract class GdiShader
    {
        public static vec3      iResolution;        // viewport resolution (in pixels)
        public static float     iTime;              // shader playback time (in seconds)
        public static float     iTimeDelta;         // render time (in seconds)
        public static int       iFrame;             // shader playback frame
        public static float[]   iChannelTime;       // channel playback time (in seconds)
        public static vec3[]    iChannelResolution; // channel resolution (in pixels)
        public static vec4      iMouse;             // mouse pixel coords. xy: current (if MLB down), zw: click
        public static samplerXX iChannel0;          // input channel. XX = 2D/Cube
        public static samplerXX iChannel1;
        public static samplerXX iChannel2;
        public static samplerXX iChannel3;
        public static vec4      iDate;              // (year, month, day, time in seconds)

        internal Bitmap bmp;
        internal int    bmpWidth;
        internal int    bmpHeight;

        private byte[]      bmpColors;
        private Rectangle   bmpRect;
        private PixelFormat bmpFormat;

        public static float abs(float v)
        {
            return Math.Abs(v);
        }
        public static vec2 abs(vec2 v)
        {
            return new vec2(abs(v.x), abs(v.y));
        }
        public static vec3 abs(vec3 v)
        {
            return new vec3(abs(v.x), abs(v.y), abs(v.z));
        }
        public static vec4 abs(vec4 v)
        {
            return new vec4(abs(v.x), abs(v.y), abs(v.z), abs(v.w));
        }
        public static float acos(float v)
        {
            return (float)Math.Acos(v);
        }
        public static float atan(float x, float y)
        {
            return (float)Math.Atan2(x, y);
        }
        public static float clamp(float x, float min, float max)
        {
            if (x < min) x = min;
            if (x > max) x = max;
            return x;
        }
        public static vec3 clamp(vec3 x, float min, float max)
        {
            return new vec3(
                clamp(x.x, min, max), 
                clamp(x.y, min, max), 
                clamp(x.z, min, max));
        }
        public static vec4 clamp(vec4 x, float min, float max)
        {
            return new vec4(
                clamp(x.x, min, max),
                clamp(x.y, min, max),
                clamp(x.z, min, max),
                clamp(x.w, min, max));
        }
        public static float cos(float v)
        {
            return (float)Math.Cos(v);
        }
        public static vec2 cos(vec2 v)
        {
            return new vec2(cos(v.x), cos(v.y));
        }
        public static vec3 cos(vec3 v)
        {
            return new vec3(
                cos(v.x),
                cos(v.y),
                cos(v.z));
        }
        public static vec4 cos(vec4 v)
        {
            return new vec4(
                cos(v.x),
                cos(v.y),
                cos(v.z),
                cos(v.w));
        }
        public static vec3 cross(vec3 x, vec3 y)
        {
            return new vec3(
                x.y * y.z - y.y * x.z,
                x.z * y.x - y.z * x.x,
                x.x * y.y - y.x * x.y);
        }
        public static float dot(float x, float y)
        {
            return x * y;
        }
        public static float dot(vec2 x, vec2 y)
        {
            return dot(x.x, y.x) + dot(x.y, y.y);
        }
        public static float dot(vec3 x, vec3 y)
        {
            return dot(x.x, y.x) + dot(x.y, y.y) + dot(x.z, y.z);
        }
        public static float dot(vec4 x, vec4 y)
        {
            return dot(x.x, y.x) + dot(x.y, y.y) + dot(x.z, y.z) + dot(x.w, y.w);
        }
        public static float exp(float x)
        {
            return (float)Math.Exp(x);
        }
        public static float exp2(float x)
        {
            return pow(2, x);
        }
        public static float floor(float v)
        {
            return (float)Math.Floor(v);
        }
        public static vec2 floor(vec2 v)
        {
            return new vec2(
                floor(v.x),
                floor(v.y));
        }
        public static vec3 floor(vec3 v)
        {
            return new vec3(
                floor(v.x),
                floor(v.y),
                floor(v.z));
        }
        public static float fract(float v)
        {
            return v - floor(v);
        }
        public static vec2 fract(vec2 v)
        {
            return new vec2(
                fract(v.x),
                fract(v.y));
        }
        public static vec3 fract(vec3 v)
        {
            return new vec3(
                fract(v.x),
                fract(v.y),
                fract(v.z));
        }
        public static vec4 fract(vec4 v)
        {
            return new vec4(
                fract(v.x),
                fract(v.y),
                fract(v.z),
                fract(v.w));
        }
        public static float greaterThan(float x, float y)
        {
            return x > y ? 1 : 0;
        }
        public static vec2 greaterThan(vec2 x, vec2 y)
        {
            return new vec2(
                greaterThan(x.x, y.x),
                greaterThan(x.y, y.y));
        }
        public static float length(vec2 v)
        {
            return sqrt(v.x * v.x + v.y * v.y);
        }
        public static float length(vec3 v)
        {
            return sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
        }
        public static float log(float x)
        {
            return (float)Math.Log(x);
        }
        public static float min(float x, float y)
        {
            return x < y ? x : y;
        }
        public static vec4 min(vec4 x, vec4 y)
        {
            return new vec4(
                min(x.x, y.x),
                min(x.y, y.y),
                min(x.z, y.z),
                min(x.w, y.w));
        }
        public static float mix(float x, float y, float a)
        {
            return x * (1 - a) + y * a;
        }
        public static vec2 mix(vec2 x, vec2 y, float a)
        {
            return new vec2(
                mix(x.x, y.x, a),
                mix(x.y, y.y, a));
        }
        public static vec3 mix(vec3 x, vec3 y, float a)
        {
            return new vec3(
                mix(x.x, y.x, a),
                mix(x.y, y.y, a),
                mix(x.z, y.z, a));
        }
        public static vec2 mix(vec2 x, vec2 y, vec2 a)
        {
            return new vec2(
                mix(x.x, y.x, a.x),
                mix(x.y, y.y, a.y));
        }
        public static vec3 mix(vec3 x, vec3 y, vec3 a)
        {
            return new vec3(
                mix(x.x, y.x, a.x),
                mix(x.y, y.y, a.y),
                mix(x.z, y.z, a.z));
        }
        public static vec4 mix(vec4 x, vec4 y, float a)
        {
            return new vec4(
                mix(x.x, y.x, a),
                mix(x.y, y.y, a),
                mix(x.z, y.z, a),
                mix(x.w, y.w, a));
        }
        public static float max(float x, float y)
        {
            return x > y ? x : y;
        }
        public static vec2 max(vec2 x, float y)
        {
            return new vec2(
                max(x.x, y),
                max(x.y, y));
        }
        public static vec3 max(vec3 x, float y)
        {
            return new vec3(
                max(x.x, y),
                max(x.y, y),
                max(x.z, y));
        }
        public static vec4 max(vec4 x, float y)
        {
            return new vec4(
                max(x.x, y),
                max(x.y, y),
                max(x.z, y),
                max(x.w, y));
        }
        public static vec2 max(vec2 x, vec2 y)
        {
            return new vec2(
                max(x.x, y.x),
                max(x.y, y.y));
        }
        public static vec3 max(vec3 x, vec3 y)
        {
            return new vec3(
                max(x.x, y.x),
                max(x.y, y.y),
                max(x.z, y.z));
        }
        public static float mod(float x, float y)
        {
            return x - y * floor(x / y);
        }
        public static vec2 mod(vec2 x, float y)
        {
            return new vec2(
                mod(x.x, y),
                mod(x.y, y));
        }
        public static vec2 mod(vec2 x, vec2 y)
        {
            return new vec2(
                mod(x.x, y.x),
                mod(x.y, y.y));
        }
        public static vec3 mod(vec3 x, float y)
        {
            return new vec3(
                mod(x.x, y),
                mod(x.y, y),
                mod(x.z, y));
        }
        public static vec3 mod(vec3 x, vec3 y)
        {
            return new vec3(
                mod(x.x, y.x),
                mod(x.y, y.y),
                mod(x.z, y.z));
        }
        public static vec4 mod(vec4 x, float y)
        {
            return new vec4(
                mod(x.x, y),
                mod(x.y, y),
                mod(x.z, y),
                mod(x.w, y));
        }
        public static float normalize(float v)
        {
            return 1;
        }
        public static vec2 normalize(vec2 v)
        {
            var l = sqrt(v.x * v.x + v.y * v.y);
            if (l > 9.99999974737875E-06)
                return new vec2(v.x / l, v.y / l);
            return new vec2();
        }
        public static vec3 normalize(vec3 v)
        {
            var l = sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
            if (l > 9.99999974737875E-06)
                return new vec3(v.x / l, v.y / l, v.z / l);
            return new vec3();
        }
        public static float pow(float x, float y)
        {
            return (float)Math.Pow(x, y);
        }
        public static vec3 pow(vec3 x, vec3 y)
        {
            return new vec3(
                pow(x.x, y.x),
                pow(x.y, y.y),
                pow(x.z, y.z));
        }
        public static float radians(float degrees)
        {
            return 3.14159265358979f * degrees / 180f;
        }
        public static float reflect(float I, float N)
        {
            return I - 2f * dot(N, I) * N;
        }
        public static vec3 reflect(vec3 I, vec3 N)
        {
            return new vec3(
                reflect(I.x, N.x),
                reflect(I.y, N.y),
                reflect(I.z, N.z));
        }
        public static float sign(float x)
        {
            if (x < 0f)
                return -1;
            if (x == 0)
                return 0;
            return 1;
        }
        public static vec3 sign(vec3 x)
        {
            return new vec3(
                sign(x.x),
                sign(x.y),
                sign(x.z));
        }
        public static vec4 sign(vec4 x)
        {
            return new vec4(
                sign(x.x),
                sign(x.y),
                sign(x.z),
                sign(x.w));
        }
        public static float sin(float v)
        {
            return (float)Math.Sin(v);
        }
        public static vec2 sin(vec2 v)
        {
            return new vec2(
                sin(v.x),
                sin(v.y));
        }
        public static vec3 sin(vec3 v)
        {
            return new vec3(
                sin(v.x),
                sin(v.y),
                sin(v.z));
        }
        public static vec4 sin(vec4 v)
        {
            return new vec4(
                sin(v.x),
                sin(v.y),
                sin(v.z),
                sin(v.w));
        }
        public static float sqrt(float x)
        {
            if (x == 0f)
                return 0f;

            // https://en.wikipedia.org/wiki/Fast_inverse_square_root
            FloatIntUnion u;
            u.i = 0;
            u.f = x;
            u.i -= 1 << 23; /* Subtract 2^m. */
            u.i >>= 1; /* Divide by 2. */
            u.i += 1 << 29; /* Add ((b + 1) / 2) * 2^m. */
            return u.f;
        }
        public static vec3 sqrt(vec3 x)
        {
            return new vec3(
                sqrt(x.x),
                sqrt(x.y),
                sqrt(x.z));
        }
        public static float smoothstep(float edge0, float edge1, float x)
        {
            var t = clamp((x - edge0) / (edge1 - edge0), 0.0f, 1.0f);
            return t * t * (3.0f - 2.0f * t);
        }
        public static vec2 smoothstep(vec2 edge0, vec2 edge1, vec2 x)
        {
            return new vec2(
                smoothstep(edge0.x, edge1.x, x.x),
                smoothstep(edge0.y, edge1.y, x.y));
        }
        public static vec2 smoothstep(float edge0, float edge1, vec2 v)
        {
            return new vec2(
                smoothstep(edge0, edge1, v.x),
                smoothstep(edge0, edge1, v.y));
        }
        public static vec3 smoothstep(float edge0, float edge1, vec3 v)
        {
            return new vec3(
                smoothstep(edge0, edge1, v.x),
                smoothstep(edge0, edge1, v.y),
                smoothstep(edge0, edge1, v.z));
        }
        public static vec4 smoothstep(float edge0, float edge1, vec4 v)
        {
            return new vec4(
                smoothstep(edge0, edge1, v.x),
                smoothstep(edge0, edge1, v.y),
                smoothstep(edge0, edge1, v.z),
                smoothstep(edge0, edge1, v.w));
        }
        public static float step(float edge, float x)
        {
            if (x < edge) return 0;
            return 1;
        }
        public static float step(float edge, vec4 x)
        {
            if (x.x < edge) return 0;
            if (x.y < edge) return 0;
            if (x.z < edge) return 0;
            if (x.w < edge) return 0;

            return 1;
        }
        public static vec3 step(vec3 edge, vec3 x)
        {
            return new vec3(
                step(edge.x, x.x),
                step(edge.y, x.y),
                step(edge.z, x.z));
        }
        public static float tan(float x)
        {
            return (float)Math.Tan(x);
        }

        public static vec4 texture(samplerXX sampler, vec2 coords)
        {
            if (sampler == null || sampler.bmp == null)
                return new vec4();

            var x = coords.x * sampler.bmp.Width;
            var y = coords.y * sampler.bmp.Height;

            if (float.IsNaN(x)) x = 0;
            if (float.IsNaN(y)) y = 0;

            x = clamp(x, 0, sampler.bmp.Width - 1);
            y = clamp(y, 0, sampler.bmp.Height - 1);

            var uc = sampler.bmp.GetPixel((int)x, sampler.bmp.Height - (int)y - 1);

            return new vec4(uc.R / 255f, uc.G / 255f, uc.B / 255f, uc.A / 255f);
        }
        public static vec4 textureLod(samplerXX sample, vec2 coords, float value)
        {
            // Hm?
            return texture(sample, coords);
        }

        public virtual void Start()
        {
            bmp = new Bitmap((int)iResolution.x, (int)iResolution.y);
            bmpWidth = bmp.Width;
            bmpHeight = bmp.Height;
            bmpColors = new byte[bmpWidth * bmpHeight * 4];
            bmpRect = new Rectangle(0, 0, bmpWidth, bmpHeight);
            bmpFormat = bmp.PixelFormat;
        
            iMouse = new vec4();
        }
        public void Step()
        {
            for (int y = 0; y < bmpHeight; y++)
            for (int x = 0; x < bmpWidth; x++)
            {
                vec4 fragColor;

                mainImage(out fragColor, new vec2(x, y));

                var r = fragColor.r * 255;
                var g = fragColor.g * 255;
                var b = fragColor.b * 255;
                var a = fragColor.a * 255;

                if (r < 0) r = 0; else if (r > 255) r = 255;
                if (g < 0) g = 0; else if (g > 255) g = 255;
                if (b < 0) b = 0; else if (b > 255) b = 255;
                if (a < 0) a = 0; else if (a > 255) a = 255;

                var colorIndex = (x + (bmpHeight - y - 1) * bmpWidth) * 4;
                bmpColors[colorIndex] = (byte)b;
                bmpColors[colorIndex + 1] = (byte)g;
                bmpColors[colorIndex + 2] = (byte)r;
                bmpColors[colorIndex + 3] = (byte)a;
            }

            var bmpData = bmp.LockBits(bmpRect, ImageLockMode.WriteOnly, bmpFormat);

            Marshal.Copy(bmpColors, 0, bmpData.Scan0, bmpColors.Length);

            bmp.UnlockBits(bmpData);
        }

        public abstract void mainImage(out vec4 fragColor, vec2 fragCoord);

        public override string ToString()
        {
            return "GdiShader";
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct FloatIntUnion
        {
            [FieldOffset(0)]
            public float f;

            [FieldOffset(0)]
            public int i;
        }
    }
}