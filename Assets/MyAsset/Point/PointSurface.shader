Shader "Custom/PointSurface"
{

    //Properties で外から設定できるように
    Properties{
        _Smoothness ("Smoothness", Range(0,1)) = 0.5 //言語が違うのでfいらない
    }


    SubShader
    {
        CGPROGRAM //ここからCG言語
        #pragma surface ConfigureSurface Standard fullforwardshadows
        #pragma target 3.0 
        struct Input{
            float3 worldPos;
        };

        float _Smoothness; // ここでPropertiesの値を取得するのを忘れずに
		void ConfigureSurface (Input input, inout SurfaceOutputStandard surface) {
			surface.Albedo.rg = saturate(input.worldPos.xy * 0.5 + 0.5);
			surface.Smoothness = _Smoothness;
		}
        ENDCG
    }

    FallBack "Diffuse"
}
