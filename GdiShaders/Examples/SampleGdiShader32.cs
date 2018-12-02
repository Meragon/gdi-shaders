namespace GdiShaders.Examples
{
    /// <summary>
    /// https://www.shadertoy.com/view/4dl3zn
    /// </summary>
    public class SampleGdiShader32 : GdiShader
    {
        // Created by inigo quilez - iq/2013
        // License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            vec2 uv = -1.0f + 2.0f * fragCoord.xy / iResolution.xy;
            uv.x *= iResolution.x / iResolution.y;

            // background	 
            vec3 color = new vec3(0.8f + 0.2f * uv.y);

            // bubbles	
            for (int i = 0; i < 40; i++)
            {
                // bubble seeds
                float pha = sin((float)(i) * 546.13f + 1.0f) * 0.5f + 0.5f;
                float siz = pow(sin((float)(i) * 651.74f + 5.0f) * 0.5f + 0.5f, 4.0f);
                float pox = sin((float)(i) * 321.55f + 4.1f) * iResolution.x / iResolution.y;

                // buble size, position and color
                float rad = 0.1f + 0.5f * siz;
                vec2 pos = new vec2(pox, -1.0f - rad + (2.0f + 2.0f * rad) * mod(pha + 0.1f * iTime * (0.2f + 0.8f * siz), 1.0f));
                float dis = length(uv - pos);
                vec3 col = mix(new vec3(0.94f, 0.3f, 0.0f), new vec3(0.1f, 0.4f, 0.8f), 0.5f + 0.5f * sin((float)(i) * 1.2f + 1.9f));
                //    col+= 8.0*smoothstep( rad*0.95, rad, dis );

                // render
                float f = length(uv - pos) / rad;
                f = sqrt(clamp(1.0f - f * f, 0.0f, 1.0f));
                color -= col.zyx * (1.0f - smoothstep(rad * 0.95f, rad, dis)) * f;
            }

            // vigneting	
            color *= sqrt(1.5f - 0.5f * length(uv));

            fragColor = new vec4(color, 1.0f);
        }

        public override string ToString()
        {
            return "32 Bubbles";
        }
    }
}
