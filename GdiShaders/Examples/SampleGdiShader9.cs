namespace GdiShaders.Examples
{
    public class SampleGdiShader9 : GdiShader
    {
        float hash(float n) { return fract(sin(n) * 753.5453123f); }
        float noise(vec3 x)
        {
            vec3 p = floor(x);
            vec3 f = fract(x);
            f = f * f * (3.0f - 2.0f * f);

            float n = p.x + p.y * 157.0f + 113.0f * p.z;
            return mix(mix(mix(hash(n + 0.0f), hash(n + 1.0f), f.x),
                    mix(hash(n + 157.0f), hash(n + 158.0f), f.x), f.y),
                mix(mix(hash(n + 113.0f), hash(n + 114.0f), f.x),
                    mix(hash(n + 270.0f), hash(n + 271.0f), f.x), f.y), f.z);
        }
        float n(vec3 x)
        {
            float s = noise(x);
            for (float i = 2f; i < 10f; i++)
            {
                s += noise(x / i) / i;

            }
            return s;
        }
        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            vec2 uv = fragCoord.xy / iResolution.xy * 10f;
            float a = abs(n(new vec3(uv + iTime * 3.14f, sin(iTime))) - n(new vec3(uv + iTime, cos(iTime + 3f))));
            fragColor = new vec4(0, .5f - pow(a, .2f) / 2f, 1f - pow(a, .2f), 1);
        }

        public override string ToString()
        {
            return "9";
        }
    }
}