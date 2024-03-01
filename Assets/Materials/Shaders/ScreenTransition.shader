// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "UI/Gradient"
{
    Properties
    {
        [HideInInspector][PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}

        _TransImg("Transition Image", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _Position("Position", Range(0., 1.)) = 0.5
        _Smoothing("Smoothing", Range(.0001, 1.)) = 0.1
        [KeywordEnum(Vignette, Up_Down, Down_Up, Left_Right, Right_Left, Fade, Image, Image_Invert)] _Type("Type", Integer) = 0

        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255

        _ColorMask("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
    }

        SubShader
        {
            Tags
            {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
                "CanUseSpriteAtlas" = "True"
            }

            Stencil
            {
                Ref[_Stencil]
                Comp[_StencilComp]
                Pass[_StencilOp]
                ReadMask[_StencilReadMask]
                WriteMask[_StencilWriteMask]
            }

            Cull Off
            Lighting Off
            ZWrite Off
            ZTest[unity_GUIZTestMode]
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask[_ColorMask]

            Pass
            {
                Name "Default"
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0
                #pragma shader_feature _TYPE_VIGNETTE _TYPE_UP_DOWN _TYPE_DOWN_UP _TYPE_LEFT_RIGHT _TYPE_RIGHT_LEFT _TYPE_FADE _TYPE_IMAGE _TYPE_IMAGE_INVERT

                #include "UnityCG.cginc"
                #include "UnityUI.cginc"

                #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
                #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

                struct appdata_t
                {
                    float4 vertex   : POSITION;
                    float4 color    : COLOR;
                    float2 texcoord : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f
                {
                    float4 vertex   : SV_POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord  : TEXCOORD0;
                    float4 worldPosition : TEXCOORD1;
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                sampler2D _MainTex;
                fixed4 _Color;
                half _Position;
                half _Smoothing;
                fixed4 _TextureSampleAdd;
                sampler2D _TransImg;
                float4 _ClipRect;
                float4 _MainTex_ST;

                v2f vert(appdata_t v)
                {
                    v2f OUT;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                    OUT.worldPosition = v.vertex;
                    OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                    OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                    OUT.color = _Color;
                    return OUT;
                }

                fixed4 frag(v2f IN) : SV_Target
                {
                    half4 color = IN.color;
                    half pos = (1 - _Position) * (1. + _Smoothing * 2) - _Smoothing;

                    #ifdef _TYPE_VIGNETTE
                    float2 uv = IN.texcoord - 0.5;

                    color.w *= smoothstep(pos - _Smoothing, pos + _Smoothing, length(float2(uv.x, uv.y)));

                    #elif _TYPE_UP_DOWN
                    color.w *= smoothstep(pos - _Smoothing, pos + _Smoothing, IN.texcoord.y);
                    #elif _TYPE_DOWN_UP
                    color.w *= smoothstep(1 - pos + _Smoothing, 1 - pos - _Smoothing, IN.texcoord.y);

                    #elif _TYPE_LEFT_RIGHT
                    color.w *= smoothstep(1 - pos + _Smoothing, 1 - pos - _Smoothing, IN.texcoord.x);
                    #elif _TYPE_RIGHT_LEFT
                    color.w *= smoothstep(pos - _Smoothing, pos + _Smoothing, IN.texcoord.x);

                    #elif _TYPE_FADE
                    color.w *= _Position;
                    #elif _TYPE_IMAGE
                    color.w *= smoothstep(pos - _Smoothing, pos + _Smoothing, tex2D(_TransImg, IN.texcoord));
                    #elif _TYPE_IMAGE_INVERT
                    color.w *= smoothstep(pos - _Smoothing, pos + _Smoothing, 1 - tex2D(_TransImg, IN.texcoord));
                    #endif

                    #ifdef UNITY_UI_CLIP_RECT
                    color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                    #endif

                    #ifdef UNITY_UI_ALPHACLIP
                    clip(color.a - 0.001);
                    #endif

                    return color;
                }
            ENDCG
            }
        }
}