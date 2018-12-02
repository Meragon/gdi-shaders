namespace GdiShaders
{
    public struct mat2
    {
        public mat2(float x1, float x2, float y1, float y2)
        {
            this.x1 = x1;
            this.x2 = x2;
            this.y1 = y1;
            this.y2 = y2;
        }

        public float x1 { get; set; }
        public float x2 { get; set; }
        public float y1 { get; set; }
        public float y2 { get; set; }

        public static mat2 operator -(mat2 left)
        {
            return new mat2(-left.x1, -left.x2, -left.y1, -left.y2);
        }
    }
}