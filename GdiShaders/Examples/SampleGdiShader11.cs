namespace GdiShaders.Examples
{
    public class SampleGdiShader11 : GdiShader
    {
        float time = iTime;

        float makePoint(float x, float y, float fx, float fy, float sx, float sy, float t)
        {
            float xx = x + sin(t * fx) * sx;
            float yy = y + cos(t * fy) * sy;
            return 1.0f / sqrt(xx * xx + yy * yy);
        }

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            time = iTime;

            vec2 p = (fragCoord.xy / iResolution.x) * 2.0f - new vec2(1.0f, iResolution.y / iResolution.x);

            p = p * 2.0f;

            float x = p.x;
            float y = p.y;

            float a =
                makePoint(x, y, 3.3f, 2.9f, 0.3f, 0.3f, time);
            a = a + makePoint(x, y, 1.9f, 2.0f, 0.4f, 0.4f, time);
            a = a + makePoint(x, y, 0.8f, 0.7f, 0.4f, 0.5f, time);
            a = a + makePoint(x, y, 2.3f, 0.1f, 0.6f, 0.3f, time);
            a = a + makePoint(x, y, 0.8f, 1.7f, 0.5f, 0.4f, time);
            a = a + makePoint(x, y, 0.3f, 1.0f, 0.4f, 0.4f, time);
            a = a + makePoint(x, y, 1.4f, 1.7f, 0.4f, 0.5f, time);
            a = a + makePoint(x, y, 1.3f, 2.1f, 0.6f, 0.3f, time);
            a = a + makePoint(x, y, 1.8f, 1.7f, 0.5f, 0.4f, time);

            float b =
                makePoint(x, y, 1.2f, 1.9f, 0.3f, 0.3f, time);
            b = b + makePoint(x, y, 0.7f, 2.7f, 0.4f, 0.4f, time);
            b = b + makePoint(x, y, 1.4f, 0.6f, 0.4f, 0.5f, time);
            b = b + makePoint(x, y, 2.6f, 0.4f, 0.6f, 0.3f, time);
            b = b + makePoint(x, y, 0.7f, 1.4f, 0.5f, 0.4f, time);
            b = b + makePoint(x, y, 0.7f, 1.7f, 0.4f, 0.4f, time);
            b = b + makePoint(x, y, 0.8f, 0.5f, 0.4f, 0.5f, time);
            b = b + makePoint(x, y, 1.4f, 0.9f, 0.6f, 0.3f, time);
            b = b + makePoint(x, y, 0.7f, 1.3f, 0.5f, 0.4f, time);

            float c =
                makePoint(x, y, 3.7f, 0.3f, 0.3f, 0.3f, time);
            c = c + makePoint(x, y, 1.9f, 1.3f, 0.4f, 0.4f, time);
            c = c + makePoint(x, y, 0.8f, 0.9f, 0.4f, 0.5f, time);
            c = c + makePoint(x, y, 1.2f, 1.7f, 0.6f, 0.3f, time);
            c = c + makePoint(x, y, 0.3f, 0.6f, 0.5f, 0.4f, time);
            c = c + makePoint(x, y, 0.3f, 0.3f, 0.4f, 0.4f, time);
            c = c + makePoint(x, y, 1.4f, 0.8f, 0.4f, 0.5f, time);
            c = c + makePoint(x, y, 0.2f, 0.6f, 0.6f, 0.3f, time);
            c = c + makePoint(x, y, 1.3f, 0.5f, 0.5f, 0.4f, time);

            vec3 d = new vec3(a, b, c) / 32.0f;

            fragColor = new vec4(d.x, d.y, d.z, 1.0f);
        }

        public override string ToString()
        {
            return "Colored light balls";
        }
    }
}