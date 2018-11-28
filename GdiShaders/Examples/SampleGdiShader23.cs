namespace GdiShaders.Examples
{
    /// <summary>
    /// https://www.shadertoy.com/view/ltKBzm
    /// </summary>
    public class SampleGdiShader23 : GdiShader
    {
        /*
	Improved Layer Starfield
	Credits:	https://www.shadertoy.com/view/4djSRW
				https://www.shadertoy.com/view/lscczl
*/
        private vec2  R;
        private float T;

        mat2 rotate(float a)
        {
            float c = cos(a);
            float s = sin(a);
            return new mat2(c, s, -s, c);
        }


        // one dimensional | 1 in 1 out
        float hash11(float p)
        {
            p = fract(p * 35.35f);
            p += dot(p, p + 45.85f);
            return fract(p * 7858.58f);
        }

        // two dimensional | 2 in 1 out
        float hash21(vec2 p)
        {
            p = fract(p * new vec2(451.45f, 231.95f));
            p += dot(p, p + 78.78f);
            return fract(p.x * p.y);
        }

        // two dimensional | 2 in 2 out
        vec2 hash22(vec2 p)
        {
            vec3 q = fract(p.xyx * new vec3(451.45f, 231.95f, 7878.5f));
            q += dot(q, q + 78.78f);
            return fract(q.xz * q.y);
        }
        
        float layer(vec2 uv)
        {

            float c = 0f;

            uv *= 5f;

            // id and coordinates per cell
            // f -> [-1, 1] to allow more size and glow variations
            // tf: stop the neighbour cells "cutting off" star glow
            vec2 i = floor(uv);
            vec2 f = 2f * fract(uv) - 1f;

            // random position for the star in the cell
            vec2 p = .3f * hash22(i);
            float d = length(f - p);

            // create fuzzier stars with random radius
            // col * (1. / d) -> glow
            c += smoothstep(.1f + .8f * hash21(i), .01f, d);
            c *= (1f / d) * .2f;

            return c;
        }
        
        vec3 render(vec2 uv)
        {
            vec3 col = new vec3(0f);

            // rotate the whole scene
            uv *= rotate(T * .1f);

            // oscillation to add more variations
            uv += 2f * new vec2(cos(T * .001f), sin(T * .001f));

            // num layers - increase for more stars
            // adjust based on your machine
            const float num = 10f;
            const float inc = 1f / num;

            for (float i = 0f; i < 1f; i += inc)
            {
                // random rotate - stop repeating stars in consequent layers
                uv *= rotate(hash11(i) * 6.28f);

                // i mapped to t -> [0, 1]
                float t = fract(i - T * .05f);

                // smoothstep is useful for scaling and fading
                float s = smoothstep(.001f, .95f, t); // z-position of layer
                float f = smoothstep(0f, 1f, t); // fade per layer
                f *= smoothstep(1f, 0f, t);

                // random offset per layer - gives each layer the
                // appearance of drifiting
                vec2 k = .1f * hash22(new vec2(i, i * 5f));
                float l = layer((uv - k) * s);

                // mix bg and fg colors
                col += mix(new vec3(.03f, .01f, .04f), new vec3(.9f, .4f, 0f), l) * f;

            }

            // optional - just some subtle noise on top
            col += .02f * hash21(uv + T * .001f);
            return col;

        }

        public override void mainImage(out vec4 O, vec2 I)
        {
            R = iResolution.xy;
            T = iTime;

            vec2 uv = (2f * I - R) / R.y;
            vec3 color = render(uv);
            O = new vec4(color, 1f);
        }

        public override string ToString()
        {
            return "Improved Starfield";
        }
    }
}