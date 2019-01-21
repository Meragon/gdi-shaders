namespace GdiShaders.Core
{
    using System;

    public struct vec3
    {
        public static implicit operator vec2(vec3 v)
        {
            return v.xy;
        }

        public static vec3 operator -(vec3 left)
        {
            return new vec3(-left.x, -left.y, -left.z);
        }

        public static vec3 operator +(vec3 left, vec3 right)
        {
            return new vec3(left.x + right.x, left.y + right.y, left.z + right.z);
        }
        public static vec3 operator +(vec3 l, float r)
        {
            return new vec3(l.x + r, l.y + r, l.z + r);
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
            return new vec3(left.x * right, left.y * right, left.z * right);
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

        public static vec3 operator *(vec3 l, mat3 r)
        {
            return new vec3(
                r.x1 * l.x + r.x2 * l.y + r.x3 * l.z,
                r.y1 * l.x + r.y2 * l.y + r.y3 * l.z,
                r.z1 * l.x + r.z2 * l.y + r.z3 * l.z);
        }

        public float x;
        public float y;
        public float r { get { return x; } set { x = value; } }
        public float g { get { return y; } set { y = value; } }
        public float b { get { return z; } set { z = value; } }
        public float z;

        public vec3(vec3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }
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
        public vec3(float _x, vec2 _yz)
        {
            x = _x;
            y = _yz.x;
            z = _yz.y;
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;

                    default:
                        throw new IndexOutOfRangeException("index");
                }
            }
            set
            {
                switch (index)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    case 2: z = value; break;

                    default:
                        throw new IndexOutOfRangeException("index");

                }
            }
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
        public vec3 xzy
        {
            get { return new vec3(x, z, y); }
            set
            {
                x = value.x;
                y = value.z;
                z = value.y;
            }
        }
        public vec2 yx { get { return new vec2(y, x); } }
        public vec3 yxz
        {
            get { return new vec3(y, x, z); }
            set
            {
                y = value.x;
                x = value.y;
                z = value.z;
            }
        }
        public vec2 yy
        {
            get { return new vec2(y, y); }
        }
        public vec2 yz
        {
            get { return new vec2(y, z); }
            set
            {
                y = value.x;
                z = value.y;
            }
        }
        public vec3 yzx { get { return new vec3(y, z, x);} }
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
        public vec2 zx
        {
            get { return new vec2(z, x); }
        }
        public vec3 zxy
        {
            get { return new vec3(z, x, y); }
        }
        public vec2 zy
        {
            get { return new vec2(z, y); }
        }
        public vec3 zyx
        {
            get { return new vec3(z, y, x); }
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}, {2}]", x, y, z);
        }
    }
}