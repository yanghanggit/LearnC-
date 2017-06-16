#version 330 core

in vec3 Normal;
in vec3 Position;

out vec4 color;

uniform vec3 cameraPos;
uniform samplerCube skybox;
uniform float ratio;

void main()
{              
    if (ratio > 0.0f)
    {
		float ratio = 1.00 / ratio;
		vec3 I = normalize(Position - cameraPos);
		vec3 R = refract(I, normalize(Normal), ratio);
		color = texture(skybox, R);
    }
    else 
    {
		vec3 I = normalize(Position - cameraPos);
		vec3 R = reflect(I, normalize(Normal));
		color = texture(skybox, R);
    }
}