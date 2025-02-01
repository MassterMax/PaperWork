Shader "Unlit/MaskShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CursorPos ("Cursor Position", Vector) = (0.5, 0.5,0,0)
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
            };

            sampler2D _MainTex;
            float4 _CursorPos;
            float _Radius;
            half4 _MainTex_TexelSize;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Picture Ratio
                float aspectRatio = _MainTex_TexelSize.z / _MainTex_TexelSize.w;

                // Скорректируйте координаты uv в соответствии с соотношением сторон
                float2 adjustedCursor = float2(_CursorPos.x * aspectRatio, _CursorPos.y);
                float2 adjustedUV = float2(uv.x * aspectRatio, uv.y);

                float dist = distance(adjustedUV, adjustedCursor);
                float alpha = smoothstep(_Radius, _Radius - 0.01, dist);

                fixed4 col = tex2D(_MainTex, i.uv);
                col.a = alpha;
                return col;
            }
            ENDCG
        }
    }
}
