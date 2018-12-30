namespace GdiShaders.Examples
{
    using GdiShaders.Core;

    /// <summary>
    /// https://www.shadertoy.com/new
    /// </summary>
    public class SampleGdiShader : GdiShader
    {
        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            // Normalized pixel coordinates (from 0 to 1)
            vec2 uv = fragCoord / iResolution.xy;

            // Time varying pixel color
            vec3 col = 0.5f + 0.5f * cos(iTime + uv.xyx + new vec3(0, 2, 4));

            // Output to screen
            fragColor = new vec4(col, 1.0f);
        }

        public override string ToString()
        {
            return "1 Colors";
        }
    }
}
