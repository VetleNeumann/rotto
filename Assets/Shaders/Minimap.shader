Shader "Custom/Minimap"
{
   Properties {
		_ShadowTex ("Projected Image", 2D) = "white" {}
		_BorderStrength("Border Strength", Float) = 5
   }
   SubShader {
      Pass {      
		Blend One OneMinusSrcAlpha
		ZWrite Off
        Offset -1, -1
         
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
		float _BorderStrength;

         uniform sampler2D _ShadowTex; 
         uniform float4x4 unity_Projector;

          struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
         };

         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 posProj : TEXCOORD0;
			float4 worldPos : TEXCOORD1;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            output.posProj = mul(unity_Projector, input.vertex);
            output.pos = UnityObjectToClipPos(input.vertex);
			output.worldPos = input.vertex;
            return output;
         }
 
 
         float4 frag(vertexOutput input) : COLOR
         {
			float px = input.posProj.x;
			float py = input.posProj.y;

			float dist = distance(input.posProj.xy, float2(0.5, 0.5)) * 2;
			if (dist < 1)
				return tex2D(_ShadowTex, input.posProj.xy) * min(1, (1 - dist) * _BorderStrength);

			return float4(0, 0, 0, 0);
         }
 
         ENDCG
      }
   }  
   Fallback "Projector/Light"
}
