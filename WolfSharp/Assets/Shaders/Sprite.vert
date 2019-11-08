#version 450

layout(set = 0, binding = 0) uniform MVP
{
    mat4 Mvp;
};

layout(location = 0) in vec3 Position;
layout(location = 1) in vec2 TexCoords;
layout(location = 0) out vec2 fsin_texCoords;

void main()
{
    vec4 pos = Mvp * vec4(Position, 1);
    gl_Position = pos;
    fsin_texCoords = TexCoords;
}