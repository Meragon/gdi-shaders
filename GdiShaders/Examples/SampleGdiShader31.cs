namespace GdiShaders.Examples
{
    using GdiShaders.Core;

    /// <summary>
    /// https://www.shadertoy.com/view/Ms2SD1
    /// Works, but camera moves in oposite direction.
    /// </summary>
    public class SampleGdiShader31 : GdiShader
    {
        /*
 * "Seascape" by Alexander Alekseev aka TDM - 2014
 * License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
 * Contact: tdmaav@gmail.com
 */

        const int NUM_STEPS = 8;
        const float PI = 3.141592f;
        const float EPSILON = 1E-3f;
        private float EPSILON_NRM;

        // sea
        const int ITER_GEOMETRY = 3;
        const int ITER_FRAGMENT = 5;
        const float SEA_HEIGHT = 0.6f;
        const float SEA_CHOPPY = 4.0f;
        const float SEA_SPEED = 0.8f;
        const float SEA_FREQ = 0.16f;
        vec3 SEA_BASE = new vec3(0.1f, 0.19f, 0.22f);
        vec3 SEA_WATER_COLOR = new vec3(0.8f, 0.9f, 0.6f);
        private float SEA_TIME;
        mat2 octave_m = new mat2(1.6f, 1.2f, -1.2f, 1.6f);

        // math
        mat3 fromEuler(vec3 ang)
        {
            vec2 a1 = new vec2(sin(ang.x), cos(ang.x));
            vec2 a2 = new vec2(sin(ang.y), cos(ang.y));
            vec2 a3 = new vec2(sin(ang.z), cos(ang.z));
            mat3 m = new mat3();
            m[0] = new vec3(a1.y * a3.y + a1.x * a2.x * a3.x, a1.y * a2.x * a3.x + a3.y * a1.x, -a2.y * a3.x);
            m[1] = new vec3(-a2.y * a1.x, a1.y * a2.y, a2.x);
            m[2] = new vec3(a3.y * a1.x * a2.x + a1.y * a3.x, a1.x * a3.x - a1.y * a3.y * a2.x, a2.y * a3.y);
            return m;
        }
        float hash(vec2 p)
        {
            float h = dot(p, new vec2(127.1f, 311.7f));
            return fract(sin(h) * 43758.5453123f);
        }
        float noise(vec2 p)
        {
            vec2 i = floor(p);
            vec2 f = fract(p);
            vec2 u = f * f * (3.0f - 2.0f * f);
            return -1.0f + 2.0f * mix(mix(hash(i + new vec2(0.0f, 0.0f)),
                                          hash(i + new vec2(1.0f, 0.0f)), u.x),
                                      mix(hash(i + new vec2(0.0f, 1.0f)),
                                          hash(i + new vec2(1.0f, 1.0f)), u.x), u.y);
        }

        // lighting
        float diffuse(vec3 n, vec3 l, float p)
        {
            return pow(dot(n, l) * 0.4f + 0.6f, p);
        }
        float specular(vec3 n, vec3 l, vec3 e, float s)
        {
            float nrm = (s + 8.0f) / (PI * 8.0f);
            return pow(max(dot(reflect(e, n), l), 0.0f), s) * nrm;
        }

        // sky
        vec3 getSkyColor(vec3 e)
        {
            e.y = max(e.y, 0.0f);
            return new vec3(pow(1.0f - e.y, 2.0f), 1.0f - e.y, 0.6f + (1.0f - e.y) * 0.4f);
        }

        // sea
        float sea_octave(vec2 uv, float choppy)
        {
            uv += noise(uv);
            vec2 wv = 1.0f - abs(sin(uv));
            vec2 swv = abs(cos(uv));
            wv = mix(wv, swv, wv);
            return pow(1.0f - pow(wv.x * wv.y, 0.65f), choppy);
        }

        float map(vec3 p)
        {
            float freq = SEA_FREQ;
            float amp = SEA_HEIGHT;
            float choppy = SEA_CHOPPY;
            vec2 uv = p.xz; uv.x *= 0.75f;

            float d, h = 0.0f;
            for (int i = 0; i < ITER_GEOMETRY; i++)
            {
                d = sea_octave((uv + SEA_TIME) * freq, choppy);
                d += sea_octave((uv - SEA_TIME) * freq, choppy);
                h += d * amp;
                uv *= octave_m; freq *= 1.9f; amp *= 0.22f;
                choppy = mix(choppy, 1.0f, 0.2f);
            }
            return p.y - h;
        }

        float map_detailed(vec3 p)
        {
            float freq = SEA_FREQ;
            float amp = SEA_HEIGHT;
            float choppy = SEA_CHOPPY;
            vec2 uv = p.xz; uv.x *= 0.75f;

            float d, h = 0.0f;
            for (int i = 0; i < ITER_FRAGMENT; i++)
            {
                d = sea_octave((uv + SEA_TIME) * freq, choppy);
                d += sea_octave((uv - SEA_TIME) * freq, choppy);
                h += d * amp;
                uv *= octave_m; freq *= 1.9f; amp *= 0.22f;
                choppy = mix(choppy, 1.0f, 0.2f);
            }
            return p.y - h;
        }

        vec3 getSeaColor(vec3 p, vec3 n, vec3 l, vec3 eye, vec3 dist)
        {
            float fresnel = clamp(1.0f - dot(n, -eye), 0.0f, 1.0f);
            fresnel = pow(fresnel, 3.0f) * 0.65f;

            vec3 reflected = getSkyColor(reflect(eye, n));
            vec3 refracted = SEA_BASE + diffuse(n, l, 80.0f) * SEA_WATER_COLOR * 0.12f;

            vec3 color = mix(refracted, reflected, fresnel);

            float atten = max(1.0f - dot(dist, dist) * 0.001f, 0.0f);
            color += SEA_WATER_COLOR * (p.y - SEA_HEIGHT) * 0.18f * atten;

            color += new vec3(specular(n, l, eye, 60.0f));

            return color;
        }

        // tracing
        vec3 getNormal(vec3 p, float eps)
        {
            vec3 n;
            n.y = map_detailed(p);
            n.x = map_detailed(new vec3(p.x + eps, p.y, p.z)) - n.y;
            n.z = map_detailed(new vec3(p.x, p.y, p.z + eps)) - n.y;
            n.y = eps;
            return normalize(n);
        }

        float heightMapTracing(vec3 ori, vec3 dir, ref vec3 p)
        {
            float tm = 0.0f;
            float tx = 1000.0f;
            float hx = map(ori + dir * tx);
            if (hx > 0.0f) return tx;
            float hm = map(ori + dir * tm);
            float tmid = 0.0f;
            for (int i = 0; i < NUM_STEPS; i++)
            {
                tmid = mix(tm, tx, hm / (hm - hx));
                p = ori + dir * tmid;
                float hmid = map(p);
                if (hmid < 0.0)
                {
                    tx = tmid;
                    hx = hmid;
                }
                else
                {
                    tm = tmid;
                    hm = hmid;
                }
            }
            return tmid;
        }

        // main
        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            EPSILON_NRM = (0.1f / iResolution.x);
            SEA_TIME = (1.0f + iTime * SEA_SPEED);

            vec2 uv = fragCoord.xy / iResolution.xy;
            uv = uv * 2.0f - 1.0f;
            uv.x *= iResolution.x / iResolution.y;
            float time = iTime * 0.3f + iMouse.x * 0.01f;

            // ray
            vec3 ang = new vec3(sin(time * 3.0f) * 0.1f, sin(time) * 0.2f + 0.3f, time);
            vec3 ori = new vec3(0.0f, 3.5f, time * 5.0f);
            vec3 dir = normalize(new vec3(uv.xy, -2.0f)); dir.z += length(uv) * 0.15f;
            dir = normalize(dir) * fromEuler(ang);

            // tracing
            vec3 p = new vec3();
            heightMapTracing(ori, dir, ref p);
            vec3 dist = p - ori;
            vec3 n = getNormal(p, dot(dist, dist) * EPSILON_NRM);
            vec3 light = normalize(new vec3(0.0f, 1.0f, 0.8f));

            // color
            vec3 color = mix(
                getSkyColor(dir),
                getSeaColor(p, n, light, dir, dist),
                pow(smoothstep(0.0f, -0.05f, dir.y), 0.3f));

            // post
            fragColor = new vec4(pow(color, new vec3(0.75f)), 1.0f);
        }

        public override string ToString()
        {
            return "31 Seascape";
        }
    }
}
