#version 330 core
in vec2 TexCoord;

out vec4 color;

uniform sampler2D ourTexture1;
uniform sampler2D ourTexture2;
//uniform float rot;

void main()
{
    color = mix(texture(ourTexture1, TexCoord), texture(ourTexture2, TexCoord), 0.2);
    //color = texture(ourTexture2, TexCoord);
    //color = vec4(rot, rot, rot, 1);
}