using UnityEngine;

/*
 Collez ceci sur un canvas pour le déplacer en face de la caméra du joueur, sur une certaine position et non incliné.
 */

public class PositionEtRotationMenu : MonoBehaviour
{
    private Canvas canvas;
    private Transform camera_transform;
    private bool mettre_a_jour_affichage = true;

    private void OnDisable()
    {
        mettre_a_jour_affichage = true;
    }

    private void Start()
    {
        canvas = GetComponent<Canvas>();
        OVRCameraRig rig = FindAnyObjectByType<OVRCameraRig>();
        camera_transform = rig.transform;
        PositionnerEtRotaterMenu();
    }

    private void Update()
    {
        if(this.gameObject.activeInHierarchy && mettre_a_jour_affichage)
        {
           PositionnerEtRotaterMenu();
            mettre_a_jour_affichage = false;
        }
    }

    void PositionnerEtRotaterMenu()
    {
        if (canvas == null || camera_transform == null)
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

        targetRotation *= Quaternion.Euler(30, 0, 0);

        // Appliquer la position et la rotation au canvas
        canvas.transform.SetPositionAndRotation(targetPosition, targetRotation);
    }
}

