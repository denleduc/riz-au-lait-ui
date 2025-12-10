using UnityEngine;
using UnityEngine.EventSystems;

public class ComportementBoutonsEntree : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private AgrandissementBoutons ab;

    void Start()
    {
        ab = GetComponent<AgrandissementBoutons>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData EventData)
    {
        Debug.Log("Pointeur Entré (Bouton Entrée utilisateur)");
        ab.Agrandir();
    }

    public void OnPointerExit (PointerEventData EventData)
    {
        Debug.Log("Pointeur Sorti (Bouton Entrée Utilisateur)");
        ab.Retrecir();
    }
}
