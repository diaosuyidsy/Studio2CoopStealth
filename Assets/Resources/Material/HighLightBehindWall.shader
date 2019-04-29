// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "HighlightBehindWall"
{
	Properties
	{
		_ASEOutlineWidth( "Outline Width", Float ) = 0
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Albedo("Albedo", 2D) = "white" {}
		_Normals("Normals", 2D) = "bump" {}
		_Speed("Speed", Float) = 1
		_Metallic("Metallic", 2D) = "white" {}
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.9512688
		[HDR]_Emission("Emission", 2D) = "white" {}
		[HDR]_EmissionColor("EmissionColor", Color) = (0,0,0,0)
		_XRayPower("XRayPower", Float) = 0
		_XRayColor("XRayColor", Color) = (0,0,0,0)
		_XRayScale("XRayScale", Float) = 0
		_XRayBias("XRayBias", Float) = 0
		_XRayIntensity("XRayIntensity", Float) = 0
		_Slice("Slice", Range( -3 , 5)) = 2.024317
		_Range("Range", Range( -20 , 20)) = 8.044421
		[HDR]_GlowColor("Glow Color", Color) = (0,0,0,0)
		_VertOffsetStrength("VertOffset Strength", Range( 0 , 1)) = 0.4
		_Thickness("Thickness", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0"}
		ZWrite Off
		ZTest Always
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline nofog alpha:fade  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		
		#include "UnityShaderVariables.cginc"
		
		
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};
		uniform float _XRayBias;
		uniform float _XRayScale;
		uniform float _XRayPower;
		uniform float4 _XRayColor;
		uniform float _XRayIntensity;
		uniform float _Slice;
		uniform float _Range;
		uniform half _ASEOutlineWidth;
		
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += ( v.normal * _ASEOutlineWidth );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNdotV32 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode32 = ( _XRayBias + _XRayScale * pow( 1.0 - fresnelNdotV32, _XRayPower ) );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float4 transform94 = mul(unity_ObjectToWorld,float4( ase_vertex3Pos , 0.0 ));
			float Gradient82 = saturate( ( ( transform94.y + _Slice ) / _Range ) );
			float temp_output_111_0 = ( 1.0 - Gradient82 );
			float HeightMask128 = temp_output_111_0;
			o.Emission = ( fresnelNode32 * _XRayColor ).rgb;
			o.Alpha = ( fresnelNode32 * (_XRayColor).a * _XRayIntensity * HeightMask128 );
			o.Normal = float3(0,0,-1);
		}
		ENDCG
		

		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		ZWrite On
		ZTest LEqual
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform float _Slice;
		uniform float _Range;
		uniform float _VertOffsetStrength;
		uniform float _Speed;
		uniform float _Thickness;
		uniform sampler2D _Normals;
		uniform float4 _Normals_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _GlowColor;
		uniform sampler2D _Emission;
		uniform float4 _Emission_ST;
		uniform float4 _EmissionColor;
		uniform sampler2D _Metallic;
		uniform float4 _Metallic_ST;
		uniform float _Smoothness;
		uniform float _Cutoff = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float4 transform94 = mul(unity_ObjectToWorld,float4( ase_vertex3Pos , 0.0 ));
			float Gradient82 = saturate( ( ( transform94.y + _Slice ) / _Range ) );
			float2 temp_cast_1 = (22.93).xx;
			float mulTime96 = _Time.y * _Speed;
			float2 panner98 = ( mulTime96 * float2( 0,-1 ) + float2( 0,0 ));
			float2 uv_TexCoord102 = v.texcoord.xy * temp_cast_1 + panner98;
			float Noise108 = step( frac( uv_TexCoord102.y ) , _Thickness );
			float3 VertOffset78 = ( ( ( ase_vertex3Pos * Gradient82 ) * _VertOffsetStrength ) * Noise108 );
			v.vertex.xyz += ( VertOffset78 + 0 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normals = i.uv_texcoord * _Normals_ST.xy + _Normals_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Normals, uv_Normals ) );
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			o.Albedo = tex2D( _Albedo, uv_Albedo ).rgb;
			float2 temp_cast_1 = (22.93).xx;
			float mulTime96 = _Time.y * _Speed;
			float2 panner98 = ( mulTime96 * float2( 0,-1 ) + float2( 0,0 ));
			float2 uv_TexCoord102 = i.uv_texcoord * temp_cast_1 + panner98;
			float Noise108 = step( frac( uv_TexCoord102.y ) , _Thickness );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float4 transform94 = mul(unity_ObjectToWorld,float4( ase_vertex3Pos , 0.0 ));
			float Gradient82 = saturate( ( ( transform94.y + _Slice ) / _Range ) );
			float4 Emmision91 = ( ( Noise108 * Gradient82 ) * _GlowColor );
			float2 uv_Emission = i.uv_texcoord * _Emission_ST.xy + _Emission_ST.zw;
			o.Emission = ( Emmision91 + ( tex2D( _Emission, uv_Emission ) * _EmissionColor ) ).rgb;
			float2 uv_Metallic = i.uv_texcoord * _Metallic_ST.xy + _Metallic_ST.zw;
			o.Metallic = tex2D( _Metallic, uv_Metallic ).r;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
			float temp_output_111_0 = ( 1.0 - Gradient82 );
			float temp_output_83_0 = ( Gradient82 * 1.0 );
			float OpacityMask114 = ( ( ( Noise108 * temp_output_111_0 ) - temp_output_83_0 ) + ( 1.0 - temp_output_83_0 ) );
			clip( OpacityMask114 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16400
-192.8;189.6;1519;605;3065.953;350.7489;3.439651;True;True
Node;AmplifyShaderEditor.CommentaryNode;44;-2841.169,-3183.743;Float;False;1559.112;450.4399;Noise;9;121;120;119;118;117;116;115;108;93;Noise;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;43;-2825.972,-2611.156;Float;False;1656.803;551.5207;Comment;8;105;101;100;99;95;94;92;82;YGradient;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;93;-2791.169,-2943.205;Float;False;Property;_Speed;Speed;3;0;Create;True;0;0;False;0;1;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;92;-2775.972,-2469.139;Float;True;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;46;-2857.572,-3722.87;Float;False;1693.032;432.8213;Comment;6;106;104;103;102;98;97;Noise2;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;95;-2581.263,-2100.456;Float;False;Property;_Slice;Slice;15;0;Create;True;0;0;False;0;2.024317;-3;-3;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;96;-2899.98,-3429.457;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ObjectToWorldTransfNode;94;-2464.305,-2468.657;Float;True;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;99;-2102.48,-2558.015;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;100;-2074.357,-2291.758;Float;False;Property;_Range;Range;16;0;Create;True;0;0;False;0;8.044421;3.323185;-20;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;98;-2723.839,-3466.549;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;97;-2807.572,-3552.801;Float;False;Constant;_NoiseScale;NoiseScale;10;0;Create;True;0;0;False;0;22.93;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;101;-1815.331,-2524.929;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;102;-2458.022,-3599.832;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FractNode;103;-2007.745,-3672.87;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;105;-1638.05,-2551.331;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;104;-1941.641,-3404.648;Float;False;Property;_Thickness;Thickness;19;0;Create;True;0;0;False;0;0;0.39;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;57;-2764.407,-1891.614;Float;False;1374.868;719.3678;Comment;11;114;113;112;111;110;109;107;90;86;83;128;OpacityMask;1,1,1,1;0;0
Node;AmplifyShaderEditor.StepOpNode;106;-1684.562,-3639.085;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;82;-1409.169,-2561.156;Float;True;Gradient;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;108;-1522.058,-3030.945;Float;True;Noise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;107;-2714.407,-1668.378;Float;False;82;Gradient;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;62;-2677.233,-592.7028;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;64;-2669.254,-387.8978;Float;False;82;Gradient;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;66;-2749.785,-1100.421;Float;False;973.1454;432.8478;Comment;5;91;89;88;87;84;Emmision;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;110;-2449.072,-1841.614;Float;False;108;Noise;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;70;-2403.293,-566.1827;Float;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;85;-2708.885,-1125.821;Float;True;108;Noise;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;69;-2398.755,-351.4348;Float;False;Property;_VertOffsetStrength;VertOffset Strength;18;0;Create;True;0;0;False;0;0.4;0.55;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;111;-2444.892,-1655.173;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;109;-2723.764,-1529.637;Float;False;82;Gradient;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;84;-2744.615,-871.6888;Float;True;82;Gradient;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;-2085.651,-564.1348;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;72;-2033.475,-415.8177;Float;False;108;Noise;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-1459.846,799.2528;Float;False;Property;_XRayBias;XRayBias;13;0;Create;True;0;0;False;0;0;0.11;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;88;-2442.877,-872.5728;Float;False;Property;_GlowColor;Glow Color;17;1;[HDR];Create;True;0;0;False;0;0,0,0,0;10.43295,1.147078,1.79377,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;-2368.874,-1034.663;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;128;-2279.662,-1500.145;Float;False;HeightMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-1472.962,904.4595;Float;False;Property;_XRayScale;XRayScale;12;0;Create;True;0;0;False;0;0;3.47;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;31;-1341.627,1112.69;Float;False;Property;_XRayColor;XRayColor;11;0;Create;True;0;0;False;0;0,0,0,0;0.9433962,0.3871484,0.5245246,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;83;-2428.854,-1405.027;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-1421.497,979.4801;Float;False;Property;_XRayPower;XRayPower;10;0;Create;True;0;0;False;0;0;1.92;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;112;-2243.165,-1646.012;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;32;-1247.159,759.1359;Float;False;Standard;TangentNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;-2183.997,-975.3718;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;127;-1082.26,1309.925;Float;False;128;HeightMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-1107.751,1229.682;Float;False;Property;_XRayIntensity;XRayIntensity;14;0;Create;True;0;0;False;0;0;0.32;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;113;-2080.277,-1490.577;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;86;-2083.845,-1361.733;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;34;-1078.749,1160.095;Float;False;FLOAT;3;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;-1769.234,-551.4688;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-863.8458,1047.253;Float;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;91;-1928.239,-945.1838;Float;True;Emmision;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;42;-909.5633,6.401886;Float;False;Property;_EmissionColor;EmissionColor;9;1;[HDR];Create;True;0;0;False;0;0,0,0,0;3.684505,0.1564176,0.1761277,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;78;-1272.061,-561.0448;Float;True;VertOffset;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;6;-994.1559,-182.7169;Float;True;Property;_Emission;Emission;8;1;[HDR];Create;True;0;0;False;0;None;c0d0f5fed56b7e444b66d52831662752;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-846.754,786.5487;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;90;-1862.222,-1447.212;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;81;-131.9061,-53.40186;Float;False;91;Emmision;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-115.9983,70.92734;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;80;-62.67673,393.0642;Float;False;78;VertOffset;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OutlineNode;37;-664.5878,953.0897;Float;False;0;True;Transparent;2;7;Back;3;0;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;114;-1658.655,-1485.879;Float;True;OpacityMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-640,464;Float;True;Property;_Occlusion;Occlusion;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;116;-1917.778,-3133.743;Float;True;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;123;402.1158,431.1236;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;119;-1660.911,-3034.168;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;121;-2435.226,-2985.904;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;1;-644.7616,-443.2419;Float;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;False;0;None;a93c558b46fa4ae41b1b7683920a4471;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;117;-2618.266,-2947.967;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;79;320.6682,182.7267;Float;False;114;OpacityMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;122;128.4196,15.10382;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;5;-644.3705,-217.191;Float;True;Property;_Normals;Normals;2;0;Create;True;0;0;False;0;None;4223c4d5d269163489439264d4d5e9d4;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;120;-2389.349,-3121.688;Float;False;Property;_Tiling;Tiling;7;0;Create;True;0;0;False;0;5,5;55,55;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;115;-1893.703,-2872.527;Float;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;118;-2200.916,-3131.734;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-638.533,259.6071;Float;True;Property;_Metallic;Metallic;4;0;Create;True;0;0;False;0;None;f92f347d479cdb9479e644911a755013;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;40;-634.9318,182.135;Float;False;Property;_Smoothness;Smoothness;5;0;Create;True;0;0;False;0;0.9512688;0.9;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;575.3018,-50.04151;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;HighlightBehindWall;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;96;0;93;0
WireConnection;94;0;92;0
WireConnection;99;0;94;2
WireConnection;99;1;95;0
WireConnection;98;1;96;0
WireConnection;101;0;99;0
WireConnection;101;1;100;0
WireConnection;102;0;97;0
WireConnection;102;1;98;0
WireConnection;103;0;102;2
WireConnection;105;0;101;0
WireConnection;106;0;103;0
WireConnection;106;1;104;0
WireConnection;82;0;105;0
WireConnection;108;0;106;0
WireConnection;70;0;62;0
WireConnection;70;1;64;0
WireConnection;111;0;107;0
WireConnection;71;0;70;0
WireConnection;71;1;69;0
WireConnection;87;0;85;0
WireConnection;87;1;84;0
WireConnection;128;0;111;0
WireConnection;83;0;109;0
WireConnection;112;0;110;0
WireConnection;112;1;111;0
WireConnection;32;1;30;0
WireConnection;32;2;29;0
WireConnection;32;3;28;0
WireConnection;89;0;87;0
WireConnection;89;1;88;0
WireConnection;113;0;112;0
WireConnection;113;1;83;0
WireConnection;86;0;83;0
WireConnection;34;0;31;0
WireConnection;76;0;71;0
WireConnection;76;1;72;0
WireConnection;35;0;32;0
WireConnection;35;1;34;0
WireConnection;35;2;33;0
WireConnection;35;3;127;0
WireConnection;91;0;89;0
WireConnection;78;0;76;0
WireConnection;36;0;32;0
WireConnection;36;1;31;0
WireConnection;90;0;113;0
WireConnection;90;1;86;0
WireConnection;41;0;6;0
WireConnection;41;1;42;0
WireConnection;37;0;36;0
WireConnection;37;2;35;0
WireConnection;114;0;90;0
WireConnection;116;0;118;0
WireConnection;123;0;80;0
WireConnection;123;1;37;0
WireConnection;119;0;116;0
WireConnection;119;1;115;0
WireConnection;121;1;117;0
WireConnection;117;0;93;0
WireConnection;122;0;81;0
WireConnection;122;1;41;0
WireConnection;118;0;120;0
WireConnection;118;1;121;0
WireConnection;0;0;1;0
WireConnection;0;1;5;0
WireConnection;0;2;122;0
WireConnection;0;3;2;0
WireConnection;0;4;40;0
WireConnection;0;10;79;0
WireConnection;0;11;123;0
ASEEND*/
//CHKSM=277F8A4206AFBA2877CC64F8B41C91BF3E1833E2