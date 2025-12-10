using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GestionTexte : MonoBehaviour
{
    public static class CouleursMessage
    {
        public const int accueil = 0;
        public const int contre = 1;
        public const int pour = 2;
    }
    /* @brief MessageAccueil() prend le texte reçu par le serveur pour en extraire les messages liés à l'accueil du joueur.
      @param texte_brut, une liste de chaine de caractères (qui contient donc les messages à filtrer.
      @return , une structure FIFO pour assurer la suppression des éléments après leur lecture.*/
    public static Queue<(string message, int index_couleur)> CreerQueueDepuisTexte(List<string> texte_brut, TextMeshProUGUI debug)
    {
        (string pref, int ind)? AnalyserLigne(string l)
        {
            int sep = l.IndexOf(';');

            if (sep < 0)
                return null;

            string prefixe_complet = l.Substring(0, sep).Trim();
            string prefixe = prefixe_complet.Trim('[', ']', ' ').ToLowerInvariant();
            debug.text += $"Prefixe trouvé : {prefixe}. Il y a {prefixe.Length} caractères dans le préfixe.\n";
            int index;
            switch (prefixe)
            {
                case "accueil":
                    {
                        index = CouleursMessage.accueil;
                        break;
                    }
                case "pour":
                    {
                        index = CouleursMessage.pour;
                        break;
                    }
                case "contre":
                    {
                        index = CouleursMessage.contre;
                        break;
                    }
                default:
                    return null;
            }
            return (prefixe, index);
        }
        IEnumerable<(string message, int index_couleur)> messages_filtres = texte_brut
            .Select(l => (l, l_analysee: AnalyserLigne(l.Trim())))
            .Where(m => m.l_analysee.HasValue)
            .Select(mbox =>
            {
                var ligne = mbox.l.Trim();
                var index_sep = ligne.IndexOf(';');

                string finalMsg = ligne.Substring(index_sep + 1).Trim();
                int finalIndex = mbox.l_analysee.Value.ind;

                return (message: finalMsg, index_couleur: finalIndex);
            });
        Queue<(string message, int index_couleur)> resultats = new(messages_filtres);
        return resultats;
    }
}
