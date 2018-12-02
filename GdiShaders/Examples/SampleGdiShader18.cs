namespace GdiShaders.Examples
{
    public class SampleGdiShader18 : GdiShader
    {
        // https://www.shadertoy.com/view/4ttGWM

        float rand(vec2 n)
        {
            return fract(sin(cos(dot(n, new vec2(12.9898f, 12.1414f)))) * 83758.5453f);
        }

        float noise(vec2 n)
        {
            vec2 d = new vec2(0.0f, 1.0f);
            vec2 b = floor(n), f = smoothstep(new vec2(0.0f), new vec2(1.0f), fract(n));
            return mix(mix(rand(b), rand(b + d.yx), f.x), mix(rand(b + d.xy), rand(b + d.yy), f.x), f.y);
        }

        float fbm(vec2 n)
        {
            float total = 0.0f, amplitude = 1.0f;
            for (int i = 0; i < 5; i++)
            {
                total += noise(n) * amplitude;
                n += n * 1.7f;
                amplitude *= 0.47f;
            }
            return total;
        }

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            vec3 c1 = new vec3(0.5f, 0.0f, 0.1f);
            vec3 c2 = new vec3(0.9f, 0.1f, 0.0f);
            vec3 c3 = new vec3(0.2f, 0.1f, 0.7f);
            vec3 c4 = new vec3(1.0f, 0.9f, 0.1f);
            vec3 c5 = new vec3(0.1f);
            vec3 c6 = new vec3(0.9f);

            vec2 speed = new vec2(0.1f, 0.9f);
            float shift = 1.327f + sin(iTime * 2.0f) / 2.4f;
            float alpha = 1.0f;

            float dist = 3.5f - sin(iTime * 0.4f) / 1.89f;

            vec2 uv = fragCoord.xy / iResolution.xy;
            vec2 p = fragCoord.xy * dist / iResolution.xx;
            p += sin(p.yx * 4.0f + new vec2(.2f, -.3f) * iTime) * 0.04f;
            p += sin(p.yx * 8.0f + new vec2(.6f, +.1f) * iTime) * 0.01f;

            p.x -= iTime / 1.1f;
            float q = fbm(p - iTime * 0.3f + 1.0f * sin(iTime + 0.5f) / 2.0f);
            float qb = fbm(p - iTime * 0.4f + 0.1f * cos(iTime) / 2.0f);
            float q2 = fbm(p - iTime * 0.44f - 5.0f * cos(iTime) / 2.0f) - 6.0f;
            float q3 = fbm(p - iTime * 0.9f - 10.0f * cos(iTime) / 15.0f) - 4.0f;
            float q4 = fbm(p - iTime * 1.4f - 20.0f * sin(iTime) / 14.0f) + 2.0f;
            q = (q + qb - .4f * q2 - 2.0f * q3 + .6f * q4) / 3.8f;
            vec2 r = new vec2(fbm(p + q / 2.0f + iTime * speed.x - p.x - p.y), fbm(p + q - iTime * speed.y));
            vec3 c = mix(c1, c2, fbm(p + r)) + mix(c3, c4, r.x) - mix(c5, c6, r.y);
            vec3 color = 1.0f / (pow(c + 1.61f, new vec3(4.0f))) * cos(shift * fragCoord.y / iResolution.y);

            color = new vec3(1.0f, .2f, .05f) / (pow((r.y + r.y) * max(.0f, p.y) + 0.1f, 4.0f)); ;
            color += (texture2D(iChannel0, uv * 0.6f + new vec2(.5f, .1f)).xyz * 0.01f * pow((r.y + r.y) * .65f, 5.0f) + 0.055f) * mix(new vec3(.9f, .4f, .3f), new vec3(.7f, .5f, .2f), uv.y);
            color = color / (1.0f + max(new vec3(0), color));
            fragColor = new vec4(color.x, color.y, color.z, alpha);
        }

        public override string ToString()
        {
            return "18 301's Fire Shader";
        }
    }
}