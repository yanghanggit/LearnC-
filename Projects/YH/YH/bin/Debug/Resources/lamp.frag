#version 330 core
out vec4 color;


uniform vec3 set_color;

void main()
{
    if (set_color.r > 0 || set_color.g > 0 || set_color.b > 0)
    {
        color = vec4(set_color, 1.0f);
    }
    else 
    {
        color = vec4(1.0f); // Set alle 4 vector values to 1.0f
    }
}