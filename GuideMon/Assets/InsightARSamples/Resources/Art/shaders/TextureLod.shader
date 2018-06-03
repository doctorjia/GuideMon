// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/TextureLod"
{
	Properties
	{
		_MainTex ("Main Tex", 2D) = "white" {}
        _MinDist ("Min Distance", Range (0.01, 0.2)) = 0.1
        _MaxDist ("Max Distance", Range (0.5, 2)) = 1
        _Color ("Main Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		 Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
     LOD 100
     
     ZWrite Off
     Blend SrcAlpha OneMinusSrcAlpha 

//      GrabPass{  }
		Pass
		{
   
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
               
			};

			sampler2D _MainTex;
//            sampler2D _GrabTexture; 
			float4 _MainTex_ST;
            float4 _Color;

            float _MinDist;
            float _MaxDist;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex); 
				o.uv = TRANSFORM_TEX(v.uv, _MainTex); 
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv); 
                col.rgb *= _Color.rgb;
                float dist = distance(_WorldSpaceCameraPos,float3(0,0,0) );
                dist = clamp(dist,_MinDist,_MaxDist);
                dist = _MaxDist-dist+_MinDist;
                float alpha = smoothstep(_MinDist,_MaxDist,dist);
                col.a = col.a<alpha?0.0:col.a;
				return col;
			}
			ENDCG
		}
	}
}
