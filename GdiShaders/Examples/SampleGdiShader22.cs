namespace GdiShaders.Examples
{
    using System;

    using GdiShaders.Core;

    /// <summary>
    /// https://www.shadertoy.com/view/XtGBDW
    /// Not fully working, mb cause I changed some UInt32 types to Int32.
    /// </summary>
    [Obsolete("Not fully working")]
    public class SampleGdiShader22 : GdiShader
    {
        private const int LEVEL = 15;
        private const int WIDTH = ((1 << LEVEL));
        private const int AREA  = (WIDTH * WIDTH);

        float HilbertIndex(vec2 Position)
        {
            vec2 Regions;
            uint Index = 0U;
            for (uint CurLevel = WIDTH / 2U; CurLevel > 0U; CurLevel /= 2U)
            {
                vec2 Region = new vec2(greaterThan((Position & new vec2(CurLevel)), new vec2(0U)));
                Index += CurLevel * CurLevel * (uint)pow(3 * Region.x, Region.y);
                if (Region.y == 0U)
                {
                    if (Region.x == 1U)
                    {
                        Position = new vec2(WIDTH - 1U) - Position;
                    }
                    Position.xy = Position.yx;
                }
            }

            return (float)(Index) / (AREA);
        }

        vec4 mirrored(vec4 v)
        {
            vec4 Mod = mod(v, 2.0f);
            return mix(Mod, 2.0f - Mod, step(1.0f, Mod));
        }

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            vec2 UV = fragCoord / iResolution.xy;
            UV.x *= iResolution.x / iResolution.y;
            UV.x -= fract((iResolution.x) / iResolution.y) / 2.0f;

            vec2 FragCoord = new vec2(UV * (float)(WIDTH));
            float Index = HilbertIndex(FragCoord);

            Index += iTime / 12.0f;
            vec2 Border = smoothstep(new vec2(0.0f), new vec2(0.0f) + new vec2(0.01f), UV) -
                          smoothstep(new vec2(1.0f) - new vec2(0.01f), new vec2(1.0f), UV);
            fragColor = mirrored(
                            new vec4(
                                Index * 7.0f,
                                Index * 11.0f,
                                Index * 13.0f,
                                1.0f)
                        ) * (Border.x * Border.y);
        }

        public override string ToString()
        {
            return "22 Inverse Hilbert Curve";
        }
    }
}