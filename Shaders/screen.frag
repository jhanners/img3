#version 330 core

in vec2 TexCoords;

uniform highp sampler2D inputTexture;
uniform vec2 screenResolution;
uniform float time;

// uniform vec3 directionToLight = normalize(vec3(-1, 1, 1));
uniform vec3 lightPosition = vec3(1.5, 1.5, 0.5);

const float pi = 3.1415926535897932384626433832795;
const float two_pi = 6.283185307179586476925286766559;

float random(vec2 st)
{
    return fract(
        sin(
            dot(
                st.xy,
                vec2(
                    12.9898,
                    78.233)
            )
        ) * 43758.5453123
    );
}

// bias
uniform vec3 palette_bias  = vec3(0.50, 0.50, 0.50);
// scale
uniform vec3 palette_scale = vec3(0.50, 0.50, 0.50);
// oscillation shift
uniform vec3 palette_osc   = vec3(1.00, 2.00, 1.50);
// phase shift
uniform vec3 palette_phase = vec3(0.00, 0.00, 0.00);

vec4 palette(in float t, in float time)
{
    vec3 pp = vec3(
        palette_phase.r + time / 20,
        palette_phase.g + time / 25,
        palette_phase.b + time / 30);
    vec3 result = palette_bias + palette_scale * cos(two_pi * (palette_osc * t + pp));
    return vec4(result.xyz, 1.0);
}

void main()
{ 
    vec4 color = texture(inputTexture, TexCoords);

    vec2 texel0 = TexCoords;
    vec4 light_color = vec4(0, 0, 0, 1);
    vec2 step = vec2(1.0 / screenResolution.x, 1.0 / screenResolution.y);
    vec3 normal;
    vec3 dx, dy;

    if (true)
    {
        vec4 color01 = texture(inputTexture, texel0 + vec2(-step.x,     0.0));
        vec4 color21 = texture(inputTexture, texel0 + vec2(+step.x,     0.0));
        vec4 color10 = texture(inputTexture, texel0 + vec2(    0.0, -step.y));
        vec4 color12 = texture(inputTexture, texel0 + vec2(    0.0, +step.y));

        if (false)
        {
            dx = vec3(2 * step.x, 0.0, color21.g - color01.g);
            dx = normalize(dx);

            dy = vec3(0.0, 2 * step.y, color12.g - color10.g);
            dy = normalize(dy);

            normal = normalize(cross(dx, dy));
        }
        else if (true)
        {
            normal = normalize(
                cross(
                    normalize(vec3(+step.x,     0.0, color21.g - color.g)),
                    normalize(vec3(    0.0, +step.y, color12.g - color.g)))
                + cross(
                    normalize(vec3(    0.0, +step.y, color12.g - color.g)),
                    normalize(vec3(-step.x,     0.0, color01.g - color.g)))
                + cross(
                    normalize(vec3(-step.x,     0.0, color01.g - color.g)),
                    normalize(vec3(    0.0, -step.y, color10.g - color.g)))
                + cross(
                    normalize(vec3(    0.0, -step.y, color10.g - color.g)),
                    normalize(vec3(+step.x,     0.0, color21.g - color.g))));
        }

        vec3 directionToLight = normalize(lightPosition - vec3(texel0.x, texel0.y, 0));
        float cos_theta = dot(normal, directionToLight);
        if (cos_theta > 0)
        {
            cos_theta *= cos_theta;
            // cos_theta *= cos_theta;
            // cos_theta *= cos_theta;
            light_color = vec4(cos_theta, cos_theta, cos_theta, 1.0);
        }
    }

    // color.r == concentration of A.
    // Unused at this time.

    // color.g == concentration of B.
    color.g *= 2;
    // color.g += 0.25;

    // color.b == velocity.x
    color.b = abs(color.b * 500);

    // color.a == velocity.y
    color.a = abs(color.a * 500);

    if (false)
    {
        gl_FragColor = vec4(color.g, color.b, color.a, 1.0);
    }
    else if (true)
    {
        vec4 chemical_color = color.g * palette(color.g, time);
        vec4 flow_color = vec4(0.0, color.b, color.a, 1.0);
        gl_FragColor = mix(chemical_color, flow_color, 0.15) + light_color * 0.5;
        // gl_FragColor = flow_color + light_color * 0.5;
    }
}
