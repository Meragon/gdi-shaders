namespace GdiShaders.Examples
{
    using GdiShaders.Core;

    public class SampleGdiShader3 : GdiShader
    {
        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            vec2 uv = fragCoord.xy / iResolution.xy;
            float ar = iResolution.x / iResolution.y;
            vec4 temp = new vec4(0.0f, 0.0f, 0.0f, 1.0f);
            vec2 center = new vec2(0.5f, 0.5f);
            vec2 fromCenter = uv - center;
            float angle = atan(fromCenter.y, fromCenter.x);
            fromCenter.x *= ar;
            float fromCenterLength = length(fromCenter);

            temp.r = 0.5f + sin(angle * 1.0f + fromCenterLength * 6.28f * 15.0f - iTime * 15.0f) * 0.5f;
            temp.r -= fromCenterLength * 2.0f;
            fragColor = temp;
        }

        public override string ToString()
        {
            return "3 Spiral";
        }
    }
}