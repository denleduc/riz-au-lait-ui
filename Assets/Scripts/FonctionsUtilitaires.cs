using System.Collections.Generic;
using UnityEngine;

/*Référence les fonctions génériques du jeu. On le colle sur un objet vide pour qu'il puisse être détecté par les autres scripts qui ont besoin de lui .
 NOTE : Les fonctions de ce script doivent être en statique pour que ça soit plus facile à intégrer.*/

public class FonctionsUtilitaires : MonoBehaviour
{
    /*@brief, FindAllWIthTag Retourne un tableau contenant tous les GameObject avec un tag passé en paramètre.
     @param1 tag, la chaine de caractère qui contient le tag.
     @return, une liste contenant chaque GameObject qui correspond au tag.*/
    public static GameObject[] FindAllWithTag(string tag)
    {
        List<GameObject> results = new();
        
        Transform[] tousLesTransforms = GameObject.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (Transform t in tousLesTransforms)
        {
            if (t.CompareTag(tag))
            {
                results.Add(t.gameObject);
            }
        }

        return results.ToArray();
    }
}
