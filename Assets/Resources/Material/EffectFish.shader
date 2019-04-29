// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "EffectFish"
{
	Properties
	{
		_Amplitude("Amplitude", Float) = 0
		_Cutoff( "Mask Clip Value", Float ) = 0.02
		_Speed("Speed", Float) = 1
		_Frequency("Frequency", Float) = 0
		_TimeOffest("Time Offest", Float) = 0
		_Slice("Slice", Range( -1 , 4)) = 2.024317
		_PositionalOffsetScaler("Positional Offset Scaler", Float) = 0
		_Range("Range", Range( -20 , 20)) = 8.044421
		_PositionalAmplitydeScaler("Positional Amplityde Scaler", Float) = 0
		_AmplitudeOffset("Amplitude Offset", Float) = 0
		[HDR]_GlowColor("Glow Color", Color) = (0,0,0,0)
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_BaseColor("Base Color", Color) = (0,0,0,0)
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_VertOffsetStrength("VertOffset Strength", Range( 0 , 1)) = 0.4
		_Thickness("Thickness", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
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
		uniform float _Amplitude;
		uniform float _Frequency;
		uniform float _TimeOffest;
		uniform float _PositionalOffsetScaler;
		uniform float _PositionalAmplitydeScaler;
		uniform float _AmplitudeOffset;
		uniform float4 _BaseColor;
		uniform float4 _GlowColor;
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform float _Cutoff = 0.02;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float4 transform18 = mul(unity_ObjectToWorld,float4( ase_vertex3Pos , 0.0 ));
			float Gradient35 = saturate( ( ( transform18.z + _Slice ) / _Range ) );
			float2 temp_cast_1 = (5.0).xx;
			float mulTime77 = _Time.y * _Speed;
			float2 panner76 = ( mulTime77 * float2( 0,-1 ) + float2( 0,0 ));
			float2 uv_TexCoord70 = v.texcoord.xy * temp_cast_1 + panner76;
			float Noise31 = step( frac( uv_TexCoord70.y ) , _Thickness );
			float4 appendResult99 = (float4(( ( _Amplitude * sin( ( ( _Frequency * _Time.y ) + _TimeOffest + ( ase_vertex3Pos.z * _PositionalOffsetScaler ) ) ) * ( ase_vertex3Pos.z * _PositionalAmplitydeScaler ) ) + _AmplitudeOffset ) , 0.0 , 0.0 , 0.0));
			float4 VertOffset63 = ( float4( ( ( ( ase_vertex3Pos * Gradient35 ) * _VertOffsetStrength ) * Noise31 ) , 0.0 ) + appendResult99 );
			v.vertex.xyz += VertOffset63.xyz;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Albedo = _BaseColor.rgb;
			float2 temp_cast_1 = (5.0).xx;
			float mulTime77 = _Time.y * _Speed;
			float2 panner76 = ( mulTime77 * float2( 0,-1 ) + float2( 0,0 ));
			float2 uv_TexCoord70 = i.uv_texcoord * temp_cast_1 + panner76;
			float Noise31 = step( frac( uv_TexCoord70.y ) , _Thickness );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float4 transform18 = mul(unity_ObjectToWorld,float4( ase_vertex3Pos , 0.0 ));
			float Gradient35 = saturate( ( ( transform18.z + _Slice ) / _Range ) );
			float4 Emmision53 = ( ( Noise31 * Gradient35 ) * _GlowColor );
			o.Emission = Emmision53.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
			float temp_output_46_0 = ( Gradient35 * 1.0 );
			float OpacityMask41 = ( ( ( Noise31 * ( 1.0 - Gradient35 ) ) - temp_output_46_0 ) + ( 1.0 - temp_output_46_0 ) );
			clip( OpacityMask41 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16301
168;290.4;1519;685;-262.3875;134.0787;1;True;True
Node;AmplifyShaderEditor.CommentaryNode;36;-1335.044,-370.022;Float;False;1656.803;551.5207;Comment;8;18;13;14;20;19;15;21;35;YGradient;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;34;-1350.241,-942.6097;Float;False;1559.112;450.4399;Noise;9;31;10;6;9;5;8;3;4;1;Noise;1,1,1,1;0;0
Node;AmplifyShaderEditor.PosVertexDataNode;13;-1285.044,-228.0053;Float;True;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;1;-1300.241,-702.0712;Float;False;Property;_Speed;Speed;2;0;Create;True;0;0;False;0;1;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ObjectToWorldTransfNode;18;-973.3773,-227.5236;Float;True;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;15;-1045.534,138.4154;Float;False;Property;_Slice;Slice;6;0;Create;True;0;0;False;0;2.024317;-1;-1;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;78;-1718.105,2119.361;Float;False;992.6456;805.6234;Adding the scaled and offset time value to the vertex's y position;4;92;89;80;79;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode;77;-1409.052,-1188.323;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;75;-1366.644,-1481.736;Float;False;1693.032;432.8213;Comment;5;70;69;71;72;73;Noise2;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;71;-1316.644,-1311.667;Float;False;Constant;_NoiseScale;NoiseScale;10;0;Create;True;0;0;False;0;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;76;-1166.609,-1243.616;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;14;-611.5524,-316.881;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-583.4293,-50.62453;Float;False;Property;_Range;Range;8;0;Create;True;0;0;False;0;8.044421;1;-20;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;79;-1612.625,2571.237;Float;False;533;302;Scales Vertex Y Position;3;88;83;82;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;80;-1605.476,2218.191;Float;False;478;329;Scales and Offests Time Input;4;86;85;84;81;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode;84;-1567.134,2338.896;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;83;-1589.029,2630.55;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;81;-1574.266,2253.64;Float;False;Property;_Frequency;Frequency;3;0;Create;True;0;0;False;0;0;7.59;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;19;-324.4032,-283.795;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;82;-1588.625,2787.237;Float;False;Property;_PositionalOffsetScaler;Positional Offset Scaler;7;0;Create;True;0;0;False;0;0;19.88;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;70;-967.0941,-1358.698;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FractNode;69;-516.817,-1431.736;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-450.713,-1163.514;Float;False;Property;_Thickness;Thickness;16;0;Create;True;0;0;False;0;0;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;87;-1517.481,3063.091;Float;False;626.0944;371.8522;Uses distance form origin as scalar multiplier of amplitude;2;94;90;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;-1265.982,2636.103;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;21;-147.1222,-310.1976;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;86;-1256.421,2281.109;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;85;-1308.74,2446.347;Float;False;Property;_TimeOffest;Time Offest;4;0;Create;True;0;0;False;0;0;0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;42;-1273.479,349.5202;Float;False;1374.868;719.3678;Comment;10;41;49;47;48;46;39;45;38;40;37;OpacityMask;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;91;-689.2958,2135.23;Float;False;516.188;327.7604;Scaling and offsetting sin output;4;98;96;95;93;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;89;-1002.431,2390.517;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;90;-1507.409,3278.709;Float;False;Property;_PositionalAmplitydeScaler;Positional Amplityde Scaler;9;0;Create;True;0;0;False;0;0;6.7;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;35;81.75873,-320.0219;Float;True;Gradient;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;72;-193.6336,-1397.951;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;37;-1223.479,572.7559;Float;False;35;Gradient;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;93;-639.2958,2185.23;Float;False;Property;_Amplitude;Amplitude;0;0;Create;True;0;0;False;0;0;0.02;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;60;-1186.305,1648.431;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;94;-1117.554,3156.447;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;61;-1178.326,1853.236;Float;False;35;Gradient;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;92;-858.4889,2408.073;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;31;-31.13018,-789.811;Float;True;Noise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;45;-1232.836,711.4971;Float;False;35;Gradient;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;40;-958.1444,399.5203;Float;False;31;Noise;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;56;-1258.857,1140.713;Float;False;973.1454;432.8478;Comment;6;52;51;50;54;55;53;Emmision;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;-412.3961,2192.183;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;95;-522.8493,2377.769;Float;False;Property;_AmplitudeOffset;Amplitude Offset;10;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;38;-953.9644,585.9605;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-907.8268,1889.699;Float;False;Property;_VertOffsetStrength;VertOffset Strength;15;0;Create;True;0;0;False;0;0.4;0.3;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-912.3651,1674.951;Float;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-752.2373,595.1218;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;-937.9259,836.1065;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;52;-1253.687,1369.445;Float;True;35;Gradient;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-594.7228,1676.999;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;68;-542.5466,1825.316;Float;False;31;Noise;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;97;-128.094,2167.251;Float;False;245.9668;274.1254;Applying result to x axis;1;99;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;98;-315.2978,2335.677;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;51;-1217.957,1115.313;Float;True;31;Noise;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;99;-59.18369,2223.919;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;-278.306,1689.665;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;47;-589.3488,750.5571;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;48;-592.9169,879.4012;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-877.946,1206.471;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;54;-951.9495,1368.561;Float;False;Property;_GlowColor;Glow Color;11;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,1.717647,1.631373,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;100;-24.29144,1875.938;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-693.0689,1265.762;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;49;-371.2936,793.9213;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;41;-167.7273,755.2549;Float;True;OpacityMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;63;218.8667,1680.089;Float;True;VertOffset;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;53;-437.3112,1295.95;Float;True;Emmision;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;57;818.1483,-553.6271;Float;False;Property;_BaseColor;Base Color;13;0;Create;True;0;0;False;0;0,0,0,0;0.3647059,0.6842226,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;9;-402.7745,-631.3932;Float;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;6;-426.8503,-892.6097;Float;True;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;4;-1127.338,-706.833;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;5;-709.9878,-890.6005;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;44;960.541,234.1362;Float;False;41;OpacityMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;59;936.8586,136.542;Float;False;Property;_Smoothness;Smoothness;14;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;10;-169.983,-793.0342;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;8;-898.4214,-880.5547;Float;False;Property;_Tiling;Tiling;5;0;Create;True;0;0;False;0;5,5;55,55;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;64;975.6071,314.3906;Float;False;63;VertOffset;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PannerNode;3;-944.2982,-744.7697;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;43;864.3202,-77.67526;Float;False;53;Emmision;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;58;933.8143,67.90987;Float;False;Property;_Metallic;Metallic;12;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1254.022,-96.18706;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;EffectFish;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.02;True;True;0;True;TransparentCutout;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;18;0;13;0
WireConnection;77;0;1;0
WireConnection;76;1;77;0
WireConnection;14;0;18;3
WireConnection;14;1;15;0
WireConnection;19;0;14;0
WireConnection;19;1;20;0
WireConnection;70;0;71;0
WireConnection;70;1;76;0
WireConnection;69;0;70;2
WireConnection;88;0;83;3
WireConnection;88;1;82;0
WireConnection;21;0;19;0
WireConnection;86;0;81;0
WireConnection;86;1;84;0
WireConnection;89;0;86;0
WireConnection;89;1;85;0
WireConnection;89;2;88;0
WireConnection;35;0;21;0
WireConnection;72;0;69;0
WireConnection;72;1;73;0
WireConnection;94;0;83;3
WireConnection;94;1;90;0
WireConnection;92;0;89;0
WireConnection;31;0;72;0
WireConnection;96;0;93;0
WireConnection;96;1;92;0
WireConnection;96;2;94;0
WireConnection;38;0;37;0
WireConnection;62;0;60;0
WireConnection;62;1;61;0
WireConnection;39;0;40;0
WireConnection;39;1;38;0
WireConnection;46;0;45;0
WireConnection;65;0;62;0
WireConnection;65;1;66;0
WireConnection;98;0;96;0
WireConnection;98;1;95;0
WireConnection;99;0;98;0
WireConnection;67;0;65;0
WireConnection;67;1;68;0
WireConnection;47;0;39;0
WireConnection;47;1;46;0
WireConnection;48;0;46;0
WireConnection;50;0;51;0
WireConnection;50;1;52;0
WireConnection;100;0;67;0
WireConnection;100;1;99;0
WireConnection;55;0;50;0
WireConnection;55;1;54;0
WireConnection;49;0;47;0
WireConnection;49;1;48;0
WireConnection;41;0;49;0
WireConnection;63;0;100;0
WireConnection;53;0;55;0
WireConnection;6;0;5;0
WireConnection;4;0;1;0
WireConnection;5;0;8;0
WireConnection;5;1;3;0
WireConnection;10;0;6;0
WireConnection;10;1;9;0
WireConnection;3;1;4;0
WireConnection;0;0;57;0
WireConnection;0;2;43;0
WireConnection;0;3;58;0
WireConnection;0;4;59;0
WireConnection;0;10;44;0
WireConnection;0;11;64;0
ASEEND*/
//CHKSM=9DE2585D4B845F403110BE2D8EBA160A56E2EE01