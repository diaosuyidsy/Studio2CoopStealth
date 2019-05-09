// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:Legacy Shaders/Bumped Diffuse,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.4705882,fgcg:0.4509804,fgcb:0.3490196,fgca:1,fgde:0.005,fgrn:0,fgrf:0.01,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:4013,x:33618,y:32841,varname:node_4013,prsc:2|diff-1260-OUT,normal-7351-OUT;n:type:ShaderForge.SFN_Tex2d,id:8555,x:32419,y:32886,ptovrint:False,ptlb:DetailAlbedo,ptin:_DetailAlbedo,varname:node_8555,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:24,x:32417,y:33068,ptovrint:False,ptlb:DetailNormal,ptin:_DetailNormal,varname:node_24,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:4206,x:32417,y:33253,ptovrint:False,ptlb:DetailMask,ptin:_DetailMask,varname:node_4206,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:3249,x:32416,y:32523,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_3249,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_OneMinus,id:6715,x:32590,y:32886,varname:node_6715,prsc:2|IN-8555-RGB;n:type:ShaderForge.SFN_Vector1,id:6250,x:32417,y:33411,varname:node_6250,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:9515,x:32417,y:33467,varname:node_9515,prsc:2,v1:0.5;n:type:ShaderForge.SFN_OneMinus,id:4699,x:32596,y:33253,varname:node_4699,prsc:2|IN-4206-RGB;n:type:ShaderForge.SFN_Blend,id:1260,x:33324,y:32854,varname:node_1260,prsc:2,blmd:0,clmp:True|SRC-3249-RGB,DST-4167-OUT;n:type:ShaderForge.SFN_Vector1,id:9328,x:32417,y:33519,varname:node_9328,prsc:2,v1:0;n:type:ShaderForge.SFN_Append,id:6034,x:33171,y:33270,varname:node_6034,prsc:2|A-3519-OUT,B-6250-OUT;n:type:ShaderForge.SFN_ComponentMask,id:3519,x:32970,y:33343,varname:node_3519,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-4569-OUT;n:type:ShaderForge.SFN_Lerp,id:4569,x:32798,y:33343,varname:node_4569,prsc:2|A-24-RGB,B-9328-OUT,T-4699-OUT;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:2630,x:32789,y:32886,varname:node_2630,prsc:2|IN-6715-OUT,IMIN-9515-OUT,IMAX-6250-OUT,OMIN-9328-OUT,OMAX-6250-OUT;n:type:ShaderForge.SFN_Subtract,id:4078,x:32977,y:32886,varname:node_4078,prsc:2|A-2630-OUT,B-4699-OUT;n:type:ShaderForge.SFN_OneMinus,id:4167,x:33149,y:32886,varname:node_4167,prsc:2|IN-4078-OUT;n:type:ShaderForge.SFN_Tex2d,id:7352,x:32416,y:32704,ptovrint:False,ptlb:BumpMap,ptin:_BumpMap,varname:node_7352,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False;n:type:ShaderForge.SFN_NormalBlend,id:7351,x:33381,y:33133,varname:node_7351,prsc:2|BSE-7352-RGB,DTL-6034-OUT;proporder:3249-7352-8555-24-4206;pass:END;sub:END;*/

Shader "Shader Forge/DestroyIt Mobile" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _BumpMap ("BumpMap", 2D) = "black" {}
        _DetailAlbedo ("DetailAlbedo", 2D) = "white" {}
        _DetailNormal ("DetailNormal", 2D) = "black" {}
        _DetailMask ("DetailMask", 2D) = "black" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles ps4 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _DetailAlbedo; uniform float4 _DetailAlbedo_ST;
            uniform sampler2D _DetailNormal; uniform float4 _DetailNormal_ST;
            uniform sampler2D _DetailMask; uniform float4 _DetailMask_ST;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
                UNITY_FOG_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 _BumpMap_var = tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap));
                float4 _DetailNormal_var = tex2D(_DetailNormal,TRANSFORM_TEX(i.uv0, _DetailNormal));
                float node_9328 = 0.0;
                float4 _DetailMask_var = tex2D(_DetailMask,TRANSFORM_TEX(i.uv0, _DetailMask));
                float3 node_4699 = (1.0 - _DetailMask_var.rgb);
                float node_6250 = 1.0;
                float3 node_7351_nrm_base = _BumpMap_var.rgb + float3(0,0,1);
                float3 node_7351_nrm_detail = float3(lerp(_DetailNormal_var.rgb,float3(node_9328,node_9328,node_9328),node_4699).rg,node_6250) * float3(-1,-1,1);
                float3 node_7351_nrm_combined = node_7351_nrm_base*dot(node_7351_nrm_base, node_7351_nrm_detail)/node_7351_nrm_base.z - node_7351_nrm_detail;
                float3 node_7351 = node_7351_nrm_combined;
                float3 normalLocal = node_7351;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float4 _DetailAlbedo_var = tex2D(_DetailAlbedo,TRANSFORM_TEX(i.uv0, _DetailAlbedo));
                float node_9515 = 0.5;
                float3 diffuseColor = saturate(min(_MainTex_var.rgb,(1.0 - ((node_9328 + ( ((1.0 - _DetailAlbedo_var.rgb) - node_9515) * (node_6250 - node_9328) ) / (node_6250 - node_9515))-node_4699))));
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Legacy Shaders/Bumped Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
