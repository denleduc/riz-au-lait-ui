using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

/* Ce script est à coller sur une bulle texte.*/
public class ComportementBulleTexte3D : MonoBehaviour
{
    private GameObject bulle;
    private const string tag1 = "Bulle3D";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform t in transform)
        {
            if(t.gameObject.CompareTag(tag1))
                bulle = t.gameObject;
        }
    }

}
