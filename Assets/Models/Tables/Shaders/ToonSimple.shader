Shader "Toon/ToonSimple" {
	Properties{
		_Color("Main Color", Color) = (0.5,0.5,0.5,1)
		_HColor("Highlights Color", Color) = (0.785,0.785,0.785,1.0)
		_SColor("Shadows Color", Color) = (0.195,0.195,0.195,1.0)
		//_ToonTint("Toon Ramp Tint", Color) = (0.5,0.5,0.5,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_ToonOffset("Toon Offset", Range(0,1)) = 0.5
		_ToonBlur("Toon Blur", Range(0,0.5)) = 0.05


	}

		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

	CGPROGRAM
			//#pragma surface surf ToonRamp
			#pragma surface surf ToonRamp noforwardadd interpolateview halfasview

					// custom lighting function that uses a texture ramp based
					// on angle between light direction and normal
					#pragma lighting ToonRamp exclude_path:prepass

						//float4 _ToonTint;
						float _ToonOffset, _ToonBlur;
		//Highlight/Shadow Colors
		float4 _HColor, _SColor;
		inline half4 LightingToonRamp(SurfaceOutput s, half3 lightDir, half atten)
		{
	#ifndef USING_DIRECTIONAL_LIGHT
			lightDir = normalize(lightDir);
	#endif
			//float d = dot(s.Normal, lightDir);
			//float3 ramp = smoothstep(_ToonOffset, _ToonOffset + _ToonBlur, d) + _ToonTint;


			//fixed d = max(0, dot(s.Normal, lightDir));
			fixed d = max(0, dot(s.Normal, lightDir)*0.5 + 0.5);
			//fixed3 ramp = tex2D(_Ramp, fixed2(ndl, ndl));

			float3 ramp = smoothstep(_ToonOffset - _ToonBlur * 0.5, _ToonOffset + _ToonBlur * 0.5, d);


			//from ToonColors
			ramp *= atten;
			_SColor = lerp(_HColor, _SColor, _SColor.a);	//Shadows intensity through alpha
			ramp = lerp(_SColor.rgb, _HColor.rgb, ramp);

			half4 c;
			//c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 0.6);
			c.rgb = s.Albedo * _LightColor0.rgb * ramp;
			c.a = 0;
			return c;
		}


	sampler2D _MainTex;
	float4 _Color;

	struct Input {
		float2 uv_MainTex : TEXCOORD0;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}
	ENDCG

		}

			Fallback "Diffuse"
}