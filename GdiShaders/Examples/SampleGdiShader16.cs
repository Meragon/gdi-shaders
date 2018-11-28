namespace GdiShaders.Examples
{
    public class SampleGdiShader16 : GdiShader
    {
        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            vec2 pos = 256.0f * fragCoord.xy / iResolution.x + iTime;

            vec3 col = new vec3(0.0f);
            for (int i = 0; i < 6; i++)
            {
                vec2 a = floor(pos);
                vec2 b = fract(pos);

                vec4 w = fract((sin(a.x * 7.0f + 31.0f * a.y + 0.01f * iTime) + new vec4(0.035f, 0.01f, 0.0f, 0.7f)) * 13.545317f); // randoms

                col += w.xyz *                                   // color
                       smoothstep(0.45f, 0.55f, w.w) *               // intensity
                       sqrt(16.0f * b.x * b.y * (1.0f - b.x) * (1.0f - b.y)); // pattern

                pos /= 2.0f; // lacunarity
                col /= 2.0f; // attenuate high frequencies
            }

            col = pow(2.5f * col, new vec3(1.0f, 1.0f, 0.7f));    // contrast and color shape

            fragColor = new vec4(col, 1.0f);
        }

        public override string ToString()
        {
            return "Tiles";
        }
    }
}