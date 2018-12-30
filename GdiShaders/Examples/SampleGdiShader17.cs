namespace GdiShaders.Examples
{
    using System;

    using GdiShaders.Core;

    /// <summary>
    /// https://www.shadertoy.com/view/4sj3zy
    /// </summary>
    public class SampleGdiShader17 : GdiShader
    {
        vec3 bgCol = new vec3(0.6f, 0.5f, 0.6f);

        //Sets size of the sphere and brightness of the shine
        float sphereScale = 0.7f;
        float sphereShine = 0.5f;

        //Main method/function
        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            //Sets diffuse colour(red, green, blue), specular colour(red, green, blue), 
            //and initial specular point position(x, y)
            vec3 sphereDiff = new vec3(0.5f, 0.0f, 0.5f);
            vec3 sphereSpec = new vec3(1.0f, 1.0f, 1.0f);
            vec2 specPoint = new vec2(0.2f, -0.1f);

            //Creates shader pixel coordinates
            vec2 uv = fragCoord.xy / iResolution.xy;

            //Sets the position of the camera
            vec2 p = uv * 2.3f - 1.0f;
            p.x *= iResolution.x / iResolution.y;

            //Rotates the sphere in a circle
            p.x += cos(-iTime) * 0.35f;
            p.y += sin(-iTime) * 0.35f;

            //Rotates the specular point with the sphere
            specPoint.x += cos(-iTime) * 0.35f;
            specPoint.y += sin(-iTime) * 0.35f;

            //Sets the radius of the sphere to the middle of the screen
            float radius = sqrt(dot(p, p));

            vec3 col = bgCol;

            //Sets the initial dark shadow around the edge of the sphere
            float f = smoothstep(sphereScale * 0.9f, sphereScale, length(p + specPoint));
            col -= mix(col, new vec3(0.0f), f) * 0.2f;

            //Only carries out the logic if the radius of the sphere is less than the scale
            if (radius < sphereScale)
            {
                vec3 bg = col;

                //Sets the diffuse colour of the sphere (solid colour)
                col = sphereDiff;

                //Adds smooth dark borders to help achieve 3D look
                f = smoothstep(sphereScale * 0.7f, sphereScale, radius);
                col = mix(col, sphereDiff * 0.45f, f);

                //Adds specular glow to help achive 3D look
                f = 1.0f - smoothstep(-0.2f, 0.6f, length(p - specPoint));
                col += f * sphereShine * sphereSpec;

                //Smoothes the edge of the sphere
                f = smoothstep(sphereScale - 0.01f, sphereScale, radius);
                col = mix(col, bg, f);
            }

            //The final output of the shader logic above
            //fragColor is a vector with 4 paramaters(red, green, blue, alpha)
            //Only 2 need to be used here, as "col" is a vector that already carries r, g, and b values
            fragColor = new vec4(col, 1);
        }

        public override string ToString()
        {
            return "17 Rotating Sphere";
        }
    }
}