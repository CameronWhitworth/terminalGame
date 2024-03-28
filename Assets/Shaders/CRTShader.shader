Shader "Custom/CRTShaderWithTerminalEffects"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Display Color", Color) = (0.0,1.0,0.0,1.0)
        _FlickerFrequency ("Flicker Frequency", Float) = 10.0
        _GhostingAmount ("Ghosting Amount", Float) = 0.005
        _ScanlineSpeed ("Scanline Speed", Float) = 1.0
        _ScanlineAmount ("Scanline Amount", Float) = 100.0
        _ScanlineIntensity ("Scanline Intensity", Float) = 0.5
        _NoiseIntensity ("Noise Intensity", Float) = 0.5
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _FlickerFrequency;
            float _GhostingAmount;
            float _ScanlineSpeed;
            float _ScanlineAmount;
            float _ScanlineIntensity;
            float _NoiseIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float rand(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898,78.233))) * 43758.5453);
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 tex = tex2D(_MainTex, i.uv);
                float flicker = sin(_Time.y * _FlickerFrequency) * 0.1 + 0.9;

                // Generate noise
                float noise = rand(i.uv + _Time.y); // Use the custom rand function
                tex.rgb += _NoiseIntensity * (noise - 0.5);

                // Enhanced ghosting effect
                float ghostOffset1 = sin(_Time.y * 2.0) * _GhostingAmount * 0.5;
                float ghostOffset2 = cos(_Time.y * 3.0) * _GhostingAmount;
                float4 ghost1 = tex2D(_MainTex, i.uv + float2(ghostOffset1, 0.0));
                float4 ghost2 = tex2D(_MainTex, i.uv + float2(ghostOffset2, 0.0));
                
                // Blend the ghost layers with the main texture
                float ghostIntensity = 0.15; // Adjust this value to control the intensity of the ghosting effect
                tex = lerp(tex, ghost1 * _Color, ghostIntensity) + lerp(tex, ghost2 * _Color, ghostIntensity * 0.5);

                float scanlineEffect = sin((i.uv.y + _Time.y * _ScanlineSpeed) * _ScanlineAmount) * _ScanlineIntensity;
                tex.rgb -= scanlineEffect;

                // Apply the _Color tint to the texture
                tex *= _Color; // This multiplies the texture color by the display color, tinting the image

                // Color, flickering, and ghosting application
                tex.rgb *= flicker; // Apply flicker effect after color to ensure it affects the colored output
                return tex;
            }


            ENDCG
        }
    }
    FallBack "Diffuse"
}
