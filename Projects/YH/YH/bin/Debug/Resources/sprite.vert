#version 330 core
  
layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texCoord;
uniform mat4 model;
uniform mat4 view;
uniform mat4 project;
out vec2 TexCoord;

void main()
{
	TexCoord = texCoord;
    gl_Position = project * view * model * vec4(position, 1.0);
}
