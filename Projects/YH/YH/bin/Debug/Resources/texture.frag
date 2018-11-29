#version 330 core
//in vec3 ourColor;
in vec2 TexCoord;

out vec4 color;

// Texture samplers
uniform sampler2D ourTexture1;
uniform sampler2D ourTexture2;
uniform sampler2D sampler2ds[10];

void main()
{
    float a = float(TexCoord[0]);
    int index = int(a);
    sampler2ds[index]; //在这里弄???
    color = texture(sampler2ds[index], TexCoord);  //在这里弄???




	// Linearly interpolate between both textures (second texture is only slightly combined)
	color = mix(texture(ourTexture1, TexCoord), texture(ourTexture2, TexCoord), 0.2);
}
