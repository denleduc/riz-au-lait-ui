using UnityEngine;
using UnityEngine.XR;

public class InteractionJoueur : MonoBehaviour
{
    [SerializeField] public GameObject hud;
    private bool hud_displayed = true;
    bool deja_presse = false;

    // Update is called once per frame
    private void Update()
    {
        InputDevice mainDroite = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        InputDevice mainGauche = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        if (mainDroite.TryGetFeatureValue(CommonUsages.triggerButton, out bool gachettePressee)
            && gachettePressee)
        {
            Debug.Log("Gachette pressée.");
            float rayonInteraction = 2f;
            Collider[] collisions = Physics.OverlapSphere(transform.position, rayonInteraction);
            foreach (Collider col in collisions)
            {
                if (col.TryGetComponent(out PNJInteraction pnj_i))
                {
                    pnj_i.Interaction();
                }
            }
        }

        if (mainGauche.TryGetFeatureValue(CommonUsages.menuButton, out bool menuPresse))
        {
            if (menuPresse && !deja_presse)
            {
                hud.SetActive(!hud_displayed);
                hud_displayed = !hud_displayed;
            }

            deja_presse = menuPresse;
        }
    }
}
