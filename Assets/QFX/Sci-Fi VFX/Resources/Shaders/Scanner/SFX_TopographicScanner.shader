// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "QFX/SFX/Scanner/TopographicScanner"
{
	Properties
	{
		_MainTex ( "Screen", 2D ) = "black" {}
		[Toggle]_Sobel("Sobel", Float) = 1
		_Intensity("Intensity", Float) = 1
		[HDR]_SobelColor("Sobel Color", Color) = (0,0,0,0)
		_SobelTiling("Sobel Tiling", Float) = 0
		_Power("Power", Range( 1 , 10)) = 0.5
		_ScanDistance("Scan Distance", Float) = 25
		_TrailColor("Trail Color", Color) = (0,0,0,0)
		_InnerColor("Inner Color", Color) = (0,0,0,0)
		_EdgeColor("Edge Color", Color) = (0,0,0,0)
		_GridColor("GridColor", Color) = (0,0,0,0)
		_EdgePower("Edge Power", Float) = 0
		_ScanWidth("Scan Width", Float) = 0
		_GridSize("Grid Size", Float) = 500
		[Toggle]_Invert("Invert", Float) = 0
		_WorldPosition("World Position", Vector) = (1,1,1,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		
		
		ZTest Always
		Cull Off
		ZWrite Off

		
		Pass
		{ 
			CGPROGRAM 

			#pragma vertex vert_img_custom 
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"


			struct appdata_img_custom
			{
				float4 vertex : POSITION;
				half2 texcoord : TEXCOORD0;
				
			};

			struct v2f_img_custom
			{
				float4 pos : SV_POSITION;
				half2 uv   : TEXCOORD0;
				half2 stereoUV : TEXCOORD2;
		#if UNITY_UV_STARTS_AT_TOP
				half4 uv2 : TEXCOORD1;
				half4 stereoUV2 : TEXCOORD3;
		#endif
				float4 ase_texcoord4 : TEXCOORD4;
			};

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_TexelSize;
			uniform half4 _MainTex_ST;
			
			uniform float4 _TrailColor;
			uniform float4 _InnerColor;
			uniform float4 _EdgeColor;
			uniform float _Invert;
			uniform float _ScanDistance;
			uniform sampler2D _CameraDepthTexture;
			uniform float3 _WorldPosition;
			uniform float _ScanWidth;
			uniform float _EdgePower;
			uniform float _GridSize;
			uniform float4 _GridColor;
			uniform float _Power;
			uniform float _Sobel;
			uniform float _SobelTiling;
			uniform float _Intensity;
			uniform float4 _SobelColor;
			float CanScan354( float CurrentDistance , float ScanDistance , float ScanWidth )
			{
				if (CurrentDistance > ScanDistance - ScanWidth && CurrentDistance < ScanDistance)
				return 1;
				else return 0;
			}
			

			v2f_img_custom vert_img_custom ( appdata_img_custom v  )
			{
				v2f_img_custom o;
				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord4 = screenPos;
				
				o.pos = UnityObjectToClipPos ( v.vertex );
				o.uv = float4( v.texcoord.xy, 1, 1 );

				#if UNITY_UV_STARTS_AT_TOP
					o.uv2 = float4( v.texcoord.xy, 1, 1 );
					o.stereoUV2 = UnityStereoScreenSpaceUVAdjust ( o.uv2, _MainTex_ST );

					if ( _MainTex_TexelSize.y < 0.0 )
						o.uv.y = 1.0 - o.uv.y;
				#endif
				o.stereoUV = UnityStereoScreenSpaceUVAdjust ( o.uv, _MainTex_ST );
				return o;
			}

			half4 frag ( v2f_img_custom i ) : SV_Target
			{
				#ifdef UNITY_UV_STARTS_AT_TOP
					half2 uv = i.uv2;
					half2 stereoUV = i.stereoUV2;
				#else
					half2 uv = i.uv;
					half2 stereoUV = i.stereoUV;
				#endif	
				
				half4 finalColor;

				// ase common template code
				float2 uv_MainTex = i.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float4 Main466 = tex2D( _MainTex, uv_MainTex );
				float ScanDistance272 = _ScanDistance;
				float4 screenPos = i.ase_texcoord4;
				float4 ase_screenPosNorm = screenPos/screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float4 tex2DNode36_g277 = tex2D( _CameraDepthTexture, ase_screenPosNorm.xy );
				#ifdef UNITY_REVERSED_Z
				float staticSwitch38_g277 = ( 1.0 - tex2DNode36_g277.r );
				#else
				float staticSwitch38_g277 = tex2DNode36_g277.r;
				#endif
				float3 appendResult39_g277 = (float3(ase_screenPosNorm.x , ase_screenPosNorm.y , staticSwitch38_g277));
				float4 appendResult42_g277 = (float4((appendResult39_g277*2.0 + -1.0) , 1.0));
				float4 temp_output_43_0_g277 = mul( unity_CameraInvProjection, appendResult42_g277 );
				float4 appendResult49_g277 = (float4(( ( (temp_output_43_0_g277).xyz / (temp_output_43_0_g277).w ) * float3( 1,1,-1 ) ) , 1.0));
				float Distance254 = length( ( mul( unity_CameraToWorld, appendResult49_g277 ) - float4( _WorldPosition , 0.0 ) ) );
				float ScanWidth269 = _ScanWidth;
				float temp_output_321_0 = ( ( ScanDistance272 - Distance254 ) / ScanWidth269 );
				float d333 = lerp(( 1.0 - temp_output_321_0 ),temp_output_321_0,_Invert);
				float4 lerpResult231 = lerp( _InnerColor , _EdgeColor , pow( d333 , _EdgePower ));
				float4 EdgeAndInner283 = lerpResult231;
				float4 lerpResult233 = lerp( _TrailColor , EdgeAndInner283 , d333);
				float4 EmissionWithTrail286 = lerpResult233;
				float2 uv248 = i.uv.xy * float2( 1,1 ) + float2( 0,0 );
				float4 FullEmission276 = ( ( EmissionWithTrail286 + ( frac( ( uv248.y * _GridSize ) ) * _GridColor ) ) * d333 );
				float CurrentDistance354 = Distance254;
				float ScanDistance354 = ScanDistance272;
				float ScanWidth354 = ScanWidth269;
				float localCanScan354 = CanScan354( CurrentDistance354 , ScanDistance354 , ScanWidth354 );
				float CanScan374 = localCanScan354;
				float4 lerpResult361 = lerp( float4(0,0,0,0) , FullEmission276 , CanScan374);
				float FinalPower263 = _Power;
				float4 ResultEmission357 = ( lerpResult361 * FinalPower263 );
				float2 localCenter138_g288 = uv_MainTex;
				float4 break477 = ( _MainTex_TexelSize * _SobelTiling );
				float temp_output_2_0_g288 = break477.x;
				float localNegStepX156_g288 = -temp_output_2_0_g288;
				float temp_output_3_0_g288 = break477.y;
				float localStepY164_g288 = temp_output_3_0_g288;
				float2 appendResult14_g296 = (float2(localNegStepX156_g288 , localStepY164_g288));
				float4 tex2DNode16_g296 = tex2D( _MainTex, ( localCenter138_g288 + appendResult14_g296 ) );
				float temp_output_2_0_g296 = (tex2DNode16_g296).r;
				float temp_output_4_0_g296 = (tex2DNode16_g296).g;
				float temp_output_5_0_g296 = (tex2DNode16_g296).b;
				float localTopLeft172_g288 = ( sqrt( ( ( ( temp_output_2_0_g296 * temp_output_2_0_g296 ) + ( temp_output_4_0_g296 * temp_output_4_0_g296 ) ) + ( temp_output_5_0_g296 * temp_output_5_0_g296 ) ) ) * _Intensity );
				float2 appendResult14_g292 = (float2(localNegStepX156_g288 , 0.0));
				float4 tex2DNode16_g292 = tex2D( _MainTex, ( localCenter138_g288 + appendResult14_g292 ) );
				float temp_output_2_0_g292 = (tex2DNode16_g292).r;
				float temp_output_4_0_g292 = (tex2DNode16_g292).g;
				float temp_output_5_0_g292 = (tex2DNode16_g292).b;
				float localLeft173_g288 = ( sqrt( ( ( ( temp_output_2_0_g292 * temp_output_2_0_g292 ) + ( temp_output_4_0_g292 * temp_output_4_0_g292 ) ) + ( temp_output_5_0_g292 * temp_output_5_0_g292 ) ) ) * _Intensity );
				float localNegStepY165_g288 = -temp_output_3_0_g288;
				float2 appendResult14_g295 = (float2(localNegStepX156_g288 , localNegStepY165_g288));
				float4 tex2DNode16_g295 = tex2D( _MainTex, ( localCenter138_g288 + appendResult14_g295 ) );
				float temp_output_2_0_g295 = (tex2DNode16_g295).r;
				float temp_output_4_0_g295 = (tex2DNode16_g295).g;
				float temp_output_5_0_g295 = (tex2DNode16_g295).b;
				float localBottomLeft174_g288 = ( sqrt( ( ( ( temp_output_2_0_g295 * temp_output_2_0_g295 ) + ( temp_output_4_0_g295 * temp_output_4_0_g295 ) ) + ( temp_output_5_0_g295 * temp_output_5_0_g295 ) ) ) * _Intensity );
				float localStepX160_g288 = temp_output_2_0_g288;
				float2 appendResult14_g289 = (float2(localStepX160_g288 , localStepY164_g288));
				float4 tex2DNode16_g289 = tex2D( _MainTex, ( localCenter138_g288 + appendResult14_g289 ) );
				float temp_output_2_0_g289 = (tex2DNode16_g289).r;
				float temp_output_4_0_g289 = (tex2DNode16_g289).g;
				float temp_output_5_0_g289 = (tex2DNode16_g289).b;
				float localTopRight177_g288 = ( sqrt( ( ( ( temp_output_2_0_g289 * temp_output_2_0_g289 ) + ( temp_output_4_0_g289 * temp_output_4_0_g289 ) ) + ( temp_output_5_0_g289 * temp_output_5_0_g289 ) ) ) * _Intensity );
				float2 appendResult14_g290 = (float2(localStepX160_g288 , 0.0));
				float4 tex2DNode16_g290 = tex2D( _MainTex, ( localCenter138_g288 + appendResult14_g290 ) );
				float temp_output_2_0_g290 = (tex2DNode16_g290).r;
				float temp_output_4_0_g290 = (tex2DNode16_g290).g;
				float temp_output_5_0_g290 = (tex2DNode16_g290).b;
				float localRight178_g288 = ( sqrt( ( ( ( temp_output_2_0_g290 * temp_output_2_0_g290 ) + ( temp_output_4_0_g290 * temp_output_4_0_g290 ) ) + ( temp_output_5_0_g290 * temp_output_5_0_g290 ) ) ) * _Intensity );
				float2 appendResult14_g291 = (float2(localStepX160_g288 , localNegStepY165_g288));
				float4 tex2DNode16_g291 = tex2D( _MainTex, ( localCenter138_g288 + appendResult14_g291 ) );
				float temp_output_2_0_g291 = (tex2DNode16_g291).r;
				float temp_output_4_0_g291 = (tex2DNode16_g291).g;
				float temp_output_5_0_g291 = (tex2DNode16_g291).b;
				float localBottomRight179_g288 = ( sqrt( ( ( ( temp_output_2_0_g291 * temp_output_2_0_g291 ) + ( temp_output_4_0_g291 * temp_output_4_0_g291 ) ) + ( temp_output_5_0_g291 * temp_output_5_0_g291 ) ) ) * _Intensity );
				float temp_output_133_0_g288 = ( ( localTopLeft172_g288 + ( localLeft173_g288 * 2 ) + localBottomLeft174_g288 + -localTopRight177_g288 + ( localRight178_g288 * -2 ) + -localBottomRight179_g288 ) / 6.0 );
				float2 appendResult14_g294 = (float2(0.0 , localStepY164_g288));
				float4 tex2DNode16_g294 = tex2D( _MainTex, ( localCenter138_g288 + appendResult14_g294 ) );
				float temp_output_2_0_g294 = (tex2DNode16_g294).r;
				float temp_output_4_0_g294 = (tex2DNode16_g294).g;
				float temp_output_5_0_g294 = (tex2DNode16_g294).b;
				float localTop175_g288 = ( sqrt( ( ( ( temp_output_2_0_g294 * temp_output_2_0_g294 ) + ( temp_output_4_0_g294 * temp_output_4_0_g294 ) ) + ( temp_output_5_0_g294 * temp_output_5_0_g294 ) ) ) * _Intensity );
				float2 appendResult14_g293 = (float2(0.0 , localNegStepY165_g288));
				float4 tex2DNode16_g293 = tex2D( _MainTex, ( localCenter138_g288 + appendResult14_g293 ) );
				float temp_output_2_0_g293 = (tex2DNode16_g293).r;
				float temp_output_4_0_g293 = (tex2DNode16_g293).g;
				float temp_output_5_0_g293 = (tex2DNode16_g293).b;
				float localBottom176_g288 = ( sqrt( ( ( ( temp_output_2_0_g293 * temp_output_2_0_g293 ) + ( temp_output_4_0_g293 * temp_output_4_0_g293 ) ) + ( temp_output_5_0_g293 * temp_output_5_0_g293 ) ) ) * _Intensity );
				float temp_output_135_0_g288 = ( ( -localTopLeft172_g288 + ( localTop175_g288 * -2 ) + -localTopRight177_g288 + localBottomLeft174_g288 + ( localBottom176_g288 * 2 ) + localBottomRight179_g288 ) / 6.0 );
				float temp_output_111_0_g288 = sqrt( ( ( temp_output_133_0_g288 * temp_output_133_0_g288 ) + ( temp_output_135_0_g288 * temp_output_135_0_g288 ) ) );
				float3 appendResult113_g288 = (float3(temp_output_111_0_g288 , temp_output_111_0_g288 , temp_output_111_0_g288));
				float4 lerpResult484 = lerp( float4( 0,0,0,0 ) , _SobelColor , CanScan374);
				float4 Sobel487 = ( float4( appendResult113_g288 , 0.0 ) * lerpResult484 );
				

				finalColor = ( Main466 + ResultEmission357 + ( ResultEmission357 * saturate( d333 ) ) + lerp(float4( 0,0,0,0 ),Sobel487,_Sobel) );

				return finalColor;
			} 
			ENDCG 
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=15600
7;29;1906;1014;4303.294;4668.709;6.856029;False;False
Node;AmplifyShaderEditor.CommentaryNode;260;-198.0923,-1469.356;Float;False;1296.108;523.6191;;4;254;35;214;39;Distance;0.5147059,0.5147059,0.5147059,1;0;0
Node;AmplifyShaderEditor.FunctionNode;492;-63.8911,-1346.616;Float;False;Reconstruct World Position From Depth;3;;277;e7094bcbcc80eb140b2a3dbe6a861de8;0;0;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector3Node;39;113.532,-1256.296;Float;False;Property;_WorldPosition;World Position;15;0;Create;True;0;0;False;0;1,1,1;0.23,0.83,-7.92;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;214;331.0686,-1345.891;Float;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;262;1163.118,-1483.29;Float;False;1613.54;637.3947;;11;368;333;367;327;321;329;374;354;271;278;275;Scan Value;0.5147059,0.5147059,0.5147059,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;209;1179.724,-559.0404;Float;False;Property;_ScanDistance;Scan Distance;6;0;Create;True;0;0;False;0;25;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;35;532.6617,-1346.092;Float;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;275;1207.187,-1145.065;Float;False;272;ScanDistance;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;272;1405.43,-559.6408;Float;False;ScanDistance;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;254;682.9337,-1351.197;Float;False;Distance;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;224;1176.401,-671.2173;Float;False;Property;_ScanWidth;Scan Width;12;0;Create;True;0;0;False;0;0;8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;278;1208.752,-1243.797;Float;False;254;Distance;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;271;1211.967,-1047.735;Float;False;269;ScanWidth;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;269;1436.095,-670.6228;Float;False;ScanWidth;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;329;1682.913,-1062.366;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;321;1837.39,-1060.981;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;368;2027.989,-954.638;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;327;1986.607,-1057.502;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;367;2173.432,-1063.153;Float;False;Property;_Invert;Invert;14;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;122;2909.676,-1493.495;Float;False;1883.25;1969.539;;29;357;332;361;267;360;375;276;359;348;349;244;286;251;250;233;243;291;225;285;283;284;231;288;248;223;222;230;308;226;Scan Emission;0.75,0.75,0.75,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;226;3075.45,-37.3346;Float;False;Property;_EdgePower;Edge Power;11;0;Create;True;0;0;False;0;0;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;308;3065.834,-181.4481;Float;False;333;d;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;333;2412.034,-1066.597;Float;False;d;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;222;3065.386,-542.3717;Float;False;Property;_InnerColor;Inner Color;8;0;Create;True;0;0;False;0;0,0,0,0;0.2720587,0.5481745,1,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;223;3066.79,-373.3684;Float;False;Property;_EdgeColor;Edge Color;9;0;Create;True;0;0;False;0;0,0,0,0;0,0.2965516,1,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;230;3286.845,-137.9109;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;288;3266.157,-790.4128;Float;False;Property;_GridSize;Grid Size;13;0;Create;True;0;0;False;0;500;100;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;248;3267.082,-921.3383;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;231;3390.479,-394.7597;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;291;3523.301,-876.5323;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;500;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;284;2968.043,-1074.705;Float;False;283;EdgeAndInner;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;283;3577.481,-400.8112;Float;False;EdgeAndInner;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;225;2970.967,-1279.392;Float;False;Property;_TrailColor;Trail Color;7;0;Create;True;0;0;False;0;0,0,0,0;0.5882352,0.2508649,0.2508649,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;285;2968.043,-963.7045;Float;False;333;d;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;233;3275.967,-1160.979;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FractNode;243;3678.466,-876.8101;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;250;3520.048,-765.0913;Float;False;Property;_GridColor;GridColor;10;0;Create;True;0;0;False;0;0,0,0,0;0.3308823,0.3308823,0.3308823,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;485;-514.5173,-317.4272;Float;False;1564.685;891.1016;;12;477;480;478;476;479;474;475;484;472;470;483;487;Sobel;0.4117647,0.4117647,0.4117647,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;286;3461.333,-1166.169;Float;False;EmissionWithTrail;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;251;3825.403,-878.2657;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;349;4048.833,-638.05;Float;False;333;d;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;474;-453.9051,-246.7384;Float;False;0;0;_MainTex_TexelSize;Pass;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;475;-443.9747,-66.74876;Float;False;Property;_SobelTiling;Sobel Tiling;2;0;Create;True;0;0;False;0;0;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;244;4077.869,-903.062;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CustomExpressionNode;354;1638.826,-1343.718;Float;False;if (CurrentDistance > ScanDistance - ScanWidth && CurrentDistance < ScanDistance)$return 1@$else return 0@;1;False;3;False;CurrentDistance;FLOAT;0;In;;False;ScanDistance;FLOAT;0;In;;False;ScanWidth;FLOAT;0;In;;CanScan;True;False;0;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;472;-279.4445,14.98946;Float;False;0;0;_MainTex;Shader;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;348;4234.832,-904.05;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;476;-179.0738,-157.9798;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;359;3679.517,4.222103;Float;False;276;FullEmission;0;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;483;100.1604,449.1251;Float;False;374;CanScan;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;478;53.73736,102.7241;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;276;4374.64,-906.5448;Float;False;FullEmission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;145;1173.584,-787.5129;Float;False;Property;_Power;Power;5;0;Create;True;0;0;False;0;0.5;1;1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;477;-14.92818,-158.9142;Float;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.ColorNode;480;62.62209,269.4274;Float;False;Property;_SobelColor;Sobel Color;1;1;[HDR];Create;True;0;0;False;0;0,0,0,0;1,0.5586207,0,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;375;3695.542,116.1128;Float;False;374;CanScan;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;360;3675.11,-177.1058;Float;False;Constant;_Color0;Color 0;27;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;374;1906.958,-1348.607;Float;False;CanScan;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;361;3960.977,-64.83894;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;484;384.3349,181.6842;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;470;316.1526,-40.28841;Float;False;SobelMain;-1;;288;481788033fe47cd4893d0d4673016cbc;0;4;2;FLOAT;50;False;3;FLOAT;50;False;4;FLOAT2;0,0;False;1;SAMPLER2D;0.0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;268;1189.597,12.13453;Float;False;1071.459;423.3039;;8;401;460;469;183;458;351;436;486;Blending;0.5147059,0.5147059,0.5147059,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;263;1484.452,-788.3406;Float;False;FinalPower;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;267;4065.895,140.1607;Float;False;263;FinalPower;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;464;1187.287,-401.336;Float;False;0;0;_MainTex;Shader;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;479;576.6359,43.40811;Float;True;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;436;1217.762,265.1118;Float;False;333;d;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;332;4268.518,-63.97078;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;458;1427.762,271.1118;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;357;4487.827,-67.68011;Float;False;ResultEmission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;351;1217.762,169.1119;Float;False;357;ResultEmission;0;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;465;1335.141,-404.773;Float;True;Property;_TextureSample0;Texture Sample 0;15;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;487;805.4736,38.33318;Float;False;Sobel;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;488;1643.674,357.9487;Float;False;487;Sobel;0;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;469;1585.762,185.1119;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;486;1818.457,244.359;Float;False;Property;_Sobel;Sobel;0;0;Create;True;0;0;False;0;1;2;0;COLOR;0,0,0,0;False;1;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;183;1217.762,73.11197;Float;False;466;Main;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;466;1628.141,-404.773;Float;False;Main;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;460;1721.562,75.71197;Float;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;401;1883.299,75.58597;Float;False;True;2;Float;ASEMaterialInspector;0;2;QFX/SFX/Scanner/TopographicScanner;c71b220b631b6344493ea3cf87110c93;0;0;SubShader 0 Pass 0;1;False;False;True;2;False;-1;False;False;True;2;False;-1;True;7;False;-1;False;True;0;False;0;False;False;False;False;False;False;False;False;False;True;2;0;;0;0;Standard;0;1;0;FLOAT4;0,0,0,0;False;0
WireConnection;214;0;492;0
WireConnection;214;1;39;0
WireConnection;35;0;214;0
WireConnection;272;0;209;0
WireConnection;254;0;35;0
WireConnection;269;0;224;0
WireConnection;329;0;275;0
WireConnection;329;1;278;0
WireConnection;321;0;329;0
WireConnection;321;1;271;0
WireConnection;368;0;321;0
WireConnection;327;0;321;0
WireConnection;367;0;327;0
WireConnection;367;1;368;0
WireConnection;333;0;367;0
WireConnection;230;0;308;0
WireConnection;230;1;226;0
WireConnection;231;0;222;0
WireConnection;231;1;223;0
WireConnection;231;2;230;0
WireConnection;291;0;248;2
WireConnection;291;1;288;0
WireConnection;283;0;231;0
WireConnection;233;0;225;0
WireConnection;233;1;284;0
WireConnection;233;2;285;0
WireConnection;243;0;291;0
WireConnection;286;0;233;0
WireConnection;251;0;243;0
WireConnection;251;1;250;0
WireConnection;244;0;286;0
WireConnection;244;1;251;0
WireConnection;354;0;278;0
WireConnection;354;1;275;0
WireConnection;354;2;271;0
WireConnection;348;0;244;0
WireConnection;348;1;349;0
WireConnection;476;0;474;0
WireConnection;476;1;475;0
WireConnection;478;2;472;0
WireConnection;276;0;348;0
WireConnection;477;0;476;0
WireConnection;374;0;354;0
WireConnection;361;0;360;0
WireConnection;361;1;359;0
WireConnection;361;2;375;0
WireConnection;484;1;480;0
WireConnection;484;2;483;0
WireConnection;470;2;477;0
WireConnection;470;3;477;1
WireConnection;470;4;478;0
WireConnection;470;1;472;0
WireConnection;263;0;145;0
WireConnection;479;0;470;0
WireConnection;479;1;484;0
WireConnection;332;0;361;0
WireConnection;332;1;267;0
WireConnection;458;0;436;0
WireConnection;357;0;332;0
WireConnection;465;0;464;0
WireConnection;487;0;479;0
WireConnection;469;0;351;0
WireConnection;469;1;458;0
WireConnection;486;1;488;0
WireConnection;466;0;465;0
WireConnection;460;0;183;0
WireConnection;460;1;351;0
WireConnection;460;2;469;0
WireConnection;460;3;486;0
WireConnection;401;0;460;0
ASEEND*/
//CHKSM=F8B6721F568CEE14EE5BAAA4C8BDAF1A6F98AD87