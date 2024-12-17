#version 330

in vec3 vertexPosition;
in vec3 vertexNormal;
in vec2 vertexTexCoord;

uniform mat4 mvp;

out vec3 fragPosition;
out vec3 fragNormal;
out vec2 fragTexCoord;

void main()
{
	fragPosition = vertexPosition;
	fragNormal = vertexNormal;
	fragTexCoord = vertexTexCoord * 100;

	gl_Position = mvp*vec4(vertexPosition, 1.0);
}