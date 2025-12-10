
/*
 Collez ce script sur un TMP_InputField si vous voulez le placer devant la caméra du joueur.
NOTE: Il faut le placer en tant que composant du TMP_InputField. 
REQUIS: un Canvas, un TMP_InputFIeld et un OVRCameraRig dans la scène.
 */

using TMPro;
using Unity.AppUI.UI;
using Unity.VisualScripting;
using UnityEngine;

public class PositionRotationEntreeUtilisateur : MonoBehaviour
{
    private Transform conteneur_transform;
    private Transform camera_transform;
    private GestionnaireApplication ga;
    private bool mettre_a_jour_affichage = true;

    private void OnDisable()
    {
        mettre_a_jour_affichage = true;
    }

    void Start()
    {
        conteneur_transform = GetComponent<Transform>();
        OVRCameraRig rig = FindAnyObjectByType<OVRCameraRig>();
        camera_transform = rig.transform;
        ga = FindAnyObjectByType<GestionnaireApplication>();

        PositionnerEtRotaterEntree();
    }

    // Si l'entrée est cachée on met à jour sa position pour que quand l'utilisateur la rappelle elle se replace vers lui.
    void Update()
    {
        if(ga.GetAffcherEntree() && mettre_a_jour_affichage)
        {
            PositionnerEtRotaterEntree();
            mettre_a_jour_affichage  = false;
        }
    }

    public void PositionnerEtRotaterEntree()
    {
        Debug.Log("PositionnerEtRotaterEntree() : Entrée.");
        if (transform == null || camera_transform == null)
        {
            Debug.LogError("CanvasParent ou CameraTransform non assigné !");
            return;
        }

        // Positionner le canvas devant le joueur
        Vector3 offset = camera_transform.forward * 0.40f + Vector3.up * 0.7f; // 40 cm devant, 70 cm plus haut
        Vector3 targetPosition = camera_transform.position + offset;

        Vector3 direction_joueur = camera_transform.forward;
        direction_joueur.y = 0;

        // Éviter des erreurs 
        if (direction_joueur == Vector3.zero)
            direction_joueur = Vector3.forward;
        else
            direction_joueur.Normalize();

        // Faire face à la caméra
        Quaternion targetRotation = Quaternion.LookRotation(direction_joueur, Vector3.up);

        targetRotation *= Quaternion.Euler(50, 0, 0);

        // Appliquer la position et la rotation au canvas
        conteneur_transform.transform.SetPositionAndRotation(targetPosition, targetRotation);
    }
}
