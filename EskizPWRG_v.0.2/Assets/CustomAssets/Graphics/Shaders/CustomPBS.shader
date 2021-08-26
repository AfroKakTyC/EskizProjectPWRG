Shader "Pavel/CustomPBS"
{
//https://wiki.unity3d.com/index.php/Physically-inspired_Shader
    Properties
    {
        _Shininess ("Shininess", Range (0.01, 1)) = 0.078125

        _MainTex("Albedo", 2D) = "white" {}
		_Color ("Color",Color) = (1,1,1,1)
		_EmissionColor ("EmissionColor",Color) = (0,0,0,1)
		_EmissionSelectColor ("EmissionSelectColor",Color) = (0,0,0,1)
		_DarknessSelectColor ("DarknessSelectColor",Color) = (1,1,1,1)
		
        //_MetallicMap("Metallic Map", 2D) = "white" {}
        //_Metallic("Metallic Scale", Float) = 1.0
		
        [NoScaleOffset] _BumpMap("Bump Map", 2D) = "bump" {}
        _BumpScale("Bump Scale", Float) = 1.0
		
        [NoScaleOffset] _GlossMap("Roughness Map", 2D) = "white" {}
		_GlossScale("Roughness", Range(0.0, 4.0)) = 1
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }

			Stencil
			{
				Ref 1
				Comp Gequal
				Pass keep
				Fail keep
			}

        CGPROGRAM
        #pragma target 3.0
		#include "UnityCG.cginc"
		
        #pragma surface surf MobileBlinnPhong /*exclude_path:prepass*/ nolightmap /*noforwardadd*/ halfasview interpolateview fullforwardshadows
        //#pragma surface surf Standard fullforwardshadows



        struct Input
        {
            float2 uv_MainTex;
			//float2 uv_MetallicMap;
			//float2 uv_BumpMap;
			//float2 uv_RoughnessMap;
        };
		
		fixed4 _Color;
		fixed4 _EmissionColor;
		fixed4 _EmissionSelectColor;
		fixed4 _DarknessSelectColor;
		
        sampler2D _MainTex;
        //sampler2D _MetallicMap;
        sampler2D _BumpMap;
        sampler2D _GlossMap;
		
        //fixed _Metallic;
        fixed _BumpScale;
        fixed _GlossScale;
        fixed _Shininess;

		float _Spec;
		
		//float4 _MainTex_ST;
		
		fixed4 LightingMobileBlinnPhong (SurfaceOutput s, fixed3 lightDir, fixed3 halfDir, fixed atten)
		{
			fixed diff = max (0, dot (s.Normal, lightDir));
			fixed nh = max (0, dot (s.Normal, halfDir));
			fixed spec = pow (nh, s.Specular*128) * s.Gloss;
   
			fixed4 c;
			c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * atten;
			UNITY_OPAQUE_ALPHA(c.a);
			return c;
		}

        void surf (Input IN, inout SurfaceOutput o)
        {
			//o.Albedo.r = _MainTex_ST.x;
			//o.Albedo.g = _MainTex_ST.y;
		
			//float2 uv = IN.uv_MainTex * _MainTex_ST.xy + _MainTex_ST.zw;
			//float2 uv = TRANSFORM_TEX(IN.uv_MainTex, _MainTex);
			float2 uv = IN.uv_MainTex;
			
			fixed4 c = tex2D (_MainTex, uv) * _Color + _EmissionColor;
			c = c * _DarknessSelectColor + _EmissionSelectColor;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			
			
            //o.Metallic = tex2D(_MetallicMap, IN.uv_MetallicMap) * _Metallic;
			//fixed4 norm = tex2D (_BumpMap, uv);
			//norm.z = norm.z * _Bump; 
			//norm = normalize(norm);

			fixed3 normal = UnpackNormal (tex2D (_BumpMap, uv));
			normal.xy = normal.xy * _BumpScale; 
			o.Normal = normalize(normal); 
			
			//fixed roug = tex2D(_RoughnessMap, uv).r;
			/*
			o.Gloss = roug;
			o.Specular = roug * _Roughness;
			*/
			//o.Gloss = _Roughness;
			//o.Specular = _Roughness;
			
            o.Gloss = tex2D (_GlossMap, uv).r * _GlossScale;
            o.Specular = _Shininess;
        }
        ENDCG
    }
    FallBack "Mobile/Diffuse"
}
