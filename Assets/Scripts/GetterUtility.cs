using UnityEngine;

public static class GetterUtility {

    public static Player GetPlayer() {
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        if (player != null) {
            return player;
        } else {
            Debug.LogError("Player not found.");
            return null;
        }
    }

    public static UIManager GetUiManager() {
        UIManager uiManager = GameObject.Find("UI Manager").GetComponent<UIManager>();

        if (uiManager != null) {
            return uiManager;
        } else {
            Debug.LogError("UI Manager not found");
            return null;
        }
    }

    public static LightManager GetLightManager() {
        LightManager lightManager = GameObject.Find("Light").GetComponent<LightManager>();

        if (lightManager != null) {
            return lightManager;
        } else {
            Debug.LogError("Light Manager not found");
            return null;
        }
    }

    public static CameraManager GetCameraManager() {
        CameraManager cameraManager = GameObject.Find("Main Camera").GetComponent<CameraManager>();
        if (cameraManager != null) {
            return cameraManager;
        } else {
            return null;
        }
    }

    public static Thunder GetThunder() {
        Thunder thunder = GameObject.Find("Thunder").GetComponent<Thunder>();

        if (thunder != null) {
            return thunder;
        } else {
            return null;
        }
    }

    public static BGMManager GetBgmManager() {
        BGMManager bgmManager = GameObject.Find("BGM Manager").GetComponent<BGMManager>();
        
        if (bgmManager != null) {
            return bgmManager;
        } else {
            return null;
        }
    }
}
