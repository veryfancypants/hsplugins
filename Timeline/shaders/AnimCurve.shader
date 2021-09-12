// shameless reverse engineer

Shader "Unlit/AnimCurve"
{
    Properties
    {
[PerRendererData] _MainTex ("Texture", 2D) = "white" {}
_Color ("Tint", Color) = (0,1,0,1)
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
           float2 in_TEXCOORD0 : TEXCOORD0;
        };
        struct vs_out
        {
            float2 vs_TEXCOORD0 : TEXCOORD0;
        };

            sampler2D _MainTex;
            fixed4 _Color;
            float4 _ClipRect;


            vs_out vert (appdata IN, out float4 outpos : SV_POSITION)
            {
              float4 u_xlat0;
              vs_out o;
              o.vs_TEXCOORD0 = IN.in_TEXCOORD0;
              outpos = UnityObjectToClipPos(IN.in_POSITION0);
              return o;
            }

            fixed4 frag(vs_out arg) : SV_Target
            {
              float4 u_xlat0;
              fixed4 SV_Target0;
              u_xlat0.x = arg.vs_TEXCOORD0.x;
              u_xlat0.y = 0.5;
              u_xlat0 = tex2D(_MainTex, u_xlat0.xy);
              u_xlat0.x = (-u_xlat0.x) + arg.vs_TEXCOORD0.y;
              u_xlat0.x = float(1.0) / abs(u_xlat0.x);
              u_xlat0.x = u_xlat0.x + 100.0;
              u_xlat0.x = 101.0 / u_xlat0.x;
              SV_Target0.w = (-u_xlat0.x) + 1.0;
              SV_Target0.xyz = _Color.xyz;
              return SV_Target0;
            }

            ENDCG
        }
    }
}
