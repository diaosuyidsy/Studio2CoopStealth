// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FOVeffect"
{
	Properties
	{
		[HDR]_Color("Color", Color) = (0,0.4764547,0.8791991,0)
		_TransitionDistance("Transition Distance", Float) = 0
		[HDR]_Color0("Color 0", Color) = (0,0.4764547,0.8791991,0)
		_TransitionFalloff("Transition Falloff", Float) = 255
		_Scale("Scale", Range( 0 , 1)) = 0
		_Texture0("Texture 0", 2D) = "white" {}
		_GridOpacity("GridOpacity", Float) = 0
		_Grid_Width("Grid_Width", Range( 0 , 1)) = 0.1
		_RadialGrid_X("RadialGrid_X", Float) = 32
		_RadialGrid_Y("RadialGrid_Y", Float) = 4
		_ScanSpeed("ScanSpeed", Float) = 0
		_ScanOpacity("ScanOpacity", Float) = 0.5
		_CirclesOpacity("CirclesOpacity", Float) = 0
		_SpeedRed("SpeedRed", Float) = -1
		_SpeedGreen("SpeedGreen", Float) = -1
		_SpeedBlue("SpeedBlue", Float) = 0.5
		_Height("Height", Float) = 1.4
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			float3 viewDir;
			INTERNAL_DATA
		};

		uniform float4 _Color0;
		uniform float4 _Color;
		uniform float _TransitionDistance;
		uniform float _TransitionFalloff;
		uniform sampler2D _Texture0;
		uniform float _Scale;
		uniform float _SpeedRed;
		uniform float _SpeedGreen;
		uniform float _Height;
		uniform float _SpeedBlue;
		uniform float _CirclesOpacity;
		uniform float _ScanSpeed;
		uniform float _ScanOpacity;
		uniform float _RadialGrid_X;
		uniform float _Grid_Width;
		uniform float _RadialGrid_Y;
		uniform float _GridOpacity;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = float3(0,0,1);
			float3 objToWorld221 = mul( unity_ObjectToWorld, float4( float3( 0,0,0 ), 1 ) ).xyz;
			float3 ase_worldPos = i.worldPos;
			float clampResult228 = clamp( pow( ( distance( objToWorld221 , ase_worldPos ) / _TransitionDistance ) , _TransitionFalloff ) , 0.0 , 1.0 );
			float4 lerpResult197 = lerp( _Color0 , _Color , clampResult228);
			o.Emission = lerpResult197.rgb;
			float temp_output_72_0 = ( _Scale / 2.0 );
			float2 temp_cast_1 = (temp_output_72_0).xx;
			float2 temp_cast_2 = (( 1.0 - temp_output_72_0 )).xx;
			float2 temp_output_77_0 = (float2( 0,0 ) + (i.uv_texcoord - temp_cast_1) * (float2( 1,1 ) - float2( 0,0 )) / (temp_cast_2 - temp_cast_1));
			float mulTime102 = _Time.y * _SpeedRed;
			float cos84 = cos( mulTime102 );
			float sin84 = sin( mulTime102 );
			float2 rotator84 = mul( temp_output_77_0 - float2( 0.5,0.5 ) , float2x2( cos84 , -sin84 , sin84 , cos84 )) + float2( 0.5,0.5 );
			float2 temp_cast_3 = (temp_output_72_0).xx;
			float2 temp_cast_4 = (( 1.0 - temp_output_72_0 )).xx;
			float mulTime104 = _Time.y * _SpeedGreen;
			float cos85 = cos( mulTime104 );
			float sin85 = sin( mulTime104 );
			float2 rotator85 = mul( temp_output_77_0 - float2( 0.5,0.5 ) , float2x2( cos85 , -sin85 , sin85 , cos85 )) + float2( 0.5,0.5 );
			float2 UV74 = i.uv_texcoord;
			float2 Offset80 = ( ( _Height - 1 ) * i.viewDir.xy * 0.2 ) + UV74;
			float clampResult70 = clamp( (0.0 + (_Scale - 0.0) * (1.0 - 0.0) / (0.6 - 0.0)) , 0.0 , 1.0 );
			float temp_output_71_0 = ( clampResult70 / 2.0 );
			float2 temp_cast_5 = (temp_output_71_0).xx;
			float2 temp_cast_6 = (( 1.0 - temp_output_71_0 )).xx;
			float mulTime105 = _Time.y * _SpeedBlue;
			float cos86 = cos( mulTime105 );
			float sin86 = sin( mulTime105 );
			float2 rotator86 = mul( (float2( 0,0 ) + (Offset80 - temp_cast_5) * (float2( 1,1 ) - float2( 0,0 )) / (temp_cast_6 - temp_cast_5)) - float2( 0.5,0.5 ) , float2x2( cos86 , -sin86 , sin86 , cos86 )) + float2( 0.5,0.5 );
			float clampResult92 = clamp( ( ( tex2D( _Texture0, rotator84 ).r + tex2D( _Texture0, rotator85 ).g + tex2D( _Texture0, rotator86 ).b ) * _CirclesOpacity ) , 0.0 , 1.0 );
			float2 break107 = (float2( -1,-1 ) + (i.uv_texcoord - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 )));
			float temp_output_113_0 = ( ( atan2( break107.x , break107.y ) / 6.28318548202515 ) + 0.5 );
			float Ring_U120 = temp_output_113_0;
			float temp_output_142_0 = frac( ( ( _Time.y * -1.0 * _ScanSpeed ) + Ring_U120 ) );
			float temp_output_143_0 = ( temp_output_142_0 * temp_output_142_0 );
			float temp_output_175_0 = length( (float2( -1,-1 ) + (i.uv_texcoord - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 ))) );
			float Ring_V116 = temp_output_175_0;
			float clampResult147 = clamp( (1.0 + (Ring_V116 - 0.1) * (0.0 - 1.0) / (1.1 - 0.1)) , 0.0 , 1.0 );
			float clampResult149 = clamp( (0.0 + (temp_output_143_0 - 0.75) * (0.3 - 0.0) / (1.0 - 0.75)) , 0.0 , 1.0 );
			float clampResult176 = clamp( ( ( temp_output_143_0 * _ScanOpacity ) + ( ( max( step( frac( ( Ring_U120 * _RadialGrid_X ) ) , _Grid_Width ) , step( frac( ( Ring_V116 * _RadialGrid_Y ) ) , _Grid_Width ) ) * _GridOpacity ) * clampResult147 * temp_output_143_0 ) + clampResult149 ) , 0.0 , 1.0 );
			float clampResult163 = clamp( ( 1.0 - temp_output_175_0 ) , 0.0 , 1.0 );
			float RadialMask119 = clampResult163;
			float Scan153 = ( ( clampResult176 * RadialMask119 ) + 0.0 );
			o.Alpha = ( ( clampResult92 + Scan153 ) + 0.0 );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows exclude_path:deferred 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.viewDir = IN.tSpace0.xyz * worldViewDir.x + IN.tSpace1.xyz * worldViewDir.y + IN.tSpace2.xyz * worldViewDir.z;
				surfIN.worldPos = worldPos;
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16400
-21.6;351.2;1248;451;5426.938;-224.7688;2.147731;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;114;-5086.424,-1845.723;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;108;-4759.129,-2009.427;Float;True;5;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,1;False;3;FLOAT2;-1,-1;False;4;FLOAT2;1,1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;107;-4416.745,-1846.244;Float;True;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.TauNode;106;-3834.907,-1680.676;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.ATan2OpNode;110;-4018.11,-1825.477;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;109;-3683.328,-1813.585;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;111;-3642.848,-1587.471;Float;False;Constant;_Float8;Float 8;1;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;174;-2717.76,-1463.856;Float;True;5;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,1;False;3;FLOAT2;-1,-1;False;4;FLOAT2;1,1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LengthOpNode;175;-2420.42,-1464.097;Float;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;113;-3466.658,-1814.742;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;120;-3116.519,-1799.503;Float;False;Ring_U;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;116;-2308.22,-1726.992;Float;False;Ring_V;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;123;-4993.857,-1143.826;Float;False;Property;_RadialGrid_X;RadialGrid_X;8;0;Create;True;0;0;False;0;32;32;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;139;-4834.116,-605.3272;Float;False;Property;_ScanSpeed;ScanSpeed;10;0;Create;True;0;0;False;0;0;0.56;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;126;-4985.873,-1034.369;Float;False;116;Ring_V;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;137;-5124.814,-825.7517;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;127;-4990.422,-950.6898;Float;False;Property;_RadialGrid_Y;RadialGrid_Y;9;0;Create;True;0;0;False;0;4;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;125;-4980.027,-1244.024;Float;False;120;Ring_U;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;138;-4637.015,-775.608;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;141;-4601.948,-606.2623;Float;False;120;Ring_U;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;124;-4778.658,-1242.031;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;128;-4777.938,-1031.833;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;129;-4637.212,-1063.529;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;140;-4396.034,-736.8916;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;130;-4642.282,-1309.494;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;133;-4699.839,-1424.444;Float;False;Property;_Grid_Width;Grid_Width;7;0;Create;True;0;0;False;0;0.1;0.056;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;67;-4614.766,527.3593;Float;False;Property;_Scale;Scale;4;0;Create;True;0;0;False;0;0;0.451;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;73;-4578.782,167.2067;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FractNode;142;-4237.479,-738.961;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;69;-4279.407,615.3772;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.6;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;132;-4425.515,-1072.013;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;131;-4443.795,-1311.192;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;145;-4229.553,-967.9583;Float;True;116;Ring_V;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;134;-4171.03,-1305.027;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;143;-4036.323,-761.3931;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;136;-4125.761,-1060.341;Float;False;Property;_GridOpacity;GridOpacity;6;0;Create;True;0;0;False;0;0;0.68;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;146;-4021.836,-979.0317;Float;True;5;0;FLOAT;0;False;1;FLOAT;0.1;False;2;FLOAT;1.1;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;70;-4082.052,615.4297;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;74;-4105.381,21.91852;Float;False;UV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ClampOpNode;147;-3806.099,-978.893;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;118;-2199.756,-1448.245;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;94;-4082.332,843.8523;Float;False;Tangent;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleDivideOpNode;71;-3914.089,532.0351;Float;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;148;-3731.085,-690.9465;Float;True;5;0;FLOAT;0;False;1;FLOAT;0.75;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;0.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;83;-4048.606,755.9905;Float;False;74;UV;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;101;-4253.647,771.5521;Float;False;Property;_Height;Height;16;0;Create;True;0;0;False;0;1.4;1.53;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;158;-3877.676,-434.9333;Float;False;Property;_ScanOpacity;ScanOpacity;11;0;Create;True;0;0;False;0;0.5;1.02;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;72;-3995.721,343.5146;Float;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;135;-3912.884,-1307.474;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;75;-3754.242,403.7353;Float;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;163;-1932.453,-1505.518;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;98;-3439.738,790.8309;Float;False;Property;_SpeedGreen;SpeedGreen;14;0;Create;True;0;0;False;0;-1;-1.59;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;157;-3545.105,-467.7318;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;99;-3443.985,885.4251;Float;False;Property;_SpeedBlue;SpeedBlue;15;0;Create;True;0;0;False;0;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;76;-3769.544,538.8152;Float;True;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;144;-3541.332,-995.7666;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;103;-3413.959,219.1489;Float;False;Property;_SpeedRed;SpeedRed;13;0;Create;True;0;0;False;0;-1;0.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;149;-3494.793,-694.0919;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ParallaxMappingNode;80;-3808.768,753.7576;Float;True;Normal;4;0;FLOAT2;0,0;False;1;FLOAT;1.3;False;2;FLOAT;0.2;False;3;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;102;-3251.861,219.8238;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;119;-1663.992,-1425.459;Float;False;RadialMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;77;-3450.377,344.4554;Float;False;5;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,0;False;3;FLOAT2;0,0;False;4;FLOAT2;1,1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;79;-3453.629,569.1945;Float;True;5;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,0;False;3;FLOAT2;0,0;False;4;FLOAT2;1,1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;150;-3257.465,-986.1221;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;105;-3240.513,839.5438;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;104;-3266.725,764.3873;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;86;-3042.749,719.5897;Float;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ClampOpNode;176;-3019.053,-964.0681;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;84;-3046,276.3267;Float;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;85;-3049.043,501.134;Float;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;-1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;152;-3090.941,-748.2563;Float;False;119;RadialMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;90;-3040.602,56.46416;Float;True;Property;_Texture0;Texture 0;5;0;Create;True;0;0;False;0;4e041143a45fae24296873a71bb19ef3;129021fc5b7863149b32aff9c58865ae;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.WorldPosInputsNode;222;-1603.687,331.3418;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TransformPositionNode;221;-1601.485,187.4655;Float;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;87;-2701.762,272.8694;Float;True;Property;_TextureSample0;Texture Sample 0;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;151;-2836.015,-964.2258;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;89;-2682.166,705.2799;Float;True;Property;_TextureSample2;Texture Sample 2;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;88;-2691.313,467.5194;Float;True;Property;_TextureSample1;Texture Sample 1;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;223;-1399.684,519.1526;Float;False;Property;_TransitionDistance;Transition Distance;1;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;201;-2531.64,-896.172;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;91;-2261.5,657.0082;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;178;-2427.263,955.3648;Float;False;Property;_CirclesOpacity;CirclesOpacity;12;0;Create;True;0;0;False;0;0;0.24;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;224;-1370.192,255.4274;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;225;-1020.701,458.2263;Float;False;Property;_TransitionFalloff;Transition Falloff;3;0;Create;True;0;0;False;0;255;300;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;226;-1047.961,295.2267;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;153;-2360.831,-988.686;Float;False;Scan;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;177;-2026.005,713.6918;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;155;-1940.183,1058.682;Float;False;153;Scan;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;92;-1769.39,659.0582;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;227;-880.8646,282.0637;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;180;-1201.314,779.3566;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;228;-565.5851,386.9626;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;93;-182.1172,13.69534;Float;False;Property;_Color;Color;0;1;[HDR];Create;True;0;0;False;0;0,0.4764547,0.8791991,0;0.7682161,0.6258149,0.1349062,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;198;-567.177,172.7944;Float;False;Property;_Color0;Color 0;2;1;[HDR];Create;True;0;0;False;0;0,0.4764547,0.8791991,0;2.049118,0.1138399,0.1138399,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;197;196.0649,196.4871;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;240;-241.4399,432.4684;Float;False;RedWave;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;204;-859.5919,780.0082;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;68;-4304.324,368.7128;Float;False;Fade;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;167;-2987.27,-1462.905;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;121;-3160.874,-1690.519;Float;True;COLOR;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;173;-3725.122,-1465.852;Float;False;68;Fade;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;171;-3199.401,-1454.898;Float;False;5;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,0;False;3;FLOAT2;0,0;False;4;FLOAT2;1,1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;122;-2881.424,-1662.263;Float;False;RadialUV;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;170;-3367.514,-1344.16;Float;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;169;-3505.86,-1438.503;Float;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;96;587.7209,-11.47288;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;FOVeffect;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;108;0;114;0
WireConnection;107;0;108;0
WireConnection;110;0;107;0
WireConnection;110;1;107;1
WireConnection;109;0;110;0
WireConnection;109;1;106;0
WireConnection;174;0;114;0
WireConnection;175;0;174;0
WireConnection;113;0;109;0
WireConnection;113;1;111;0
WireConnection;120;0;113;0
WireConnection;116;0;175;0
WireConnection;138;0;137;2
WireConnection;138;2;139;0
WireConnection;124;0;125;0
WireConnection;124;1;123;0
WireConnection;128;0;126;0
WireConnection;128;1;127;0
WireConnection;129;0;128;0
WireConnection;140;0;138;0
WireConnection;140;1;141;0
WireConnection;130;0;124;0
WireConnection;142;0;140;0
WireConnection;69;0;67;0
WireConnection;132;0;129;0
WireConnection;132;1;133;0
WireConnection;131;0;130;0
WireConnection;131;1;133;0
WireConnection;134;0;131;0
WireConnection;134;1;132;0
WireConnection;143;0;142;0
WireConnection;143;1;142;0
WireConnection;146;0;145;0
WireConnection;70;0;69;0
WireConnection;74;0;73;0
WireConnection;147;0;146;0
WireConnection;118;0;175;0
WireConnection;71;0;70;0
WireConnection;148;0;143;0
WireConnection;72;0;67;0
WireConnection;135;0;134;0
WireConnection;135;1;136;0
WireConnection;75;1;72;0
WireConnection;163;0;118;0
WireConnection;157;0;143;0
WireConnection;157;1;158;0
WireConnection;76;1;71;0
WireConnection;144;0;135;0
WireConnection;144;1;147;0
WireConnection;144;2;143;0
WireConnection;149;0;148;0
WireConnection;80;0;83;0
WireConnection;80;1;101;0
WireConnection;80;3;94;0
WireConnection;102;0;103;0
WireConnection;119;0;163;0
WireConnection;77;0;73;0
WireConnection;77;1;72;0
WireConnection;77;2;75;0
WireConnection;79;0;80;0
WireConnection;79;1;71;0
WireConnection;79;2;76;0
WireConnection;150;0;157;0
WireConnection;150;1;144;0
WireConnection;150;2;149;0
WireConnection;105;0;99;0
WireConnection;104;0;98;0
WireConnection;86;0;79;0
WireConnection;86;2;105;0
WireConnection;176;0;150;0
WireConnection;84;0;77;0
WireConnection;84;2;102;0
WireConnection;85;0;77;0
WireConnection;85;2;104;0
WireConnection;87;0;90;0
WireConnection;87;1;84;0
WireConnection;151;0;176;0
WireConnection;151;1;152;0
WireConnection;89;0;90;0
WireConnection;89;1;86;0
WireConnection;88;0;90;0
WireConnection;88;1;85;0
WireConnection;201;0;151;0
WireConnection;91;0;87;1
WireConnection;91;1;88;2
WireConnection;91;2;89;3
WireConnection;224;0;221;0
WireConnection;224;1;222;0
WireConnection;226;0;224;0
WireConnection;226;1;223;0
WireConnection;153;0;201;0
WireConnection;177;0;91;0
WireConnection;177;1;178;0
WireConnection;92;0;177;0
WireConnection;227;0;226;0
WireConnection;227;1;225;0
WireConnection;180;0;92;0
WireConnection;180;1;155;0
WireConnection;228;0;227;0
WireConnection;197;0;198;0
WireConnection;197;1;93;0
WireConnection;197;2;228;0
WireConnection;240;0;228;0
WireConnection;204;0;180;0
WireConnection;68;0;67;0
WireConnection;167;0;171;0
WireConnection;121;0;113;0
WireConnection;171;0;114;0
WireConnection;171;1;169;0
WireConnection;171;2;170;0
WireConnection;122;0;121;0
WireConnection;170;1;169;0
WireConnection;169;0;173;0
WireConnection;96;2;197;0
WireConnection;96;9;204;0
ASEEND*/
//CHKSM=F7E811A63284E94631A17B104700898A5D440ECB