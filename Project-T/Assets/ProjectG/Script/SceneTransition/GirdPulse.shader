Shader "UI/GridPulse"
{
    Properties
    {
        _Color ("Grid Color", Color) = (1,1,1,1)
        _Spacing ("Grid Spacing", Float) = 50
        _MinThickness ("Min Thickness", Float) = 1.0
        _AddThickness ("Thickness Amplitude", Float) = 2.0
        _Speed ("Pulse Speed", Float) = 3.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 screenPos : TEXCOORD1; // Dùng để tính kích thước pixel thực
            };

            float4 _Color;
            float _Spacing;
            float _MinThickness;
            float _AddThickness;
            float _Speed;
            float4 _PanelSize; // Được truyền từ C# (Width, Height, 0, 0)

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            // Hàm vẽ đường Grid (có khử răng cưa)
            float grid(float2 uv, float thickness)
            {
                // Quy đổi UV (0-1) sang Pixel Space dựa trên kích thước Panel
                float2 pos = uv * _PanelSize.xy;
                
                // Tính khoảng cách đến đường lưới gần nhất
                // fmod trả về phần dư, giúp lặp lại lưới
                float2 wrapped = fmod(pos, _Spacing);
                float2 dist = min(wrapped, _Spacing - wrapped);
                
                // Khoảng cách tối thiểu theo trục x hoặc y
                float minDist = min(dist.x, dist.y);

                // Sử dụng fwidth để làm mềm biên (anti-aliasing)
                // 0.5 * thickness vì line nằm giữa tâm
                float delta = fwidth(minDist);
                float alpha = smoothstep(thickness * 0.5, (thickness * 0.5) - delta, minDist);
                
                return alpha;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Công thức: min + thickness * sin(time)
                // _Time.y trong Unity là thời gian tính bằng giây
                float timeVal = _Time.y * _Speed;
                float currentThickness = _MinThickness + _AddThickness * sin(timeVal);

                // Đảm bảo không bị âm
                currentThickness = max(0.0, currentThickness);

                float alpha = grid(i.uv, currentThickness);
                
                return fixed4(_Color.rgb, _Color.a * alpha);
            }
            ENDCG
        }
    }
}