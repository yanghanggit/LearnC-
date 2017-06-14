#version 330 core
out vec4 color;

uniform bool linearize_depth;

 
float LinearizeDepth(float depth, float near, float far) 
{
    float z = depth * 2.0 - 1.0; // Back to NDC 
    return (2.0 * near * far) / (far + near - z * (far - near));	
}

void main()
{   
    if (linearize_depth)
    {
		const float near = 1.0; 
		const float far = 100.0;      
		float depth = LinearizeDepth(gl_FragCoord.z, near, far) / far; // divide by far to get depth in range [0,1] for visualization purposes.
		color = vec4(vec3(depth), 1.0f);
    }    
	else 
    {
        color = vec4(vec3(gl_FragCoord.z), 1.0f);
    }
}