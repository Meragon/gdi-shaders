namespace GdiShaders.Examples
{
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
                t = iTime;
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
}