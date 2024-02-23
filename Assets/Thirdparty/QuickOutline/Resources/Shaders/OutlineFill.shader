//
//  OutlineFill.shader
//  QuickOutline
//
//  Created by Chris Nolet on 2/21/18.
//  Copyright © 2018 Chris Nolet. All rights reserved.
//

Shader "Custom/Outline Fill" {
  Properties {
    [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 0

    _OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
    _DitherStrength("Dither Strength", Range(0.0, 1.0)) = 0.5
  }

  SubShader {
    Tags {
      "Queue" = "Transparent+110"
      "RenderType" = "Transparent"
      "DisableBatching" = "True"
    }

    Pass {
      Name "Fill"
      Cull Off
      ZTest [_ZTest]
      ZWrite Off
      Blend SrcAlpha OneMinusSrcAlpha
      ColorMask RGB

      Stencil {
        Ref 1
        Comp NotEqual
      }

      CGPROGRAM
      #include "UnityCG.cginc"

      #pragma vertex vert
      #pragma fragment frag

      struct appdata {
        float4 vertex : POSITION;
      };

      struct v2f {
        float4 position : SV_POSITION;
        fixed4 color : COLOR;
      };

      uniform fixed4 _OutlineColor;
      uniform float _DitherStrength;

      v2f vert(appdata input) {
        v2f output;

        output.position = UnityViewToClipPos(UnityObjectToViewPos(input.vertex));
        output.color = _OutlineColor;

        return output;
      }

      half dither(half In, half4 uv) {
          half DITHER_THRESHOLDS[16] =
          {
              1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
              13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
              4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
              16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
          };
          uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
          return In - DITHER_THRESHOLDS[index];
      }

      fixed4 frag(v2f input) : SV_Target {
        clip(dither(_DitherStrength, input.position));
        return input.color;
      }
      ENDCG
    }
  }
}
