#version 330 core

//out vec4 color;

in vec2 TexCoords;

out vec4 color;

uniform sampler2D texture1;


void main()
{
    color = vec4(0.0, 1.0, 0.0, 1.0);
    //color = texture(texture1, TexCoords);
} 