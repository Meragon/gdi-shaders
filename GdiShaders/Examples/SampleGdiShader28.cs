#define SHOW_GRID

namespace GdiShaders.Examples
{
    /// <summary>
    /// https://www.shadertoy.com/view/llcBD7
    /// </summary>
    public class SampleGdiShader28 : GdiShader
    {
        /*
	
	Random Quadtree
	---------------

	Just a simple non-overlapping quadtree demonstration to accompany my 
	Quadtree Truchet example. I've cut the code back to make it a little
	easier to digest.


	// More elaborate quadtree example.
	Quadtree Truchet - Shane
	https://www.shadertoy.com/view/4t3BW4
	

*/

        // vec2 to vec2 hash.
        vec2 hash22(vec2 p)
        {

            // Faster, but doesn't disperse things quite as nicely.
            return fract(new vec2(262144, 32768) * sin(dot(p, new vec2(57, 27))));

        }

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {
            // Screen coordinates.
            vec2 uv = (fragCoord - iResolution.xy * .5f) / iResolution.y;

            // Scaling and translation.
            vec2 oP = uv * 4f + new vec2(.5f, iTime / 2f);


            // Background.
            vec3 bg = new vec3(.125f) * new vec3(1, .9f, .95f);
            float pat = clamp(sin((oP.x - oP.y) * 6.283f * iResolution.y / 22.5f) + .75f, 0f, 1f);
            bg *= (hash22(oP).x * .15f + 1f) * (pat * .35f + .65f);

            // Scene color. Initiated to the background.
            vec3 col = bg;

            // Distance file values.
            vec4 d = new vec4(1E-5f);

            // Initial cell dimension.
            float dim = 1f;

            // Random entries -- One for each layer. The X values represent the chance that
            // a tile for that particular layer will be rendered. For instance, the large
            // tile will have a 35% chance, the middle tiles, 70%, and the remaining smaller
            // tiles will have a 100% chance. I.e., they'll fill in the rest of the squares.
            vec2[] rndTh = new []{ new vec2(.35f, .5f), new vec2(.7f, .5f), new vec2(1, .5f) };


            for (int k = 0; k < 3; k++)
            {

                vec2 ip = floor(oP * dim);

                vec2 rnd = hash22(ip);


                if (rnd.x < rndTh[k].x)
                {


                    // Local cell coordinate.
                    vec2 p = oP - (ip + .5f) / dim; // Equivalent to: mod(oP, 1./dim) - .5/dim;


                    // Reusing "rnd" to calculate a new random number. Not absolutely necessary,
                    // but I wanted to mix things up a bit more.
                    rnd = fract(rnd * 27.63f + (float)(k * 57 + 1));


                    // Using a unique random cell ID to create an offset.
                    vec2 off = (rnd - .5f) * .33f / dim;

                    // Animation.
                    //vec2 rndA = sin(rnd*6.2831853 + rnd.x*iTime + vec2(0, 1.57))*.5;
                    //vec2 off = rndA*.25/dim;


                    // An offset disk... or disc, as some people spell it. :)
                    // If rendering outside the loop, you'd have to take an overall minimum, and keep
                    // some copies, etc.
                    // I.e.: d.x = min(d.x, (length(p - off) - .44/1.4/dim));
                    //
                    d.x = length(p - off) - .3f / dim;

                    // Grid lines.
                    const float lwg = .015f;
                    d.y = abs(max(abs(p.x), abs(p.y)) - .5f / dim) - lwg / 2f;


                    // RENDERING.
                    //
                    // The rendering is performed inside the loop for simplicity, but it doesn't
                    // need to be. Most of the following lines are for decorative purposes.

                    // Render the grid lines.
                    float fo = 4f/ iResolution.y;
                    vec3 pCol = mix(col, new vec3(1, .9f, .95f), .5f - pat * .5f) * max(1f - d.y / (lwg / 2f), 0f);
#if SHOW_GRID
                    col = mix(col, new vec3(0), (1f - smoothstep(0f, fo * 3f, d.y - .02f)) * .35f);
                    col = mix(col, pCol, (1f - smoothstep(0f, fo, d.y)) * .6f);
#endif


                    // Render the disks. This is a lazy way to do things, but it gets the job done.
                    // I got a little bored and kept adding layers, but rendering a simple circle
                    // with an outline would get the point across.
                    fo = 10f/ iResolution.y / sqrt(dim);
                    pCol = new vec3(1, .1f, .3f);
                    //pCol = mix(pCol.xzy, pCol, step(.5, fract(rnd.y*113.97 + .51)));
                    float sh = max(.8f - d.x * dim * 2f, 0f); // Distance field-based shading.

                    col = mix(col, new vec3(0), (1f - smoothstep(0f, fo * 5f, d.x)) * .35f);
                    col = mix(col, new vec3(0), 1f - smoothstep(0f, fo, d.x));
                    col = mix(col, pCol * sh, 1f - smoothstep(0f, fo, d.x + .015f));
                    col = mix(col, new vec3(0), 1f - smoothstep(0f, fo, d.x + .25f / 1.4f / dim));
                    col = mix(col, pCol * .6f * (pat * .5f + .5f), 1f - smoothstep(0f, fo, d.x + .25f / 1.4f / dim + .015f));
                    col = mix(col, new vec3(0), 1f - smoothstep(0f, fo, d.x + .375f / 1.4f / dim));
                    col = mix(col, bg * .8f, 1f - smoothstep(0f, fo, d.x + .375f / 1.4f / dim + .015f));


                    // Since we don't need to worry about neighbors
                    break;

                }

                // Subdividing. I.e., decrease the cell size by doubling the frequency.
                dim *= 2f;

            }

            // A bit of gradential color mixing.
            col = mix(col.xzy, col, sign(uv.y) * uv.y * uv.y * 2f + .5f);

            // Mild spotlight.
            col *= max(1.25f - length(uv) * .25f, 0f);



            // Rough gamma correction.
            fragColor = new vec4(sqrt(max(col, 0f)), 1);
        }

        public override string ToString()
        {
            return "Random Quadtree";
        }
    }
}
