namespace GdiShaders.Examples
{
    /// <summary>
    /// https://www.shadertoy.com/view/4dfXDn
    /// Looks like working
    /// </summary>
    class SampleGdiShader29 : GdiShader
    {
        /**

	Hi all,

	This is just my playground for a bunch of 2D stuff:

	Some distance functions and blend functions
	Cone marched 2D Soft shadows
	Use the mouse to control the 3rd light

*/



        //////////////////////////////////////
        // Combine distance field functions //
        //////////////////////////////////////


        float smoothMerge(float d1, float d2, float k)
        {
            float h = clamp(0.5f + 0.5f * (d2 - d1) / k, 0.0f, 1.0f);
            return mix(d2, d1, h) - k * h * (1.0f - h);
        }


        float merge(float d1, float d2)
        {
            return min(d1, d2);
        }


        float mergeExclude(float d1, float d2)
        {
            return min(max(-d1, d2), max(-d2, d1));
        }


        float substract(float d1, float d2)
        {
            return max(-d1, d2);
        }


        float intersect(float d1, float d2)
        {
            return max(d1, d2);
        }


        //////////////////////////////
        // Rotation and translation //
        //////////////////////////////


        vec2 rotateCCW(vec2 p, float a)
        {
            mat2 m = new mat2(cos(a), sin(a), -sin(a), cos(a));
            return p * m;
        }


        vec2 rotateCW(vec2 p, float a)
        {
            mat2 m = new mat2(cos(a), -sin(a), sin(a), cos(a));
            return p * m;
        }


        vec2 translate(vec2 p, vec2 t)
        {
            return p - t;
        }


        //////////////////////////////
        // Distance field functions //
        //////////////////////////////


        float pie(vec2 p, float angle)
        {
            angle = radians(angle) / 2.0f;
            vec2 n = new vec2(cos(angle), sin(angle));
            return abs(p).x * n.x + p.y * n.y;
        }


        float circleDist(vec2 p, float radius)
        {
            return length(p) - radius;
        }


        float triangleDist(vec2 p, float radius)
        {
            return max(abs(p).x * 0.866025f +
                           p.y * 0.5f, -p.y)
                        - radius * 0.5f;
        }


        float triangleDist(vec2 p, float width, float height)
        {
            vec2 n = normalize(new vec2(height, width / 2.0f));
            return max(abs(p).x * n.x + p.y * n.y - (height * n.y), -p.y);
        }


        float semiCircleDist(vec2 p, float radius, float angle, float width)
        {
            width /= 2.0f;
            radius -= width;
            return substract(pie(p, angle),
                             abs(circleDist(p, radius)) - width);
        }


        float boxDist(vec2 p, vec2 size, float radius)
        {
            size -= new vec2(radius);
            vec2 d = abs(p) - size;
            return min(max(d.x, d.y), 0.0f) + length(max(d, 0.0f)) - radius;
        }


        float lineDist(vec2 p, vec2 start, vec2 end, float width)
        {
            vec2 dir = start - end;
            float lngth = length(dir);
            dir /= lngth;
            vec2 proj = max(0.0f, min(lngth, dot((start - p), dir))) * dir;
            return length((start - p) - proj) - (width / 2.0f);
        }


        ///////////////////////
        // Masks for drawing //
        ///////////////////////


        float fillMask(float dist)
        {
            return clamp(-dist, 0.0f, 1.0f);
        }


        float innerBorderMask(float dist, float width)
        {
            //dist += 1.0;
            float alpha1 = clamp(dist + width, 0.0f, 1.0f);
            float alpha2 = clamp(dist, 0.0f, 1.0f);
            return alpha1 - alpha2;
        }


        float outerBorderMask(float dist, float width)
        {
            //dist += 1.0;
            float alpha1 = clamp(dist, 0.0f, 1.0f);
            float alpha2 = clamp(dist - width, 0.0f, 1.0f);
            return alpha1 - alpha2;
        }


        ///////////////
        // The scene //
        ///////////////


        float sceneDist(vec2 p)
        {
            float c = circleDist(translate(p, new vec2(100, 250)), 40.0f);
            float b1 = boxDist(translate(p, new vec2(200, 250)), new vec2(40, 40), 0.0f);
            float b2 = boxDist(translate(p, new vec2(300, 250)), new vec2(40, 40), 10.0f);
            float l = lineDist(p, new vec2(370, 220), new vec2(430, 280), 10.0f);
            float t1 = triangleDist(translate(p, new vec2(500, 210)), 80.0f, 80.0f);
            float t2 = triangleDist(rotateCW(translate(p, new vec2(600, 250)), iTime), 40.0f);

            float m = merge(c, b1);
            m = merge(m, b2);
            m = merge(m, l);
            m = merge(m, t1);
            m = merge(m, t2);

            float b3 = boxDist(translate(p, new vec2(100, sin(iTime * 3.0f + 1.0f) * 40.0f + 100.0f)), new vec2(40, 15), 0.0f);
            float c2 = circleDist(translate(p, new vec2(100, 100)), 30.0f);
            float s = substract(b3, c2);

            float b4 = boxDist(translate(p, new vec2(200, sin(iTime * 3.0f + 2.0f) * 40.0f + 100.0f)), new vec2(40, 15), 0.0f);
            float c3 = circleDist(translate(p, new vec2(200, 100)), 30.0f);
            float i = intersect(b4, c3);

            float b5 = boxDist(translate(p, new vec2(300, sin(iTime * 3.0f + 3.0f) * 40.0f + 100.0f)), new vec2(40, 15), 0.0f);
            float c4 = circleDist(translate(p, new vec2(300, 100)), 30.0f);
            float a = merge(b5, c4);

            float b6 = boxDist(translate(p, new vec2(400, 100)), new vec2(40, 15), 0.0f);
            float c5 = circleDist(translate(p, new vec2(400, 100)), 30.0f);
            float sm = smoothMerge(b6, c5, 10.0f);

            float sc = semiCircleDist(translate(p, new vec2(500, 100)), 40.0f, 90.0f, 10.0f);

            float b7 = boxDist(translate(p, new vec2(600, sin(iTime * 3.0f + 3.0f) * 40.0f + 100.0f)), new vec2(40, 15), 0.0f);
            float c6 = circleDist(translate(p, new vec2(600, 100)), 30.0f);
            float e = mergeExclude(b7, c6);

            m = merge(m, s);
            m = merge(m, i);
            m = merge(m, a);
            m = merge(m, sm);
            m = merge(m, sc);
            m = merge(m, e);

            return m;
        }


        float sceneSmooth(vec2 p, float r)
        {
            float accum = sceneDist(p);
            accum += sceneDist(p + new vec2(0.0f, r));
            accum += sceneDist(p + new vec2(0.0f, -r));
            accum += sceneDist(p + new vec2(r, 0.0f));
            accum += sceneDist(p + new vec2(-r, 0.0f));
            return accum / 5.0f;
        }


        //////////////////////
        // Shadow and light //
        //////////////////////


        float shadow(vec2 p, vec2 pos, float radius)
        {
            vec2 dir = normalize(pos - p);
            float dl = length(p - pos);

            // fraction of light visible, starts at one radius (second half added in the end);
            float lf = radius * dl;

            // distance traveled
            float dt = 0.01f;

            for (int i = 0; i < 64; ++i)
            {
                // distance to scene at current position
                float sd = sceneDist(p + dir * dt);

                // early out when this ray is guaranteed to be full shadow
                if (sd < -radius)
                    return 0.0f;

                // width of cone-overlap at light
                // 0 in center, so 50% overlap: add one radius outside of loop to get total coverage
                // should be '(sd / dt) * dl', but '*dl' outside of loop
                lf = min(lf, sd / dt);

                // move ahead
                dt += max(1.0f, abs(sd));
                if (dt > dl) break;
            }

            // multiply by dl to get the real projected overlap (moved out of loop)
            // add one radius, before between -radius and + radius
            // normalize to 1 ( / 2*radius)
            lf = clamp((lf * dl + radius) / (2.0f * radius), 0.0f, 1.0f);
            lf = smoothstep(0.0f, 1.0f, lf);
            return lf;
        }



        vec4 drawLight(vec2 p, vec2 pos, vec4 color, float dist, float range, float radius)
        {
            // distance to light
            float ld = length(p - pos);

            // out of range
            if (ld > range) return new vec4(0.0f);

            // shadow and falloff
            float shad = shadow(p, pos, radius);
            float fall = (range - ld) / range;
            fall *= fall;
            float source = fillMask(circleDist(p - pos, radius));
            return (shad * fall + source) * color;
        }


        float luminance(vec4 col)
        {
            return 0.2126f * col.r + 0.7152f * col.g + 0.0722f * col.b;
        }


        void setLuminance(ref vec4 col, float lum)
        {
            lum /= luminance(col);
            col *= lum;
        }


        float AO(vec2 p, float dist, float radius, float intensity)
        {
            float a = clamp(dist / radius, 0.0f, 1.0f) - 1.0f;
            return 1.0f - (pow(abs(a), 5.0f) + 1.0f) * intensity + (1.0f - intensity);
            return smoothstep(0.0f, 1.0f, dist / radius);
        }


        /////////////////
        // The program //
        /////////////////


        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            vec2 p = fragCoord.xy + new vec2(0.5f);
            vec2 c = iResolution.xy / 2.0f;

            //float dist = sceneSmooth(p, 5.0);
            float dist = sceneDist(p);

            vec2 light1Pos = iMouse.xy;
            vec4 light1Col = new vec4(0.75f, 1.0f, 0.5f, 1.0f);
            setLuminance(ref light1Col, 0.4f);

            vec2 light2Pos = new vec2(iResolution.x * (sin(iTime + 3.1415f) + 1.2f) / 2.4f, 175.0f);
            vec4 light2Col = new vec4(1.0f, 0.75f, 0.5f, 1.0f);
            setLuminance(ref light2Col, 0.5f);

            vec2 light3Pos = new vec2(iResolution.x * (sin(iTime) + 1.2f) / 2.4f, 340.0f);
            vec4 light3Col = new vec4(0.5f, 0.75f, 1.0f, 1.0f);
            setLuminance(ref light3Col, 0.6f);

            // gradient
            vec4 col = new vec4(0.5f, 0.5f, 0.5f, 1.0f) * (1.0f - length(c - p) / iResolution.x);
            // grid
            col *= clamp(min(mod(p.y, 10.0f), mod(p.x, 10.0f)), 0.9f, 1.0f);
            // ambient occlusion
            col *= AO(p, sceneSmooth(p, 10.0f), 40.0f, 0.4f);
            //col *= 1.0-AO(p, sceneDist(p), 40.0, 1.0);
            // light
            col += drawLight(p, light1Pos, light1Col, dist, 150.0f, 6.0f);
            col += drawLight(p, light2Pos, light2Col, dist, 200.0f, 8.0f);
            col += drawLight(p, light3Pos, light3Col, dist, 300.0f, 12.0f);
            // shape fill
            col = mix(col, new vec4(1.0f, 0.4f, 0.0f, 1.0f), fillMask(dist));
            // shape outline
            col = mix(col, new vec4(0.1f, 0.1f, 0.1f, 1.0f), innerBorderMask(dist, 1.5f));

            fragColor = clamp(col, 0.0f, 1.0f);
        }

        public override string ToString()
        {
            return "29 2d signed distance functions";
        }
    }
}
