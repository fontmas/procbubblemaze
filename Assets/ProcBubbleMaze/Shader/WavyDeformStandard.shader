// Created by Mario Madureira Fontes
// www.mario.pro.br
// December - 2015
//
// This shader was inspired on the shader created by mr5iveThou5and at:
// https://www.shadertoy.com/view/Xdt3RN
//
// This script has GPL 3.0 LICENSE:  http://www.gnu.org/licenses/gpl-3.0.en.html
//
// Enjoy!
 
Shader "Deformation/WavyDeformStandard"
{
	Properties {

		_WaveSpeed ("Wave Speed", Range(0.01,1)) = 0.095
                _Power ("Power Wave", Range(1,20)) = 2.0

		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}
		
		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0

	}

	CGINCLUDE
		#define UNITY_SETUP_BRDF_INPUT MetallicSetup
	ENDCG

	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Diffuse" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard vertex:vert alpha:fade
		
        //#pragma multi_compile SHADOWS_NATIVE SHADOWS_CUBE
		//#include "UnityCG.cginc"
		//#include "AutoLight.cginc"
		// Use shader model 3.0 target, to get nicer looking lighting
		//#pragma target 3.0

		sampler2D _MainTex;
		float _WaveSpeed;
		float _Power;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// vertex input: position, normal
		struct appdata {
			float4 vertex : POSITION;
			float4 texcoord : TEXCOORD0;
		};

		struct v2f {
			float4 pos : POSITION;
			float2 uv: TEXCOORD0;
		};

		// hash and noise functions from iq's example: https://www.shadertoy.com/view/4sfGzS
		float hash( float n ) { return frac(sin(n)*753.5453123); }
		float noise( in float3 x )
		{
		    float3 p = floor(x);
		    float3 f = frac(x);
		    f = f*f*(3.0-2.0*f);

		    float n = p.x + p.y*157.0 + 113.0*p.z;
		    return (lerp(lerp(lerp( hash(n+  0.0), hash(n+  1.0),f.x),
		               lerp( hash(n+157.0), hash(n+158.0),f.x),f.y),
		               lerp(lerp( hash(n+113.0), hash(n+114.0),f.x),
		               lerp( hash(n+270.0), hash(n+271.0),f.x),f.y),f.z));
		}

		float turbulence( float3 p ) {
			return (length(p)-0.5+ noise((p+float3(0, 0, _Time[1]/_WaveSpeed)) * _Power) * 0.25);
		}

		void vert (inout appdata_full v) {
			float3 p = float3(v.vertex.x,v.vertex.y,v.vertex.z);
			v.vertex.xyz += v.normal * turbulence(p);
		}

		struct Input {
			float2 uv_MainTex;
		};	

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Standard"
}
