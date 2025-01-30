#version 330

// Input vertex attributes
in vec3 vertexPosition;
in vec2 vertexTexCoord;
in vec3 vertexNormal;
in vec4 vertexColor;

// Input uniform values
uniform mat4 matModel;
uniform mat4 matView;
uniform mat4 matProjection;

// NOTE: Add here your custom variables


void main()
{
    // Expanding on vertex position (from model space)
    vec4 worldPos = matModel * vec4(vertexPosition + normalize(vertexNormal) * 0.05, 1.0);


    // Calculate final vertex position
    gl_Position = matProjection * matView * worldPos;
}
