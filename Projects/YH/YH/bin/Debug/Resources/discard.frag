// Fragment shader:
// ================
#version 330 core
in vec2 TexCoords;

out vec4 color;

uniform sampler2D texture1;
uniform bool discard_pixel;

void main()
{             
    vec4 texColor = texture(texture1, TexCoords);
    if (discard_pixel)
    {
        if(texColor.a < 0.1)
	    {
	        discard;
	    }
    }
    color = texColor;
}