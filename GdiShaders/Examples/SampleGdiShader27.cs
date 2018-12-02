// DEFINES: Feel free to try them out.

// Default colored setting. Not applicable when using the stacked tiles option.
// When turned off, the color is white.
#define SPECTRUM_COLORED

// Pink -- Less bland than white, and has a velvety feel... Gets overridden by the spectrum 
// color option, so only works when "SPECTRUM_COLORED" is commented out.
//#define PINK

// Showing the different tile layers stacked on top of one another. Aesthetically, I prefer 
// this more, because it has a raised look about it. However, you can't make out the general 
// pattern as well, so it's off by default.
//#define STACKED_TILES

// This option produces art deco looking patterns, which are probably more interesting, but 
// I wanted the default pattern to be more simplistic. 
//#define INCLUDE_LINE_TILES


namespace GdiShaders.Examples
{
    using System;

    /// <summary>
    /// https://www.shadertoy.com/view/4t3BW4
    /// Something is wrong.
    /// </summary>
    [Obsolete("Not fully working")]
    public class SampleGdiShader27 : GdiShader
    {
        /*

	Quadtree Truchet
	----------------

    A multiscale, multitile, overlapped, weaved Truchet pattern -- However, since
	that description is a little verbose, I figured that a quadtree Truchet was as 
	good a description as any. :) The mild weave effect is provided via the
	"INCLUDE_LINE_TILES" define.

	In order to produce a varied looking Truchet pattern, there are a couple of
	simple things you can try: One is to use more than one tile, and the other is 
	to stitch weaved tiles together to produce a cool under-over effect. There are 
    a few examples on Shadertoy of each, which are easy enough to find -- Just do
	a search for "Truchet" and look for the multitile and weaved examples.

    Lesser known variations include using Truchet tiles that overlap one another, 
    and stitching together multiscaled tiles -- usually on something like a quadtree 
    grid. This example uses elements of all of the aforementioned.

	In the past, I've combined two non-overlapping tile scales, but had never 
    considered taking it beyond that... until I came across Christopher Carlson's
	article, "Multi-Scale Truchet Patterns." If you follow the link below and refer
	to the construction process, you'll see that the idea behind it is almost 
	rundimentary. As a consequence, I figured that it'd take me five minutes to put 
	the ideas into pixel shader form. Unfortunately, they say the dumber you are, 
	the more overconfident you'll be, and to cut a long story short... It took me 
	longer than five minutes. :D

	The code below is somewhat obfuscated and strewn with defines - The defines are
	my fault, since I wanted to provide a few rendering options. However, the 
	remaining complication boils down to the necessity to render overlapping tiles
	on a quadtree grid in an environment that doesn't allow random pixel access. The 
	only example along those lines I could find on here was IQ's hierachical Voronoi 
	demonstration, which is pretty cool, but it contains a lot of nested iterations.
	Rendering tiles in that manner wasn't really sufficient, so I had to write
	things in a way that used fewer iterations, but it was at the cost of legibility.

	Either way, the idea is pretty simple: Construct a grid, randomly render some 
	Truchet tiles, subdivide the remaining squares into four, randomly render some 
    more tiles in reverse color order, then continue ad infinitum. By the way, I
	constructed this on the fly using the best method I could think of at the time.
	However, if anyone out there has a more elegant solution, feel free to post it. :)
	
	Naturally, the idea can be extended to 3D. Three levels with this particular 
	setup might be a little slow. However, two levels using a non overlapping tile
	is definitely doable, so I intend to produce an example along those lines in the 
	near future.


	Based on the following:

	Multi-Scale Truchet Patterns  - Christopher Carlson
	https://christophercarlson.com/portfolio/multi-scale-truchet-patterns/
    Linking paper containing more detail:
    http://archive.bridgesmathart.org/2018/bridges2018-39.pdf

	Quadtree Related:

	// Considers overlap.
	https://www.shadertoy.com/view/Xll3zX
	Voronoi - hierarchical - IQ

    // No overlap, but I really like this one.
    SDF Raymarch Quadtree - Paniq
	https://www.shadertoy.com/view/MlffW8

	// Multilevel, and nice and simple.
	quadtree - 4 - FabriceNeyret2
	https://www.shadertoy.com/view/ltlyRH

	// A really simple non-overlapping quadtree example.
	Random Quadtree - Shane
	https://www.shadertoy.com/view/llcBD7

*/

        // vec2 to vec2 hash.
        vec2 hash22(vec2 p)
        {

            // Faster, but doesn't disperse things quite as nicely. However, when framerate
            // is an issue, and it often is, this is a good one to use. Basically, it's a tweaked 
            // amalgamation I put together, based on a couple of other random algorithms I've 
            // seen around... so use it with caution, because I make a tonne of mistakes. :)
            float n = sin(dot(p, new vec2(57, 27)));

            return fract(new vec2(262144, 32768) * n);

            /*
            // Animated.
            p = fract(vec2(262144, 32768)*n); 
            // Note the ".35," insted of ".5" that you'd expect to see. .
            return sin(p*6.2831853 + iTime/2.)*.24;
            */
        }

        // Standard 2D rotation formula.
        mat2 r2(float a) { float c = cos(a), s = sin(a); return new mat2(c, s, -s, c); }

        /*
        // IQ's 2D unsigned box formula.
        float sBox(vec2 p, vec2 b){ return length(max(abs(p) - b, 0.)); }

        // IQ's 2D signed box formula.
        float sBoxU(vec2 p, vec2 b){

          vec2 d = abs(p) - b;
          return min(max(d.x, d.y), 0.) + length(max(d, 0.));
        }
        */

        public override void mainImage(out vec4 fragColor, vec2 fragCoord)
        {

            // Screen coordinates.    
            vec2 uv = (fragCoord - iResolution.xy * .5f) / iResolution.y;

            // Scaling, rotation and transalation.
            vec2 oP = uv * 5f;
            oP *= r2(sin(iTime / 8f) * 3.14159f / 8f);
            oP -= new vec2(cos(iTime / 8f) * 0f, -iTime);


            // Distance field values -- One for each color. They're "vec4"s to hold the three 
            // layers and an an unused spare. The grid vector holds grid values, strangely enough. :)
            vec4 d = new vec4(1E-5f), d2 = new vec4(1E-5f), grid = new vec4(1E-5f);

            // Random constants for each layer. The X values are Truchet flipping threshold
            // values, and the Y values represent the chance that a particular sized tile
            // will render.
            //
            // The final Y entry needs to fill in the remaiming grid spaces, so it must have a 100% 
            // chance of success -- I'd rather not say how long it took me to figure that out. :D
            vec2[] rndTh = new vec2[] { new vec2(.5f, .35f), new vec2(.5f, .7f), new vec2(.5f, 1f) };


            // The scale dimentions. Gets multiplied by two each iteration. 
            float dim = 1f;



            // If you didn't need to worry about overlap, you wouldn't need to consider neighboring
            // cell rendering, which would make this far less complicated - One loop and a break.

            // Three tile levels. 
            for (int k = 0; k < 3; k++)
            {

                // Base cell ID.
                vec2 ip = floor(oP * dim);

                // Abje reminded me that for a 2x2 neighbor check, just make the following changes:
                //vec2 ip = floor(oP*dim + .5);
                //for(int j=-1; j<=0; j++){
                //for(int i=-1; i<=0; i++){
                //
                // In this particular case, I'm using a 3x3 sweep because I need the internal field pattern 
                // overlay to be balanced. However, in general, Abje's faster suggestion is the way to go.


                for (int j = -1; j <= 1; j++)
                {
                    for (int i = -1; i <= 1; i++)
                    {

                        // The neighboring cell ID.
                        vec2 rndIJ = hash22(ip + new vec2(i, j));

                        // The cell IDs for the previous dimension, or dimensions, as the case may be.
                        // Because the tiles overlap, rendering order matters. In this case, the tiles 
                        // need to be laid down from largest (k = 0) to smallest (k = 2). If a large tile
                        // has taken up the space, you need to check on the next iterations and skip --
                        // so as not to lay smaller tiles over the larger ones.
                        //
                        // So why not just break from the loop? Unfortunately, there are neighboring
                        // cells to check, and the IDs need to be calculated from the perspective of 
                        // each cell neighbor... Yeah, I'm confused too. You can either take my word
                        // for it, or better yet, come up with a more elegant solution. :)
                        vec2 rndIJ2 = hash22(floor((ip + new vec2(i, j)) / 2f));
                        vec2 rndIJ4 = hash22(floor((ip + new vec2(i, j)) / 4f));

                        // If the previous large tile has been rendered, continue.
                        if (k == 1 && rndIJ2.y < rndTh[0].y) continue;
                        // If any of the two previous larger tiles have been rendered, continue.
                        if (k == 2 && (rndIJ2.y < rndTh[1].y || rndIJ4.y < rndTh[0].y)) continue;


                        // If the random cell ID at this particular scale is below a certain threshold, 
                        // render the tile. The code block below is a little messy, due to to fact that I
                        // wanted to render a few different tile styles without bloating things too much.
                        // This meant a bunch of random coordinate flipping, reflecting, etc. As mentioned,
                        // I'll provide a much simpler example later.                
                        //
                        if (rndIJ.y < rndTh[k].y)
                        {

                            // Local cell coordinates. The following is equivalent to:
                            // vec2 p = mod(oP, 1./dim) - .5/dim - vec2(i, j)/dim;
                            vec2 p = oP - (ip + .5f + new vec2(i, j)) / dim;


                            // The grid square.
                            float square = max(abs(p.x), abs(p.y)) - .5f / dim;

                            // The grid lines.
                            const float lwg = .01f;
                            float gr = abs(square) - lwg / 2f;
                            grid.x = min(grid.x, gr);


                            // TILE COLOR ONE.

                            // Standard Truchet rotation and flipping -- based on a random cell ID.
                            if (rndIJ.x < rndTh[k].x) p.xy = p.yx;
                            if (fract(rndIJ.x * 57.543f + .37f) < rndTh[k].x) p.x = -p.x;



                            // Rotating by 90 degrees, then reflecting across both axes by the correct
                            // distance to produce four circles on the midway points of the grid boundary
                            // lines... A lot of this stuff is just practice. Do it often enough and 
                            // it'll become second nature... sometimes. :)
                            vec2 p2 = abs(new vec2(p.y - p.x, p.x + p.y) * .7071f) - new vec2(.5f, .5f) * .7071f / dim;
                            float c3 = length(p2) - .5f / 3f / dim;

                            float c, c2;

                            // Truchet arc one.
                            c = abs(length(p - new vec2(-.5f, .5f) / dim) - .5f / dim) - .5f / 3f / dim;

                            // Truchet arc two.
                            if (fract(rndIJ.x * 157.763f + .49f) > .35f)
                            {
                                c2 = abs(length(p - new vec2(.5f, -.5f) / dim) - .5f / dim) - .5f / 3f / dim;
                            }
                            else
                            {
                                // Circles at the mid boundary lines -- instead of an arc.
                                // c2 = 1e5; // In some situations, just this would work.
                                c2 = length(p - new vec2(.5f, 0) / dim) - .5f / 3f / dim;
                                c2 = min(c2, length(p - new vec2(0, -.5f) / dim) - .5f / 3f / dim);
                            }


                            // Randomly overiding some arcs with lines.
#if INCLUDE_LINE_TILES
                            if (fract(rndIJ.x * 113.467f + .51f) < .35f)
                            {
                                c = abs(p.x) - .5f / 3f/ dim;
                            }
                            if (fract(rndIJ.x * 123.853f + .49f) < .35f)
                            {
                                c2 = abs(p.y) - .5f / 3f/ dim;
                            }
#endif


                            // Truch arcs, lines, or dots -- as the case may be.
                            float truchet = min(c, c2);

                            // Carving out a mild channel around the line to give a faux weave effect.
# if INCLUDE_LINE_TILES
                            float lne = abs(c - .5f / 12f/ 4f) - .5f / 12f/ 4f;
                            truchet = max(truchet, -lne);
#endif

                            // Each tile has two colors. This is the first, and it's rendered on top.
                            c = min(c3, max(square, truchet));
                            d[k] = min(d[k], c); // Tile color one.


                            // TILE COLOR TWO.
                            // Repeat trick, to render four circles at the grid vertices.
                            p = abs(p) - .5f / dim;
                            float l = length(p);
                            // Four circles at the grid vertices and the square.
                            c = min(l - 1f / 3f / dim, square);
                            //c = max(c, -truchet);
                            //c = max(c, -c3);
                            d2[k] = min(d2[k], c); // Tile color two.

                            // Rendering some circles at the actual grid vertices. Mouse down to see it.
                            grid.y = min(grid.y, l - .5f / 8f / sqrt(dim)); //.05/(dim*.35 + .65)
                            grid.z = min(grid.z, l);
                            grid.w = dim;


                        }



                    }
                }

                // Subdividing. I.e., decrease the tile size by doubling the frequency.
                dim *= 2f;


            }


            // The scene color. Initiated to grey.
            vec3 col = new vec3(.25f);


            // Just a simple lined pattern.
            float pat3 = clamp(sin((oP.x - oP.y) * 6.283f * iResolution.y / 24f) * 1f + .9f, 0f, 1f) * .25f + .75f;
            // Resolution based falloff... Insert "too may different devices these days" rant here. :D
            float fo = 5f / iResolution.y;


            // Tile colors. 
            vec3 pCol2 = new vec3(.125f);
            vec3 pCol1 = new vec3(1);

            //The spectrum color option overides the pink option.
#if SPECTRUM_COLORED
            pCol1 = new vec3(.7f, 1.4f, .4f);
#else
            // Pink version.
#if PINK
            pCol1 = mix(new vec3(1f, .1f, .2f), new vec3(1, .1f, .5f), uv.y * .5f + .5f); ;
            pCol2 = new vec3(.1f, .02f, .06f);
#endif
#endif




#if STACKED_TILES
            // I provided this as an option becaue I thought it might be useful
            // to see the tile layering process.

            float pw = .02f;
            d -= pw / 2f;
            d2 -= pw / 2f;

            // Render each two-colored tile, switching colors on alternating iterations.
            for (int k = 0; k < 3; k++)
            {

                col = mix(col, new vec3(0), (1f - smoothstep(0f, fo * 5f, d2[k])) * .35f);
                col = mix(col, new vec3(0), 1f - smoothstep(0f, fo, d2[k]));
                col = mix(col, pCol2, 1f - smoothstep(0f, fo, d2[k] + pw));

                col = mix(col, new vec3(0), (1f - smoothstep(0f, fo * 5f, d[k])) * .35f);
                col = mix(col, new vec3(0), 1f - smoothstep(0f, fo, d[k]));
                col = mix(col, pCol1, 1f - smoothstep(0f, fo, d[k] + pw));

                vec3 temp = pCol1; pCol1 = pCol2; pCol2 = temp;
            }

            col *= pat3;

#else

            // Combining the tile layers into a continuous surface. I'd like to say that
            // I applied years of topological knowledge to arrive at this, but like most
            // things, I threw a bunch of formulas at the screen in frustration until I 
            // fluked the solution. :D There was a bit of logic applied though. :)
            d.x = max(d2.x, -d.x);
            d.x = min(max(d.x, -d2.y), d.y);
            d.x = max(min(d.x, d2.z), -d.z);

            // A couple of distance field patterns and a shade.
            float pat = clamp(-sin(d.x * 6.283f * 20f) - .0f, 0f, 1f);
            float pat2 = clamp(sin(d.x * 6.283f * 16f) * 1f + .9f, 0f, 1f) * .3f + .7f;
            float sh = clamp(.75f + d.x * 2f, 0f, 1f);

#if SPECTRUM_COLORED

            col *= pat;

            // Render the combined shape.
            d.x = -(d.x + .03f);

            col = mix(col, new vec3(0), (1f - smoothstep(0f, fo * 5f, d.x)));
            col = mix(col, new vec3(0), 1f - smoothstep(0f, fo, d.x));
            col = mix(col, new vec3(.8f, 1.2f, .6f), 1f - smoothstep(0f, fo * 2f, d.x + .02f));
            col = mix(col, new vec3(0), 1f - smoothstep(0f, fo * 2f, d.x + .03f));
            col = mix(col, new vec3(.7f, 1.4f, .4f) * pat2, 1f - smoothstep(0f, fo * 2f, d.x + .05f));

            col *= sh;

#else

            //d.x -= .01;
            col = pCol1;

            // Render the combined shape.
            col = mix(col, new vec3(0), (1f - smoothstep(0f, fo * 5f, d.x)) * .35f);
            col = mix(col, new vec3(0), 1f - smoothstep(0f, fo, d.x));
            col = mix(col, pCol2, 1f - smoothstep(0f, fo, d.x + .02f));


            col *= pat3; // Line decroation.
#endif

#endif



            // Mild spotlight.
            col *= max(1.15f - length(uv) * .5f, 0f);


            // Click the left mouse button to show the underlying quadtree grid structure. It's
            // helpful to see the cell borders to see the random tile constructions.
            if (iMouse.z > 0f)
            {


                vec3 vCol1 = new vec3(.8f, 1f, .7f);
                vec3 vCol2 = new vec3(1f, .7f, .4f);

#if PINK
                vCol1 = vCol1.zxy;
                vCol2 = vCol2.zyx;
#endif

                // Grid lines.
                vec3 bg = col;
                col = mix(col, new vec3(0), (1f - smoothstep(0f, .02f, grid.x - .02f)) * .7f);
                col = mix(col, vCol1 + bg / 2f, 1f - smoothstep(0f, .01f, grid.x));

                // Circles on the grid vertices.
                fo = 10f / iResolution.y / sqrt(grid.w);
                col = mix(col, new vec3(0), (1f - smoothstep(0f, fo * 3f, grid.y - .02f)) * .5f);
                col = mix(col, new vec3(0), 1f - smoothstep(0f, fo, grid.y - .02f));
                col = mix(col, vCol2, 1f - smoothstep(0f, fo, grid.y));
                col = mix(col, new vec3(0), 1f - smoothstep(0f, fo, grid.z - .02f / sqrt(grid.w)));
            }


            // Mix the colors, if the spectrum option is chosen.
#if SPECTRUM_COLORED
            col = mix(col, col.yxz, uv.y * .75f + .5f); //.zxy
            col = mix(col, col.zxy, uv.x * .7f + .5f); //.zxy
#endif


            // Rough gamma correction, and output to the screen.
            fragColor = new vec4(sqrt(max(col, 0f)), 1);
        }

        public override string ToString()
        {
            return "27 Quadtree Truchet";
        }
    }
}
