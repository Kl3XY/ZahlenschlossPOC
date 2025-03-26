Shader "Custom/ColoredMovingNoise_Pixelated"
{
    Properties
    {
        _Color1 ("Farbe 1", Color) = (1, 0, 0, 1)
        _Color2 ("Farbe 2", Color) = (0, 0, 1, 1)
        _Speed  ("Geschwindigkeit", Float) = 1.0
        _Scale  ("Skalierung", Float) = 2.0
        _Contrast ("Kontrast", Float) = 1.0
        _PixelSize ("Pixelgröße", Float) = 16.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        Pass
        {
            CGPROGRAM
            // Direktiven für Vertex- und Fragment-Shader
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            // Material-Properties als Uniforms
            fixed4 _Color1;
            fixed4 _Color2;
            float _Speed;
            float _Scale;
            float _Contrast;
            float _PixelSize;
            
            // Vertex-Shader-Strukturen
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };
            
            // Vertex-Shader: Überträgt UVs und transformiert die Vertices
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = v.uv;
                return o;
            }
            
            // Hilfsfunktion: Hash für 2D Rauschen
            float hash (float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453123);
            }
            
            // Einfaches 2D Noise
            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                
                float a = hash(i);
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));
                
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }
            
            // Fraktales Rauschen (fBm) mit mehreren Oktaven
            float fbm(float2 p)
            {
                float total = 0.0;
                float amplitude = 1.0;
                float frequency = 1.0;
                const int OCTAVES = 5;
                for (int i = 0; i < OCTAVES; i++)
                {
                    total += noise(p * frequency) * amplitude;
                    amplitude *= 0.5;
                    frequency *= 2.0;
                }
                return total;
            }
            
            // Fragment-Shader: Erzeugt ein farbiges, sich bewegendes Muster und pixeliert das Endergebnis
            fixed4 frag(v2f i) : SV_Target
            {
                // Pixelierung: Quantisiere die UV-Koordinaten
                float2 uv = i.uv;
                uv = floor(uv * _PixelSize) / _PixelSize;
                
                // Wende Skalierung und zeitliche Verschiebung an, um Bewegung zu erzeugen
                uv = uv * _Scale;
                uv += _Time.x * _Speed;
                
                // Berechne unabhängiges Noise für jeden Farbkanal (leicht versetzt)
                float nr = fbm(uv + float2(0.0, 0.0));
                float ng = fbm(uv + float2(5.2, 1.3));
                float nb = fbm(uv + float2(1.7, 9.2));
                float3 noiseColor = float3(nr, ng, nb);
                
                // Optionale Kontrastanpassung
                noiseColor = saturate(pow(noiseColor, float3(_Contrast, _Contrast, _Contrast)));
                
                // Interpoliere zwischen _Color1 und _Color2 basierend auf dem Noise
                float3 finalColor = lerp(_Color1.rgb, _Color2.rgb, noiseColor);
                
                return fixed4(finalColor, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}