Shader "VideoPlaneNoLight" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" { }
        _texCoordScaleX ("Texture Coordinate Scale", float) = 1.0
        _texCoordScaleY ("Texture Coordinate Scale", float) = 1.0
        _isWhite("Bg White",Int) = 1
	}
	SubShader {
		Pass {
		   Name "VideoPlaneNoLight"
		

			Lighting Off
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

            uniform float _texCoordScaleX,_texCoordScaleY;
            uniform int _isWhite;
            float4x4 _TextureRotation;

            struct Vertex
			{
				float4 position : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct TexCoordInOut
			{
				float4 position : SV_POSITION;
				float2 texcoord : TEXCOORD0;
			};

			TexCoordInOut vert (Vertex vertex)
			{
				TexCoordInOut o;
                o.position = UnityObjectToClipPos(vertex.position); 
                o.texcoord = float2((vertex.texcoord.x - 0.5f) * _texCoordScaleX + 0.5f, -(vertex.texcoord.y - 0.5f) * _texCoordScaleY + 0.5f);
                o.texcoord = mul(_TextureRotation, float4(o.texcoord,0,1)).xy;
                
	            
				return o;
			}
			 // samplers
            sampler2D _MainTex;

			fixed4 frag (TexCoordInOut i) : SV_Target
			{
				// sample the texture
                float2 texcoord = i.texcoord;
                return tex2D(_MainTex, texcoord).rgba*_isWhite;
			}
			ENDCG
		}
	}
}
