Shader "Unlit/motion_background"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Opacity("Opacity", Range(0, 1)) = 0.5
        _Speed("Speed", float) = 0.5
        _Scale("Scale", float) = 1
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType"="Opaque"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Opacity;
            float _Speed;
            float _Scale;

            interpolator vert(meshdata v)
            {
                interpolator i;
                i.vertex = UnityObjectToClipPos(v.vertex);
                
                float aspecRatio = _ScreenParams.x / _ScreenParams.y;

                i.uv = v.uv;
                i.uv.x *= aspecRatio;
                i.uv *= _Scale;

                i.uv += frac(float2(_Time.y * _Speed * sin(UNITY_PI / 6), _Time.y * _Speed * cos(UNITY_PI / 6)));
                
                return i;
            }

            fixed4 frag(interpolator i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, frac(i.uv));

                col.a *= _Opacity;
                
                return col;
            }
            ENDCG
        }
    }
}