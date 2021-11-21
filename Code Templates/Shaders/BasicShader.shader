Shader "MyShaders/DepthShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        CGINCLUDE
        sampler2D _MainTex;
        float4 _MainTex_ST;
        ENDCG

        Pass
        {
            CGPROGRAM
            #pragma vertex vertex
            #pragma fragment fragment

            #include "UnityCG.cginc"

            struct vertexInput
            {
                float4 vertexPosition : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct fragmentInput
            {
                float4 clipPosition : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fragmentInput vertex (vertexInput v_input)
            {
                fragmentInput f_input;
                f_input.clipPosition = UnityObjectToClipPos(v_input.vertexPosition);
                f_input.uv = TRANSFORM_TEX(v_input.uv, _MainTex);
                return f_input;
            }

            fixed4 fragment (fragmentInput f_input) : SV_Target
            {
                // sample the texture
                fixed4 finalColor = tex2D(_MainTex, f_input.uv);
                return finalColor;
            }
            ENDCG
        }
    }
}
