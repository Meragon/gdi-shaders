namespace GdiShaders.Examples
{
    using GdiShaders.Core;

    /// <summary>
    /// https://www.shadertoy.com/view/XsXXDn
    /// </summary>
    public class SampleGdiShader30 : GdiShader
    {
        // http://www.pouet.net/prod.php?which=57245
        // If you intend to reuse this shader, please add credits to 'Danilo Guanabara'

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            var t = iTime;
            var r = iResolution.xy;

            vec3 c = new vec3();
            float l = 0, z = t;
            for (int i = 0; i < 3; i++)
            {
                vec2 uv, p = fragCoord.xy / r;
                uv = p;
                p -= .5f;
                p.x *= r.x / r.y;
                z += .07f;
                l = length(p);
                uv += p / l * (sin(z) + 1f) * abs(sin(l * 9f- z * 2f));
                c[i] = .01f / length(abs(mod(uv, 1f) - .5f));
            }
            fragColor = new vec4(c / l, t);
        }

        public override string ToString()
        {
            return "30 Creation by Silexars";
        }
    }
}
