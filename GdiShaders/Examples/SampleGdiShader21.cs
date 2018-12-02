namespace GdiShaders.Examples
{
    /// <summary>
    /// https://www.shadertoy.com/view/llVBWh
    /// </summary>
    public class SampleGdiShader21 : GdiShader
    {
        vec4 C = new vec4(0.211324865405187f, 0.366025403784439f, -0.577350269189626f, 0.024390243902439f);

        // Simplex 2D noise
        vec3 permute(vec3 x) { return mod(((x * 34.0f) + 1.0f) * x, 289.0f); }

        float snoise(vec2 v)
        {
            vec2 i = floor(v + dot(v, C.yy));
            vec2 x0 = v - i + dot(i, C.xx);
            vec2 i1;
            i1 = (x0.x > x0.y) ? new vec2(1.0f, 0.0f) : new vec2(0.0f, 1.0f);
            vec4 x12 = x0.xyxy + C.xxzz;
            x12.xy -= i1;
            i = mod(i, 289.0f);
            vec3 p = permute(permute(i.y + new vec3(0.0f, i1.y, 1.0f))
                             + i.x + new vec3(0.0f, i1.x, 1.0f));
            vec3 m = max(0.5f - new vec3(dot(x0, x0), dot(x12.xy, x12.xy), dot(x12.zw, x12.zw)), 0.0f);
            m = m * m;
            m = m * m;
            vec3 x = 2.0f * fract(p * C.www) - 1.0f;
            vec3 h = abs(x) - 0.5f;
            vec3 ox = floor(x + 0.5f);
            vec3 a0 = x - ox;
            m *= 1.79284291400159f - 0.85373472095314f * (a0 * a0 + h * h);
            vec3 g = new vec3();
            g.x = a0.x * x0.x + h.x * x0.y;
            g.yz = a0.yz * x12.xz + h.yz * x12.yw;
            return 130.0f * dot(m, g);
        }


        float octavenoise(vec2 uv)
        {
            float v0 = 1.000f * snoise(uv * 1.0f);
            float v1 = 0.500f * snoise(uv * 2.0f);
            float v2 = 0.250f * snoise(uv * 4.0f);
            float v3 = 0.125f * snoise(uv * 8.0f);
            return (v0 + v1 + v2 + v3) / 1.875f;
        }


        /* discontinuous pseudorandom uniformly distributed in [-0.5, +0.5]^3 */
        vec3 random3(vec3 c)
        {
            float j = 4096.0f * sin(dot(c, new vec3(17.0f, 59.4f, 15.0f)));
            vec3 r;
            r.z = fract(512.0f * j);
            j *= .125f;
            r.x = fract(512.0f * j);
            j *= .125f;
            r.y = fract(512.0f * j);
            return r - 0.5f;
        }

        const float F3 = 0.3333333f;
        const float G3 = 0.1666667f;
        float       snoise3(vec3 p)
        {

            vec3 s = floor(p + dot(p, new vec3(F3)));
            vec3 x = p - s + dot(s, new vec3(G3));

            vec3 e = step(new vec3(0.0f), x - x.yzx);
            vec3 i1 = e * (1.0f - e.zxy);
            vec3 i2 = 1.0f - e.zxy * (1.0f - e);

            vec3 x1 = x - i1 + G3;
            vec3 x2 = x - i2 + 2.0f * G3;
            vec3 x3 = x - 1.0f + 3.0f * G3;

            vec4 w, d;

            w.x = dot(x, x);
            w.y = dot(x1, x1);
            w.z = dot(x2, x2);
            w.w = dot(x3, x3);

            w = max(0.6f - w, 0.0f);

            d.x = dot(random3(s), x);
            d.y = dot(random3(s + i1), x1);
            d.z = dot(random3(s + i2), x2);
            d.w = dot(random3(s + 1.0f), x3);

            w *= w;
            w *= w;
            d *= w;

            return dot(d, new vec4(52.0f));
        }


        // Sample 9 locations, and count how many are on water.
        int watercount(vec2 uv)
        {
            const float e = 0.017f;
            vec2[] delta = new vec2[9];

            delta[0] = new vec2(-e, -e);
            delta[1] = new vec2(0, -e);
            delta[2] = new vec2(e, -e);

            delta[3] = new vec2(-e, 0);
            delta[4] = new vec2(0, 0);
            delta[5] = new vec2(e, 0);

            delta[6] = new vec2(-e, e);
            delta[7] = new vec2(0, e);
            delta[8] = new vec2(e, e);

            int numwater = 0;
            for (int i = 0; i < 9; ++i)
            {
                vec2 s = uv + delta[i];
                float v = octavenoise(s);
                if (v > 0.0)
                    numwater += 1;
            }
            return numwater;
        }

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            vec2 uv = fragCoord / iResolution.xy;

            // If you count the samples that fall on land, you'll know if it is inland, out on sea, or coastal.
            int numw = watercount(uv);

            // We'll assume it is coastal for now, and use whitish to render surf.
            vec3 col = new vec3(0.9f, 0.9f, 1);

            float cutoff = 5.0f + snoise(new vec2(iTime, iTime) + uv * 60.0f) * 4.0f;

            if (numw == 0)
            {
                // No water samples: firmly in-land.
                col = new vec3(0.2f, 0.6f, 0.2f);
            }
            else if (numw > (int)cutoff)
            {
                // 'Many' water samples: firmly out on sea:
                col = new vec3(0.4f, 0.54f, 0.9f);
                float tgt = 0.0f;
                float wave = snoise3(new vec3(uv * 50.0f, iTime));
                float diff = abs(tgt - wave);
                diff = diff * 12.0f;
                diff = min(diff, 1.0f);
                col = mix(new vec3(1, 1, 1), col, diff);
            }

            fragColor = new vec4(col, 1.0f);
        }

        public override string ToString()
        {
            return "21 Watery World";
        }
    }
}