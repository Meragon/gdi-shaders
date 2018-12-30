// Set to 1 to hash twice. Slower, but less patterns.
//#define DOUBLE_HASH


namespace GdiShaders.Examples
{
    using GdiShaders.Core;

    /// <summary>
    /// https://www.shadertoy.com/view/ldfyzl
    /// </summary>
    public class SampleGdiShader36 : GdiShader
    {
        public override void Start()
        {
            base.Start();

            iChannel0 = samplerXX.FromImage(ImageNames.Abstract1);
        }

        /*

A quick experiment with rain drop ripples.

This effect was written for and used in the launch scene of the
64kB intro "H - Immersion", by Ctrl-Alt-Test.

 > http://www.ctrl-alt-test.fr/productions/h-immersion/
 > https://www.youtube.com/watch?v=27PN1SsXbjM

-- 
Zavie / Ctrl-Alt-Test

*/

        // Maximum number of cells a ripple can cross.
        private const int MAX_RADIUS = 2;

        // Hash functions shamefully stolen from:
        // https://www.shadertoy.com/view/4djSRW
        private const float HASHSCALE1 = .1031f;
        private vec3 HASHSCALE3 = new vec3(.1031f, .1030f, .0973f);

        float hash12(vec2 p)
        {
            vec3 p3 = fract(new vec3(p.xyx) * HASHSCALE1);
            p3 += dot(p3, p3.yzx + 19.19f);
            return fract((p3.x + p3.y) * p3.z);
        }

        vec2 hash22(vec2 p)
        {
            vec3 p3 = fract(new vec3(p.xyx) * HASHSCALE3);
            p3 += dot(p3, p3.yzx + 19.19f);
            return fract((p3.xx + p3.yz) * p3.zy);

        }

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            float resolution = 10f * exp2(-3f* iMouse.x / iResolution.x);
            vec2 uv = fragCoord.xy / iResolution.y * resolution;
            vec2 p0 = floor(uv);

            vec2 circles = new vec2(0f);
            for (int j = -MAX_RADIUS; j <= MAX_RADIUS; ++j)
            {
                for (int i = -MAX_RADIUS; i <= MAX_RADIUS; ++i)
                {
                    vec2 pi = p0 + new vec2(i, j);
#if DOUBLE_HASH
                    vec2 hsh = hash22(pi);
#else
            vec2 hsh = pi;
#endif
                    vec2 p = pi + hash22(hsh);

                    float t = fract(0.3f * iTime + hash12(hsh));
                    vec2 v = p - uv;
                    float d = length(v) - ((float)(MAX_RADIUS) + 1f) * t;

                    float h = 1e-3f;
                    float d1 = d - h;
                    float d2 = d + h;
                    float p1 = sin(31f* d1) * smoothstep(-0.6f, -0.3f, d1) * smoothstep(0f, -0.3f, d1);
                    float p2 = sin(31f* d2) * smoothstep(-0.6f, -0.3f, d2) * smoothstep(0f, -0.3f, d2);
                    circles += 0.5f * normalize(v) * ((p2 - p1) / (2f * h) * (1f - t) * (1f - t));
                }
            }
            circles /= (float)((MAX_RADIUS * 2 + 1) * (MAX_RADIUS * 2 + 1));

            float intensity = mix(0.01f, 0.15f, smoothstep(0.1f, 0.6f, abs(fract(0.05f * iTime + 0.5f) * 2f- 1f)));
            vec3 n = new vec3(circles, sqrt(1f - dot(circles, circles)));
            vec3 color = texture(iChannel0, uv / resolution - intensity * n.xy).rgb + 5f* pow(clamp(dot(n, normalize(new vec3(1f, 0.7f, 0.5f))), 0f, 1f), 6f);
            fragColor = new vec4(color, 1.0f);
        }


        public override string ToString()
        {
            return "36 Rainier mood";
        }
    }
}
