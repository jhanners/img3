#version 330 core

uniform highp sampler2D inputTexture;
uniform vec2 screenResolution;
uniform vec4 brush;
uniform vec2 brushVelocity;
uniform float feed = 0.014;
uniform float kill = 0.045;
// waves
// uniform float feed = 0.014;
// uniform float kill = 0.045;
// "default"
// uniform float feed = 0.037;
// uniform float kill = 0.060;
uniform vec2 diffusion = vec2(1.0, 0.5);
// uniform vec2 diffusion = vec2(0.2, 0.1);
// uniform vec2 diffusion = vec2(0.2097, 0.105);
uniform float deltaTime;
uniform float time;
uniform int laplacian;
uniform bool flow = true;

out vec4 FragColor;

float pi = 3.1415927;
float _1_over_sqrt_2 = 0.7071067811865475;
float _1_over_sqrt_4 = 0.5;                // 1/2
float _1_over_sqrt_5 = 0.4472135954999579;
float _1_over_sqrt_8 = 0.5 * _1_over_sqrt_2;

float velocity_decay = 0.0000001;

float rand(float n){return fract(sin(n) * 43758.5453123);}

void main()
{
    // Wipe brush.
    if (brush.a == 1.0)
    {
        FragColor = vec4(1, 0, 0, 0);
        return;
    }

    vec2 step = vec2(1.0 / screenResolution.x, 1.0 / screenResolution.y);
    vec2 texel0 = gl_FragCoord.xy / screenResolution.xy;
    
    vec2 texel;
    vec2 velocity;

    if (flow)
    {
        // fluid flow

        vec2 velocity0 = texture(inputTexture, texel0).ba;
        velocity0.x *= screenResolution.y / screenResolution.x;
        vec2 texel1 = texel0 - velocity0 * 50.0 * deltaTime;

        vec2 v00 = texture(inputTexture, texel1 + vec2(+step.x, +step.y)).ba;
        vec2 v01 = texture(inputTexture, texel1 + vec2(+step.x,       0)).ba;
        vec2 v02 = texture(inputTexture, texel1 + vec2(+step.x, -step.y)).ba;
        vec2 v10 = texture(inputTexture, texel1 + vec2(      0, +step.y)).ba;
        vec2 v11 = texture(inputTexture, texel1 + vec2(      0,       0)).ba;
        vec2 v12 = texture(inputTexture, texel1 + vec2(      0, -step.y)).ba;
        vec2 v20 = texture(inputTexture, texel1 + vec2(-step.x, +step.y)).ba;
        vec2 v21 = texture(inputTexture, texel1 + vec2(-step.x,       0)).ba;
        vec2 v22 = texture(inputTexture, texel1 + vec2(-step.x, -step.y)).ba;

        velocity = v11 + (dot(v00 + v22, vec2(1,1)) +
                          dot(v02 + v20, vec2(1,-1)) * vec2(1,-1) +
                          (v01 + v21 - v10 - v12) * vec2(2,-2) +
                          v11 * -4) / 8;

        texel = texel0 - v11 * 50.0 * deltaTime;
    }
    else
    {
        texel = texel0;
        velocity = vec2(0, 0);
    }

    // diffusion

    vec2 uv = texture(inputTexture, texel).rg;

    vec2 lapl;

    switch (laplacian)
    {
        case 1: 
            {
                // 3x3 kernel - 1 / distance
                vec2 uv00 = texture(inputTexture, texel + vec2(-step.x, -step.y)).rg;
                vec2 uv01 = texture(inputTexture, texel + vec2(    0.0, -step.y)).rg;
                vec2 uv02 = texture(inputTexture, texel + vec2( step.x, -step.y)).rg;
                vec2 uv10 = texture(inputTexture, texel + vec2(-step.x,     0.0)).rg;
                vec2 uv12 = texture(inputTexture, texel + vec2( step.x,     0.0)).rg;
                vec2 uv20 = texture(inputTexture, texel + vec2(-step.x, +step.y)).rg;
                vec2 uv21 = texture(inputTexture, texel + vec2(    0.0, +step.y)).rg;
                vec2 uv22 = texture(inputTexture, texel + vec2( step.x, +step.y)).rg;

                // kernel center weight.
                float lapl_center = 4 + 4 * _1_over_sqrt_2;

                lapl = _1_over_sqrt_2 * (uv00 + uv02 + uv20 + uv22) + 1 * (uv01 + uv10 + uv12 + uv21) - lapl_center * uv;
            }
            break;

        case 2:
            {
                // 3x3 kernel - 1 / distance^2
                vec2 uv00 = texture(inputTexture, texel + vec2(-step.x, -step.y)).rg;
                vec2 uv01 = texture(inputTexture, texel + vec2(    0.0, -step.y)).rg;
                vec2 uv02 = texture(inputTexture, texel + vec2( step.x, -step.y)).rg;

                vec2 uv10 = texture(inputTexture, texel + vec2(-step.x,     0.0)).rg;
                vec2 uv12 = texture(inputTexture, texel + vec2( step.x,     0.0)).rg;

                vec2 uv20 = texture(inputTexture, texel + vec2(-step.x, +step.y)).rg;
                vec2 uv21 = texture(inputTexture, texel + vec2(    0.0, +step.y)).rg;
                vec2 uv22 = texture(inputTexture, texel + vec2( step.x, +step.y)).rg;

                lapl = 0.5 * (uv00 + uv02 + uv20 + uv22) + 1 * (uv01 + uv10 + uv12 + uv21) - 6 * uv;
            }
            break;

        case 3:
            {
                // 3x3 cross kernel (no corners)
                vec2 uv0 = texture(inputTexture, texel + vec2(-step.x,     0.0)).rg;
                vec2 uv1 = texture(inputTexture, texel + vec2(+step.x,     0.0)).rg;
                vec2 uv2 = texture(inputTexture, texel + vec2(    0.0, -step.y)).rg;
                vec2 uv3 = texture(inputTexture, texel + vec2(    0.0, +step.y)).rg;

                lapl = uv0 + uv1 + uv2 + uv3 - 4.0 * uv;
            }
            break;

        case 4:
            {
                // 3x3 cross kernel (no corners, with a bit of noise)

                float rnd0 = 1.0 + rand(uv.r + step.x) / 100.0;
                float rnd1 = 1.0 + rand(uv.r - step.x) / 100.0;
                float rnd2 = 1.0 + rand(uv.g + step.x) / 100.0;
                float rnd3 = 1.0 + rand(uv.g - step.x) / 100.0;

                vec2 uv0 = texture(inputTexture, texel + vec2(-step.x,     0.0)).rg;
                vec2 uv1 = texture(inputTexture, texel + vec2(+step.x,     0.0)).rg;
                vec2 uv2 = texture(inputTexture, texel + vec2(    0.0, -step.y)).rg;
                vec2 uv3 = texture(inputTexture, texel + vec2(    0.0, +step.y)).rg;

                lapl = rnd0 * uv0 + rnd1 * uv1 + rnd2 * uv2 + rnd3 * uv3 - (rnd0 + rnd1 + rnd2 + rnd3) * uv;
            }
            break;
    }

    float du = diffusion.r * lapl.r - uv.r * uv.g * uv.g + feed * (1.0 - uv.r);
    float dv = diffusion.g * lapl.g + uv.r * uv.g * uv.g - (feed + kill) * uv.g;
    vec2 dst = uv + 1000 * deltaTime * vec2(du, dv);

    // If left mouse button is down, we'll paint with the brush.
    if (brush.a == 2.0)
    {
        vec2 dist2 = (texel.xy - brush.xy) * screenResolution;
        float dist = sqrt(dot(dist2, dist2));

        // If we're within a few pixels of the brush, add chemical B.
        if (dist < 5.0)
        {
            dst.r = 0.0;
            dst.g = 0.9;
        }

        // If we're close to the brush, give velocity a nudge.
        if (dist < 500.0)
        {
            velocity += 0.1 * brushVelocity / (dist * dist);
        }
    }

    // Spinning fountain at the center of the screen.
    if (false)
    {
        vec2 dist2 = (texel.xy - vec2(0.5,0.5)) * screenResolution;
        float dist = sqrt(dot(dist2, dist2));
        if (dist < 50.0)
        {
            float magnitude = 0.001; // * cos(time) * sin(time);
            velocity += magnitude * vec2(sin(time * 1.0), cos(time * 1.0)) / (dist * dist);
        }
    }

    // Pushing outward from the center.
    if (true)
    {
        vec2 dist2 = (texel.xy - vec2(0.5,0.5)) * screenResolution;
        float dist = sqrt(dot(dist2, dist2));
        float radius = max(screenResolution.x, screenResolution.y);
        if (dist <= radius)
        {
            float height = sqrt(radius * radius - dist * dist);
            height /= radius;
            velocity += 0.000000001 * height * dist2;
        }
    }

    if (false)
    {
        vec2 dist2 = (texel.xy - vec2(0.5,0.45)) * screenResolution;
        float dist = sqrt(dot(dist2, dist2));
        if (dist < 500.0)
        {
            float magnitude = abs(sin(time)) * deltaTime * 10;
            velocity += 0.1 * vec2(magnitude, 0) / (dist * dist);
        }
    }
    
    // decay velocity

    if (false)
    {
        if (velocity.x > 0)
        {
            velocity.x = max(0, velocity.x - velocity_decay);
        }
        else if (velocity.x < 0)
        {
            velocity.x = min(0, velocity.x + velocity_decay);
        }

        if (velocity.y > 0)
        {
            velocity.y = max(0, velocity.y - velocity_decay);
        }
        else if (velocity.y < 0)
        {
            velocity.y = min(0, velocity.y + velocity_decay);
        }
    }

    if (true)
    {
        velocity *= 0.999;
    }

    // clamp output
    dst.r = clamp(dst.r, 0.0, 1.0);
    dst.g = clamp(dst.g, 0.0, 1.0);

    FragColor = vec4(dst.r, dst.g, velocity.x, velocity.y);
}