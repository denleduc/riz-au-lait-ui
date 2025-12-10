using UnityEngine;

/* Collez ce script au canvas de la fenêtre des préférences pour la placer correctement dans l'espace.*/
public class PositionEtRotationFenetrePreferences : MonoBehaviour
{
    private Canvas canvas;
    private Transform camera_transform;
    private CanvasGroup prefab;

    private void Start()
    {
        prefab = GameObject.FindWithTag("FenetrePrefs").GetComponent<CanvasGroup>();
        canvas = GetComponent<Canvas>();
        OVRCameraRig rig = FindAnyObjectByType<OVRCameraRig>();
        camera_transform = rig.transform;

        PositionnerEtRotaterFenetre();
    }

    private void Update()
    {
        // Si c pas visible on repositionne
        if (prefab.alpha == 0f)
        {
            PositionnerEtRotaterFenetre();
        }

    }

    public void PositionnerEtRotaterFenetre()
    {
        Debug.Log("PositionnerEtRotaterFenetre() : Entrée.");
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
