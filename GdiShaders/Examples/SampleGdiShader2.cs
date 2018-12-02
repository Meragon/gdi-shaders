namespace GdiShaders.Examples
{
    using System;

    [Obsolete("almost working")]
    public class SampleGdiShader2 : GdiShader
    {
        float s = 0.95f, a = 0.006f, scale = 3f;

        float d_box(vec2 q)
        {
            float d = max(abs(q.x), abs(q.y));
            return smoothstep(s, s + a, d) + smoothstep(s - a, s - a - a, d);
        }
        float d_sph(vec2 q, float s)
        {
            float d = pow(pow(q.x, 3.0f) + pow(q.y, 3.0f), 1f / 3f);
            return smoothstep(s, s + a, d) + smoothstep(s - a, s - a - a, d);
        }

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            s = 0.95f; a = 0.006f; scale = 3f;

            vec2 q = (-iResolution.xy + 2.0f * fragCoord) / iResolution.y;
            vec3 col = new vec3(1.0f) * d_box(q);
            float iter = mod(floor(iTime), 7.0f);
            for (int i = 0; i < 4; ++i)
            {
                if (i < (int)(iter + 0.5f))
                {
                    q = abs(q);
                    if (q.x > q.y) q.xy = q.yx;
                    s *= 1.0f / scale;
                    col *= d_sph(q, s);
                    q -= s * (scale - 1f);
                    if (q.x < -0.5 * s * (scale - 1.0)) q.x += s * (scale - 1.0f);
                }
                else
                {
                    col *= d_sph(q, fract(iTime) * s / scale);
                }
            }
            fragColor = new vec4(col, 1f);
        }

        public override string ToString()
        {
            return "2 Squares";
        }
    }
}