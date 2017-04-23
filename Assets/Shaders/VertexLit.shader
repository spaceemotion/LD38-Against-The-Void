
Shader "VertexLit" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 400
           
        CGPROGRAM
        #pragma surface surf BlinnPhong
         
         
        sampler2D _MainTex;
        fixed4 _Color;
         
        struct Input {
            float2 uv_MainTex;
            half4 color : COLOR0;
        };
         
        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = tex.rgb * _Color.rgb * IN.color.rgb;
        }
        ENDCG
    }
     
    FallBack "Specular"
}
 
 