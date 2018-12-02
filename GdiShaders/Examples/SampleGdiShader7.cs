namespace GdiShaders.Examples
{
    /// <summary>
    /// https://www.shadertoy.com/view/XsfGRn
    /// </summary>
    public class SampleGdiShader7 : GdiShader
    {
        // Created by inigo quilez - iq/2013
        // License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            vec2 p = (2.0f * fragCoord - iResolution.xy) / min(iResolution.y, iResolution.x);

            p.y -= 0.25f;

            // background color
            vec3 bcol = new vec3(1.0f, 0.8f, 0.7f - 0.07f * p.y) * (1.0f - 0.25f * length(p));

            // animate
            float tt = mod(iTime, 1.5f) / 1.5f;
            float ss = pow(tt, .2f) * 0.5f + 0.5f;
            ss = 1.0f + ss * 0.5f * sin(tt * 6.2831f * 3.0f + p.y * 0.5f) * exp(-tt * 4.0f);
            p *= new vec2(0.5f, 1.5f) + ss * new vec2(0.5f, -0.5f);


            // shape
            float a = atan(p.x, p.y) / 3.141593f;
            float r = length(p);
            float h = abs(a);
            float d = (13.0f * h - 22.0f * h * h + 10.0f * h * h * h) / (6.0f - 5.0f * h);

            // color
            float s = 0.75f + 0.75f * p.x;
            s *= 1.0f - 0.25f * r;
            s = 0.5f + 0.6f * s;
            s *= 0.5f + 0.5f * pow(1.0f - clamp(r / d, 0.0f, 1.0f), 0.1f);
            vec3 hcol = new vec3(1.0f, 0.5f * r, 0.3f) * s;

            vec3 col = mix(bcol, hcol, smoothstep(-0.01f, 0.01f, d - r));

            fragColor = new vec4(col, 1.0f);
        }

        public override string ToString()
        {
            return "7 Heart - 2D";
        }
    }
}