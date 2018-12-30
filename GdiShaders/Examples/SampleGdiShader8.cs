namespace GdiShaders.Examples
{
    using GdiShaders.Core;

    public class SampleGdiShader8 : GdiShader
    {
        const float TAU      = 6.28318530718f;
        const int   MAX_ITER = 5;

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            float time = iTime * .5f + 23.0f;
            // uv should be the 0-1 uv of texture...
            vec2 uv = fragCoord.xy / iResolution.xy;

            vec2 p = mod(uv * TAU, TAU) - 250.0f;
            vec2 i = new vec2(p);
            float c = 1.0f;
            float inten = .005f;

            for (int n = 0; n < MAX_ITER; n++)
            {
                float t = time * (1.0f - (3.5f / (float)(n + 1)));
                i = p + new vec2(cos(t - i.x) + sin(t + i.y), sin(t - i.y) + cos(t + i.x));
                c += 1.0f / length(new vec2(p.x / (sin(i.x + t) / inten), p.y / (cos(i.y + t) / inten)));
            }
            c /= (float)(MAX_ITER);
            c = 1.17f - pow(c, 1.4f);
            vec3 colour = new vec3(pow(abs(c), 8.0f));
            colour = clamp(colour + new vec3(0.0f, 0.35f, 0.5f), 0.0f, 1.0f);
            fragColor = new vec4(colour, 1.0f);
        }

        public override string ToString()
        {
            return "8 Caustic";
        }
    }
}