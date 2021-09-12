// shameless reverse engineer

Shader "Unlit/Checker"
{
    Properties
    {
[PerRendererData] _MainTex ("Texture", 2D) = "white" {}
_Color ("Tint", Color) = (1,0,1,1)
 _CheckerSize ("Checker Size", Float) = 8
 _RandomFlag ("Random Flag", Float) = 0
[MaterialToggle]  _DrawChecker ("Draw Checker", Float) = 0
_StencilComp ("Stencil Comparison", Float) = 8
_Stencil ("Stencil ID", Float) = 0
_StencilOp ("Stencil Operation", Float) = 0
_StencilWriteMask ("Stencil Write Mask", Float) = 255
_StencilReadMask ("Stencil Read Mask", Float) = 255
_ColorMask ("Color Mask", Float) = 15
_ClipRect ("Clip Rect", vector) = (-32767, -32767, 32767, 32767)
    }
      SubShader
{
     Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" "PreviewType" = "Plane" "CanUseSpriteAtlas" = "true" }
  Stencil {
   Ref[_Stencil]
   ReadMask[_StencilReadMask]
   WriteMask[_StencilWriteMask]
   Comp[_StencilComp]
   Pass[_StencilOp]
  }
      Blend SrcAlpha OneMinusSrcAlpha
    ColorMask[_ColorMask]
    Pass
    {
        CGPROGRAM

        #pragma vertex vert
        #pragma fragment frag

        #include "UnityCG.cginc"
        #include "UnityUI.cginc"

        struct appdata
        {
           float4 in_POSITION0 : POSITION;
           float4 in_COLOR0 : COLOR;
           float2 in_TEXCOORD0 : TEXCOORD0;
        };
struct vs_out
{
  float4 vs_COLOR0 : COLOR;
  float2 vs_TEXCOORD0 : TEXCOORD0;
  float4 vs_TEXCOORD1 : TEXCOORD1;
};

            sampler2D _MainTex;
            fixed4 _Color;
            float _CheckerSize;
            float _DrawChecker;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float _RandomFlag;


            vs_out vert (appdata IN, out float4 outpos : SV_POSITION)
            {
              float4 u_xlat0;
              vs_out o;
              o.vs_COLOR0 = IN.in_COLOR0 * _Color;
              o.vs_TEXCOORD0.xy = IN.in_TEXCOORD0.xy;
              o.vs_TEXCOORD1 = IN.in_POSITION0;
              outpos = UnityObjectToClipPos(o.vs_TEXCOORD1);
              return o;
            }

            fixed4 frag(vs_out arg) : SV_Target
            {
              float4 vs_COLOR0 = arg.vs_COLOR0;
              float2 vs_TEXCOORD0 = arg.vs_TEXCOORD0;
              float4 vs_TEXCOORD1 = arg.vs_TEXCOORD1;
              float4 SV_Target0;
              float4 u_xlat0;
              float4 u_xlat1;
              bool4 u_xlatb1;
              float4 u_xlat2;
              float2 u_xlat3;
              bool u_xlatb3;

                u_xlat0.x = float(uint(_CheckerSize));
                u_xlat0.xy = vs_TEXCOORD1.xy / u_xlat0.xx;
                u_xlat0.xy = floor(u_xlat0.xy);
                u_xlat0.x = abs(u_xlat0.y) + abs(u_xlat0.x);
                u_xlat3.x = u_xlat0.x + u_xlat0.x;
                u_xlatb3 = u_xlat3.x >= (-u_xlat3.x);
                u_xlat3.xy = (bool(u_xlatb3)) ? float2(2.0, 0.5) : float2(-2.0, -0.5);
                u_xlat0.x = u_xlat3.y * u_xlat0.x;
                u_xlat0.x = u_xlat0.x-floor(u_xlat0.x);
                u_xlat0.x = u_xlat0.x * u_xlat3.x;
                float checker_state = u_xlat0.x * _DrawChecker;

                u_xlat1 = tex2D(_MainTex, vs_TEXCOORD0.xy);
                u_xlat1 = u_xlat1 * vs_COLOR0;
                u_xlat0 = u_xlat1 + u_xlat1 * float4(-0.666666627, -0.666666627, -0.666666627, -0.333333313) * checker_state;
                bool test = (vs_TEXCOORD1.x >= _ClipRect.x) &&
                    (vs_TEXCOORD1.y >= _ClipRect.y) && 
                    (_ClipRect.z >= vs_TEXCOORD1.x) &&  
                    (_ClipRect.w >= vs_TEXCOORD1.y);
                
                SV_Target0.w = u_xlat0.w * (test ? 1.0 : 0.0);
                SV_Target0.xyz = u_xlat0.xyz;
                return SV_Target0;
            }

            ENDCG
        }
    }
}
