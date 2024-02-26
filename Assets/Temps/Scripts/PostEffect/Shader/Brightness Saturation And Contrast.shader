Shader "URP/Brightness Saturation And Contrast"{
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Brightness ("Brightness", Float) = 1
		_Saturation("Saturation", Float) = 1
		_Contrast("Contrast", Float) = 1
	}
	SubShader {
		Pass {  
			ZTest Always Cull Off ZWrite Off
			
			HLSLPROGRAM  
			#pragma vertex vert  
			#pragma fragment frag  
			  
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"  
			  
			sampler2D _MainTex;  
			half _Brightness;
			half _Saturation;
			half _Contrast;
			  
			struct v2f {
				float4 pos : SV_POSITION;
				half2 uv: TEXCOORD0;
			};
			  
			v2f vert(appdata_img v) {
				v2f o;
				o.pos = TransformObjectToHClip(v.vertex.xyz);
				o.uv = v.texcoord;
						 
				return o;
			}
		
			fixed4 frag(v2f i) : SV_Target {
				fixed4 renderTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);  
				  
				fixed3 finalColor = renderTex.rgb * _Brightness;
				
				fixed luminance = 0.2125 * renderTex.r + 0.7154 * renderTex.g + 0.0721 * renderTex.b;
				fixed3 luminanceColor = fixed3(luminance, luminance, luminance);
				finalColor = lerp(luminanceColor, finalColor, _Saturation);
				
				fixed3 avgColor = fixed3(0.5, 0.5, 0.5);
				finalColor = lerp(avgColor, finalColor, _Contrast);
				
				return fixed4(finalColor, renderTex.a);  
			}  
			  
			ENDHLSL
		}  
	}
	
	Fallback Off
}
