namespace GdiShaders.Examples
{
    public class SampleGdiShader13 : GdiShader
    {
        float length2(vec2 p) { return dot(p, p); }

        float noise(vec2 p)
        {
            return fract(sin(fract(sin(p.x) * (4313.13311f)) + p.y) * 3131.0011f);
        }

        float worley(vec2 p)
        {
            float d = 1e30f;
            for (int xo = -1; xo <= 1; ++xo)
            for (int yo = -1; yo <= 1; ++yo)
            {
                vec2 tp = floor(p) + new vec2(xo, yo);
                d = min(d, length2(p - tp - new vec2(noise(tp))));
            }
            return 3f * exp(-4f * abs(2f * d - 1f));
        }

        float fworley(vec2 p)
        {
            return sqrt(sqrt(sqrt(
                pow(worley(p + iTime), 2) *
                worley(p * 2f + 1.3f + iTime * .5f) *
                worley(p * 4f + 2.3f + iTime * .25f) *
                worley(p * 8 + 3.3f + iTime * .125f) *
                worley(p * 32f + 4.3f + iTime * .125f) *
                sqrt(worley(p * 64f + 5.3f + iTime * .0625f)) *
                sqrt(sqrt(worley(p * 128f + 7.3f))))));
        }

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            vec2 uv = fragCoord.xy / iResolution.xy;
            float t = fworley(uv * iResolution.xy / 600f);
            t *= exp(-length2(abs(2f * uv - 1f)));
            float r = length(abs(2f * uv - 1f) * iResolution.xy);
            fragColor = new vec4(t * new vec3(1.8f, 1.8f * t, .1f + pow(t, 2f - t)), 1f);
        }
    }
}