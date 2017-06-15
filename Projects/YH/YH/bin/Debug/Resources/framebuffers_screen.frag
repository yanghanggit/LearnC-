#version 330 core
in vec2 TexCoords;

out vec4 color;

uniform sampler2D screenTexture;
uniform int post_processing;

void main()
{
    switch (post_processing)
    {
    case 0:
	    {
            //default
	        color = texture(screenTexture, TexCoords);
	        break;
	    }
        
    case 1:
        {
            //Inversion
            color = vec4(vec3(1.0 - texture(screenTexture, TexCoords)), 1.0);
            break;
        }

    case 2:
        {
            //Grayscale
			color = texture(screenTexture, TexCoords);
			float average = 0.2126 * color.r + 0.7152 * color.g + 0.0722 * color.b;
			color = vec4(average, average, average, 1.0);
            break;
        }

    case 3:
        {
            //Kernel effects
            const float offset = 1.0 / 300;  

            vec2 offsets[9] = vec2[](
		        vec2(-offset, offset),  // top-left
		        vec2(0.0f,    offset),  // top-center
		        vec2(offset,  offset),  // top-right
		        vec2(-offset, 0.0f),    // center-left
		        vec2(0.0f,    0.0f),    // center-center
		        vec2(offset,  0.0f),    // center-right
		        vec2(-offset, -offset), // bottom-left
		        vec2(0.0f,    -offset), // bottom-center
		        vec2(offset,  -offset)  // bottom-right
		    );

		    const float kernel[9] = float[](
		        -1, -1, -1,
		        -1,  9, -1,
		        -1, -1, -1
		    );

		    vec3 sampleTex[9];
		    for(int i = 0; i < 9; i++)
		    {
		        sampleTex[i] = vec3(texture(screenTexture, TexCoords.st + offsets[i]));
		    }
		    vec3 col = vec3(0.0);
            
		    for(int i = 0; i < 9; i++)
            {
                col += sampleTex[i] * kernel[i];
            }
		        

		    color = vec4(col, 1.0);
            break;
        }

    default:
	    {
            color = texture(screenTexture, TexCoords);
            break;
	    }
    }



    //color = vec4(1, 0, 0, 1);
    /*
    vec2 offsets[9] = vec2[](
        vec2(-offset, offset),  // top-left
        vec2(0.0f,    offset),  // top-center
        vec2(offset,  offset),  // top-right
        vec2(-offset, 0.0f),    // center-left
        vec2(0.0f,    0.0f),    // center-center
        vec2(offset,  0.0f),    // center-right
        vec2(-offset, -offset), // bottom-left
        vec2(0.0f,    -offset), // bottom-center
        vec2(offset,  -offset)  // bottom-right    
    );

    float kernel[9] = float[](
        -1, -1, -1,
        -1,  9, -1,
        -1, -1, -1
    );
    
    vec3 sampleTex[9];
    for(int i = 0; i < 9; i++)
    {
        sampleTex[i] = vec3(texture(screenTexture, TexCoords.st + offsets[i]));
    }
    vec3 col;
    for(int i = 0; i < 9; i++)
        col += sampleTex[i] * kernel[i];
    
    color = vec4(col, 1.0);
    */
} 
