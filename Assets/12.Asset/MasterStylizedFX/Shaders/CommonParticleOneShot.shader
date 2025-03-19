// Made with Amplify Shader Editor v1.9.3.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "CommonParticleOneShot"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[Header(Main)][NoScaleOffset]_Main("Main", 2D) = "white" {}
		_Tiling("Tiling", Vector) = (1,1,0,0)
		_Offset("Offset", Vector) = (0,0,0,0)
		_Scroll("Scroll", Vector) = (1,0,0,0)
		[Header(LimitUV)]_LimitUVRange("LimitUVRange", Vector) = (0,1,0,1)
		[Toggle]_LimitUV("LimitUV", Float) = 0
		[Header(StretchUV)]_StretchUVDes("StretchUVDes", Vector) = (0,0,0,0)
		_StretchMultiplier("StretchMultiplier", Vector) = (0,0,0,0)
		[Toggle]_Stretch("Stretch", Float) = 0
		[Header(Mask)][NoScaleOffset]_NoiseMask("NoiseMask", 2D) = "white" {}
		[Toggle]_Mask("Mask", Float) = 1
		_MaskScroll("MaskScroll", Vector) = (0,0,0,0)
		_MaskTiling("MaskTiling", Vector) = (0,0,0,0)
		_MaskOffset("MaskOffset", Vector) = (0,0,0,0)
		_Feather("Feather", Range( 0 , 1)) = 0
		[Header(StaticMask)]_StaticMask("StaticMask", 2D) = "white" {}
		_SmoothStep("SmoothStep", Vector) = (0,1,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

		[HideInInspector][NoScaleOffset] unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }

		Cull Off
		HLSLINCLUDE
		#pragma target 2.0
		#pragma prefer_hlslcc gles
		// ensure rendering platforms toggle list is visible

		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
		ENDHLSL

		
		Pass
		{
			Name "Sprite Unlit"
			Tags { "LightMode"="Universal2D" }

			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZTest LEqual
			ZWrite Off
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM

			#define ASE_SRP_VERSION 140010


			#pragma vertex vert
			#pragma fragment frag

			#define _SURFACE_TYPE_TRANSPARENT 1
			#define SHADERPASS SHADERPASS_SPRITEUNLIT

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/SurfaceData2D.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging2D.hlsl"

			#define ASE_NEEDS_FRAG_COLOR


			sampler2D _Main;
			sampler2D _NoiseMask;
			sampler2D _StaticMask;
			CBUFFER_START( UnityPerMaterial )
			float4 _LimitUVRange;
			float4 _StaticMask_ST;
			float2 _Tiling;
			float2 _Scroll;
			float2 _Offset;
			float2 _StretchMultiplier;
			float2 _StretchUVDes;
			float2 _MaskTiling;
			float2 _MaskOffset;
			float2 _MaskScroll;
			float2 _SmoothStep;
			float _LimitUV;
			float _Stretch;
			float _Mask;
			float _Feather;
			CBUFFER_END


			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 uv0 : TEXCOORD0;
				float4 color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float4 texCoord0 : TEXCOORD0;
				float4 color : TEXCOORD1;
				float3 positionWS : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			#if ETC1_EXTERNAL_ALPHA
				TEXTURE2D( _AlphaTex ); SAMPLER( sampler_AlphaTex );
				float _EnableAlphaTexture;
			#endif

			float4 _RendererColor;

			
			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				o.ase_texcoord3 = v.ase_texcoord1;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif
				v.normal = v.normal;
				v.tangent.xyz = v.tangent.xyz;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( v.positionOS.xyz );

				o.texCoord0 = v.uv0;
				o.color = v.color;
				o.positionCS = vertexInput.positionCS;
				o.positionWS = vertexInput.positionWS;

				return o;
			}

			half4 frag( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float3 texCoord76 = IN.texCoord0.xyz;
				texCoord76.xy = IN.texCoord0.xyz.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_cast_0 = (0.0).xx;
				float2 texCoord102 = IN.texCoord0.xy * float2( 1,1 ) + float2( 0,0 );
				float4 texCoord119 = IN.texCoord0;
				texCoord119.xy = IN.texCoord0.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult106 = (float2(( _StretchMultiplier * ( _StretchUVDes - texCoord102 ) * texCoord119.w )));
				float2 texCoord74 = IN.texCoord0.xy * _Tiling + ( ( texCoord76.z * _Scroll ) + _Offset + (( _Stretch )?( appendResult106 ):( temp_cast_0 )) );
				float4 appendResult84 = (float4(_LimitUVRange.x , _LimitUVRange.z , 0.0 , 0.0));
				float4 appendResult85 = (float4(_LimitUVRange.y , _LimitUVRange.w , 0.0 , 0.0));
				float2 clampResult80 = clamp( texCoord74 , appendResult84.xy , appendResult85.xy );
				float4 tex2DNode73 = tex2D( _Main, (( _LimitUV )?( clampResult80 ):( texCoord74 )) );
				float2 texCoord120 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float4 texCoord137 = IN.ase_texcoord3;
				texCoord137.xy = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float lerpResult172 = lerp( ( 0.0 - _Feather ) , ( 1.0 + _Feather ) , texCoord137.z);
				float4 texCoord157 = IN.ase_texcoord3;
				texCoord157.xy = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord156 = IN.texCoord0.xy * _MaskTiling + ( _MaskOffset + ( texCoord157.w * _MaskScroll ) );
				float smoothstepResult155 = smoothstep( ( lerpResult172 - _Feather ) , ( lerpResult172 + _Feather ) , tex2D( _NoiseMask, texCoord156 ).r);
				float2 uv_StaticMask = IN.texCoord0.xy * _StaticMask_ST.xy + _StaticMask_ST.zw;
				float smoothstepResult165 = smoothstep( _SmoothStep.x , _SmoothStep.y , tex2D( _StaticMask, uv_StaticMask ).r);
				float4 appendResult188 = (float4(( IN.color * tex2DNode73 * ( 1.0 + texCoord120.x ) ).rgb , ( ( IN.color.a * tex2DNode73.a ) * (( _Mask )?( smoothstepResult155 ):( 1.0 )) * smoothstepResult165 )));
				
				float4 Color = appendResult188;

				#if ETC1_EXTERNAL_ALPHA
					float4 alpha = SAMPLE_TEXTURE2D( _AlphaTex, sampler_AlphaTex, IN.texCoord0.xy );
					Color.a = lerp( Color.a, alpha.r, _EnableAlphaTexture );
				#endif

				#if defined(DEBUG_DISPLAY)
				SurfaceData2D surfaceData;
				InitializeSurfaceData(Color.rgb, Color.a, surfaceData);
				InputData2D inputData;
				InitializeInputData(IN.positionWS.xy, half2(IN.texCoord0.xy), inputData);
				half4 debugColor = 0;

				SETUP_DEBUG_DATA_2D(inputData, IN.positionWS);

				if (CanDebugOverrideOutputColor(surfaceData, inputData, debugColor))
				{
					return debugColor;
				}
				#endif

				Color *= IN.color * _RendererColor;
				return Color;
			}

			ENDHLSL
		}
		
		Pass
		{
			
			Name "Sprite Unlit Forward"
            Tags { "LightMode"="UniversalForward" }

			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZTest LEqual
			ZWrite Off
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM

			#define ASE_SRP_VERSION 140010


			#pragma vertex vert
			#pragma fragment frag

			#define _SURFACE_TYPE_TRANSPARENT 1
			#define SHADERPASS SHADERPASS_SPRITEFORWARD

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/SurfaceData2D.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging2D.hlsl"

			#define ASE_NEEDS_FRAG_COLOR


			sampler2D _Main;
			sampler2D _NoiseMask;
			sampler2D _StaticMask;
			CBUFFER_START( UnityPerMaterial )
			float4 _LimitUVRange;
			float4 _StaticMask_ST;
			float2 _Tiling;
			float2 _Scroll;
			float2 _Offset;
			float2 _StretchMultiplier;
			float2 _StretchUVDes;
			float2 _MaskTiling;
			float2 _MaskOffset;
			float2 _MaskScroll;
			float2 _SmoothStep;
			float _LimitUV;
			float _Stretch;
			float _Mask;
			float _Feather;
			CBUFFER_END


			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 uv0 : TEXCOORD0;
				float4 color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float4 texCoord0 : TEXCOORD0;
				float4 color : TEXCOORD1;
				float3 positionWS : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			#if ETC1_EXTERNAL_ALPHA
				TEXTURE2D( _AlphaTex ); SAMPLER( sampler_AlphaTex );
				float _EnableAlphaTexture;
			#endif

			float4 _RendererColor;

			
			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				o.ase_texcoord3 = v.ase_texcoord1;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif
				v.normal = v.normal;
				v.tangent.xyz = v.tangent.xyz;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( v.positionOS.xyz );

				o.texCoord0 = v.uv0;
				o.color = v.color;
				o.positionCS = vertexInput.positionCS;
				o.positionWS = vertexInput.positionWS;

				return o;
			}

			half4 frag( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float3 texCoord76 = IN.texCoord0.xyz;
				texCoord76.xy = IN.texCoord0.xyz.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_cast_0 = (0.0).xx;
				float2 texCoord102 = IN.texCoord0.xy * float2( 1,1 ) + float2( 0,0 );
				float4 texCoord119 = IN.texCoord0;
				texCoord119.xy = IN.texCoord0.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult106 = (float2(( _StretchMultiplier * ( _StretchUVDes - texCoord102 ) * texCoord119.w )));
				float2 texCoord74 = IN.texCoord0.xy * _Tiling + ( ( texCoord76.z * _Scroll ) + _Offset + (( _Stretch )?( appendResult106 ):( temp_cast_0 )) );
				float4 appendResult84 = (float4(_LimitUVRange.x , _LimitUVRange.z , 0.0 , 0.0));
				float4 appendResult85 = (float4(_LimitUVRange.y , _LimitUVRange.w , 0.0 , 0.0));
				float2 clampResult80 = clamp( texCoord74 , appendResult84.xy , appendResult85.xy );
				float4 tex2DNode73 = tex2D( _Main, (( _LimitUV )?( clampResult80 ):( texCoord74 )) );
				float2 texCoord120 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float4 texCoord137 = IN.ase_texcoord3;
				texCoord137.xy = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float lerpResult172 = lerp( ( 0.0 - _Feather ) , ( 1.0 + _Feather ) , texCoord137.z);
				float4 texCoord157 = IN.ase_texcoord3;
				texCoord157.xy = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord156 = IN.texCoord0.xy * _MaskTiling + ( _MaskOffset + ( texCoord157.w * _MaskScroll ) );
				float smoothstepResult155 = smoothstep( ( lerpResult172 - _Feather ) , ( lerpResult172 + _Feather ) , tex2D( _NoiseMask, texCoord156 ).r);
				float2 uv_StaticMask = IN.texCoord0.xy * _StaticMask_ST.xy + _StaticMask_ST.zw;
				float smoothstepResult165 = smoothstep( _SmoothStep.x , _SmoothStep.y , tex2D( _StaticMask, uv_StaticMask ).r);
				float4 appendResult188 = (float4(( IN.color * tex2DNode73 * ( 1.0 + texCoord120.x ) ).rgb , ( ( IN.color.a * tex2DNode73.a ) * (( _Mask )?( smoothstepResult155 ):( 1.0 )) * smoothstepResult165 )));
				
				float4 Color = appendResult188;

				#if ETC1_EXTERNAL_ALPHA
					float4 alpha = SAMPLE_TEXTURE2D( _AlphaTex, sampler_AlphaTex, IN.texCoord0.xy );
					Color.a = lerp( Color.a, alpha.r, _EnableAlphaTexture );
				#endif


				#if defined(DEBUG_DISPLAY)
				SurfaceData2D surfaceData;
				InitializeSurfaceData(Color.rgb, Color.a, surfaceData);
				InputData2D inputData;
				InitializeInputData(IN.positionWS.xy, half2(IN.texCoord0.xy), inputData);
				half4 debugColor = 0;

				SETUP_DEBUG_DATA_2D(inputData, IN.positionWS);

				if (CanDebugOverrideOutputColor(surfaceData, inputData, debugColor))
				{
					return debugColor;
				}
				#endif

				Color *= IN.color * _RendererColor;
				return Color;
			}

			ENDHLSL
		}
		
        Pass
        {
			
            Name "SceneSelectionPass"
            Tags { "LightMode"="SceneSelectionPass" }

            Cull Off

            HLSLPROGRAM

			#define ASE_SRP_VERSION 140010


			#pragma vertex vert
			#pragma fragment frag

            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX
            #define SHADERPASS SHADERPASS_DEPTHONLY
			#define SCENESELECTIONPASS 1


            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_FRAG_COLOR


			sampler2D _Main;
			sampler2D _NoiseMask;
			sampler2D _StaticMask;
			CBUFFER_START( UnityPerMaterial )
			float4 _LimitUVRange;
			float4 _StaticMask_ST;
			float2 _Tiling;
			float2 _Scroll;
			float2 _Offset;
			float2 _StretchMultiplier;
			float2 _StretchUVDes;
			float2 _MaskTiling;
			float2 _MaskOffset;
			float2 _MaskScroll;
			float2 _SmoothStep;
			float _LimitUV;
			float _Stretch;
			float _Mask;
			float _Feather;
			CBUFFER_END


            struct VertexInput
			{
				float3 positionOS : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};


            int _ObjectId;
            int _PassValue;

			
			VertexOutput vert(VertexInput v )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				o.ase_color = v.ase_color;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				VertexPositionInputs vertexInput = GetVertexPositionInputs(v.positionOS.xyz);
				float3 positionWS = TransformObjectToWorld(v.positionOS);
				o.positionCS = TransformWorldToHClip(positionWS);

				return o;
			}

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				float3 texCoord76 = IN.ase_texcoord.xyz;
				texCoord76.xy = IN.ase_texcoord.xyz.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_cast_0 = (0.0).xx;
				float2 texCoord102 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float4 texCoord119 = IN.ase_texcoord;
				texCoord119.xy = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult106 = (float2(( _StretchMultiplier * ( _StretchUVDes - texCoord102 ) * texCoord119.w )));
				float2 texCoord74 = IN.ase_texcoord.xy * _Tiling + ( ( texCoord76.z * _Scroll ) + _Offset + (( _Stretch )?( appendResult106 ):( temp_cast_0 )) );
				float4 appendResult84 = (float4(_LimitUVRange.x , _LimitUVRange.z , 0.0 , 0.0));
				float4 appendResult85 = (float4(_LimitUVRange.y , _LimitUVRange.w , 0.0 , 0.0));
				float2 clampResult80 = clamp( texCoord74 , appendResult84.xy , appendResult85.xy );
				float4 tex2DNode73 = tex2D( _Main, (( _LimitUV )?( clampResult80 ):( texCoord74 )) );
				float2 texCoord120 = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float4 texCoord137 = IN.ase_texcoord1;
				texCoord137.xy = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float lerpResult172 = lerp( ( 0.0 - _Feather ) , ( 1.0 + _Feather ) , texCoord137.z);
				float4 texCoord157 = IN.ase_texcoord1;
				texCoord157.xy = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord156 = IN.ase_texcoord.xy * _MaskTiling + ( _MaskOffset + ( texCoord157.w * _MaskScroll ) );
				float smoothstepResult155 = smoothstep( ( lerpResult172 - _Feather ) , ( lerpResult172 + _Feather ) , tex2D( _NoiseMask, texCoord156 ).r);
				float2 uv_StaticMask = IN.ase_texcoord.xy * _StaticMask_ST.xy + _StaticMask_ST.zw;
				float smoothstepResult165 = smoothstep( _SmoothStep.x , _SmoothStep.y , tex2D( _StaticMask, uv_StaticMask ).r);
				float4 appendResult188 = (float4(( IN.ase_color * tex2DNode73 * ( 1.0 + texCoord120.x ) ).rgb , ( ( IN.ase_color.a * tex2DNode73.a ) * (( _Mask )?( smoothstepResult155 ):( 1.0 )) * smoothstepResult165 )));
				
				float4 Color = appendResult188;

				half4 outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
				return outColor;
			}

            ENDHLSL
        }

		
        Pass
        {
			
            Name "ScenePickingPass"
            Tags { "LightMode"="Picking" }

            Cull Off

            HLSLPROGRAM

			#define ASE_SRP_VERSION 140010


			#pragma vertex vert
			#pragma fragment frag

            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX
            #define SHADERPASS SHADERPASS_DEPTHONLY
			#define SCENEPICKINGPASS 1


            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

        	#define ASE_NEEDS_FRAG_COLOR


			sampler2D _Main;
			sampler2D _NoiseMask;
			sampler2D _StaticMask;
			CBUFFER_START( UnityPerMaterial )
			float4 _LimitUVRange;
			float4 _StaticMask_ST;
			float2 _Tiling;
			float2 _Scroll;
			float2 _Offset;
			float2 _StretchMultiplier;
			float2 _StretchUVDes;
			float2 _MaskTiling;
			float2 _MaskOffset;
			float2 _MaskScroll;
			float2 _SmoothStep;
			float _LimitUV;
			float _Stretch;
			float _Mask;
			float _Feather;
			CBUFFER_END


            struct VertexInput
			{
				float3 positionOS : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

            float4 _SelectionID;

			
			VertexOutput vert(VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				o.ase_color = v.ase_color;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				VertexPositionInputs vertexInput = GetVertexPositionInputs(v.positionOS.xyz);
				float3 positionWS = TransformObjectToWorld(v.positionOS);
				o.positionCS = TransformWorldToHClip(positionWS);

				return o;
			}

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				float3 texCoord76 = IN.ase_texcoord.xyz;
				texCoord76.xy = IN.ase_texcoord.xyz.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_cast_0 = (0.0).xx;
				float2 texCoord102 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float4 texCoord119 = IN.ase_texcoord;
				texCoord119.xy = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult106 = (float2(( _StretchMultiplier * ( _StretchUVDes - texCoord102 ) * texCoord119.w )));
				float2 texCoord74 = IN.ase_texcoord.xy * _Tiling + ( ( texCoord76.z * _Scroll ) + _Offset + (( _Stretch )?( appendResult106 ):( temp_cast_0 )) );
				float4 appendResult84 = (float4(_LimitUVRange.x , _LimitUVRange.z , 0.0 , 0.0));
				float4 appendResult85 = (float4(_LimitUVRange.y , _LimitUVRange.w , 0.0 , 0.0));
				float2 clampResult80 = clamp( texCoord74 , appendResult84.xy , appendResult85.xy );
				float4 tex2DNode73 = tex2D( _Main, (( _LimitUV )?( clampResult80 ):( texCoord74 )) );
				float2 texCoord120 = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float4 texCoord137 = IN.ase_texcoord1;
				texCoord137.xy = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float lerpResult172 = lerp( ( 0.0 - _Feather ) , ( 1.0 + _Feather ) , texCoord137.z);
				float4 texCoord157 = IN.ase_texcoord1;
				texCoord157.xy = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord156 = IN.ase_texcoord.xy * _MaskTiling + ( _MaskOffset + ( texCoord157.w * _MaskScroll ) );
				float smoothstepResult155 = smoothstep( ( lerpResult172 - _Feather ) , ( lerpResult172 + _Feather ) , tex2D( _NoiseMask, texCoord156 ).r);
				float2 uv_StaticMask = IN.ase_texcoord.xy * _StaticMask_ST.xy + _StaticMask_ST.zw;
				float smoothstepResult165 = smoothstep( _SmoothStep.x , _SmoothStep.y , tex2D( _StaticMask, uv_StaticMask ).r);
				float4 appendResult188 = (float4(( IN.ase_color * tex2DNode73 * ( 1.0 + texCoord120.x ) ).rgb , ( ( IN.ase_color.a * tex2DNode73.a ) * (( _Mask )?( smoothstepResult155 ):( 1.0 )) * smoothstepResult165 )));
				
				float4 Color = appendResult188;
				half4 outColor = _SelectionID;
				return outColor;
			}

            ENDHLSL
        }
		
	}
	CustomEditor "ASEMaterialInspector"
	Fallback "Hidden/InternalErrorShader"
	
}
/*ASEBEGIN
Version=19302
Node;AmplifyShaderEditor.CommentaryNode;107;-97.24704,1076.068;Inherit;False;981.2;834.1053;StretchUV;9;119;106;114;118;103;102;115;123;124;;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;115;-77.15573,1593.25;Inherit;False;Property;_StretchUVDes;StretchUVDes;6;1;[Header];Create;True;1;StretchUV;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;102;-73.93272,1408.671;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;103;175.7717,1477.859;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;118;207.5538,1247.947;Inherit;False;Property;_StretchMultiplier;StretchMultiplier;7;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;119;115.9227,1689.141;Inherit;False;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;87;164.4255,-288.6086;Inherit;False;1122.08;1253.271;Scroll;7;74;78;82;81;77;76;75;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;419.421,1426.666;Inherit;False;3;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;160;1302.843,700.6349;Inherit;False;781.2883;525.4138;NoiseScroll;7;156;158;159;157;161;162;163;;1,1,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;106;565.1396,1403.484;Inherit;False;FLOAT2;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;124;469.7593,1201.906;Inherit;False;Constant;_Float1;Float 1;9;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;75;225.251,169.3847;Inherit;False;Property;_Scroll;Scroll;3;0;Create;True;0;0;0;False;0;False;1,0;0,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;76;223.2823,-11.87827;Inherit;False;0;-1;3;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;157;1383.586,750.6349;Inherit;False;1;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;159;1372.48,1010.377;Inherit;False;Property;_MaskScroll;MaskScroll;11;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;86;1321.36,-797.9218;Inherit;False;1078.309;819.3665;UVlimitation;5;80;85;84;83;109;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ToggleSwitchNode;123;641.1641,1216.096;Inherit;False;Property;_Stretch;Stretch;8;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;146;2160.628,535.6385;Inherit;False;1444.062;1209.11;Mask;11;170;169;137;125;147;155;148;171;172;173;174;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;463.8764,57.88174;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;81;227.2378,643.7848;Inherit;False;Property;_Offset;Offset;2;1;[Header];Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;158;1650.095,937.4907;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;162;1644.921,1093.576;Inherit;False;Property;_MaskOffset;MaskOffset;13;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;82;787.2185,100.1137;Inherit;False;3;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;78;772.4404,-184.6765;Inherit;False;Property;_Tiling;Tiling;1;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector4Node;83;1367.36,-657.3679;Inherit;False;Property;_LimitUVRange;LimitUVRange;4;1;[Header];Create;True;1;LimitUV;0;0;False;0;False;0,1,0,1;0,1,0,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;173;2365.489,1175.01;Inherit;False;Constant;_Float3;Float 3;19;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;174;2405.195,1359.424;Inherit;False;Constant;_Float5;Float 5;19;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;170;2189.666,1281.503;Inherit;False;Property;_Feather;Feather;14;0;Create;True;0;0;0;False;0;False;0;0.223;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;161;1608.19,748.5315;Inherit;False;Property;_MaskTiling;MaskTiling;12;0;Create;True;0;0;0;False;0;False;0,0;0.5,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;163;1904.504,1022.324;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;84;1594.783,-747.9218;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;85;1595.883,-532.4218;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;74;1020.081,-44.5886;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;137;2261.345,1507.165;Inherit;False;1;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;171;2530.214,1162.544;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;169;2557.224,1287.075;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;156;1842.131,810.5052;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;80;1889.626,-389.0704;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;172;2698.201,1299.677;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;167;2765.758,1850.221;Inherit;False;715.4617;522.7068;StaticMask;3;166;165;164;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;98;2929.765,-423.1228;Inherit;False;527.905;715.6232;VertexColor;5;97;95;96;120;122;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ToggleSwitchNode;109;2179.447,-325.5556;Inherit;False;Property;_LimitUV;LimitUV;5;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;175;2984.314,1256.771;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;176;2970.993,1420.424;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;125;2604.117,826.5101;Inherit;True;Property;_NoiseMask;NoiseMask;9;2;[Header];[NoScaleOffset];Create;True;1;Mask;0;0;False;0;False;-1;None;4e3951c538fc8a647a4a10a99b480987;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;73;2583.71,-121.8407;Inherit;True;Property;_Main;Main;0;2;[Header];[NoScaleOffset];Create;True;1;Main;0;0;False;0;False;-1;None;02dd6d2e0f8581346871201572c67500;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;96;2956.925,-172.8337;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;148;2824.794,646.5159;Inherit;False;Constant;_Float2;Float 2;12;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;164;2815.758,1900.221;Inherit;True;Property;_StaticMask;StaticMask;15;1;[Header];Create;True;1;StaticMask;0;0;False;0;False;-1;None;5a20d78a06391a74195b3a898673fb63;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;166;2867.849,2130.773;Inherit;False;Property;_SmoothStep;SmoothStep;16;0;Create;True;0;0;0;False;0;False;0,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SmoothstepOpNode;155;3270.459,987.067;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;120;2967.5,-290.9236;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;122;2976.133,-363.7119;Inherit;False;Constant;_Float0;Float 0;8;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;97;3214.735,62.38358;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;165;3214.41,1984.56;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;147;3171.941,733.5972;Inherit;False;Property;_Mask;Mask;10;0;Create;True;0;0;0;False;0;False;1;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;121;3237.433,-342.9119;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;3218.178,-102.1768;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;3559.417,220.5364;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;188;3784.679,63.41165;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;189;3934.74,-36.03175;Float;False;True;-1;2;ASEMaterialInspector;0;15;CommonParticleOneShot;cf964e524c8e69742b1d21fbe2ebcc4a;True;Sprite Unlit;0;0;Sprite Unlit;4;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;True;12;all;0;False;True;2;5;False;;10;False;;3;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;Hidden/InternalErrorShader;0;0;Standard;3;Vertex Position;1;0;Debug Display;0;0;External Alpha;0;0;0;4;True;True;True;True;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;190;3934.74,-36.03175;Float;False;False;-1;2;ASEMaterialInspector;0;1;New Amplify Shader;cf964e524c8e69742b1d21fbe2ebcc4a;True;Sprite Unlit Forward;0;1;Sprite Unlit Forward;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;True;12;all;0;False;True;2;5;False;;10;False;;3;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalForward;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;191;3934.74,-36.03175;Float;False;False;-1;2;ASEMaterialInspector;0;1;New Amplify Shader;cf964e524c8e69742b1d21fbe2ebcc4a;True;SceneSelectionPass;0;2;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;192;3934.74,-36.03175;Float;False;False;-1;2;ASEMaterialInspector;0;1;New Amplify Shader;cf964e524c8e69742b1d21fbe2ebcc4a;True;ScenePickingPass;0;3;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
WireConnection;103;0;115;0
WireConnection;103;1;102;0
WireConnection;114;0;118;0
WireConnection;114;1;103;0
WireConnection;114;2;119;4
WireConnection;106;0;114;0
WireConnection;123;0;124;0
WireConnection;123;1;106;0
WireConnection;77;0;76;3
WireConnection;77;1;75;0
WireConnection;158;0;157;4
WireConnection;158;1;159;0
WireConnection;82;0;77;0
WireConnection;82;1;81;0
WireConnection;82;2;123;0
WireConnection;163;0;162;0
WireConnection;163;1;158;0
WireConnection;84;0;83;1
WireConnection;84;1;83;3
WireConnection;85;0;83;2
WireConnection;85;1;83;4
WireConnection;74;0;78;0
WireConnection;74;1;82;0
WireConnection;171;0;173;0
WireConnection;171;1;170;0
WireConnection;169;0;174;0
WireConnection;169;1;170;0
WireConnection;156;0;161;0
WireConnection;156;1;163;0
WireConnection;80;0;74;0
WireConnection;80;1;84;0
WireConnection;80;2;85;0
WireConnection;172;0;171;0
WireConnection;172;1;169;0
WireConnection;172;2;137;3
WireConnection;109;0;74;0
WireConnection;109;1;80;0
WireConnection;175;0;172;0
WireConnection;175;1;170;0
WireConnection;176;0;172;0
WireConnection;176;1;170;0
WireConnection;125;1;156;0
WireConnection;73;1;109;0
WireConnection;155;0;125;1
WireConnection;155;1;175;0
WireConnection;155;2;176;0
WireConnection;97;0;96;4
WireConnection;97;1;73;4
WireConnection;165;0;164;1
WireConnection;165;1;166;1
WireConnection;165;2;166;2
WireConnection;147;0;148;0
WireConnection;147;1;155;0
WireConnection;121;0;122;0
WireConnection;121;1;120;1
WireConnection;95;0;96;0
WireConnection;95;1;73;0
WireConnection;95;2;121;0
WireConnection;126;0;97;0
WireConnection;126;1;147;0
WireConnection;126;2;165;0
WireConnection;188;0;95;0
WireConnection;188;3;126;0
WireConnection;189;1;188;0
ASEEND*/
//CHKSM=4F76CB649DC9C932C3037D3B8C48F3E2BAD91D26