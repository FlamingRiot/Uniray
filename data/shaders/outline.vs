#version 330

// Input vertex attributes
in vec3 vertexPosition;
in vec2 vertexTexCoord;
in vec3 vertexNormal;
in vec4 vertexColor;

// Input uniform values
uniform mat4 mvp;
uniform mat4 matModel;
uniform mat4 matNormal;

// NOTE: Add here your custom variables

void main()
{
    vec3 inflatedPosition = vertexPosition + vertexNormal * 0.015;

    // Calculate final vertex position
    gl_Position = mvp*vec4(inflatedPosition, 1.0);
}
