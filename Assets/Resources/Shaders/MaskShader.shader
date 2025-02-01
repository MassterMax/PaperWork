Shader "Unlit/MaskShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CursorPos ("Cursor Position", Vector) = (0,0,0,0)
        _Radius ("Mask Radius", Float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
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
                float2 screenPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _CursorPos;
            float _Radius;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.screenPos = ComputeScreenPos(o.vertex).xy;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                fixed4 col = tex2D(_MainTex, uv);

                // Вычисляем расстояние от курсора до текущего пикселя
                // float dist = distance(i.screenPos, _CursorPos.xy);
                float dist = distance(uv, _CursorPos.xy);

                // Маскируем пиксели вне круга
                // col.a *= step(dist, _Radius);
                col.a *= smoothstep(_Radius + 0.05, _Radius - 0.05, dist);
                return col;
            }
            ENDCG
        }
    }
}
