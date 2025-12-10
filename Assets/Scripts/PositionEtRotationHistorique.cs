using System.Collections;
using UnityEngine;

public class PositionEtRotationHistorique : MonoBehaviour
{
    private RectTransform historique;
    private Transform camera_transform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        historique = GameObject.FindWithTag("HistoriqueMsg").GetComponent<RectTransform>();
        camera_transform = FindAnyObjectByType<OVRCameraRig>().centerEyeAnchor;
        StartCoroutine(InitialiserHistorique());
    }

    private IEnumerator InitialiserHistorique()
    {
        // On attend la prochaine image (càd la première image du programme) pour être sûr d'avoir la bonne hauteur.
        yield return null;
        float taille_y_historique = historique.sizeDelta.y;
        float taille_y_a_l_echelle = historique.lossyScale.y;
        float decalage_vertical_centre = (taille_y_historique / 2.0f) * taille_y_a_l_echelle;
        float distance = 0.50f;
        float decalage_droit = 0.30f;

        Vector3 offset = camera_transform.forward * distance +
                     camera_transform.up * (0.15f - decalage_vertical_centre) +
                     camera_transform.right * decalage_droit;
        Vector3 targetPosition = camera_transform.position + offset;

        Vector3 directionCamera = camera_transform.position - targetPosition;
        directionCamera.y = 0;
        Quaternion regarderCamera = Quaternion.LookRotation(-directionCamera);
        historique.transform.SetPositionAndRotation(targetPosition, regarderCamera);
    }
}
