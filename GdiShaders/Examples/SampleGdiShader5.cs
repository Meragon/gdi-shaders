namespace GdiShaders.Examples
{
    using System;

    [Obsolete("very slow")]
    public class SampleGdiShader5 : GdiShader
    {
        const float iterations = 17f;
        const float formuparam = 0.53f;

        const float volsteps = 20f;
        const float stepsize = 0.1f;

        const float zoom  = 0.800f;
        const float tile  = 0.850f;
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
            float time = iTime * speed + .25f;

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
}