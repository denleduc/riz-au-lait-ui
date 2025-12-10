using UnityEngine;

/*Ce script permet de placer l'historique des messages devant l'utilisateur.*/
public class ComportementHistoriqueMessages : MonoBehaviour
{
    private RectTransform historique;
    void Start()
    {
        historique = GameObject.FindWithTag("HistoriqueMsg").GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
