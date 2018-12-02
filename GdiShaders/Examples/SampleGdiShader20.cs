namespace GdiShaders.Examples
{
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
            float time = floor((iTime + 3f) * frametime) / frametime;
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

        public override string ToString()
        {
            return "20 Blur";
        }
    }
}