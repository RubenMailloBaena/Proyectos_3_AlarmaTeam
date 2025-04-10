using System.Collections.Generic;
using UnityEngine;

public class GhostCommand : ICheatCommand
{
    public string Name => "g";

    private static bool isGhostMode = false;

    public void Execute(string[] args)
    {
        GameObject player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.LogWarning("Player not found");
            return;
        }

        GameObject ghostCameraObj = FindGameObjectByName("GhostCamera");
        if (ghostCameraObj == null)
        {
            Debug.LogWarning("GhostCamera not found");
            return;
        }

        GameObject safeZone = GameObject.Find("SafeZone");
        if (safeZone == null)
        {
            Debug.LogWarning("SafeZone not found");
            return;
        }

        Camera playerCam = player.GetComponentInChildren<Camera>(true);
        Camera ghostCam = ghostCameraObj.GetComponentInChildren<Camera>(true);
        if (playerCam == null || ghostCam == null)
        {
            Debug.LogWarning("Cameras not found");
            return;
        }

        bool newGhostMode = !isGhostMode;
        if (newGhostMode)
        {
            // ACTIVAR GHOST MODE:
            // 1. Posicionar la GhostCamera en la posici�n del Player (transferir solo el yaw)
            ghostCameraObj.transform.position = player.transform.position;
            ghostCameraObj.transform.rotation = Quaternion.Euler(0, player.transform.eulerAngles.y, 0);
            ghostCameraObj.SetActive(true);

            // 2. Mover el Player a la SafeZone para que no le afecten cosas mientras est� en ghost
            player.transform.position = safeZone.transform.position;
            player.transform.rotation = safeZone.transform.rotation;
        }
        else
        {
            // DESACTIVAR GHOST MODE:
            // Teletransportar al Player a la posici�n donde ha quedado la GhostCamera
            player.transform.position = ghostCameraObj.transform.position;
            player.transform.rotation = Quaternion.Euler(0, ghostCameraObj.transform.eulerAngles.y, 0);
            ghostCameraObj.SetActive(false);
        }

        AudioListener playerListener = playerCam.GetComponent<AudioListener>();
        AudioListener ghostListener = ghostCam.GetComponent<AudioListener>();
        if (playerListener != null)
            playerListener.enabled = !newGhostMode;
        if (ghostListener != null)
            ghostListener.enabled = newGhostMode;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isGhostMode = newGhostMode;
        Debug.Log(isGhostMode ? "Ghost Mode ON" : "Ghost Mode OFF");
    }

    // M�todo auxiliar para buscar objetos por nombre, incluso si est�n desactivados
    private GameObject FindGameObjectByName(string name)
    {
        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)))
        {
            if (go.name == name)
                return go;
        }
        return null;
    }
}
