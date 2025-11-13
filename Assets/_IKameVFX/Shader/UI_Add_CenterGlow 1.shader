// Made with Amplify Shader Editor v1.9.8.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Hovl/Particles/UI_Add_CenterGlow"
{
    Properties
    {
        _MainTex ("Particle Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

        _Noise("Noise", 2D) = "white" {}
        _Flow("Flow", 2D) = "white" {}
        _Mask("Mask", 2D) = "white" {}
        _SpeedMainTexUVNoiseZW("Speed MainTex U/V + Noise Z/W", Vector) = (0,0,0,0)
        _DistortionSpeedXYPowerZ("Distortion Speed XY Power Z", Vector) = (0,0,0,0)
        _Emission("Emission", Float) = 2
        _Color("Color", Color) = (1,1,1,1)
        [Toggle]_Usecenterglow("Use center glow?", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)]_ZTest("ZTest", Int) = 4
        [Toggle]_CustomUVOffset("CustomUVOffset", Float) = 0
        _FillAmount("FillAmount", Range( 0 , 2)) = 0
        [HideInInspector] _texcoord( "", 2D ) = "white" {}

    }

    SubShader
    {
		LOD 0

        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }

        Stencil
        {
        	Ref [_Stencil]
        	ReadMask [_StencilReadMask]
        	WriteMask [_StencilWriteMask]
        	Comp [_StencilComp]
        	Pass [_StencilOp]
        }


        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend One One
        ColorMask [_ColorMask]

        
        Pass
        {
            Name "Default"
        CGPROGRAM
            #define ASE_VERSION 19801

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.5

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "UnityShaderVariables.cginc"
            #define ASE_NEEDS_FRAG_COLOR


            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 ase_texcoord2 : TEXCOORD2;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4  mask : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
                float4 ase_texcoord3 : TEXCOORD3;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float _UIMaskSoftnessX;
            float _UIMaskSoftnessY;

            uniform int _ZTest;
            uniform float _Usecenterglow;
            uniform float4 _SpeedMainTexUVNoiseZW;
            uniform float _CustomUVOffset;
            uniform sampler2D _Flow;
            uniform float4 _DistortionSpeedXYPowerZ;
            uniform float4 _Flow_ST;
            uniform sampler2D _Mask;
            uniform float4 _Mask_ST;
            uniform sampler2D _Noise;
            uniform float4 _Noise_ST;
            uniform float _FillAmount;
            uniform float _Emission;


            v2f vert(appdata_t v )
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.ase_texcoord3 = v.ase_texcoord2;

                v.vertex.xyz +=  float3( 0, 0, 0 ) ;

                float4 vPosition = UnityObjectToClipPos(v.vertex);
                OUT.worldPosition = v.vertex;
                OUT.vertex = vPosition;

                float2 pixelSize = vPosition.w;
                pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                float2 maskUV = (v.vertex.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
                OUT.texcoord = v.texcoord;
                OUT.mask = float4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));

                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN ) : SV_Target
            {
                //Round up the alpha color coming from the interpolator (to 1.0/256.0 steps)
                //The incoming alpha could have numerical instability, which makes it very sensible to
                //HDR color transparency blend, when it blends with the world's texture.
                const half alphaPrecision = half(0xff);
                const half invAlphaPrecision = half(1.0/alphaPrecision);
                IN.color.a = round(IN.color.a * alphaPrecision)*invAlphaPrecision;

                float2 appendResult21 = (float2(_SpeedMainTexUVNoiseZW.x , _SpeedMainTexUVNoiseZW.y));
                float2 uv_MainTex = IN.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                float4 texCoord115 = IN.ase_texcoord3;
                texCoord115.xy = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
                float2 appendResult126 = (float2((uv_MainTex.x*texCoord115.x + texCoord115.z) , (uv_MainTex.y*texCoord115.y + texCoord115.w)));
                float2 panner107 = ( 1.0 * _Time.y * appendResult21 + (( _CustomUVOffset )?( appendResult126 ):( uv_MainTex )));
                float2 appendResult100 = (float2(_DistortionSpeedXYPowerZ.x , _DistortionSpeedXYPowerZ.y));
                float4 uvs4_Flow = float4(IN.texcoord.xy,0,0);
                uvs4_Flow.xy = float4(IN.texcoord.xy,0,0).xy * _Flow_ST.xy + _Flow_ST.zw;
                float2 panner110 = ( 1.0 * _Time.y * appendResult100 + (uvs4_Flow).xy);
                float2 uv_Mask = IN.texcoord.xy * _Mask_ST.xy + _Mask_ST.zw;
                float4 tex2DNode33 = tex2D( _Mask, uv_Mask );
                float Flowpower102 = _DistortionSpeedXYPowerZ.z;
                float2 appendResult22 = (float2(_SpeedMainTexUVNoiseZW.z , _SpeedMainTexUVNoiseZW.w));
                float2 uv_Noise = IN.texcoord.xy * _Noise_ST.xy + _Noise_ST.zw;
                float2 panner108 = ( 1.0 * _Time.y * appendResult22 + uv_Noise);
                float4 temp_output_30_0 = ( tex2D( _MainTex, ( panner107 - ( (( tex2D( _Flow, panner110 ) * tex2DNode33.a )).rg * Flowpower102 ) ) ) * _Color * IN.color * tex2D( _Noise, panner108 ) );
                float clampResult38 = clamp( ( ( tex2DNode33.a + 1.0 ) - ( uvs4_Flow.z + _FillAmount ) ) , 0.0 , 1.0 );
                

                half4 color = ( (( _Usecenterglow )?( ( temp_output_30_0 * clampResult38 ) ):( temp_output_30_0 )) * ( _Emission + uvs4_Flow.z ) );

                #ifdef UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
                color.a *= m.x * m.y;
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                color.rgb *= color.a;
color.a = pow(color.a,1.2f);
                return color;
            }
        ENDCG
        }
    }
    CustomEditor "AmplifyShaderEditor.MaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19801
Node;AmplifyShaderEditor.CommentaryNode;104;-4130.993,490.5418;Inherit;False;1910.996;537.6462;Texture distortion;12;91;33;100;102;99;94;95;103;92;59;98;110;;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector4Node;99;-3968.293,619.481;Float;False;Property;_DistortionSpeedXYPowerZ;Distortion Speed XY Power Z;4;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;98;-3968,800;Inherit;False;0;91;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;100;-3535.482,654.5021;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;59;-3583.603,566.496;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;109;-3401.27,-330.4436;Inherit;False;1037.896;533.6285;Textures movement;11;107;108;29;21;89;22;15;115;122;124;126;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;138;-2659.417,-529.8935;Inherit;False;0;0;_MainTex;Shader;False;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;110;-3339.196,596.5295;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;115;-3584,-272;Inherit;False;2;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;29;-3200,-336;Inherit;False;0;-1;2;3;2;SAMPLER2D;0,0,0,0;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;91;-3152.937,567.9764;Inherit;True;Property;_Flow;Flow;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ScaleAndOffsetNode;122;-2944,-304;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;124;-2976,-192;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;33;-2992,800;Inherit;True;Property;_Mask;Mask;2;0;Create;True;0;0;0;False;0;False;-1;93ea19c7d0848ed47806cc339c1b1536;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.Vector4Node;15;-3351.27,-101.4007;Float;False;Property;_SpeedMainTexUVNoiseZW;Speed MainTex U/V + Noise Z/W;3;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;102;-3556.945,748.0421;Float;False;Flowpower;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;92;-2762.212,550.0183;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;126;-2750,-288;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;94;-2609.926,543.6367;Inherit;False;True;True;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;103;-2605.07,630.9626;Inherit;False;102;Flowpower;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;127;-2624,-384;Inherit;False;Property;_CustomUVOffset;CustomUVOffset;9;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;21;-2656,-160;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-2388.997,542.6455;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;107;-2432,-240;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;22;-2766.722,70.18491;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;89;-2861.858,-55.04038;Inherit;False;0;14;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;146;-3792,1120;Inherit;False;Property;_FillAmount;FillAmount;10;0;Create;True;0;0;0;False;0;False;0;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;145;-2679.209,1129.846;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;96;-2048,-160;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;108;-2577.237,-21.63752;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;147;-2944,1232;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;37;-2528,1216;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;31;-1700.012,122.3577;Float;False;Property;_Color;Color;6;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;False;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode;14;-1741.579,-96.7786;Inherit;True;Property;_Noise;Noise;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode;13;-1706.192,-354.2159;Inherit;True;Property;_MainTex;MainTex;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;0,0,0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.VertexColorNode;32;-1717.413,304.058;Inherit;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-1152,-32;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;38;-2192,1264;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-1056,256;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-656,400;Float;True;Property;_Emission;Emission;5;0;Create;True;0;0;0;False;0;False;2;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;90;-896,48;Float;False;Property;_Usecenterglow;Use center glow?;7;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;113;-512,240;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;112;-960,1120;Inherit;False;Property;_ZTest;ZTest;8;1;[Enum];Create;True;0;1;Option1;0;1;UnityEngine.Rendering.CompareFunction;True;0;False;4;4;False;0;1;INT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-448,64;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;137;-128,160;Float;False;True;-1;3;AmplifyShaderEditor.MaterialInspector;0;19;Hovl/Particles/UI_Add_CenterGlow;72845e343a75ab044b6c4a21787ebea9;True;Default;0;0;Default;2;True;True;4;1;False;;1;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;True;True;True;True;True;0;True;_ColorMask;False;False;False;False;False;False;False;True;True;0;True;_Stencil;255;True;_StencilReadMask;255;True;_StencilWriteMask;0;True;_StencilComp;0;True;_StencilOp;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;0;True;unity_GUIZTestMode;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;False;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;100;0;99;1
WireConnection;100;1;99;2
WireConnection;59;0;98;0
WireConnection;110;0;59;0
WireConnection;110;2;100;0
WireConnection;29;2;138;0
WireConnection;91;1;110;0
WireConnection;122;0;29;1
WireConnection;122;1;115;1
WireConnection;122;2;115;3
WireConnection;124;0;29;2
WireConnection;124;1;115;2
WireConnection;124;2;115;4
WireConnection;102;0;99;3
WireConnection;92;0;91;0
WireConnection;92;1;33;4
WireConnection;126;0;122;0
WireConnection;126;1;124;0
WireConnection;94;0;92;0
WireConnection;127;0;29;0
WireConnection;127;1;126;0
WireConnection;21;0;15;1
WireConnection;21;1;15;2
WireConnection;95;0;94;0
WireConnection;95;1;103;0
WireConnection;107;0;127;0
WireConnection;107;2;21;0
WireConnection;22;0;15;3
WireConnection;22;1;15;4
WireConnection;145;0;33;4
WireConnection;96;0;107;0
WireConnection;96;1;95;0
WireConnection;108;0;89;0
WireConnection;108;2;22;0
WireConnection;147;0;98;3
WireConnection;147;1;146;0
WireConnection;37;0;145;0
WireConnection;37;1;147;0
WireConnection;14;1;108;0
WireConnection;13;0;138;0
WireConnection;13;1;96;0
WireConnection;30;0;13;0
WireConnection;30;1;31;0
WireConnection;30;2;32;0
WireConnection;30;3;14;0
WireConnection;38;0;37;0
WireConnection;41;0;30;0
WireConnection;41;1;38;0
WireConnection;90;0;30;0
WireConnection;90;1;41;0
WireConnection;113;0;52;0
WireConnection;113;1;98;3
WireConnection;51;0;90;0
WireConnection;51;1;113;0
WireConnection;137;0;51;0
ASEEND*/
//CHKSM=74C08EA890C8DDE29DBB91F84CBEDAABA21A2E35