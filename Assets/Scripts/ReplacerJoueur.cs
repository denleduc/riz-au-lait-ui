using UnityEngine;

public class ReplacerJoueur : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning("Entrée dans la collision " + other.name);
        if (other.CompareTag("Player"))
            other.transform.position = new Vector3(0f, 1f, 4f);
    }

}
