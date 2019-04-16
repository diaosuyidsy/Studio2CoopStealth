// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Visual/EnemyFOVShader"
{
	Properties
	{
		_TransitionDistance("Transition Distance", Float) = 0
		_TransitionDis("TransitionDis", Float) = 0
		_TransitionFalloff("Transition Falloff", Float) = 255
		_Falloff("Falloff", Float) = 255
		_FoundColor("FoundColor", Color) = (1,0.1462264,0.1462264,1)
		_Transparency("Transparency", Float) = -0.5
		_VisionColor("VisionColor", Color) = (1,0.998481,0.7311321,1)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldPos;
		};

		uniform float4 _FoundColor;
		uniform float4 _VisionColor;
		uniform float _TransitionDistance;
		uniform float _TransitionFalloff;
		uniform float _Transparency;
		uniform float _TransitionDis;
		uniform float _Falloff;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 objToWorld18 = mul( unity_ObjectToWorld, float4( float3( 0,0,0 ), 1 ) ).xyz;
			float3 ase_worldPos = i.worldPos;
			float clampResult10 = clamp( pow( ( distance( objToWorld18 , ase_worldPos ) / _TransitionDistance ) , _TransitionFalloff ) , 0.0 , 1.0 );
			float4 lerpResult14 = lerp( _FoundColor , _VisionColor , clampResult10);
			o.Albedo = lerpResult14.rgb;
			float3 objToWorld20 = mul( unity_ObjectToWorld, float4( float3( 0,0,0 ), 1 ) ).xyz;
			float clampResult27 = clamp( pow( ( distance( objToWorld20 , ase_worldPos ) / _TransitionDis ) , _Falloff ) , 0.0 , 1.0 );
			float lerpResult30 = lerp( _Transparency , 1.0 , clampResult27);
			o.Alpha = lerpResult30;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows 

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
				float3 worldPos : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldPos = worldPos;
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
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
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
0;371.2;1095;413;894.1931;-98.19589;1;True;False
Node;AmplifyShaderEditor.TransformPositionNode;18;-1340.125,43.54541;Float;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;1;-1342.326,187.4217;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;21;-1328.654,617.9702;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TransformPositionNode;20;-1326.453,474.0938;Float;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;5;-1011.548,326.0005;Float;False;Property;_TransitionDistance;Transition Distance;0;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-1011.749,710.3063;Float;False;Property;_TransitionDis;TransitionDis;1;0;Create;True;0;0;False;0;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;22;-1095.16,542.0558;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;4;-1108.831,111.5073;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-759.34,314.3062;Float;False;Property;_TransitionFalloff;Transition Falloff;2;0;Create;True;0;0;False;0;255;300;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-772.0877,759.1943;Float;False;Property;_Falloff;Falloff;3;0;Create;True;0;0;False;0;255;-0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;25;-810.5877,580.4603;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;6;-786.6001,151.3066;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;26;-609.3323,564.5425;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;7;-616.6663,140.9814;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;27;-425.9402,559.0651;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;12;-369.3913,-157.6047;Float;False;Property;_VisionColor;VisionColor;6;0;Create;True;0;0;False;0;1,0.998481,0.7311321,1;1,0.998481,0.7311321,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;19;-297.1277,268.2972;Float;False;Property;_Transparency;Transparency;5;0;Create;True;0;0;False;0;-0.5;-0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;13;-366.9157,-335.3922;Float;False;Property;_FoundColor;FoundColor;4;0;Create;True;0;0;False;0;1,0.1462264,0.1462264,1;1,0.1462262,0.1462262,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;31;-295.4659,370.3486;Float;False;Constant;_Float2;Float 2;4;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;10;-439.6116,128.5166;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;14;-43.06374,-186.6839;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;30;-65.12743,296.2157;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;323.0073,-192.5329;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Visual/EnemyFOVShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;22;0;20;0
WireConnection;22;1;21;0
WireConnection;4;0;18;0
WireConnection;4;1;1;0
WireConnection;25;0;22;0
WireConnection;25;1;23;0
WireConnection;6;0;4;0
WireConnection;6;1;5;0
WireConnection;26;0;25;0
WireConnection;26;1;24;0
WireConnection;7;0;6;0
WireConnection;7;1;8;0
WireConnection;27;0;26;0
WireConnection;10;0;7;0
WireConnection;14;0;13;0
WireConnection;14;1;12;0
WireConnection;14;2;10;0
WireConnection;30;0;19;0
WireConnection;30;1;31;0
WireConnection;30;2;27;0
WireConnection;0;0;14;0
WireConnection;0;9;30;0
ASEEND*/
//CHKSM=F69A51D6FF564BA909C961ABFB41D69A3FFBA92D