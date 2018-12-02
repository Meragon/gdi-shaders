namespace GdiShaders.Examples
{
    using System;
    
    public class SampleGdiShader14 : GdiShader
    {
        float hash(float n)
        {
            return fract(sin(n) * 43745658.5453123f);
        }

        float noise(vec2 pos)
        {
            return fract(sin(dot(pos * 0.001f, new vec2(24.12357f, 36.789f))) * 12345.123f);
        }

        float noise(float r)
        {
            return fract(sin(dot(new vec2(r, -r) * 0.001f, new vec2(24.12357f, 36.789f))) * 12345.123f);
        }


        float wave(float amplitude, float offset, float frequency, float phase, float t)
        {
            return offset + amplitude * sin(t * frequency + phase);
        }

        float wave(float amplitude, float offset, float frequency, float t)
        {
            return offset + amplitude * sin(t * frequency);
        }

        float wave2(float min, float max, float frequency, float phase, float t)
        {
            float amplitude = max - min;
            return min + 0.5f * amplitude + amplitude * sin(t * frequency + phase);
        }

        float wave2(float min, float max, float frequency, float t)
        {
            float amplitude = max - min;
            return min + 0.5f * amplitude + amplitude * sin(t * frequency);
        }

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            float colorSin = 0.0f;
            float colorLine = 0.0f;
            const int nSini = 50;
            const int nLinei = 30;

            float nSin = (float)(nSini);
            float nLine = (float)(nLinei);

            vec2 mouse = iMouse.xy + new vec2(0.5f * iResolution.x, 0.1f * iResolution.y);
            mouse = mod(mouse, iResolution.xy);
            float line = mouse.y;
            // Sin waves
            for (int ii = 0; ii < nSini; ii++)
            {
                float i = (float)(ii);
                float amplitude = mouse.x * 1.0f * noise(i * 0.2454f) * sin(iTime + noise(i) * 100.0f);
                float offset = mouse.y;
                float frequency = 0.1f * noise(i * 100.2454f);
                float phase = 0.02f * i * noise(i * 10.2454f) * 10.0f * iTime * mouse.x / iResolution.x;
                line += i * 0.003f * wave(amplitude, offset, frequency, phase, fragCoord.x);
                colorSin += 0.5f / abs(line - fragCoord.y);
            }

            // Grid	
            for (int ii = 0; ii < nLinei; ii++)
            {
                float i = (float)(ii);
                float lx = (i / nLine) * (iResolution.x + 10.0f);
                float ly = (i / nLine) * (iResolution.y + 10.0f);
                colorLine += 0.07f / abs(fragCoord.x - lx);
                colorLine += 0.07f / abs(fragCoord.y - ly);
            }
            vec3 c = colorSin * new vec3(0.2654f, 0.4578f, 0.654f);
            c += colorLine * new vec3(0.254f, 0.6578f, 0.554f);
            fragColor = new vec4(c, 1.0f);
        }

        public override string ToString()
        {
            return "14 Lines";
        }
    }
}