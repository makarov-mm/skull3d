#version 460 core

in vec2 vUV;
in vec3 vWorldPos;
in mat3 vTBN;

out vec4 FragColor;

uniform sampler2D uDiffuse;
uniform sampler2D uNormalMap;
uniform vec3 uLightDir;
uniform vec3 uViewPos;
uniform float uTime;
uniform float uEffectMix;
uniform float uObjectAlpha;
uniform float uFloorY;
uniform int uRenderMode;   // 0 = model, 1 = procedural mirror floor
uniform int uMirrorPass;   // 0 = normal model, 1 = reflected model

float saturate(float v)
{
    return clamp(v, 0.0, 1.0);
}

vec3 proceduralEnvironment(vec3 r)
{
    float sky = saturate(r.y * 0.5 + 0.5);
    vec3 horizon = vec3(0.07, 0.08, 0.10);
    vec3 zenith = vec3(0.12, 0.32, 0.58);
    vec3 ground = vec3(0.035, 0.032, 0.030);

    vec3 env = mix(ground, mix(horizon, zenith, sky), smoothstep(-0.15, 0.75, r.y));

    float sun = pow(saturate(dot(normalize(r), normalize(vec3(-0.35, 0.72, 0.60)))), 96.0);
    env += vec3(1.0, 0.82, 0.45) * sun * 2.5;

    return env;
}

vec4 renderFloor()
{
    vec2 p = vWorldPos.xz;
    vec2 coord = p * 0.16;
    vec2 fw = max(fwidth(coord), vec2(0.001));
    vec2 grid = abs(fract(coord - 0.5) - 0.5) / fw;
    float line = 1.0 - saturate(min(grid.x, grid.y));

    float majorX = 1.0 - smoothstep(0.0, 0.025, abs(fract(p.x * 0.04 - 0.5) - 0.5));
    float majorZ = 1.0 - smoothstep(0.0, 0.025, abs(fract(p.y * 0.04 - 0.5) - 0.5));
    float major = max(majorX, majorZ) * 0.35;

    float radial = length(p) / 55.0;
    float fade = 1.0 - smoothstep(0.55, 1.0, radial);

    vec3 base = vec3(0.025, 0.030, 0.038);
    vec3 gridColor = vec3(0.10, 0.52, 0.85) * line + vec3(0.40, 0.75, 1.0) * major;
    vec3 color = base + gridColor * fade;

    float alpha = 0.32 * fade + 0.18 * line * fade;
    return vec4(color, alpha);
}

void main()
{
    if (uRenderMode == 1)
    {
        FragColor = renderFloor();
        return;
    }

    vec3 albedo = texture(uDiffuse, vUV).rgb;

    vec3 normalTS = texture(uNormalMap, vUV).rgb * 2.0 - 1.0;
    normalTS.xy *= mix(0.0, 1.25, uEffectMix);
    normalTS = normalize(normalTS);

    vec3 N = normalize(vTBN * normalTS);
    vec3 L = normalize(-uLightDir);
    vec3 V = normalize(uViewPos - vWorldPos);
    vec3 H = normalize(L + V);

    float NoL = saturate(dot(N, L));
    float NoV = saturate(dot(N, V));
    float NoH = saturate(dot(N, H));

    vec3 ambient = albedo * 0.14;
    vec3 diffuse = albedo * NoL;

    float specPower = mix(32.0, 96.0, uEffectMix);
    float spec = pow(NoH, specPower) * mix(0.18, 0.52, uEffectMix);

    vec3 R = reflect(-V, N);
    vec3 env = proceduralEnvironment(R);
    float fresnel = pow(1.0 - NoV, 3.0);
    float reflectionAmount = uEffectMix * (0.08 + 0.55 * fresnel);

    vec3 color = ambient + diffuse + vec3(spec);
    color = mix(color, env, reflectionAmount);

    float rim = pow(1.0 - NoV, 2.2);
    vec3 rimColor = vec3(0.12, 0.70, 1.0) * rim * 0.85 * uEffectMix;
    color += rimColor;

    float scan = sin((vWorldPos.y - uFloorY) * 1.25 + uTime * 3.2) * 0.5 + 0.5;
    scan = pow(scan, 28.0);
    float heightFade = 1.0 - smoothstep(36.0, 58.0, abs(vWorldPos.y - uFloorY));
    color += vec3(0.05, 0.55, 1.0) * scan * heightFade * 0.35 * uEffectMix;

    float alpha = uObjectAlpha;

    if (uMirrorPass != 0)
    {
        float mirrorFade = 1.0 - smoothstep(0.0, 45.0, abs(vWorldPos.y - uFloorY));
        color = mix(color, vec3(0.02, 0.22, 0.38), 0.55);
        color *= 0.72;
        alpha *= 0.34 * mirrorFade;
    }

    color = pow(max(color, vec3(0.0)), vec3(1.0 / 2.2));
    FragColor = vec4(color, alpha);
}
