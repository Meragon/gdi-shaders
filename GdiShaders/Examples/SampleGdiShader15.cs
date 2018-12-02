namespace GdiShaders.Examples
{
    using System;
    
    public class SampleGdiShader15 : GdiShader
    {
        float t = iTime;

        mat2 m(float a) { float c = cos(a), s = sin(a); return new mat2(c, -s, s, c); }

        float map(vec3 p)
        {
            p.xz *= m(t * 0.4f); p.xy *= m(t * 0.3f);
            vec3 q = p * 2f + t;
            return length(p + new vec3(sin(t * 0.7f))) * log(length(p) + 1f) + sin(q.x + sin(q.z + sin(q.y))) * 0.5f - 1f;
        }

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            t = iTime;

            vec2 p = fragCoord.xy / iResolution.y - new vec2(.9f, .5f);
            vec3 cl = new vec3(0);
            float d = 2.5f;
            for (int i = 0; i <= 5; i++)
            {
                p = new vec3(0, 0, 5f) + normalize(new vec3(p, -1f)) * d;
                float rz = map(new vec3(p, 0));
                float f = clamp((rz - map(new vec3(p + .1f, 0))) * 0.5f, -.1f, 1f);
                vec3 l = new vec3(0.1f, 0.3f, .4f) + new vec3(5f, 2.5f, 3f) * f;
                cl = cl * l + (1f - smoothstep(0f, 2.5f, rz)) * .7f * l;
                d += min(rz, 1f);
            }
            fragColor = new vec4(cl, 1f);
        }

        public override string ToString()
        {
            return "15";
        }
    }
}