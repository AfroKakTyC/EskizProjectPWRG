Shader "Custom/Hole2"
{
    //SubShader {
    //		// Очередь должа стоять после объектов которые смогут опускаться в дырку (шар,
    //		// дыра-цилиндр), но перед теми в которых мы хотим выколоть дыру (стол) 
    //		Tags { "Queue" = "Geometry+593" }
    // 
    //		// Не рисовать никаких цветов, только Z-буфер
    // 		ColorMask 0
    //		Ztest LEqual
    // 
    //		// Во время прохода ничего не делаем 
    //		Pass {}
    //	}
    Properties{
           _MainTex("Base (RGB)", 2D) = "white" {}
           _Color("Color (RGBA)", Color) = (1, 1, 1, 1)
    }

        SubShader{
            Tags {"Queue" = "Geometry-1" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
            //ZWrite On
            ZWrite Off
               //ZTest Less
               Blend Zero One
               //Blend SrcAlpha OneMinusSrcAlpha
            //Cull back
            //LOD 100

            Pass {
                Stencil
               {
                   Ref 2
                   Comp always
                   Pass replace
               }
               CGPROGRAM

                   #pragma vertex vert alpha
                   #pragma fragment frag alpha

                   #include "UnityCG.cginc"

                   struct appdata_t {
                       float4 vertex : POSITION;
                       float2 texcoord : TEXCOORD0;
                   };

                   struct v2f {
                       float4 vertex : SV_POSITION;
                       half2 texcoord : TEXCOORD0;
                   };

                   sampler2D _MainTex;
                   float4 _MainTex_ST;
                   float4 _Color;

                   v2f vert(appdata_t v)
                   {
                       v2f o;
                       o.vertex = UnityObjectToClipPos(v.vertex);
                       // ADDED BY BERNIE:
                       v.texcoord.x = 1 - v.texcoord.x;
                       o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                       return o;
                   }

                   fixed4 frag(v2f i) : SV_Target
                   {
                       fixed4 col = tex2D(_MainTex, i.texcoord) * _Color;
                       return col;
                   }
               ENDCG
           }
           }
}
