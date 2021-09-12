// shameless reverse engineer

Shader "Unlit/Tiling"
{
    Properties
    {
[PerRendererData] _MainTex ("Texture", 2D) = "white" {}
_BlockLength ("BlockLength", Float) = 10
_Divisions ("Divisions", Float) = 10
_TilingX ("Tiling", Float) = 1
_Color ("Tint", Color) = (1,1,1,1)
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

float4 _ClipRect;
float _BlockLength;
float _Divisions;
float _TilingX;
fixed4 _Color;


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
              float2 vs_TEXCOORD0 = arg.vs_TEXCOORD0;
              float4 vs_TEXCOORD1 = arg.vs_TEXCOORD1;
              float4 SV_Target0;
              float2 u_xlat0;
              float2 u_xlat1;
              bool2 u_xlatb1;
              float2 u_xlat2;
              bool2 u_xlatb2;
              float u_xlat4;
              bool u_xlatb4;
              float u_xlat6;

              u_xlat0.x = _TilingX * 10.0;
              u_xlat0.x = _BlockLength / u_xlat0.x;
              u_xlat2.x = vs_TEXCOORD0.x * _Divisions;
              u_xlat0.x = u_xlat2.x / u_xlat0.x;
              u_xlatb2.x = u_xlat0.x >= (-u_xlat0.x);
              u_xlat2.x = (u_xlatb2.x) ? 1.0 : -1.0;
              u_xlat4 = u_xlat2.x * u_xlat0.x;
              u_xlat0.x = u_xlat0.x + 0.5;
              u_xlat0.x = floor(u_xlat0.x);
              u_xlat4 = u_xlat4-floor(u_xlat4);
              u_xlat2.x = u_xlat2.x * u_xlat4 + -0.5;
              u_xlat2.x = u_xlat2.x + u_xlat2.x;
              u_xlat2.x = -abs(u_xlat2.x) + 1.0;
              u_xlat2.x = 4.0 / u_xlat2.x;
              u_xlat2.x = u_xlat2.x + 32.0;
              u_xlat0.y = 36.0 / u_xlat2.x;
              u_xlat4 = u_xlat0.x * _Divisions;
              u_xlatb4 = u_xlat4 >= (-u_xlat4);
              u_xlat4 = (u_xlatb4) ? _Divisions : (-_Divisions);
              u_xlat6 = float(1.0) / u_xlat4;
              u_xlat0.x = u_xlat6 * u_xlat0.x;
              u_xlat0.x = u_xlat0.x-floor(u_xlat0.x);
              u_xlat0.x = u_xlat0.x * u_xlat4;
              u_xlat0.x = u_xlat0.x / _Divisions;
              u_xlat0.xy = (-u_xlat0.xy) + float2(1.0, 1.0);
              u_xlat0.x = floor(u_xlat0.x);
              u_xlat4 = (-vs_TEXCOORD0.y) + 1.0;
              u_xlat0.x = (-u_xlat0.x) + u_xlat4;
              u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
              u_xlat0.x = (-u_xlat0.x) + 1.0;
              u_xlat0.x = u_xlat0.x * 1.25;
              u_xlat0.x = min(u_xlat0.x, 1.0);
              u_xlat0.x = u_xlat0.y * u_xlat0.x;
              u_xlatb2.x = vs_TEXCOORD1.x >= _ClipRect.x;
              u_xlatb2.y = vs_TEXCOORD1.y >= _ClipRect.y;
              u_xlat2.x = u_xlatb2.x ? float(1.0) : 0.0;
              u_xlat2.y = u_xlatb2.y ? float(1.0) : 0.0;              
              u_xlatb1.x = _ClipRect.z >= vs_TEXCOORD1.x;
              u_xlatb1.y = _ClipRect.w >= vs_TEXCOORD1.y;
              u_xlat1.x = u_xlatb1.x ? float(1.0) : 0.0;
              u_xlat1.y = u_xlatb1.y ? float(1.0) : 0.0;              
              u_xlat2.xy = u_xlat2.xy * u_xlat1.xy;
              u_xlat2.x = u_xlat2.y * u_xlat2.x;
              SV_Target0.w = u_xlat2.x * u_xlat0.x;
              SV_Target0.xyz = float3(0.0, 0.0, 0.0);
              return SV_Target0;
            }

            ENDCG
        }
    }
}
