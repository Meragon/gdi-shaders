namespace GdiShaders.Examples
{
    using System;

    [Obsolete("Not working, very slow")]
    public class SampleGdiShader4 : GdiShader
    {
        float noise(vec3 p) //Thx to Las^Mercury
        {
            vec3 i = floor(p);
            vec4 a = dot(i, new vec3(1f, 57f, 21f)) + new vec4(0f, 57f, 21f, 78f);
            vec3 f = cos((p - i) * acos(-1f)) * (-.5f) + .5f;
            a = mix(sin(cos(a) * a), sin(cos(1f + a) * (1f + a)), f.x);
            a.xy = mix(a.xz, a.yw, f.y);
            return mix(a.x, a.y, f.z);
        }

        float sphere(vec3 p, vec4 spr)
        {
            return length(spr.xyz - p) - spr.w;
        }

        float flame(vec3 p)
        {
            float d = sphere(p * new vec3(1f, .5f, 1f), new vec4(.0f, -1f, .0f, 1f));
            return d + (noise(p + new vec3(.0f, iTime * 2f, .0f)) + noise(p * 3f) * .5f) * .25f * (p.y);
        }

        float scene(vec3 p)
        {
            return min(100f - length(p), abs(flame(p)));
        }

        vec4 raymarch(vec3 org, vec3 dir)
        {
            float d = 0.0f, glow = 0.0f, eps = 0.02f;
            vec3 p = org;
            bool glowed = false;

            for (int i = 0; i < 64; i++)
            {
                d = scene(p) + eps;
                p += d * dir;
                if (d > eps)
                {
                    if (flame(p) < .0)
                        glowed = true;
                    if (glowed)
                        glow = (float)(i) / 64f;
                }
            }
            return new vec4(p, glow);
        }

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            vec2 v = -1.0f + 2.0f * fragCoord.xy / iResolution.xy;
            v.x *= iResolution.x / iResolution.y;

            vec3 org = new vec3(0f, -2f, 4f);
            vec3 dir = normalize(new vec3(v.x * 1.6f, -v.y, -1.5f));

            vec4 p = raymarch(org, dir);
            float glow = p.w;

            vec4 col = mix(new vec4(1f, .5f, .1f, 1f), new vec4(0.1f, .5f, 1f, 1f), p.y * .02f + .4f);

            fragColor = mix(new vec4(0f), col, pow(glow * 2f, 4f));
            //fragColor = mix(vec4(1.), mix(vec4(1.,.5,.1,1.),vec4(0.1,.5,1.,1.),p.y*.02+.4), pow(glow*2.,4.));
        }

        public override string ToString()
        {
            return "4 Flame";
        }
    }
}