namespace GdiShaders.Core
{
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
        public static vec2 operator -(float left, vec2 right)
        {
            return new vec2(left - right.x, left - right.y);
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
            return new vec2(r.x1 * l.x + r.x2 * l.y, r.y1 * l.x + r.y2 * l.y);
        }
        public static vec2 operator *(mat2 l, vec2 r)
        {
            return r * l;
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
        public static vec2 operator &(vec2 left, vec2 right)
        {
            return new vec2((int)left.x & (int)right.x, (int)left.y & (int)right.y);
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
        public vec3 xyx { get { return new vec3(x, y, x); } }
        public vec4 xyxy
        {
            get {return new vec4(x, y, x, y); }
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
}