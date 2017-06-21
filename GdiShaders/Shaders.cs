using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GdiShaders
{

    public class SampleGdiShader : GdiShader
    {
        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            vec2 uv = fragCoord.xy / iResolution.xy;
            fragColor = new vec4(uv, 0.5f + 0.5f * sin(iGlobalTime), 1.0f);
        }

        public override string ToString()
        {
            return "Colors";
        }
    }
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
            float iter = mod(floor(iGlobalTime), 7.0f);
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
                    col *= d_sph(q, fract(iGlobalTime) * s / scale);
                }
            }
            fragColor = new vec4(col, 1f);
        }

        public override string ToString()
        {
            return "Squares";
        }
    }
    public class SampleGdiShader3 : GdiShader
    {
        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            vec2 uv = fragCoord.xy / iResolution.xy;
            float ar = iResolution.x / iResolution.y;
            vec4 temp = new vec4(0.0f, 0.0f, 0.0f, 1.0f);
            vec2 center = new vec2(0.5f, 0.5f);
            vec2 fromCenter = uv - center;
            float angle = atan(fromCenter.y, fromCenter.x);
            fromCenter.x *= ar;
            float fromCenterLength = length(fromCenter);

            temp.r = 0.5f + sin(angle * 1.0f + fromCenterLength * 6.28f * 15.0f - iGlobalTime * 15.0f) * 0.5f;
            temp.r -= fromCenterLength * 2.0f;
            fragColor = temp;
        }

        public override string ToString()
        {
            return "Spiral";
        }
    }
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
            return d + (noise(p + new vec3(.0f, iGlobalTime * 2f, .0f)) + noise(p * 3f) * .5f) * .25f * (p.y);
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
            return "Flame";
        }
    }
    [Obsolete("very slow")]
    public class SampleGdiShader5 : GdiShader
    {
        const float iterations = 17f;
        const float formuparam = 0.53f;

        const float volsteps = 20f;
        const float stepsize = 0.1f;

        const float zoom = 0.800f;
        const float tile = 0.850f;
        const float speed = 0.010f;

        const float brightness = 0.0015f;
        const float darkmatter = 0.300f;
        const float distfading = 0.730f;
        const float saturation = 0.850f;


        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            //get coords and direction
            vec2 uv = fragCoord.xy / iResolution.xy - .5f;
            uv.y *= iResolution.y / iResolution.x;
            vec3 dir = new vec3(uv * zoom, 1f);
            float time = iGlobalTime * speed + .25f;

            //mouse rotation
            float a1 = .5f + iMouse.x / iResolution.x * 2f;
            float a2 = .8f + iMouse.y / iResolution.y * 2f;
            mat2 rot1 = new mat2(cos(a1), sin(a1), -sin(a1), cos(a1));
            mat2 rot2 = new mat2(cos(a2), sin(a2), -sin(a2), cos(a2));
            dir.xz *= rot1;
            dir.xy *= rot2;
            vec3 from = new vec3(1f, .5f, 0.5f);
            from += new vec3(time * 2f, time, -2f);
            from.xz *= rot1;
            from.xy *= rot2;

            //volumetric rendering
            float s = 0.1f, fade = 1f;
            vec3 v = new vec3(0f);
            for (int r = 0; r < volsteps; r++)
            {
                vec3 p = from + s * dir * .5f;
                p = abs(new vec3(tile) - mod(p, new vec3(tile * 2f))); // tiling fold
                float pa, a = pa = 0f;
                for (int i = 0; i < iterations; i++)
                {
                    p = abs(p) / dot(p, p) - formuparam; // the magic formula
                    a += abs(length(p) - pa); // absolute sum of average change
                    pa = length(p);
                }
                float dm = max(0f, darkmatter - a * a * .001f); //dark matter
                a *= a * a; // add contrast
                if (r > 6) fade *= 1f - dm; // dark matter, don't render near
                //v+=vec3(dm,dm*.5,0.);
                v += fade;
                v += new vec3(s, s * s, s * s * s * s) * a * brightness * fade; // coloring based on distance
                fade *= distfading; // distance fading
                s += stepsize;
            }
            v = mix(new vec3(length(v)), v, saturation); //color adjust
            fragColor = new vec4(v * .01f, 1f);

        }

        public override string ToString()
        {
            return "Stars";
        }
    }
    public class SampleGdiShader6 : GdiShader
    {
        vec3 COLOR1 = new vec3(0.0f, 0.0f, 0.3f);
        vec3 COLOR2 = new vec3(0.5f, 0.0f, 0.0f);
        float BLOCK_WIDTH = 0.01f;

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            vec2 uv = fragCoord.xy / iResolution.xy;

            // To create the BG pattern
            vec3 final_color = new vec3(1.0f);
            vec3 bg_color = new vec3(0.0f);
            vec3 wave_color = new vec3(0.0f);

            float c1 = mod(uv.x, 2.0f * BLOCK_WIDTH);
            c1 = step(BLOCK_WIDTH, c1);

            float c2 = mod(uv.y, 2.0f * BLOCK_WIDTH);
            c2 = step(BLOCK_WIDTH, c2);

            bg_color = mix(uv.x * COLOR1, uv.y * COLOR2, c1 * c2);


            // To create the waves
            float wave_width = 0.01f;
            uv = -1.0f + 2.0f * uv;
            uv.y += 0.1f;
            for (float i = 0.0f; i < 10.0f; i++)
            {

                uv.y += (0.07f * sin(uv.x + i / 7.0f + iGlobalTime));
                wave_width = abs(1.0f / (150.0f * uv.y));
                wave_color += new vec3(wave_width * 1.9f, wave_width, wave_width * 1.5f);
            }

            final_color = bg_color + wave_color;


            fragColor = new vec4(final_color, 1.0f);
        }

        public override string ToString()
        {
            return "Lines";
        }
    }
    public class SampleGdiShader7 : GdiShader
    {
        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            vec2 p = (2.0f * fragCoord - iResolution.xy) / min(iResolution.y, iResolution.x);

            p.y -= 0.25f;

            // background color
            vec3 bcol = new vec3(1.0f, 0.8f, 0.7f - 0.07f * p.y) * (1.0f - 0.25f * length(p));

            // animate
            float tt = mod(iGlobalTime, 1.5f) / 1.5f;
            float ss = pow(tt, .2f) * 0.5f + 0.5f;
            ss = 1.0f + ss * 0.5f * sin(tt * 6.2831f * 3.0f + p.y * 0.5f) * exp(-tt * 4.0f);
            p *= new vec2(0.5f, 1.5f) + ss * new vec2(0.5f, -0.5f);


            // shape
            float a = atan(p.x, p.y) / 3.141593f;
            float r = length(p);
            float h = abs(a);
            float d = (13.0f * h - 22.0f * h * h + 10.0f * h * h * h) / (6.0f - 5.0f * h);

            // color
            float s = 0.75f + 0.75f * p.x;
            s *= 1.0f - 0.25f * r;
            s = 0.5f + 0.6f * s;
            s *= 0.5f + 0.5f * pow(1.0f - clamp(r / d, 0.0f, 1.0f), 0.1f);
            vec3 hcol = new vec3(1.0f, 0.5f * r, 0.3f) * s;

            vec3 col = mix(bcol, hcol, smoothstep(-0.01f, 0.01f, d - r));

            fragColor = new vec4(col, 1.0f);
        }

        public override string ToString()
        {
            return "Heart";
        }
    }
    public class SampleGdiShader8 : GdiShader
    {
        const float TAU = 6.28318530718f;
        const int MAX_ITER = 5;

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            float time = iGlobalTime * .5f + 23.0f;
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
            return "Caustic";
        }
    }
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
            float a = abs(n(new vec3(uv + iGlobalTime * 3.14f, sin(iGlobalTime))) - n(new vec3(uv + iGlobalTime, cos(iGlobalTime + 3f))));
            fragColor = new vec4(0, .5f - pow(a, .2f) / 2f, 1f - pow(a, .2f), 1);
        }
    }
    [Obsolete("very slow")]
    public class SampleGdiShader10 : GdiShader
    {
        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            vec2 p = fragCoord / iResolution.xy;

            float time = 30.0f + 0.1f * iGlobalTime;
            vec2 cc = 1.1f * new vec2(0.5f * cos(0.1f * time) - 0.25f * cos(0.2f * time),
                                0.5f * sin(0.1f * time) - 0.25f * sin(0.2f * time));

            vec4 dmin = new vec4(1000.0f);
            vec2 z = (-1.0f + 2.0f * p) * new vec2(1.7f, 1.0f);
            for (int i = 0; i < 64; i++)
            {
                z = cc + new vec2(z.x * z.x - z.y * z.y, 2.0f * z.x * z.y);

                dmin = min(dmin, new vec4(abs(0.0f + z.y + 0.5f * sin(z.x)),
                                    abs(1.0f + z.x + 0.5f * sin(z.y)),
                                    dot(z, z),
                                    length(fract(z) - 0.5f)));
            }

            vec3 color = new vec3(dmin.w);
            color = mix(color, new vec3(1.00f, 0.80f, 0.60f), min(1.0f, pow(dmin.x * 0.25f, 0.20f)));
            color = mix(color, new vec3(0.72f, 0.70f, 0.60f), min(1.0f, pow(dmin.y * 0.50f, 0.50f)));
            color = mix(color, new vec3(1.00f, 1.00f, 1.00f), 1.0f - min(1.0f, pow(dmin.z * 1.00f, 0.15f)));

            color = 1.25f * color * color;

            color *= 0.5f + 0.5f * pow(16.0f * p.x * (1.0f - p.x) * p.y * (1.0f - p.y), 0.15f);

            fragColor = new vec4(color, 1.0f);
        }
    }
    public class SampleGdiShader11 : GdiShader
    {
        float time = iGlobalTime;

        float makePoint(float x, float y, float fx, float fy, float sx, float sy, float t)
        {
            float xx = x + sin(t * fx) * sx;
            float yy = y + cos(t * fy) * sy;
            return 1.0f / sqrt(xx * xx + yy * yy);
        }

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            time = iGlobalTime;

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
    public class SampleGdiShader12 : GdiShader
    {
        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            float sum = 0.0f;
            float size = .0020f;
            vec2 tpos = fragCoord.xy / iResolution.xy - .5f;
            float px, py;
            float scale = 2.0f;
            float basex = -0.5f;
            float basey = 0.0f;
            float x = basex + (iMouse.x - .5f) * scale;
            float y = basey + (iMouse.y - .5f) * scale;
            float t;
            if (true) // change to false to control with mouse
            {
                t = iGlobalTime;
                float t1 = t;
                float scale1 = .3f;
                float t2 = t * .61223f;
                float scale2 = .5f;
                x = basex + scale1 * cos(t1) + scale2 * cos(t2);
                y = basey + scale1 * sin(t1) + scale2 * sin(t2);
            }

            vec2 position = 2.0f * tpos + new vec2(basex, basey);

            int NUM = 30;

            float u, v;
            u = v = .317f;

            for (int j = 0; j < 4; ++j)
            {
                px = py = 0.0f;
                float x0, y0;
                x0 = x + u;
                y0 = y + v;
                for (int i = 0; i < NUM; ++i)
                {
                    t = (px * px - py * py) + x0;
                    py = (2.0f * px * py) + y0;
                    px = t;
                    float dist = length(new vec2(px, py) - position);
                    if (dist > .0001f)
                        sum += size / dist;
                    else break;
                }
                t = u;
                u = -v;
                v = t;
            }

            float val = sum;

            iChannel0.bmp = bmp;

            vec3 color;
            color = new vec3(val, val * 0.66666f, 0.0f);
            tpos *= 1.2f;
            float INDENT = .001f;
            vec3 tcolor;
            if (tpos.x > -.5f + INDENT && tpos.y > -.5f + INDENT &&
                tpos.x < .5f - INDENT && tpos.y < .5f - INDENT)
                tcolor = .9f * texture2D(iChannel0, tpos + .5f).rgb;
            else tcolor = new vec3(0.0f);
            fragColor = new vec4(max(color, tcolor), 1.0f);
        }

        public override string ToString()
        {
            return "^";
        }
    }
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
                pow(worley(p + iGlobalTime), 2) *
                worley(p * 2f + 1.3f + iGlobalTime * .5f) *
                worley(p * 4f + 2.3f + iGlobalTime * .25f) *
                worley(p * 8 + 3.3f + iGlobalTime * .125f) *
                worley(p * 32f + 4.3f + iGlobalTime * .125f) *
                sqrt(worley(p * 64f + 5.3f + iGlobalTime * .0625f)) *
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
    [Obsolete("slow")]
    public class SampleGdiShader14 : GdiShader
    {
        float hash(float n)
        {
            return fract(sin(n) * 43745658.5453123f);
        }

        float noise(vec2 pos)
        {
            return fract(sin(dot(pos * 0.001f, new vec2(24.12357f, 36.789f))) * 12345.123f);
        }

        float noise(float r)
        {
            return fract(sin(dot(new vec2(r, -r) * 0.001f, new vec2(24.12357f, 36.789f))) * 12345.123f);
        }


        float wave(float amplitude, float offset, float frequency, float phase, float t)
        {
            return offset + amplitude * sin(t * frequency + phase);
        }

        float wave(float amplitude, float offset, float frequency, float t)
        {
            return offset + amplitude * sin(t * frequency);
        }

        float wave2(float min, float max, float frequency, float phase, float t)
        {
            float amplitude = max - min;
            return min + 0.5f * amplitude + amplitude * sin(t * frequency + phase);
        }

        float wave2(float min, float max, float frequency, float t)
        {
            float amplitude = max - min;
            return min + 0.5f * amplitude + amplitude * sin(t * frequency);
        }

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            float colorSin = 0.0f;
            float colorLine = 0.0f;
            const int nSini = 50;
            const int nLinei = 30;

            float nSin = (float)(nSini);
            float nLine = (float)(nLinei);

            vec2 mouse = iMouse.xy + new vec2(0.5f * iResolution.x, 0.1f * iResolution.y);
            mouse = mod(mouse, iResolution.xy);
            float line = mouse.y;
            // Sin waves
            for (int ii = 0; ii < nSini; ii++)
            {
                float i = (float)(ii);
                float amplitude = mouse.x * 1.0f * noise(i * 0.2454f) * sin(iGlobalTime + noise(i) * 100.0f);
                float offset = mouse.y;
                float frequency = 0.1f * noise(i * 100.2454f);
                float phase = 0.02f * i * noise(i * 10.2454f) * 10.0f * iGlobalTime * mouse.x / iResolution.x;
                line += i * 0.003f * wave(amplitude, offset, frequency, phase, fragCoord.x);
                colorSin += 0.5f / abs(line - fragCoord.y);
            }

            // Grid	
            for (int ii = 0; ii < nLinei; ii++)
            {
                float i = (float)(ii);
                float lx = (i / nLine) * (iResolution.x + 10.0f);
                float ly = (i / nLine) * (iResolution.y + 10.0f);
                colorLine += 0.07f / abs(fragCoord.x - lx);
                colorLine += 0.07f / abs(fragCoord.y - ly);
            }
            vec3 c = colorSin * new vec3(0.2654f, 0.4578f, 0.654f);
            c += colorLine * new vec3(0.254f, 0.6578f, 0.554f);
            fragColor = new vec4(c, 1.0f);
        }

        public override string ToString()
        {
            return "Lines";
        }
    }
    [Obsolete("slow")]
    public class SampleGdiShader15 : GdiShader
    {
        float t = iGlobalTime;

        mat2 m(float a) { float c = cos(a), s = sin(a); return new mat2(c, -s, s, c); }

        float map(vec3 p)
        {
            p.xz *= m(t * 0.4f); p.xy *= m(t * 0.3f);
            vec3 q = p * 2f + t;
            return length(p + new vec3(sin(t * 0.7f))) * log(length(p) + 1f) + sin(q.x + sin(q.z + sin(q.y))) * 0.5f - 1f;
        }

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            t = iGlobalTime;

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
    }
    public class SampleGdiShader16 : GdiShader
    {
        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            vec2 pos = 256.0f * fragCoord.xy / iResolution.x + iGlobalTime;

            vec3 col = new vec3(0.0f);
            for (int i = 0; i < 6; i++)
            {
                vec2 a = floor(pos);
                vec2 b = fract(pos);

                vec4 w = fract((sin(a.x * 7.0f + 31.0f * a.y + 0.01f * iGlobalTime) + new vec4(0.035f, 0.01f, 0.0f, 0.7f)) * 13.545317f); // randoms

                col += w.xyz *                                   // color
                       smoothstep(0.45f, 0.55f, w.w) *               // intensity
                       sqrt(16.0f * b.x * b.y * (1.0f - b.x) * (1.0f - b.y)); // pattern

                pos /= 2.0f; // lacunarity
                col /= 2.0f; // attenuate high frequencies
            }

            col = pow(2.5f * col, new vec3(1.0f, 1.0f, 0.7f));    // contrast and color shape

            fragColor = new vec4(col, 1.0f);
        }

        public override string ToString()
        {
            return "Tiles";
        }
    }
    [Obsolete("shadow issue")]
    public class SampleGdiShader17 : GdiShader
    {
        vec3 bgCol = new vec3(0.6f, 0.5f, 0.6f);

        //Sets size of the sphere and brightness of the shine
        float sphereScale = 0.7f;
        float sphereShine = 0.5f;

        //Sets diffuse colour(red, green, blue), specular colour(red, green, blue), 
        //and initial specular point position(x, y)
        vec3 sphereDiff = new vec3(0.5f, 0.0f, 0.5f);
        vec3 sphereSpec = new vec3(1.0f, 1.0f, 1.0f);
        vec2 specPoint = new vec2(0.2f, -0.1f);

        //Main method/function
        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            //Creates shader pixel coordinates
            vec2 uv = fragCoord.xy / iResolution.xy;

            //Sets the position of the camera
            vec2 p = uv * 2.3f - 1.0f;
            p.x *= iResolution.x / iResolution.y;

            //Rotates the sphere in a circle
            p.x += cos(-iGlobalTime) * 0.35f;
            p.y += sin(-iGlobalTime) * 0.35f;

            //Rotates the specular point with the sphere
            specPoint.x += cos(-iGlobalTime) * 0.35f;
            specPoint.y += sin(-iGlobalTime) * 0.35f;

            //Sets the radius of the sphere to the middle of the screen
            float radius = sqrt(dot(p, p));

            vec3 col = bgCol;

            //Sets the initial dark shadow around the edge of the sphere
            float f = smoothstep(sphereScale * 0.9f, sphereScale, length(p + specPoint));
            col -= mix(col, new vec3(0.0f), f) * 0.2f;

            //Only carries out the logic if the radius of the sphere is less than the scale
            if (radius < sphereScale)
            {
                vec3 bg = col;

                //Sets the diffuse colour of the sphere (solid colour)
                col = sphereDiff;

                //Adds smooth dark borders to help achieve 3D look
                f = smoothstep(sphereScale * 0.7f, sphereScale, radius);
                col = mix(col, sphereDiff * 0.45f, f);

                //Adds specular glow to help achive 3D look
                f = 1.0f - smoothstep(-0.2f, 0.6f, length(p - specPoint));
                col += f * sphereShine * sphereSpec;

                //Smoothes the edge of the sphere
                f = smoothstep(sphereScale - 0.01f, sphereScale, radius);
                col = mix(col, bg, f);
            }

            //The final output of the shader logic above
            //fragColor is a vector with 4 paramaters(red, green, blue, alpha)
            //Only 2 need to be used here, as "col" is a vector that already carries r, g, and b values
            fragColor = new vec4(col, 1);
        }

        public override string ToString()
        {
            return "Sphere";
        }
    }
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
            float shift = 1.327f + sin(iGlobalTime * 2.0f) / 2.4f;
            float alpha = 1.0f;

            float dist = 3.5f - sin(iGlobalTime * 0.4f) / 1.89f;

            vec2 uv = fragCoord.xy / iResolution.xy;
            vec2 p = fragCoord.xy * dist / iResolution.xx;
            p += sin(p.yx * 4.0f + new vec2(.2f, -.3f) * iGlobalTime) * 0.04f;
            p += sin(p.yx * 8.0f + new vec2(.6f, +.1f) * iGlobalTime) * 0.01f;

            p.x -= iGlobalTime / 1.1f;
            float q = fbm(p - iGlobalTime * 0.3f + 1.0f * sin(iGlobalTime + 0.5f) / 2.0f);
            float qb = fbm(p - iGlobalTime * 0.4f + 0.1f * cos(iGlobalTime) / 2.0f);
            float q2 = fbm(p - iGlobalTime * 0.44f - 5.0f * cos(iGlobalTime) / 2.0f) - 6.0f;
            float q3 = fbm(p - iGlobalTime * 0.9f - 10.0f * cos(iGlobalTime) / 15.0f) - 4.0f;
            float q4 = fbm(p - iGlobalTime * 1.4f - 20.0f * sin(iGlobalTime) / 14.0f) + 2.0f;
            q = (q + qb - .4f * q2 - 2.0f * q3 + .6f * q4) / 3.8f;
            vec2 r = new vec2(fbm(p + q / 2.0f + iGlobalTime * speed.x - p.x - p.y), fbm(p + q - iGlobalTime * speed.y));
            vec3 c = mix(c1, c2, fbm(p + r)) + mix(c3, c4, r.x) - mix(c5, c6, r.y);
            vec3 color = 1.0f / (pow(c + 1.61f, new vec3(4.0f))) * cos(shift * fragCoord.y / iResolution.y);

            color = new vec3(1.0f, .2f, .05f) / (pow((r.y + r.y) * max(.0f, p.y) + 0.1f, 4.0f)); ;
            color += (texture2D(iChannel0, uv * 0.6f + new vec2(.5f, .1f)).xyz * 0.01f * pow((r.y + r.y) * .65f, 5.0f) + 0.055f) * mix(new vec3(.9f, .4f, .3f), new vec3(.7f, .5f, .2f), uv.y);
            color = color / (1.0f + max(new vec3(0), color));
            fragColor = new vec4(color.x, color.y, color.z, alpha);
        }

        public override string ToString()
        {
            return "301's Fire Shader";
        }
    }
    [Obsolete("height issue, slow")]
    public class SampleGdiShader19 : GdiShader
    {
        // https://www.shadertoy.com/view/4dG3RK

        // ray computation vars
        const float PI = 3.14159265359f;
        const float fov = 50.0f;
        const float fovx = PI * fov / 360.0f;
        float fovy = 0;
        float ulen = 0;
        float vlen = 0;

        public SampleGdiShader19()
        {
            ulen = tan(fovx);
            vlen = tan(fovy);
        }

        // epsilon-type values
        const float S = 0.01f;
        const float EPSILON = 0.01f;

        // const delta vectors for normal calculation
        vec3 deltax = new vec3(S, 0, 0);
        vec3 deltay = new vec3(0, S, 0);
        vec3 deltaz = new vec3(0, 0, S);

        float distanceToNearestSurface(vec3 p)
        {
            float s = 1.0f;
            vec3 d = abs(p) - new vec3(s);
            return min(max(d.x, max(d.y, d.z)), 0.0f)
                + length(max(d, 0.0f));
        }


        // better normal implementation with half the sample points
        // used in the blog post method
        vec3 computeSurfaceNormal(vec3 p)
        {
            float d = distanceToNearestSurface(p);
            return normalize(new vec3(
                distanceToNearestSurface(p + deltax) - d,
                distanceToNearestSurface(p + deltay) - d,
                distanceToNearestSurface(p + deltaz) - d
            ));
        }


        vec3 computeLambert(vec3 p, vec3 n, vec3 l)
        {
            return new vec3(dot(normalize(l - p), n));
        }

        vec3 intersectWithWorld(vec3 p, vec3 dir)
        {
            float dist = 0.0f;
            float nearest = 0.0f;
            vec3 result = new vec3(0.0f);
            for (int i = 0; i < 20; i++)
            {
                nearest = distanceToNearestSurface(p + dir * dist);
                if (nearest < EPSILON)
                {
                    vec3 hit = p + dir * dist;
                    vec3 light = new vec3(100.0f * sin(iGlobalTime), 30.0f * cos(iGlobalTime), 50.0f * cos(iGlobalTime));
                    result = computeLambert(hit, computeSurfaceNormal(hit), light);
                    break;
                }
                dist += nearest;
            }
            return result;
        }

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            fovy = fovx * iResolution.y / iResolution.x;

            vec2 uv = fragCoord / iResolution.xy;

            float cameraDistance = 10.0f;
            vec3 cameraPosition = new vec3(10.0f * sin(iGlobalTime), 0.0f, 10.0f * cos(iGlobalTime));
            vec3 cameraDirection = new vec3(-1.0f * sin(iGlobalTime), 0.0f, -1.0f * cos(iGlobalTime));
            vec3 cameraUp = new vec3(0.0f, 1.0f, 0.0f);

            // generate the ray for this pixel
            vec2 camUV = uv * 2.0f - new vec2(1.0f, 1.0f);
            vec3 nright = normalize(cross(cameraUp, cameraDirection));
            vec3 pixel = cameraPosition + cameraDirection + nright * camUV.x * ulen + cameraUp * camUV.y * vlen;
            vec3 rayDirection = normalize(pixel - cameraPosition);

            vec3 pixelColour = intersectWithWorld(cameraPosition, rayDirection);
            fragColor = new vec4(pixelColour, 1.0f);
        }

        public override string ToString()
        {
            return "Sphere Tracing 103";
        }
    }

    public class SampleGdiShader20 : GdiShader
    {
        vec4 circle(vec2 p, vec2 center, float radius)
        {
            return mix(new vec4(1, 1, 1, 0), new vec4(1, 0, 0, 1), smoothstep(radius + 0.005f, radius - 0.005f, length(p - center)));
        }

        vec4 scene(vec2 uv, float t)
        {
            return circle(uv, new vec2(0, sin(t * 16.0f) * (sin(t) * 0.5f + 0.5f) * 0.5f), 0.2f);
        }

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            vec2 resol = iResolution.xy / new vec2(6, 1);
            vec2 coord = mod(fragCoord.xy, resol);
            float view = floor(fragCoord.x / resol.x);

            vec2 uv = coord / resol;
            uv = uv * 2.0f - new vec2(1);
            uv.x *= resol.x / resol.y;

            fragColor = new vec4(1, 1, 1, 1);

            float frametime = (60f / (floor(view / 2f) + 1f));
            float time = floor((iGlobalTime + 3f) * frametime) / frametime;
            vec4 mainCol = scene(uv, time);

            vec4 blurCol = new vec4(0, 0, 0, 0);
            for (int i = 0; i < 32; i++)
            {
                if ((i < 8 || view >= 2.0f) && (i < 16 || view >= 4.0f))
                {
                    blurCol += scene(uv, time - (float)i * (1f / 15f / 32f));
                }
            }
            blurCol /= pow(2f, floor(view / 2f) + 3f);

            if (mod(view, 2f) == 0f)
                fragColor = mainCol;
            else
                fragColor = blurCol;

            if (iMouse.z > 0f && mod(view, 2f) == mod(floor(iMouse.z / resol.x), 2f))
                fragColor = new vec4(0, 0, 0, 1);
        }
    }

    public class SampleGdiShader21 : GdiShader
    {
        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            fragColor = new vec4(0, 0, 0, fragCoord.y / iResolution.y);
        }

        public override string ToString()
        {
            return "Alpha fade";
        }
    }
    public class SampleGdiShader22 : GdiShader
    {
        float RoundRect(vec2 distFromCenter, vec2 halfSize, float radius)
        {
            return clamp(length(max(abs(distFromCenter) - (halfSize - radius), new vec2(0.0f))) - radius, 0.0f, 1.0f);
        }

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            vec2 st = fragCoord.xy;

            vec2 size = new vec2(iResolution.x, iResolution.y);
            vec2 halfSize = size / 2.0f;
            vec2 rectCenter = halfSize;

            float radius = 12f;
            float pct = RoundRect(st - rectCenter, halfSize, radius);

            var rectColor = new vec4(.5f, .5f, .5f, 1);

            fragColor = mix(rectColor, new vec4(0.0f), pct);
        }

        public override string ToString()
        {
            return "Roundrect";
        }
    }
}
