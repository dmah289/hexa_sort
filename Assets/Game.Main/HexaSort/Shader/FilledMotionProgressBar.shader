Shader "dmah/FilledMotionProgressBar"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _Opacity ("Opacity", Range(0, 1)) = 0.5
        _Speed ("Speed", float) = 0.5
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "CanUseSpriteAtlas" = "True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct meshdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct interpolator
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex,_MaskTex;
            float4 _MainTex_ST,_MaskTex_ST;
            float _Speed;

            interpolator vert(meshdata v)
            {
                interpolator i;
                i.vertex = UnityObjectToClipPos(v.vertex);
                
                i.uv.xy = v.uv;
                i.uv.zw = v.vertex.xy;
                i.uv.zw = i.uv.zw * _MaskTex_ST.xy + _MaskTex_ST.zw;
                i.uv.z += _Time.y * _Speed;
                
                return i;
            }

            fixed4 frag(interpolator i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv.xy);
                fixed4 mask = tex2D(_MaskTex, frac(i.uv.zw));
                col.rgb = lerp(col.rgb, mask.g*col.rgb, mask.a);

                return col;
            }
            ENDCG
        }
    }
}