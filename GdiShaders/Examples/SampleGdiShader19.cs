namespace GdiShaders.Examples
{
    using System;

    [Obsolete("height issue, slow")]
    public class SampleGdiShader19 : GdiShader
    {
        // https://www.shadertoy.com/view/4dG3RK

        // ray computation vars
        const float PI   = 3.14159265359f;
        const float fov  = 50.0f;
        const float fovx = PI * fov / 360.0f;
        float       fovy = 0;
        float       ulen = 0;
        float       vlen = 0;

        public SampleGdiShader19()
        {
            ulen = tan(fovx);
            vlen = tan(fovy);
        }

        // epsilon-type values
        const float S       = 0.01f;
        const float EPSILON = 0.01f;

        // const delta vectors for normal calculation
        vec3 deltax = new vec3(S, 0, 0);
        vec3 deltay = new vec3(0, S, 0);
        vec3 deltaz = new vec3(0, 0, S);

        float distanceToNearestSurface(vec3 p)
        {
            float s = 1.0f;
            vec3 d = abs(p) - new vec3(s);
            return min(max(d.x, max(d.y, d.z)), 0.0f)
                   + length(max(d, 0.0f));
        }


        // better normal implementation with half the sample points
        // used in the blog post method
        vec3 computeSurfaceNormal(vec3 p)
        {
            float d = distanceToNearestSurface(p);
            return normalize(new vec3(
                distanceToNearestSurface(p + deltax) - d,
                distanceToNearestSurface(p + deltay) - d,
                distanceToNearestSurface(p + deltaz) - d
            ));
        }


        vec3 computeLambert(vec3 p, vec3 n, vec3 l)
        {
            return new vec3(dot(normalize(l - p), n));
        }

        vec3 intersectWithWorld(vec3 p, vec3 dir)
        {
            float dist = 0.0f;
            float nearest = 0.0f;
            vec3 result = new vec3(0.0f);
            for (int i = 0; i < 20; i++)
            {
                nearest = distanceToNearestSurface(p + dir * dist);
                if (nearest < EPSILON)
                {
                    vec3 hit = p + dir * dist;
                    vec3 light = new vec3(100.0f * sin(iTime), 30.0f * cos(iTime), 50.0f * cos(iTime));
                    result = computeLambert(hit, computeSurfaceNormal(hit), light);
                    break;
                }
                dist += nearest;
            }
            return result;
        }

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            fovy = fovx * iResolution.y / iResolution.x;

            vec2 uv = fragCoord / iResolution.xy;

            float cameraDistance = 10.0f;
            vec3 cameraPosition = new vec3(10.0f * sin(iTime), 0.0f, 10.0f * cos(iTime));
            vec3 cameraDirection = new vec3(-1.0f * sin(iTime), 0.0f, -1.0f * cos(iTime));
            vec3 cameraUp = new vec3(0.0f, 1.0f, 0.0f);

            // generate the ray for this pixel
            vec2 camUV = uv * 2.0f - new vec2(1.0f, 1.0f);
            vec3 nright = normalize(cross(cameraUp, cameraDirection));
            vec3 pixel = cameraPosition + cameraDirection + nright * camUV.x * ulen + cameraUp * camUV.y * vlen;
            vec3 rayDirection = normalize(pixel - cameraPosition);

            vec3 pixelColour = intersectWithWorld(cameraPosition, rayDirection);
            fragColor = new vec4(pixelColour, 1.0f);
        }

        public override string ToString()
        {
            return "Sphere Tracing 103";
        }
    }
}