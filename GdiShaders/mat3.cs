namespace GdiShaders
{
    public struct mat3
    {
        public float x1 { get; set; }
        public float x2 { get; set; }
        public float x3 { get; set; }
        public float y1 { get; set; }
        public float y2 { get; set; }
        public float y3 { get; set; }
        public float z1 { get; set; }
        public float z2 { get; set; }
        public float z3 { get; set; }

        public vec3 this[int column]
        {
            set
            {
                switch (column)
                {
                    case 0:
                        x1 = value.x;
                        y1 = value.y;
                        z1 = value.z;
                        break;
                    case 1:
                        x2 = value.x;
                        y2 = value.y;
                        z2 = value.z;
                        break;
                    case 2:
                        x3 = value.x;
                        y3 = value.y;
                        z3 = value.z;
                        break;
                }
            }
        }
    }
}
