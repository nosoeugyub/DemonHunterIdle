Shader "Unlit/Black"
{
     Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {} // Mask texture
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Stencil
            {
                Ref 1              // Reference value to compare against
                Comp Equal         // Comparison function
                Pass Keep          // What to do when stencil test passes
                Fail Keep          // What to do when stencil test fails
            }

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
            float4 _MainTex_ST;

            sampler2D _MaskTex;
            float4 _MaskTex_ST;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the main texture and mask texture
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 mask = tex2D(_MaskTex, i.uv);

                // Grayscale conversion (preserving alpha)
                float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));

                // Apply masking by multiplying with the mask texture's alpha
                float maskAlpha = mask.a;
                return fixed4(gray, gray, gray, col.a * maskAlpha);
            }
            ENDCG
        }
    }
    FallBack "Transparent/Cutout/Diffuse"
}
