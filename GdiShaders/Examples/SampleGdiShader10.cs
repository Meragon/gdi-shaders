namespace GdiShaders.Examples
{
    using System;
    
    public class SampleGdiShader10 : GdiShader
    {
        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            vec2 p = fragCoord / iResolution.xy;

            float time = 30.0f + 0.1f * iTime;
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

        public override string ToString()
        {
            return "10 Fractal";
        }
    }
}