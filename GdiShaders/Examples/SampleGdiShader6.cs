namespace GdiShaders.Examples
{
    /// <summary>
    /// https://www.shadertoy.com/view/4dsGzH
    /// </summary>
    public class SampleGdiShader6 : GdiShader
    {
        vec3  COLOR1      = new vec3(0.0f, 0.0f, 0.3f);
        vec3  COLOR2      = new vec3(0.5f, 0.0f, 0.0f);
        float BLOCK_WIDTH = 0.01f;

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            vec2 uv = fragCoord.xy / iResolution.xy;

            // To create the BG pattern
            vec3 final_color = new vec3(1.0f);
            vec3 bg_color = new vec3(0.0f);
            vec3 wave_color = new vec3(0.0f);

            float c1 = mod(uv.x, 2.0f * BLOCK_WIDTH);
            c1 = step(BLOCK_WIDTH, c1);

            float c2 = mod(uv.y, 2.0f * BLOCK_WIDTH);
            c2 = step(BLOCK_WIDTH, c2);

            bg_color = mix(uv.x * COLOR1, uv.y * COLOR2, c1 * c2);


            // To create the waves
            float wave_width = 0.01f;
            uv = -1.0f + 2.0f * uv;
            uv.y += 0.1f;
            for (float i = 0.0f; i < 10.0f; i++)
            {

                uv.y += (0.07f * sin(uv.x + i / 7.0f + iTime));
                wave_width = abs(1.0f / (150.0f * uv.y));
                wave_color += new vec3(wave_width * 1.9f, wave_width, wave_width * 1.5f);
            }

            final_color = bg_color + wave_color;


            fragColor = new vec4(final_color, 1.0f);
        }

        public override string ToString()
        {
            return "6 Waves";
        }
    }
}