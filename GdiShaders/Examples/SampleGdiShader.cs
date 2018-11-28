namespace GdiShaders.Examples
{
    public class SampleGdiShader : GdiShader
    {
        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            vec2 uv = fragCoord.xy / iResolution.xy;
            fragColor = new vec4(uv, 0.5f + 0.5f * sin(iTime), 1.0f);
        }

        public override string ToString()
        {
            return "Colors";
        }
    }
}
