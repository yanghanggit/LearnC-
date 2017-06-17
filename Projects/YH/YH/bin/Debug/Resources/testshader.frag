#version 330 core

out vec4 color;

uniform bool test_frag_coord;
uniform float test_middle;

void main()
{
    if (test_frag_coord)
    {
        if(gl_FragCoord.x < test_middle)
        {   
            color = vec4(0.0f, 0.0f, 1.0f, 1.0f);
        }
	    else
        {
            color = vec4(0.0f, 1.0f, 0.0f, 1.0f);
        }
    }  
    else
    {
        color = vec4(1.0f, 0.0f, 0.0f, 1.0f);
    }
}