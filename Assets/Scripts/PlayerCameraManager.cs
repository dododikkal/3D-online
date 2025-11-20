// Bu script "RunnerPrefab" içindeki FPS kamerasının ayarlarını yönetir.
// GÜNCELLENMİŞ VERSİYON: Yedek kamera olsa bile kendi kamerasını zorla açar.
using UnityEngine;
using Unity.Netcode;

public class PlayerCameraManager : NetworkBehaviour
{
    [SerializeField]
    private Camera fpsCamera;
    
    [SerializeField]
    private AudioListener fpsListener;

    private void Awake()
    {
        // Eğer Inspector'dan sürüklemeyi unuttuysan, otomatik bul
        if (fpsCamera == null) fpsCamera = GetComponentInChildren<Camera>();
        if (fpsListener == null) fpsListener = GetComponentInChildren<AudioListener>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // GÜVENLİK: Kamera yoksa işlemi durdur
        if (fpsCamera == null)
        {
            Debug.LogError("HATA: Karakter kamerasını bulamadım!");
            return;
        }

        // 1. BAŞKASININ KAMERASINI KAPAT
        if (!IsOwner)
        {
            fpsCamera.enabled = false;
            if(fpsListener != null) fpsListener.enabled = false;
            return; 
        }

        // 2. KENDİ KAMERAMI AÇ (Lobi kamerasını bastırır)
        fpsCamera.enabled = true;
        // Derinliği artır ki diğer kameraların önüne geçsin
        fpsCamera.depth = 1; 
        if(fpsListener != null) fpsListener.enabled = true;

        // 3. GÖRÜŞ AYARI (Layer)
        string layerName = "GorunmezYol";
        int invisibleLayerIndex = LayerMask.NameToLayer(layerName);

        if (invisibleLayerIndex != -1)
        {
            if (IsHost)
            {
                // HOST -> Görmesin
                fpsCamera.cullingMask &= ~(1 << invisibleLayerIndex);
            }
            else
            {
                // CLIENT -> Görsün
                fpsCamera.cullingMask |= (1 << invisibleLayerIndex);
            }
        }
    }
}