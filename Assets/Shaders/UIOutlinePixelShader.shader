Shader "SimpleMystiaUI/UIOutlinePixel"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth ("Outline Width", Range(0, 10)) = 1
        
        [Toggle] _ShowOriginal ("Show Original Image", Float) = 0
        
        _UVOffsetX ("UV Offset X", Range(-1, 1)) = 0
        _UVOffsetY ("UV Offset Y", Range(-1, 1)) = 0
        
        // Required for UI.Mask
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
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            fixed4 _Color;
            fixed4 _OutlineColor;
            float _OutlineWidth;
            float _UVOffsetX;
            float _UVOffsetY;
            float _ShowOriginal;
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _ClipRect;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = v.texcoord;
                OUT.texcoord.x += _UVOffsetX;
                OUT.texcoord.y += _UVOffsetY;
                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = tex2D(_MainTex, frac(IN.texcoord)) * IN.color;
                
                // Sample texture around current pixel for outline
                float2 up = float2(0, _MainTex_TexelSize.y * _OutlineWidth);
                float2 down = float2(0, -_MainTex_TexelSize.y * _OutlineWidth);
                float2 left = float2(-_MainTex_TexelSize.x * _OutlineWidth, 0);
                float2 right = float2(_MainTex_TexelSize.x * _OutlineWidth, 0);
                
                half4 pixelUp = tex2D(_MainTex, frac(IN.texcoord + up));
                half4 pixelDown = tex2D(_MainTex, frac(IN.texcoord + down));
                half4 pixelLeft = tex2D(_MainTex, frac(IN.texcoord + left));
                half4 pixelRight = tex2D(_MainTex, frac(IN.texcoord + right));

                float outline = 0;
                if (color.a < 0.1) // If current pixel is transparent
                {
                    // Check if any neighboring pixel is not transparent
                    outline = max(max(pixelUp.a, pixelDown.a), max(pixelLeft.a, pixelRight.a));
                }

                // If ShowOriginal is off, make the original image transparent
                color.a *= _ShowOriginal;
                
                // Blend between texture color and outline color
                color = lerp(color, _OutlineColor, outline * _OutlineColor.a);
                
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                return color;
            }
            ENDCG
        }
    }
} 