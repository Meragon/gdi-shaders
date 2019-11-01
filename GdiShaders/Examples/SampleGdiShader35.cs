namespace GdiShaders.Examples
{
    using System;

    using GdiShaders.Core;

    /// <summary>
    /// https://www.shadertoy.com/view/4dXGR4
    /// </summary>
    [Obsolete("Something wrong with sampling")]
    public class SampleGdiShader35 : GdiShader
    {
        public override void Start()
        {
            base.Start();

            iChannel0 = samplerXX.FromImage(ImageNames.Organic2);
            iChannel0.WrapMode = WrapModes.Repeat;
        }

        // based on https://www.shadertoy.com/view/lsf3RH by
        // trisomie21 (THANKS!)
        // My apologies for the ugly code.

        float snoise(vec3 uv, float res)    // by trisomie21
        {
            vec3 s = new vec3(1e0f, 1e2f, 1e4f);

            uv *= res;

            vec3 uv0 = floor(mod(uv, res)) * s;
            vec3 uv1 = floor(mod(uv + new vec3(1f), res)) * s;

            vec3 f = fract(uv); f = f * f * (3.0f - 2.0f * f);

            vec4 v = new vec4(uv0.x + uv0.y + uv0.z, uv1.x + uv0.y + uv0.z,
                uv0.x + uv1.y + uv0.z, uv1.x + uv1.y + uv0.z);

            vec4 r = fract(sin(v * 1e-3f) * 1e5f);
            float r0 = mix(mix(r.x, r.y, f.x), mix(r.z, r.w, f.x), f.y);

            r = fract(sin((v + uv1.z - uv0.z) * 1e-3f) * 1e5f);
            float r1 = mix(mix(r.x, r.y, f.x), mix(r.z, r.w, f.x), f.y);

            return mix(r0, r1, f.z) * 2f - 1f;
        }

        float[] freqs = new float[4];

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            freqs[0] = texture(iChannel1, new vec2(0.01f, 0.25f)).x;
            freqs[1] = texture(iChannel1, new vec2(0.07f, 0.25f)).x;
            freqs[2] = texture(iChannel1, new vec2(0.15f, 0.25f)).x;
            freqs[3] = texture(iChannel1, new vec2(0.30f, 0.25f)).x;

            float brightness = freqs[1] * 0.25f + freqs[2] * 0.25f;
            float radius = 0.24f + brightness * 0.2f;
            float invRadius = 1.0f / radius;

            vec3 orange = new vec3(0.8f, 0.65f, 0.3f);
            vec3 orangeRed = new vec3(0.8f, 0.35f, 0.1f);
            float time = iTime * 0.1f;
            float aspect = iResolution.x / iResolution.y;
            vec2 uv = fragCoord.xy / iResolution.xy;
            vec2 p = -0.5f + uv;
            p.x *= aspect;

            float fade = pow(length(2.0f * p), 0.5f);
            float fVal1 = 1.0f - fade;
            float fVal2 = 1.0f - fade;

            float angle = atan(p.x, p.y) / 6.2832f;
            float dist = length(p);
            vec3 coord = new vec3(angle, dist, time * 0.1f);

            float newTime1 = abs(snoise(coord + new vec3(0.0f, -time * (0.35f + brightness * 0.001f), time * 0.015f), 15.0f));
            float newTime2 = abs(snoise(coord + new vec3(0.0f, -time * (0.15f + brightness * 0.001f), time * 0.015f), 45.0f));
            for (int i = 1; i <= 7; i++)
            {
                float power = pow(2.0f, (float)(i + 1));
                fVal1 += (0.5f / power) * snoise(coord + new vec3(0.0f, -time, time * 0.2f), (power * (10.0f) * (newTime1 + 1.0f)));
                fVal2 += (0.5f / power) * snoise(coord + new vec3(0.0f, -time, time * 0.2f), (power * (25.0f) * (newTime2 + 1.0f)));
            }

            float corona = pow(fVal1 * max(1.1f - fade, 0.0f), 2.0f) * 50.0f;
            corona += pow(fVal2 * max(1.1f - fade, 0.0f), 2.0f) * 50.0f;
            corona *= 1.2f - newTime1;
            vec3 sphereNormal = new vec3(0.0f, 0.0f, 1.0f);
            vec3 dir = new vec3(0.0f);
            vec3 center = new vec3(0.5f, 0.5f, 1.0f);
            vec3 starSphere = new vec3(0.0f);

            vec2 sp = -1.0f + 2.0f * uv;
            sp.x *= aspect;
            sp *= (2.0f - brightness);
            float r = dot(sp, sp);
            float f = (1.0f - sqrt(abs(1.0f - r))) / (r) + brightness * 0.5f;
            if (dist < radius)
            {
                corona *= pow(dist * invRadius, 24.0f);
                vec2 newUv;
                newUv.x = sp.x * f;
                newUv.y = sp.y * f;
                newUv += new vec2(time, 0.0f);

                vec3 texSample = texture(iChannel0, newUv).rgb;
                float uOff = (texSample.g * brightness * 4.5f + time);
                vec2 starUV = newUv + new vec2(uOff, 0.0f);
                starSphere = texture(iChannel0, starUV).rgb;
            }

            float starGlow = min(max(1.0f - dist * (1.0f - brightness), 0.0f), 1.0f);
            //fragColor.rgb	= vec3( r );
            fragColor = new vec4(0);
            fragColor.rgb = new vec3(f * (0.75f + brightness * 0.3f) * orange) + starSphere + corona * orange + starGlow * orangeRed;
            fragColor.a = 1.0f;
        }

        public override string ToString()
        {
            return "35 Main Sequence Star";
        }
    }
}
