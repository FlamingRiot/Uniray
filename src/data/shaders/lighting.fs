#version 330

in vec3 fragPosition;
in vec3 fragNormal;
in vec2 fragTexCoord;

uniform vec3 viewPos;

uniform sampler2D texture0;

out vec4 pixelColor;

void main()
{
    vec2 uv = fract(fragTexCoord);
    vec4 texelColor = texture(texture0, fragTexCoord);

    float dist = length(fragPosition - viewPos);
    vec4 fogColor = vec4(0.55, 0.55, 0.6, 1.0); // Color of the fog
    float fogThreshold = 125; // Distance from camera
    float fog = clamp(dist/fogThreshold, 0.0, 1.0);

    pixelColor = mix(texelColor, fogColor, fog);
}