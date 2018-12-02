namespace GdiShaders.Examples
{
    /// <summary>
    /// https://www.shadertoy.com/view/ltKBDD
    /// </summary>
    public class SampleGdiShader24 : GdiShader
    {
        const float PI = 3.1415f;

        public override void mainImage(out vec4 col, vec2 p)
        {
            vec2 uv = (p.xy - 0.5f * iResolution.xy) / iResolution.y;
            vec2 coord = uv;
            vec2 cp = new vec2(atan(uv.x, uv.y), length(uv));
            uv = new vec2(cp.x / (2.0f * PI) + 0.5f + iTime * 0.1f + cp.y * cos(iTime) * 0.15f, cp.y);
            col = (abs(new vec4(smoothstep(0.45f, 0.45f + sin(iTime), length(coord))) * new vec4(1.0f, 2.0f, 0.9f, 1.0f)) + smoothstep(0.0f, max(-iTime + 5.0f, 0.21f), min(fract(uv.x * 8.0f), fract(1.0f - uv.x * 8.0f)) * 0.5f + 0.2f - uv.y) * new vec4(5.0f, normalize(length(cp * PI)) * .8f, normalize(length(cp * 5.0f * PI)) * 0.4f, 1.0f));
        }

        public override string ToString()
        {
            return "24 Cool shader";
        }
    }
}
