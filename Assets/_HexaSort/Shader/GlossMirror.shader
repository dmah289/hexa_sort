Shader "Custom/GlossMirror"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}  // Texture của UI
        _BaseColor ("Base Color", Color) = (1,1,1,1)  // Màu nền UI
        _ReflectColor ("Reflection Color", Color) = (1,1,1,1) // Màu vệt sáng
        _Shininess ("Shininess", Range(1, 20)) = 10   // Độ sắc nét của vệt sáng
        _Speed ("Scan Speed", Range(0.1, 5)) = 1      // Tốc độ quét
        _Interval ("Scan Interval", Range(0.5, 5)) = 2 // Khoảng nghỉ giữa lần quét
        _Width ("Scan Width", Range(0.05, 0.5)) = 0.2 // Độ rộng của vệt sáng
        _Delay ("Delay Before Scan", Range(0, 5)) = 1 // Độ trễ trước khi quét
    }
    
    SubShader
    {
        Tags
        {
            "Queue"="Overlay" "RenderType"="Transparent"
        }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha // Đảm bảo giữ bo góc UI
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
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
            float4 _BaseColor;
            float4 _ReflectColor;
            float _Shininess;
            float _Speed;
            float _Interval;
            float _Width;
            float _Delay;

            // Vertex Shader
            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Fragment Shader
            fixed4 frag(v2f i) : SV_Target
            {
                // Tổng thời gian 1 chu kỳ (quét + nghỉ)
                float cycleTime = _Interval + _Delay;
                float timeMod = fmod(_Time.y * _Speed, cycleTime);

                // Nếu trong thời gian delay, vệt sáng chưa chạy
                if (timeMod < _Delay)
                {
                    return tex2D(_MainTex, i.uv) * _BaseColor;
                }

                // Điều chỉnh tiến trình quét từ ngoài UI vào và ra ngoài UI
                float scanProgress = (timeMod - _Delay) / _Interval;  // Tiến trình từ 0 -> 1 trong khoảng quét
                float scanStart = -1.5;  // Mở rộng vệt sáng xuất phát từ xa
                float scanEnd = 1.5;  // Đẩy vệt sáng ra ngoài UI

                // Tính toán vị trí vệt sáng
                float scanLine = i.uv.x + i.uv.y - lerp(scanStart, scanEnd, scanProgress);

                // Làm mượt vệt sáng, đảm bảo xuất hiện và kết thúc tự nhiên
                float scanEffect = smoothstep(-_Width * 2, _Width, scanLine) * smoothstep(1.5, 1.5 - _Width, scanLine);

                // Tăng độ sắc nét bằng shininess
                scanEffect = pow(scanEffect, _Shininess);

                // Màu phản chiếu
                fixed4 reflection = scanEffect * _ReflectColor;
                reflection.a *= tex2D(_MainTex, i.uv).a; // Giữ alpha UI để không làm mất bo góc

                // Kết hợp màu UI và vệt sáng
                return tex2D(_MainTex, i.uv) * _BaseColor + reflection;
            }
            ENDCG
        }
    }
}
