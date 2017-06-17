#version 330 core

out vec4 color;
  
uniform vec3 objectColor;
uniform vec3 lightColor;

void main()
{
    if(gl_FrontFacing)
    {
        color = vec4(lightColor * objectColor, 1.0f);
    } 
    else
    {
        discard;
    }
}