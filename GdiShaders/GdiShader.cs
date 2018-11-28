using System;
using System.Drawing;

// GLSL
public class GdiShader
{
    public static vec3 iResolution;           // viewport resolution (in pixels)
    public static float iTime;           // shader playback time (in seconds)
    public static float iTimeDelta;            // render time (in seconds)
    public static int iFrame;                // shader playback frame
    public static float[] iChannelTime;       // channel playback time (in seconds)
    public static vec3[] iChannelResolution; // channel resolution (in pixels)
    public static vec4 iMouse;                // mouse pixel coords. xy: current (if MLB down), zw: click
    public static samplerXX iChannel0;          // input channel. XX = 2D/Cube
    public static samplerXX iChannel1;
    public static samplerXX iChannel2;
    public static samplerXX iChannel3;
    public static vec4 iDate;                 // (year, month, day, time in seconds)

    internal Bitmap bmp;
    internal int bmpWidth;
    internal int bmpHeight;
    internal Color[] bmpColors;

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
        var n = new vec3();
        n.x = clamp(x.x, min, max);
        n.y = clamp(x.y, min, max);
        n.z = clamp(x.z, min, max);
        return n;
    }
    public static float cos(float v)
    {
        return (float)Math.Cos(v);
    }
    public static vec3 cos(vec3 v)
    {
        var n = new vec3();
        n.x = cos(v.x);
        n.y = cos(v.y);
        n.z = cos(v.z);
        return n;
    }
    public static vec4 cos(vec4 v)
    {
        var n = new vec4();
        n.x = cos(v.x);
        n.y = cos(v.y);
        n.z = cos(v.z);
        n.w = cos(v.w);
        return n;
    }
    public static vec3 cross(vec3 x, vec3 y)
    {
        var n = new vec3();
        n.x = x.y * y.z - y.y * x.z;
        n.y = x.z * y.x - y.z * x.x;
        n.z = x.x * y.y - y.x * x.y;
        return n;
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
    public static float floor(float v)
    {
        return (float)Math.Floor(v);
    }
    public static vec2 floor(vec2 v)
    {
        var n = new vec2();
        n.x = floor(v.x);
        n.y = floor(v.y);
        return n;
    }
    public static vec3 floor(vec3 v)
    {
        var n = new vec3();
        n.x = floor(v.x);
        n.y = floor(v.y);
        n.z = floor(v.z);
        return n;
    }
    public static float fract(float v)
    {
        return v - floor(v);
    }
    public static vec2 fract(vec2 v)
    {
        var n = new vec2();
        n.x = fract(v.x);
        n.y = fract(v.y);
        return n;
    }
    public static vec3 fract(vec3 v)
    {
        var n = new vec3();
        n.x = fract(v.x);
        n.y = fract(v.y);
        n.z = fract(v.z);
        return n;
    }
    public static vec4 fract(vec4 v)
    {
        var n = new vec4();
        n.x = fract(v.x);
        n.y = fract(v.y);
        n.z = fract(v.z);
        n.w = fract(v.w);
        return n;
    }
    public static float greaterThan(float x, float y)
    {
        return x > y ? 1 : 0;
    }
    public static vec2 greaterThan(vec2 x, vec2 y)
    {
        var n = new vec2();
        n.x = greaterThan(x.x, y.x);
        n.y = greaterThan(x.y, y.y);
        return n;
    }
    public static float length(vec2 v)
    {
        return (float)Math.Sqrt(v.x * v.x + v.y * v.y);
    }
    public static float length(vec3 v)
    {
        return (float)Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
    }
    public static float log(float x)
    {
        return (float)Math.Log(x);
    }
    public static float min(float x, float y)
    {
        return Math.Min(x, y);
    }
    public static vec4 min(vec4 x, vec4 y)
    {
        var n = new vec4();
        n.x = min(x.x, y.x);
        n.y = min(x.y, y.y);
        n.z = min(x.z, y.z);
        n.w = min(x.w, y.w);
        return n;
    }
    public static float mix(float x, float y, float a)
    {
        return x * (1 - a) + y * a;
    }
    public static vec2 mix(vec2 x, vec2 y, float a)
    {
        var n = new vec2();
        n.x = mix(x.x, y.x, a);
        n.y = mix(x.y, y.y, a);
        return n;
    }
    public static vec3 mix(vec3 x, vec3 y, float a)
    {
        var n = new vec3();
        n.x = mix(x.x, y.x, a);
        n.y = mix(x.y, y.y, a);
        n.z = mix(x.z, y.z, a);
        return n;
    }
    public static vec3 mix(vec3 x, vec3 y, vec3 a)
    {
        var n = new vec3();
        n.x = mix(x.x, y.x, a.x);
        n.y = mix(x.y, y.y, a.y);
        n.z = mix(x.z, y.z, a.z);
        return n;
    }
    public static vec4 mix(vec4 x, vec4 y, float a)
    {
        var n = new vec4();
        n.x = mix(x.x, y.x, a);
        n.y = mix(x.y, y.y, a);
        n.z = mix(x.z, y.z, a);
        n.w = mix(x.w, y.w, a);
        return n;
    }
    public static float max(float x, float y)
    {
        return Math.Max(x, y);
    }
    public static vec3 max(vec3 x, float y)
    {
        var n = new vec3();
        n.x = max(x.x, y);
        n.y = max(x.y, y);
        n.z = max(x.z, y);
        return n;
    }
    public static vec4 max(vec4 x, float y)
    {
        var n = new vec4();
        n.x = max(x.x, y);
        n.y = max(x.y, y);
        n.z = max(x.z, y);
        n.w = max(x.w, y);
        return n;
    }
    public static vec2 max(vec2 x, vec2 y)
    {
        var n = new vec2();
        n.x = max(x.x, y.x);
        n.y = max(x.y, y.y);
        return n;
    }
    public static vec3 max(vec3 x, vec3 y)
    {
        var n = new vec3();
        n.x = max(x.x, y.x);
        n.y = max(x.y, y.y);
        n.z = max(x.z, y.z);
        return n;
    }
    public static float mod(float x, float y)
    {
        return x - y * floor(x / y);
    }
    public static vec2 mod(vec2 x, float y)
    {
        var n = new vec2();
        n.x = mod(x.x, y);
        n.y = mod(x.y, y);
        return n;
    }
    public static vec2 mod(vec2 x, vec2 y)
    {
        var n = new vec2();
        n.x = mod(x.x, y.x);
        n.y = mod(x.y, y.y);
        return n;
    }
    public static vec3 mod(vec3 x, float y)
    {
        var n = new vec3();
        n.x = mod(x.x, y);
        n.y = mod(x.y, y);
        n.z = mod(x.z, y);
        return n;
    }
    public static vec3 mod(vec3 x, vec3 y)
    {
        var n = new vec3();
        n.x = mod(x.x, y.x);
        n.y = mod(x.y, y.y);
        n.z = mod(x.z, y.z);
        return n;
    }
    public static vec4 mod(vec4 x, float y)
    {
        var n = new vec4();
        n.x = mod(x.x, y);
        n.y = mod(x.y, y);
        n.z = mod(x.z, y);
        n.w = mod(x.w, y);
        return n;
    }
    public static float normalize(float v)
    {
        return 1;
    }
    public static vec3 normalize(vec3 v)
    {
        var n = new vec3();
        var l = (float)Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
        
        if (l > 9.99999974737875E-06)
        {
            n.x = v.x / l;
            n.y = v.y / l;
            n.z = v.z / l;
        }
        return n;
    }
    public static float pow(float x, float y)
    {
        return (float)Math.Pow(x, y);
    }
    public static vec3 pow(vec3 x, vec3 y)
    {
        var n = new vec3();
        n.x = pow(x.x, y.x);
        n.y = pow(x.y, y.y);
        n.z = pow(x.z, y.z);
        return n;
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
        var n = new vec3();
        n.x = sign(x.x);
        n.y = sign(x.y);
        n.z = sign(x.z);
        return n;
    }
    public static vec4 sign(vec4 x)
    {
        var n = new vec4();
        n.x = sign(x.x);
        n.y = sign(x.y);
        n.z = sign(x.z);
        n.w = sign(x.w);
        return n;
    }
    public static float sin(float v)
    {
        return (float)Math.Sin(v);
    }
    public static vec2 sin(vec2 v)
    {
        var n = new vec2();
        n.x = sin(v.x);
        n.y = sin(v.y);
        return n;
    }
    public static vec3 sin(vec3 v)
    {
        var n = new vec3();
        n.x = sin(v.x);
        n.y = sin(v.y);
        n.z = sin(v.z);
        return n;
    }
    public static vec4 sin(vec4 v)
    {
        var n = new vec4();
        n.x = sin(v.x);
        n.y = sin(v.y);
        n.z = sin(v.z);
        n.w = sin(v.w);
        return n;
    }
    public static float sqrt(float x)
    {
        return (float)Math.Sqrt(x);
    }
    public static vec3 sqrt(vec3 x)
    {
        var n = new vec3();
        n.x = sqrt(x.x);
        n.y = sqrt(x.y);
        n.z = sqrt(x.z);
        return n;
    }
    public static float smoothstep(float edge0, float edge1, float x)
    {
        var t = clamp((x - edge0) / (edge1 - edge0), 0.0f, 1.0f);
        return t * t * (3.0f - 2.0f * t);
    }
    public static vec2 smoothstep(vec2 edge0, vec2 edge1, vec2 x)
    {
        var n = new vec2();
        n.x = smoothstep(edge0.x, edge1.x, x.x);
        n.y = smoothstep(edge0.y, edge1.y, x.y);
        return n;
    }
    public static vec2 smoothstep(float edge0, float edge1, vec2 v)
    {
        var n = new vec2();
        n.x = smoothstep(edge0, edge1, v.x);
        n.y = smoothstep(edge0, edge1, v.y);
        return n;
    }
    public static vec3 smoothstep(float edge0, float edge1, vec3 v)
    {
        var n = new vec3();
        n.x = smoothstep(edge0, edge1, v.x);
        n.y = smoothstep(edge0, edge1, v.y);
        n.z = smoothstep(edge0, edge1, v.z);
        return n;
    }
    public static vec4 smoothstep(float edge0, float edge1, vec4 v)
    {
        var n = new vec4();
        n.x = smoothstep(edge0, edge1, v.x);
        n.y = smoothstep(edge0, edge1, v.y);
        n.z = smoothstep(edge0, edge1, v.z);
        n.w = smoothstep(edge0, edge1, v.w);
        return n;
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
        var n = new vec3();
        n.x = step(edge.x, x.x);
        n.y = step(edge.y, x.y);
        n.z = step(edge.z, x.z);
        return n;
    }
    public static float tan(float x)
    {
        return (float)Math.Tan(x);
    }

    public vec4 texture2D(samplerXX sampler, vec2 coords)
    {
        var uc = sampler.bmp.GetPixel((int)coords.x, (int)coords.y);
        var v4 = new vec4();
        v4.r = uc.R / 255;
        v4.g = uc.G / 255;
        v4.b = uc.B / 255;
        v4.a = uc.A / 255;
        return v4;
    }

    public virtual void Draw(System.Windows.Forms.PaintEventArgs args)
    {
        args.Graphics.DrawImage(bmp, 0, 0, bmpWidth, bmpHeight);
    }
    public virtual void Start()
    {
        bmp = new System.Drawing.Bitmap((int)iResolution.x, (int)iResolution.y);
        bmpWidth = bmp.Width;
        bmpHeight = bmp.Height;
        bmpColors = new Color[bmpWidth * bmpHeight];

        iMouse = new vec4();
    }
    public void Step()
    {
        for (int y = 0; y < bmpHeight; y++)
        for (int x = 0; x < bmpWidth; x++)
        {
            vec4 fragColor;

            mainImage(out fragColor, new vec2(x, y));

            var r = (int)(fragColor.r * 255);
            var g = (int)(fragColor.g * 255);
            var b = (int)(fragColor.b * 255);
            var a = (int)(fragColor.a * 255);

            if (r < 0) r = 0; else if (r > 255) r = 255;
            if (g < 0) g = 0; else if (g > 255) g = 255;
            if (b < 0) b = 0; else if (b > 255) b = 255;
            if (a < 0) a = 0; else if (a > 255) a = 255;

            var color = Color.FromArgb(a, r, g, b);
            var colorIndex = x + y * bmpWidth;
            if (color != bmpColors[colorIndex])
            {
                bmpColors[colorIndex] = color;
                bmp.SetPixel(x, bmpHeight - y - 1, color); // Updside-down
            }
        }
    }

    public virtual void mainImage(out vec4 fragColor, vec2 fragCoord)
    {
        fragColor = new vec4();
    }

    public override string ToString()
    {
        return "GdiShader";
    }
}