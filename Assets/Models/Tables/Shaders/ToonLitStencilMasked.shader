Shader "Toon/Lit StencilMask" {
	Properties {
		_Color ("Main Color", Color) = (0.5,0.5,0.5,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_HColor("Highlights Color", Color) = (0.785,0.785,0.785,1.0)
		_SColor("Shadows Color", Color) = (0.195,0.195,0.195,1.0)
		//_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {} 
				_ToonOffset("Toon Offset", Range(0,1)) = 0.5
		_ToonBlur("Toon Blur", Range(0,0.5)) = 0.05
		

		_ID("Mask ID", Int) = 1

	}

	SubShader {
		Tags { "RenderType"="Opaque" "Queue" = "Geometry+2"}
		LOD 200
		
		Stencil {
			Ref [_ID]
			Comp equal
		}

		
CGPROGRAM
#pragma surface surf ToonRamp

//sampler2D _Ramp;


						//float4 _ToonTint;
						float _ToonOffset, _ToonBlur;
// custom lighting function that uses a texture ramp based
// on angle between light direction and normal
		//Highlight/Shadow Colors
		float4 _HColor, _SColor;
#pragma lighting ToonRamp exclude_path:prepass
inline half4 LightingToonRamp (SurfaceOutput s, half3 lightDir, half atten)
{
	#ifndef USING_DIRECTIONAL_LIGHT
	lightDir = normalize(lightDir);
	#endif
	
	half d = dot (s.Normal, lightDir)*0.5 + 0.5;
	//half3 ramp = tex2D (_Ramp, float2(d,d)).rgb;
	
	float3 ramp = smoothstep(_ToonOffset - _ToonBlur * 0.5, _ToonOffset + _ToonBlur * 0.5, d);
	
				//from ToonColors
			ramp *= atten;
			_SColor = lerp(_HColor, _SColor, _SColor.a);	//Shadows intensity through alpha
			ramp = lerp(_SColor.rgb, _HColor.rgb, ramp);
	
	half4 c;
	c.rgb = s.Albedo * _LightColor0.rgb * ramp;
	c.a = 0;
	return c;
}


sampler2D _MainTex;
float4 _Color;

struct Input {
	float2 uv_MainTex : TEXCOORD0;
};

void surf (Input IN, inout SurfaceOutput o) {
	half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	//c = pow(c, 0.4545);	// gamma correction
	//c = pow(c, 0.4545);	// gamma correction
	//c = pow(c, 1.0 / 2.8);
	o.Albedo = c.rgb;
	o.Alpha = c.a;
}
ENDCG

	} 

	Fallback "Diffuse"
}
