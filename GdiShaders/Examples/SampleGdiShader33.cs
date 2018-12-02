namespace GdiShaders.Examples
{
    /// <summary>
    /// https://www.shadertoy.com/view/MdSGDm
    /// </summary>
    public class SampleGdiShader33 : GdiShader
    {
        // The MIT License
        // Copyright © 2014 Inigo Quilez
        // Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


        // Analytic motion blur, for 2D spheres (disks).
        //
        // (Linearly) Moving Disk - pixel/ray overlap test. The resulting equation is a quadratic 
        // that can be solved to compute time coverage of the swept disk behind the pixel over the
        // aperture of the camera (a full frame at 24 hz in this test).



        // draw a disk with motion blur
        vec3 diskWithMotionBlur(vec3 col, vec2 uv, vec3 sph, vec2 cd, vec3 sphcol)
        {
            vec2 xc = uv - sph.xy;
            float a = dot(cd, cd);
            float b = dot(cd, xc);
            float c = dot(xc, xc) - sph.z * sph.z;
            float h = b * b - a * c;
            if (h > 0.0)
            {
                h = sqrt(h);

                float ta = max(0.0f, (-b - h) / a);
                float tb = min(1.0f, (-b + h) / a);

                if (ta < tb) // we can comment this conditional, in fact
                    col = mix(col, sphcol, clamp(2.0f * (tb - ta), 0.0f, 1.0f));
            }

            return col;
        }


        vec3 hash3(float n) { return fract(sin(new vec3(n, n + 1.0f, n + 2.0f)) * 43758.5453123f); }
        vec4 hash4(float n) { return fract(sin(new vec4(n, n + 1.0f, n + 2.0f, n + 3.0f)) * 43758.5453123f); }

        const float speed = 8.0f;
        vec2 getPosition(float time, vec4 id) { return new vec2(0.9f * sin((speed * (0.75f + 0.5f * id.z)) * time + 20.0f * id.x), 0.75f * cos(speed * (0.75f + 0.5f * id.w) * time + 20.0f * id.y)); }
        vec2 getVelocity(float time, vec4 id) { return new vec2(speed * 0.9f * cos((speed * (0.75f + 0.5f * id.z)) * time + 20.0f * id.x), -speed * 0.75f * sin(speed * (0.75f + 0.5f * id.w) * time + 20.0f * id.y)); }

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            vec2 p = (2.0f * fragCoord.xy - iResolution.xy) / iResolution.y;

            vec3 col = new vec3(0.2f) + 0.05f * p.y;

            for (int i = 0; i < 16; i++)
            {
                vec4 off = hash4((float)(i) * 13.13f);
                vec3 sph = new vec3(getPosition(iTime, off), 0.02f + 0.1f * off.x);
                vec2 cd = getVelocity(iTime, off) / 24.0f;
                vec3 sphcol = 0.7f + 0.3f * sin(3.0f * off.z + new vec3(4.0f, 0.0f, 2.0f));

                col = diskWithMotionBlur(col, p, sph, cd, sphcol);
            }

            col += (1.0f / 255.0f) * hash3(p.x + 13.0f * p.y);

            fragColor = new vec4(col, 1.0f);
        }

        public override string ToString()
        {
            return "33 Analytic Motionblur 2D";
        }
    }
}
