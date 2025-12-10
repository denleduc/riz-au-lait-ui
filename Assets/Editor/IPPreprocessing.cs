using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Linq;

/*Ce script permet d'injecter l'adresse IP du PC au casque (ce qui permet à terme de se connecter au serveur du PC s'il est disponible depuis le casque.*/
public class IPPreprocessing : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        string ip = GetLocalIPv4() ?? "127.0.0.1";
        string code = $@"
public static class BuildConstants
{{
    public const string LocalIP = ""{ip}"";
}}";
        File.WriteAllText("Assets/Scripts/BuildConstants.cs", code);
        Debug.Log($"[BuildPreprocessor] Adresse IP injectée dans BuildConstants.cs : {ip}");
    }
    private string GetLocalIPv4()
    {
        // Parcourir toutes les interfaces réseau du système
        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            // 1. Filtrer les interfaces non opérationnelles ou non physiques
            if (ni.OperationalStatus != OperationalStatus.Up ||
                (ni.NetworkInterfaceType != NetworkInterfaceType.Wireless80211 && // Wi-Fi
                 ni.NetworkInterfaceType != NetworkInterfaceType.Ethernet))     // Ethernet
            {
                continue;
            }

            // 2. Trouver l'information IP
            IPInterfaceProperties ipProps = ni.GetIPProperties();

            // 3. Parcourir les adresses IP liées à cette interface
            foreach (UnicastIPAddressInformation ip in ipProps.UnicastAddresses)
            {
                // 4. Filtrer pour obtenir uniquement une IPv4 publique (non-boucle locale)
                if (ip.Address.AddressFamily == AddressFamily.InterNetwork &&
                    !System.Net.IPAddress.IsLoopback(ip.Address))
                {
                    // Optionnel mais recommandé : Assurez-vous qu'il y a une passerelle (Gateway)
                    if (ipProps.GatewayAddresses.Any())
                    {
                        return ip.Address.ToString();
                    }
                }
            }
        }
        return null;
    }
}
