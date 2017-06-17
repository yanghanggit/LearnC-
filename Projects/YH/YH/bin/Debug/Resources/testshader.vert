#version 330 core

layout (location = 0) in vec3 position;

uniform float point_size;

void main()
{
	gl_Position = vec4(position.x, position.y, position.z, 1.0);
	gl_PointSize = point_size;
}

