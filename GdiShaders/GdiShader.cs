using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Windows.Forms;

// GLSL
public class GdiShader
{
    public static vec3 iResolution;           // viewport resolution (in pixels)
    public static float iGlobalTime;           // shader playback time (in seconds)
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

    internal System.Drawing.Bitmap bmp;
    private static object bmpLocker = new object();

    public virtual void Draw(System.Windows.Forms.PaintEventArgs args)
    {
        args.Graphics.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
    }
    public virtual void Start()
    {
        bmp = new System.Drawing.Bitmap((int)iResolution.x, (int)iResolution.y);

        iMouse = new vec4();
    }
    public void Step()
    {
        for (int y = 0; y < bmp.Height; y++)
            for (int x = 0; x < bmp.Width; x++)
            {
                vec4 fragColor;

                mainImage(out fragColor, new vec2(x, y));

                int r = (int)(fragColor.r * 255);
                int g = (int)(fragColor.g * 255);
                int b = (int)(fragColor.b * 255);
                int a = (int)(fragColor.a * 255);

                if (r > 255) r = 255;
                if (g > 255) g = 255;
                if (b > 255) b = 255;
                if (a > 255) a = 255;
                if (r < 0) r = 0;
                if (g < 0) g = 0;
                if (b < 0) b = 0;
                if (a < 0) a = 0;

                var color = Color.FromArgb(a, r, g, b);

                bmp.SetPixel(x, bmp.Height - y - 1, color); // Updside-down
            }
    }

    public virtual void mainImage(out vec4 fragColor, vec2 fragCoord)
    {
        fragColor = new vec4();
    }

    public float abs(float v)
    {
        return Math.Abs(v);
    }
    public vec2 abs(vec2 v)
    {
        return new vec2(abs(v.x), abs(v.y));
    }
    public vec3 abs(vec3 v)
    {
        return new vec3(abs(v.x), abs(v.y), abs(v.z));
    }
    public vec4 abs(vec4 v)
    {
        return new vec4(abs(v.x), abs(v.y), abs(v.z), abs(v.w));
    }
    public float acos(float v)
    {
        return (float)Math.Acos(v);
    }
    public float atan(float x, float y)
    {
        return (float)Math.Atan2(x, y);
    }
    public float clamp(float x, float min, float max)
    {
        if (x < min) x = min;
        if (x > max) x = max;
        return x;
    }
    public vec3 clamp(vec3 x, float min, float max)
    {
        var n = new vec3();
        n.x = clamp(x.x, min, max);
        n.y = clamp(x.y, min, max);
        n.z = clamp(x.z, min, max);
        return n;
    }
    public float cos(float v)
    {
        return (float)Math.Cos(v);
    }
    public vec3 cos(vec3 v)
    {
        v.x = cos(v.x);
        v.y = cos(v.y);
        v.z = cos(v.z);
        return v;
    }
    public vec4 cos(vec4 v)
    {
        v.x = cos(v.x);
        v.y = cos(v.y);
        v.z = cos(v.z);
        v.w = cos(v.w);
        return v;
    }
    public vec3 cross(vec3 x, vec3 y)
    {
        var n = new vec3();
        n.x = x.y * y.z - y.y * x.z;
        n.y = x.z * y.x - y.z * x.x;
        n.z = x.x * y.y - y.x * x.y;
        return n;
    }
    public float dot(float x, float y)
    {
        return x * y;
    }
    public float dot(vec2 x, vec2 y)
    {
        return dot(x.x, y.x) + dot(x.y, y.y);
    }
    public float dot(vec3 x, vec3 y)
    {
        return dot(x.x, y.x) + dot(x.y, y.y) + dot(x.z, y.z);
    }
    public float exp(float x)
    {
        return (float)Math.Exp(x);
    }
    public float floor(float v)
    {
        return (float)Math.Floor(v);
    }
    public vec2 floor(vec2 v)
    {
        var n = new vec2();
        n.x = floor(v.x);
        n.y = floor(v.y);
        return n;
    }
    public vec3 floor(vec3 v)
    {
        var n = new vec3();
        n.x = floor(v.x);
        n.y = floor(v.y);
        n.z = floor(v.z);
        return n;
    }
    public float fract(float v)
    {
        return v - floor(v);
    }
    public vec2 fract(vec2 v)
    {
        var n = new vec2();
        n.x = fract(v.x);
        n.y = fract(v.y);
        return n;
    }
    public vec3 fract(vec3 v)
    {
        var n = new vec3();
        n.x = fract(v.x);
        n.y = fract(v.y);
        n.z = fract(v.z);
        return n;
    }
    public vec4 fract(vec4 v)
    {
        var n = new vec4();
        n.x = fract(v.x);
        n.y = fract(v.y);
        n.z = fract(v.z);
        n.w = fract(v.w);
        return n;
    }
    public float length(vec2 v)
    {
        return abs((float)Math.Sqrt(v.x * v.x + v.y * v.y));
    }
    public float length(vec3 v)
    {
        return abs((float)Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z));
    }
    public float log(float x)
    {
        return (float)Math.Log(x);
    }
    public float min(float x, float y)
    {
        return Math.Min(x, y);
    }
    public vec4 min(vec4 x, vec4 y)
    {
        var n = new vec4();
        n.x = min(x.x, y.x);
        n.y = min(x.y, y.y);
        n.z = min(x.z, y.z);
        n.w = min(x.w, y.w);
        return n;
    }
    public float mix(float x, float y, float a)
    {
        return x * (1 - a) + y * a;
    }
    public vec2 mix(vec2 x, vec2 y, float a)
    {
        var n = new vec2();
        n.x = mix(x.x, y.x, a);
        n.y = mix(x.y, y.y, a);
        return n;
    }
    public vec3 mix(vec3 x, vec3 y, float a)
    {
        var n = new vec3();
        n.x = mix(x.x, y.x, a);
        n.y = mix(x.y, y.y, a);
        n.z = mix(x.z, y.z, a);
        return n;
    }
    public vec4 mix(vec4 x, vec4 y, float a)
    {
        var n = new vec4();
        n.x = mix(x.x, y.x, a);
        n.y = mix(x.y, y.y, a);
        n.z = mix(x.z, y.z, a);
        n.w = mix(x.w, y.w, a);
        return n;
    }
    public float max(float x, float y)
    {
        return Math.Max(x, y);
    }
    public vec3 max(vec3 x, float y)
    {
        var n = new vec3();
        n.x = max(x.x, y);
        n.y = max(x.y, y);
        n.z = max(x.z, y);
        return n;
    }
    public vec2 max(vec2 x, vec2 y)
    {
        var n = new vec2();
        n.x = max(x.x, y.x);
        n.y = max(x.y, y.y);
        return n;
    }
    public vec3 max(vec3 x, vec3 y)
    {
        var n = new vec3();
        n.x = max(x.x, y.x);
        n.y = max(x.y, y.y);
        n.z = max(x.z, y.z);
        return n;
    }
    public float mod(float x, float y)
    {
        return x - y * floor(x / y);
    }
    public vec2 mod(vec2 x, float y)
    {
        var n = new vec2();
        n.x = mod(x.x, y);
        n.y = mod(x.y, y);
        return n;
    }
    public vec2 mod(vec2 x, vec2 y)
    {
        var n = new vec2();
        n.x = mod(x.x, y.x);
        n.y = mod(x.y, y.y);
        return n;
    }
    public vec3 mod(vec3 x, vec3 y)
    {
        var n = new vec3();
        n.x = mod(x.x, y.x);
        n.y = mod(x.y, y.y);
        n.z = mod(x.z, y.z);
        return n;
    }
    public vec3 normalize(vec3 v)
    {
        float l = (float)Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
        v.x /= l;
        v.y /= l;
        v.z /= l;
        return v;
    }
    public float pow(float x, float y)
    {
        return (float)Math.Pow(x, y);
    }
    public vec3 pow(vec3 x, vec3 y)
    {
        var n = new vec3();
        n.x = pow(x.x, y.x);
        n.y = pow(x.y, y.y);
        n.z = pow(x.z, y.z);
        return n;
    }
    public float sign(float x)
    {
        if (x < 0f)
            return -1;
        if (x == 0)
            return 0;
        return 1;
    }
    public vec4 sign(vec4 x)
    {
        var n = new vec4();
        n.x = sign(x.x);
        n.y = sign(x.y);
        n.z = sign(x.z);
        n.w = sign(x.w);
        return n;
    }
    public float sin(float v)
    {
        return (float)Math.Sin(v);
    }
    public vec2 sin(vec2 v)
    {
        var n = new vec2();
        n.x = sin(v.x);
        n.y = sin(v.y);
        return n;
    }
    public vec3 sin(vec3 v)
    {
        var n = new vec3();
        n.x = sin(v.x);
        n.y = sin(v.y);
        n.z = sin(v.z);
        return n;
    }
    public vec4 sin(vec4 v)
    {
        var n = new vec4();
        n.x = sin(v.x);
        n.y = sin(v.y);
        n.z = sin(v.z);
        n.w = sin(v.w);
        return n;
    }
    public float sqrt(float x)
    {
        return (float)Math.Sqrt(x);
    }
    public float smoothstep(float edge0, float edge1, float x)
    {
        var t = clamp((x - edge0) / (edge1 - edge0), 0.0f, 1.0f);
        return t * t * (3.0f - 2.0f * t);
    }
    public vec2 smoothstep(vec2 edge0, vec2 edge1, vec2 x)
    {
        var n = new vec2();
        n.x = smoothstep(edge0.x, edge1.x, x.x);
        n.y = smoothstep(edge0.y, edge1.y, x.y);
        return n;
    }
    public vec2 smoothstep(float edge0, float edge1, vec2 v)
    {
        var n = new vec2();
        n.x = smoothstep(edge0, edge1, v.x);
        n.y = smoothstep(edge0, edge1, v.y);
        return n;
    }
    public vec3 smoothstep(float edge0, float edge1, vec3 v)
    {
        var n = new vec3();
        n.x = smoothstep(edge0, edge1, v.x);
        n.y = smoothstep(edge0, edge1, v.y);
        n.z = smoothstep(edge0, edge1, v.z);
        return n;
    }
    public vec4 smoothstep(float edge0, float edge1, vec4 v)
    {
        var n = new vec4();
        n.x = smoothstep(edge0, edge1, v.x);
        n.y = smoothstep(edge0, edge1, v.y);
        n.z = smoothstep(edge0, edge1, v.z);
        n.w = smoothstep(edge0, edge1, v.w);
        return n;
    }
    public float step(float edge, float x)
    {
        if (x < edge) return 0;
        return 1;
    }
    public float tan(float x)
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

    public override string ToString()
    {
        return "GdiShader";
    }
}

public class mat2
{
    public float x1 { get; set; }
    public float x2 { get; set; }
    public float y1 { get; set; }
    public float y2 { get; set; }

    public mat2(float _x1, float _y1, float _x2, float _y2)
    {
        x1 = _x1;
        x2 = _x2;
        y1 = _y1;
        y2 = _y2;
    }
}
public struct vec2
{
    public static vec2 operator +(vec2 left, vec2 right)
    {
        return new vec2(left.x + right.x, left.y + right.y);
    }
    public static vec2 operator +(float l, vec2 r)
    {
        return new vec2(r.x + l, r.y + l);
    }
    public static vec2 operator +(vec2 l, float r)
    {
        return new vec2(l.x + r, l.y + r);
    }
    public static vec2 operator -(vec2 v)
    {
        v.x = -v.x;
        v.y = -v.y;
        return v;
    }
    public static vec2 operator -(vec2 left, float right)
    {
        return new vec2(left.x - right, left.y - right);
    }
    public static vec2 operator -(vec2 left, vec2 right)
    {
        return new vec2(left.x - right.x, left.y - right.y);
    }
    public static vec2 operator *(vec2 left, vec2 right)
    {
        return new vec2(left.x * right.x, left.y * right.y);
    }
    public static vec2 operator *(vec2 left, float right)
    {
        return new vec2(left.x * right, left.y * right);
    }
    public static vec2 operator *(float left, vec2 right)
    {
        return new vec2(left * right.x, left * right.y);
    }
    public static vec2 operator *(vec2 l, mat2 r)
    {
        var n = new vec2();
        n.x = r.x1 * l.x + r.x2 * l.y;
        n.y = r.y1 * l.x + r.y2 * l.y;
        return n;
    }
    public static vec2 operator /(vec2 left, vec2 right)
    {
        return new vec2(left.x / right.x, left.y / right.y);
    }
    public static vec2 operator /(float left, vec2 right)
    {
        return new vec2(left / right.x, left / right.y);
    }
    public static vec2 operator /(vec2 left, float right)
    {
        return new vec2(left.x / right, left.y / right);
    }

    public float x;
    public float y;
    
    public vec2(float all)
    {
        x = all;
        y = all;
    }
    public vec2(float _x, float _y)
    {
        x = _x;
        y = _y;
    }
    public vec2(vec2 v)
    {
        x = v.x;
        y = v.y;
    }

    public vec2 xx
    {
        get { return new vec2(x, x); }
    }
    public vec2 xy
    {
        get { return this; }
        set
        {
            x = value.x;
            y = value.y;
        }
    }
    public vec2 yx { get { return new vec2(y, x); } }
    public vec2 yy
    {
        get { return new vec2(y, y); }
    }

    public override string ToString()
    {
        return string.Format("[{0}, {1}]", x, y);
    }
}
public struct vec3
{
    public static implicit operator vec2(vec3 v)
    {
        return v.xy;
    }

    public static vec3 operator +(vec3 left, vec3 right)
    {
        return new vec3(left.x + right.x, left.y + right.y, left.z + right.z);
    }
    public static vec3 operator +(vec3 l, float r)
    {
        var n = new vec3();
        n.x = l.x + r;
        n.y = l.y + r;
        n.z = l.z + r;
        return n;
    }
    public static vec3 operator +(float l, vec3 r)
    {
        return new vec3(l + r.x, l + r.y, l + r.z);
    }
    public static vec3 operator -(vec3 left, vec3 right)
    {
        return new vec3(left.x - right.x, left.y - right.y, left.z - right.z);
    }
    public static vec3 operator -(float l, vec3 r)
    {
        return new vec3(l - r.x, l - r.y, l - r.z);
    }
    public static vec3 operator -(vec3 l, float r)
    {
        return new vec3(l.x - r, l.y - r, l.z - r);
    }
    public static vec3 operator *(vec3 left, float right)
    {
        var n = new vec3();
        n.x = left.x * right;
        n.y = left.y * right;
        n.z = left.z * right;
        return n;
    }
    public static vec3 operator *(float l, vec3 r)
    {
        return new vec3(r.x * l, r.y * l, r.z * l);
    }
    public static vec3 operator *(vec3 l, vec3 r)
    {
        return new vec3(l.x * r.x, l.y * r.y, l.z * r.z);
    }
    public static vec3 operator /(vec3 left, float right)
    {
        return new vec3(left.x / right, left.y / right, left.z / right);
    }
    public static vec3 operator /(float l, vec3 r)
    {
        return new vec3(l / r.x, l / r.y, l / r.z);
    }
    public static vec3 operator /(vec3 l, vec3 r)
    {
        return new vec3(l.x / r.x, l.y / r.y, l.z / r.z);
    }

    public float x;
    public float y;
    public float r { get { return x; } set { x = value; } }
    public float g { get { return y; } set { y = value; } }
    public float b { get { return z; } set { z = value; } }
    public float z;
    
    public vec3(float all)
    {
        x = all;
        y = all;
        z = all;
    }
    public vec3(float _x, float _y)
    {
        x = _x;
        y = _y;
        z = 0;
    }
    public vec3(float _x, float _y, float _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }
    public vec3(vec2 v, float _z)
    {
        x = v.x;
        y = v.y;
        z = _z;
    }

    public vec2 xx
    {
        get { return new vec2(x, x); }
    }
    public vec2 xy
    {
        get { return new vec2(x, y); }
        set
        {
            x = value.x;
            y = value.y;
        }
    }
    public vec2 yx { get { return new vec2(y, x); } }
    public vec2 yy
    {
        get { return new vec2(y, y); }
    }
    public vec3 rgb
    {
        get { return this; }
        set
        {
            x = value.x;
            y = value.y;
            z = value.z;
        }
    }
    public vec3 xyz
    {
        get { return this; }
        set { rgb = value; }
    }
    public vec2 xz
    {
        get { return new vec2(x, z); }
        set
        {
            x = value.x;
            z = value.y;
        }
    }
    public vec2 yz
    {
        get { return new vec2(y, z); }
    }

    public override string ToString()
    {
        return string.Format("[{0}, {1}, {2}]", x, y, z);
    }
}
public struct vec4
{
    public static vec4 operator +(vec4 l, vec4 r)
    {
        return new vec4(l.x + r.x, l.y + r.y, l.z + r.z, l.w + r.w);
    }
    public static vec4 operator +(vec3 l, vec4 r)
    {
        return new vec4(l.x + r.x, l.y + r.y, l.z + r.z, r.z);
    }
    public static vec4 operator +(float l, vec4 r)
    {
        return new vec4(r.x + l, r.y + l, r.z + l, r.w + l);
    }
    public static vec4 operator -(vec4 l, float r)
    {
        return new vec4(l.x - r, l.y - r, l.z - r, l.w - r);
    }
    public static vec4 operator -(vec4 l, vec4 r)
    {
        return new vec4(l.x - r.x, l.y - r.y, l.z - r.z, l.w - r.w);
    }
    public static vec4 operator *(vec4 l, float r)
    {
        return new vec4(l.x * r, l.y * r, l.z * r, l.w * r);
    }
    public static vec4 operator *(float l, vec4 r)
    {
        return new vec4(l * r.x, l * r.y, l * r.z, l * r.w);
    }
    public static vec4 operator *(vec4 l, vec4 r)
    {
        return new vec4(l.x * r.x, l.y * r.y, l.z * r.z, l.w * r.w);
    }
    public static vec4 operator /(vec4 l, vec4 r)
    {
        return new vec4(l.x / r.x, l.y / r.y, l.z / r.z, l.w / r.z);
    }
    public static vec4 operator /(vec4 l, float r)
    {
        return new vec4(l.x / r, l.y / r, l.z / r, l.w / r);
    }

    public float a { get { return w; } set { w = value; } }
    public float x;
    public float y;
    public float z;
    public float w;

    public float r { get { return x; } set { x = value; } }
    public float g { get { return y; } set { y = value; } }
    public float b { get { return z; } set { z = value; } }

    public vec4(float all)
    {
        x = all;
        y = all;
        z = all;
        w = all;
    }
    public vec4(float _x, float _y)
    {
        x = _x;
        y = _y;
        z = 0;
        w = 0;
    }
    public vec4(float _x, float _y, float _z)
    {
        x = _x;
        y = _y;
        z = _z;
        w = 0;
    }
    public vec4(float _x, float _y, float _z, float _w)
    {
        x = _x;
        y = _y;
        z = _z;
        w = _w;
    }
    public vec4(vec2 v1, vec2 v2)
    {
        x = v1.x;
        y = v1.y;
        z = v2.x;
        w = v2.y;
    }
    public vec4(vec2 v2, float _z, float _w)
    {
        x = v2.x;
        y = v2.y;
        z = _z;
        w = _w;
    }
    public vec4(vec3 v3, float _w)
    {
        x = v3.x;
        y = v3.y;
        z = v3.z;
        w = _w;
    }

    public vec2 xx
    {
        get { return new vec2(x, x); }
    }
    public vec2 xy
    {
        get { return new vec2(x, y); }
        set
        {
            x = value.x;
            y = value.y;
        }
    }
    public vec2 yx { get { return new vec2(y, x); } }
    public vec2 yy
    {
        get { return new vec2(y, y); }
    }
    public vec3 rgb
    {
        get { return new vec3(r, g, b); }
        set
        {
            x = value.x;
            y = value.y;
            z = value.z;
        }
    }
    public vec3 xyz
    {
        get { return new vec3(x, y, z); }
        set { rgb = value; }
    }
    public vec2 xz
    {
        get { return new vec2(x, z); }
        set
        {
            x = value.x;
            z = value.y;
        }
    }
    public vec2 yz
    {
        get { return new vec2(y, z); }
    }
    public vec2 yw { get { return new vec2(y, w); } }

    public override string ToString()
    {
        return string.Format("[{0}, {1}, {2}, {3}]", x, y, z, w);
    }
}
public class samplerXX
{
    public Bitmap bmp = new Bitmap((int)GdiShader.iResolution.x, (int)GdiShader.iResolution.y);

    public samplerXX()
    {

    }
}



