Shader "Custom/UI_Masque"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        // Propriétés requises pour l'UI Unity
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
            "ForceStereoDrawCall"="True"
        }

        // C'est ici qu'on force l'écriture dans le Pochoir (Stencil)
        Stencil
        {
            Ref 1           // On écrit le chiffre 1
            Comp Always     // Toujours (peu importe ce qu'il y avait avant)
            Pass Replace    // On remplace la valeur existante par 1
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        
        // MAGIE : ColorMask 0 empêche le dessin des couleurs (Rend l'objet invisible)
        ColorMask 0

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            #include "UnityShaderVariables.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float4 _MainTex_ST;
            fixed4 _TextureSampleAdd;

            v2f vert(appdata_t v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(o.worldPosition);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // On lit la texture (le sprite flou)
                half4 color = (tex2D(_MainTex, i.texcoord) + _TextureSampleAdd) * i.color;
    
                // Ancien code : Détecte le bord et lisse la coupure avec une largeur dynamique.
                // float alphaWidth = fwidth(color.a) * 0.5;

                // NOUVEAU CODE : Rayon de flou plus grand pour le smoothstep.
                // Nous définissons une largeur de lissage fixe et légèrement plus grande (0.001)
                // pour garantir la présence du lissage.
                float alphaWidth = fwidth(color.a) * 3.0; // Multiplier fwidth par 1.5 ou 2.0
                if (alphaWidth < 0.001) alphaWidth = 0.001; // S'assurer qu'il y a toujours un minimum de flou

                // Utilise smoothstep pour lisser la coupure alpha
                float smoothedAlpha = smoothstep(0.0, alphaWidth, color.a);
    
                // Coupure finale pour le Stencil (on ne veut pas écrire dans le pochoir si c'est transparent)
                // On utilise la valeur lissée.
                clip (smoothedAlpha - 0.01);
    
                return color; // ColorMask 0 empêche le dessin de la couleur
            }
            ENDCG
        }
    }
}