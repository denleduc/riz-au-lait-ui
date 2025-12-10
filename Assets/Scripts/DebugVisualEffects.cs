using UnityEngine;
using UnityEngine.UI;

/*Donné par ChatGPT pour débuguer les problèmes liés aux canvas en les affichant en rouge pâle. À mettre dans un objet vide avec Add component. S'active tout seul.*/

[ExecuteAlways]
public class DebugVisualEffects : MonoBehaviour
{
        void Start()
        {
            foreach (Canvas canvas in FindObjectsByType<Canvas>(FindObjectsSortMode.None))
            {
                var existingOutline = canvas.transform.Find("CanvasDebugBordure");

                GameObject outline;
                if (existingOutline != null)
                {
                    // Il existe déjà, on le réutilise
                    outline = existingOutline.gameObject;
                }
                else
                {
                    // Sinon, on le crée
                    outline = new GameObject("CanvasDebugBordure");
                    outline.transform.SetParent(canvas.transform, false);
                }

                var img = outline.AddComponent<Image>();
                img.color = new Color(1f, 0f, 0f, 0.1f); // rouge transparent

                var rect = img.GetComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = rect.offsetMax = Vector2.zero;
            }
        }
}
