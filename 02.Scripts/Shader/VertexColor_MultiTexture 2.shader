Shader "Custom/VertexColor_MultiTexture"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _RTex("Albedo (RGB)", 2D) = "white" {} //R
        _GTex("Albedo (RGB)", 2D) = "white" {} //G
        _BTex("Albedo (RGB)", 2D) = "white" {}
        _BumpMap("_BumpMap",2D) = "White"{}

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
       // #pragma surface surf Standard fullforwardshadows
        #pragma surface surf Standard fullforwardshadows


        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _BumpMap;

        sampler2D _MainTex,_RTex,_GTex,_BTex;


        struct Input
        {
            float2 uv_MainTex;
            float2 uv_RTex;
            float2 uv_GTex;
            float2 uv_BTex;

            float4 color:COLOR;
            float2 uv_BumpMap;
        };

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {  
            fixed3 n = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

            fixed4 MainTex = tex2D (_MainTex, IN.uv_MainTex);
            fixed4 RTex = tex2D(_RTex, IN.uv_MainTex);
            fixed4 GTex = tex2D(_GTex, IN.uv_MainTex);
            fixed4 BTex = tex2D(_BTex, IN.uv_MainTex);

            o.Albedo = lerp(MainTex.rgb,RTex.rgb,IN.color.r);
            o.Albedo = lerp(o.Albedo, GTex.rgb, IN.color.g);
            o.Albedo = lerp(o.Albedo, BTex.rgb, IN.color.b);

            o.Normal = n;

            o.Alpha = MainTex.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
