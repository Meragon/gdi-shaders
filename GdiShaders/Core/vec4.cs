namespace GdiShaders.Core
{
    using System;

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
        public static vec4 operator +(vec4 l, float r)
        {
            return new vec4(l.x + r, l.y + r, l.z + r, l.w + r);
        }
        public static vec4 operator -(float l, vec4 r)
        {
            return new vec4(l - r.x, l - r.y, l - r.z, l - r.w);
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

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;

                    default:
                        throw new NotImplementedException(); // ?
                }
            }
            set
            {
                switch (index)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    case 2: z = value; break;
                    case 3: w = value; break;

                    default:
                        throw new NotImplementedException(); // ?
                }
            }
        }

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

        public vec3 www { get { return new vec3(w, w, w); } }
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
        public vec4 xxzz
        {
            get { return new vec4(x, x, z, z); }
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
        public vec2 zw { get { return new vec2(z, w); } }

        public override string ToString()
        {
            return string.Format("[{0}, {1}, {2}, {3}]", x, y, z, w);
        }
    }
}