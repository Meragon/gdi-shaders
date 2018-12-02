namespace GdiShaders.Examples
{
    using System;

    /// <summary>
    /// https://www.shadertoy.com/view/MscBRs
    /// Glow is not working: L54 commented.
    /// </summary>
    [Obsolete("Not fully working")]
    public class SampleGdiShader25 : GdiShader
    {
        // @lsdlive

        // This was my shader for the shader showdown at Outline demoparty 2018 in Nederland.
        // Shader showdown is a live-coding competition where two participants are
        // facing each other during 25 minutes.
        // (Round 1)

        // I don't have access to the code I typed at the event, so it might be
        // slightly different.

        // Original algorithm on shadertoy from fb39ca4: https://www.shadertoy.com/view/4dX3zl
        // I used the implementation from shane: https://www.shadertoy.com/view/MdVSDh

        // Thanks to shadertoy community & shader showdown paris.

        // This is under CC-BY-NC-SA (shadertoy default licence)


        mat2 r2d(float a)
        {
            float c = cos(a), s = sin(a);
            return new mat2(c, s, -s, c);
        }

        vec2 path(float t)
        {
            float a = sin(t * .2f + 1.5f), b = sin(t * .2f);
            return new vec2(2f * a, a * b);
        }

        float g = 0f;
        float de(vec3 p)
        {
            p.xy -= path(p.z);

            float d = -length(p.xy) + 4f; // tunnel (inverted cylinder)

            p.xy += new vec2(cos(p.z + iTime) * sin(iTime), cos(p.z + iTime));
            p.z -= 6f + iTime * 6f;
            d = min(d, dot(p, normalize(sign(p))) - 1f); // octahedron (LJ's formula)
                                                         // I added this in the last 1-2 minutes, but I'm not sure if I like it actually!

            // Trick inspired by balkhan's shadertoys.
            // Usually, in raymarch shaders it gives a glow effect,
            // here, it gives a colors patchwork & transparent voxels effects.
            //g += .015f / (.01f + d * d); // TODO: this one is not working, so it's commented.
            return d;
        }

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            vec2 uv = fragCoord / iResolution.xy - .5f;
            uv.x *= iResolution.x / iResolution.y;

            float dt = iTime * 6f;
            vec3 ro = new vec3(0, 0, -5f + dt);
            vec3 ta = new vec3(0, 0, dt);

            ro.xy += path(ro.z);
            ta.xy += path(ta.z);

            vec3 fwd = normalize(ta - ro);
            vec3 right = cross(fwd, new vec3(0, 1, 0));
            vec3 up = cross(right, fwd);
            vec3 rd = normalize(fwd + uv.x * right + uv.y * up);

            rd.xy *= r2d(sin(-ro.x / 3.14f) * .3f);

            // Raycast in 3d to get voxels.
            // Algorithm fully explained here in 2D (just look at dde algo):
            // http://lodev.org/cgtutor/raycasting.html
            // Basically, tracing a ray in a 3d grid space, and looking for 
            // each voxel (think pixel with a third dimension) traversed by the ray.
            vec3 p = floor(ro) + .5f;
            vec3 mask = new vec3();
            vec3 drd = 1f / abs(rd);
            rd = sign(rd);
            vec3 side = drd * (rd * (p - ro) + .5f);

            float t = 0f, ri = 0f;
            for (float i = 0f; i < 1f; i += .01f)
            {
                ri = i;

                /*
                // sphere tracing algorithm (for comparison)
                p = ro + rd * t;
                float d = de(p);
                if(d<.001) break;
                t += d;
                */

                if (de(p) < 0f) break;// distance field
                                      // we test if we are inside the surface

                mask = step(side, side.yzx) * step(side, side.zxy);
                // minimum value between x,y,z, output 0 or 1

                side += drd * mask;
                p += rd * mask;
            }
            t = length(p - ro);

            vec3 c = new vec3(1) * length(mask * new vec3(1f, .5f, .75f));
            c = mix(new vec3(.2f, .2f, .7f), new vec3(.2f, .1f, .2f), c);
            c += g * .4f;
            c.r += sin(iTime) * .2f + sin(p.z * .5f - iTime * 6f);// red rings
            c = mix(c, new vec3(.2f, .1f, .2f), 1f - exp(-.001f * t * t));// fog

            fragColor = new vec4(c, 1.0f);
        }

        public override string ToString()
        {
            return "25 Outline #1: Voxel tunnel";
        }
    }
}
