// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/EnemyHitEffect"
{
     Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1) // 추가된 색깔 지정
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" }
        LOD 100

        Pass
        {
            ZTest Always
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            // External include removed as per request
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 fogCoord : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color; // 색깔 속성

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); // UnityObjectToClipPos 대체
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                o.fogCoord = o.vertex.xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color; // 색깔 적용
                col.rgb = lerp(col.rgb, _Color.rgb, _Color.a); // 투명도를 통한 색 보정
                return col;
            }
            ENDCG
        }
    }
}
