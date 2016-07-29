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
 
Shader "Deformation/WavyDeformVerVertexLit"
{
 
Properties
{
    _Color ("Color", Color) = (1,1,1,1)
    _MainTex ("Albedo (RGB)", 2D) = "white" { }
    _WaveSpeed ("Wave Speed", Range(0.01,1)) = 0.95
    _Power ("Power Wave", Range(1,20)) = 2.0
}
 
SubShader
{
   
    Pass
    {
      // CULL Off
      LOD 200
      
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"
      #include "AutoLight.cginc"
       
      float4 _Color;
      sampler2D _MainTex;
      float _WaveSpeed;
      float _Power;
      float _AngleX;
      float _AngleY;
      float _AngleZ;
      float _SizeX;
      float _SizeY;
      float _SizeZ;
 
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
			return (length(p)-0.5+ noise((p+float3(0, 0, _Time[1]/_WaveSpeed)) * _Power) * 0.25)+1;
		}
 
      v2f vert (appdata v) {
          v2f o;
 
 		float3 p = float3(v.vertex.x,v.vertex.y,v.vertex.z);
 		float t = turbulence(p);
 		//if(t <= 0.0) t *= -1;
 		
 		// v.vertex.w = NORMAL of the vertex
        v.vertex.w=t*v.vertex.w;
        
        o.pos = mul( UNITY_MATRIX_MVP, v.vertex );
        
        o.uv = v.texcoord;
 
        return o;
      }

       
      float4 frag (v2f i) : COLOR
      {
         float4 color = tex2D(_MainTex, i.uv) * _Color;
         return color;
      }
 
      ENDCG
    }
}
   Fallback "VertexLit"
}