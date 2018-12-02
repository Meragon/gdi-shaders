namespace GdiShaders.Examples
{
    /// <summary>
    /// https://www.shadertoy.com/view/XtGGRt
    /// </summary>
    public class SampleGdiShader34 : GdiShader
    {
        //Auroras by nimitz 2017 (twitter: @stormoid)

        /*

            There are two main hurdles I encountered rendering this effect. 
            First, the nature of the texture that needs to be generated to get a believable effect
            needs to be very specific, with large scale band-like structures, small scale non-smooth variations
            to create the trail-like effect, a method for animating said texture smoothly and finally doing all
            of this cheaply enough to be able to evaluate it several times per fragment/pixel.

            The second obstacle is the need to render a large volume while keeping the computational cost low.
            Since the effect requires the trails to extend way up in the atmosphere to look good, this means
            that the evaluated volume cannot be as constrained as with cloud effects. My solution was to make
            the sample stride increase polynomially, which works very well as long as the trails are lower opcaity than
            the rest of the effect. Which is always the case for auroras.

            After that, there were some issues with getting the correct emission curves and removing banding at lowered
            sample densities, this was fixed by a combination of sample number influenced dithering and slight sample blending.

            N.B. the base setup is from an old shader and ideally the effect would take an arbitrary ray origin and
            direction. But this was not required for this demo and would be trivial to fix.
        */

        private float time;
        private vec2 gl_FragCoord;

        mat2 mm2(float a) { float c = cos(a), s = sin(a); return new mat2(c, s, -s, c); }
        mat2 m2 = new mat2(0.95534f, 0.29552f, -0.29552f, 0.95534f);
        float tri(float x) { return clamp(abs(fract(x) - .5f), 0.01f, 0.49f); }
        vec2 tri2(vec2 p) { return new vec2(tri(p.x) + tri(p.y), tri(p.y + tri(p.x))); }

        float triNoise2d(vec2 p, float spd)
        {
            float z = 1.8f;
            float z2 = 2.5f;
            float rz = 0f;
            p *= mm2(p.x * 0.06f);
            vec2 bp = p;
            for (float i = 0f; i < 5f; i++)
            {
                vec2 dg = tri2(bp * 1.85f) * .75f;
                dg *= mm2(time * spd);
                p -= dg / z2;

                bp *= 1.3f;
                z2 *= .45f;
                z *= .42f;
                p *= 1.21f + (rz - 1.0f) * .02f;

                rz += tri(p.x + tri(p.y)) * z;
                p *= -m2;
            }
            return clamp(1f / pow(rz * 29f, 1.3f), 0f, .55f);
        }

        float hash21(vec2 n) { return fract(sin(dot(n, new vec2(12.9898f, 4.1414f))) * 43758.5453f); }
        vec4 aurora(vec3 ro, vec3 rd)
        {
            vec4 col = new vec4(0);
            vec4 avgCol = new vec4(0);

            for (float i = 0f; i < 50f; i++)
            {
                float of = 0.006f * hash21(gl_FragCoord.xy) * smoothstep(0f, 15f, i);
                float pt = ((.8f + pow(i, 1.4f) * .002f) - ro.y) / (rd.y * 2f+ 0.4f);
                pt -= of;
                vec3 bpos = ro + pt * rd;
                vec2 p = bpos.zx;
                float rzt = triNoise2d(p, 0.06f);
                vec4 col2 = new vec4(0, 0, 0, rzt);
                col2.rgb = (sin(1f - new vec3(2.15f, -.5f, 1.2f) + i * 0.043f) * 0.5f + 0.5f) * rzt;
                avgCol = mix(avgCol, col2, .5f);
                col += avgCol * exp2(-i * 0.065f - 2.5f) * smoothstep(0f, 5f, i);

            }

            col *= (clamp(rd.y * 15f + .4f, 0f, 1f));


            //return clamp(pow(col,vec4(1.3))*1.5,0.,1.);
            //return clamp(pow(col,vec4(1.7))*2.,0.,1.);
            //return clamp(pow(col,vec4(1.5))*2.5,0.,1.);
            //return clamp(pow(col,vec4(1.8))*1.5,0.,1.);

            //return smoothstep(0.,1.1,pow(col,vec4(1.))*1.5);
            return col * 1.8f;
            //return pow(col,vec4(1.))*2.
        }


        //-------------------Background and Stars--------------------

        //From Dave_Hoskins (https://www.shadertoy.com/view/4djSRW)
        vec3 hash33(vec3 p)
        {
            p = fract(p * new vec3(443.8975f, 397.2973f, 491.1871f));
            p += dot(p.zxy, p.yxz + 19.27f);
            return fract(new vec3(p.x * p.y, p.z * p.x, p.y * p.z));
        }

        vec3 stars(vec3 p)
        {
            vec3 c = new vec3(0f);
            float res = iResolution.x * 1f;

            for (float i = 0f; i < 4f; i++)
            {
                vec3 q = fract(p * (.15f * res)) - 0.5f;
                vec3 id = floor(p * (.15f * res));
                vec2 rn = hash33(id).xy;
                float c2 = 1f- smoothstep(0f, .6f, length(q));
                c2 *= step(rn.x, .0005f + i * i * 0.001f);
                c += c2 * (mix(new vec3(1.0f, 0.49f, 0.1f), new vec3(0.75f, 0.9f, 1f), rn.y) * 0.1f + 0.9f);
                p *= 1.3f;
            }
            return c * c * .8f;
        }

        vec3 bg(vec3 rd)
        {
            float sd = dot(normalize(new vec3(-0.5f, -0.6f, 0.9f)), rd) * 0.5f + 0.5f;
            sd = pow(sd, 5f);
            vec3 col = mix(new vec3(0.05f, 0.1f, 0.2f), new vec3(0.1f, 0.05f, 0.2f), sd);
            return col * .63f;
        }
        //-----------------------------------------------------------


        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            time = iTime;
            gl_FragCoord = fragCoord;

             vec2 q = fragCoord.xy / iResolution.xy;
            vec2 p = q - 0.5f;
            p.x *= iResolution.x / iResolution.y;

            vec3 ro = new vec3(0, 0, -6.7f);
            vec3 rd = normalize(new vec3(p, 1.3f));
            vec2 mo = iMouse.xy / iResolution.xy - .5f;
            mo = (mo.x == -.5f && mo.y == -.5f) ? mo = new vec2(-0.1f, 0.1f) : mo;
            mo.x *= iResolution.x / iResolution.y;
            rd.yz *= mm2(mo.y);
            rd.xz *= mm2(mo.x + sin(time * 0.05f) * 0.2f);

            vec3 col = new vec3(0f);
            vec3 brd = rd;
            float fade = smoothstep(0f, 0.01f, abs(brd.y)) * 0.1f + 0.9f;

            col = bg(rd) * fade;

            if (rd.y > 0f)
            {
                vec4 aur = smoothstep(0f, 1.5f, aurora(ro, rd)) * fade;
                col += stars(rd);
                col = col * (1f- aur.a) + aur.rgb;
            }
            else //Reflections
            {
                rd.y = abs(rd.y);
                col = bg(rd) * fade * 0.6f;
                vec4 aur = smoothstep(0.0f, 2.5f, aurora(ro, rd));
                col += stars(rd) * 0.1f;
                col = col * (1f- aur.a) + aur.rgb;
                vec3 pos = ro + ((0.5f - ro.y) / rd.y) * rd;
                float nz2 = triNoise2d(pos.xz * new vec2(.5f, .7f), 0f);
                col += mix(new vec3(0.2f, 0.25f, 0.5f) * 0.08f, new vec3(0.3f, 0.3f, 0.5f) * 0.7f, nz2 * 0.4f);
            }

            fragColor = new vec4(col, 1f);
        }

        public override string ToString()
        {
            return "34 Auroras";
        }
    }
}
