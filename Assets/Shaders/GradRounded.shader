Shader "UI/GradientRounded"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Color2 ("Tint 2", Color) = (1,1,1,1)
        _Radius ("Radius", Range(0, 0.5)) = 0.1
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 texcoord  : TEXCOORD0;
                float4 vertex   : SV_POSITION;
                float4 color    : COLOR;
                float2 pos      : TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _Color2;
            float _Radius;
            float4 _MainTex_TexelSize;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color;
                OUT.pos = IN.texcoord;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float gradient = (IN.pos.x + IN.pos.y) * 0.5;
                fixed4 color = lerp(_Color2, _Color, gradient);
                
                float2 rounded = abs(IN.pos - 0.5) * 2.0;
                float distanceFromEdge = max(rounded.x, rounded.y);
                float radius = 1.0 - _Radius * 2.0;
                
                if (distanceFromEdge > radius)
                {
                    float alpha = 1.0 - smoothstep(radius, 1.0, distanceFromEdge);
                    color.a *= alpha;
                }
                
                half4 texColor = tex2D(_MainTex, IN.texcoord);
                color = color * texColor * IN.color;
                
                return color;
            }
            ENDCG
        }
    }
}