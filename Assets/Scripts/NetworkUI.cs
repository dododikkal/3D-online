using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;

public class NetworkUI : MonoBehaviour
{
    [Header("UI Referansları")]
    [SerializeField] private GameObject startScreen; // Başlangıç Ekranı (Resim)
    [SerializeField] private GameObject connectionScreen; // Host/Client Butonları
    
    [Header("Bağlantı Ayarları")]
    [SerializeField] private TMP_InputField ipInputField; 
    [SerializeField] private TMP_Text statusText;

    void Start()
    {
        if(startScreen) startScreen.SetActive(true);
        if(connectionScreen) connectionScreen.SetActive(false);
    }

    // --- EKRAN GEÇİŞİ ---
    public void OpenConnectionMenu()
    {
        if(startScreen) startScreen.SetActive(false);
        if(connectionScreen) connectionScreen.SetActive(true);
    }

    public void StartHost()
    {
        UpdateStatus("Host Başlatılıyor...");
        // Host her zaman 0.0.0.0 dinlemeli ki Hamachi'den gelen girebilsin
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData("0.0.0.0", 7777);
        
        NetworkManager.Singleton.StartHost();
        CloseAllUI();
    }

    public void StartClient()
    {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        
        // Varsayılan IP (Eğer kutu boşsa kendine bağlan)
        string hedefIP = "127.0.0.1"; 

        // --- DÜZELTİLEN KISIM BURASI ---
        if (ipInputField != null)
        {
            // Sadece baştaki ve sondaki boşlukları siler (.Trim)
            // İçerik neyse onu direkt alır. Harf varmış yokmuş bakmaz, karışmaz.
            string girilenYazi = ipInputField.text.Trim();

            // Eğer kutu boş değilse, yazılanı IP kabul et
            if (!string.IsNullOrEmpty(girilenYazi))
            {
                hedefIP = girilenYazi;
            }
            // Kutu boşsa yukarıdaki 127.0.0.1 kalır.
        }

        // Hedef IP'yi sisteme ver
        transport.SetConnectionData(hedefIP, 7777);
        
        UpdateStatus("Bağlanılıyor: " + hedefIP); // Ekranda hangi IP'ye gittiğini gör
        
        bool basarili = NetworkManager.Singleton.StartClient();
        
        if(!basarili) UpdateStatus("Bağlantı Başlatılamadı!");

        CloseAllUI();
    }

    void CloseAllUI()
    {
        if(startScreen) startScreen.SetActive(false);
        if(connectionScreen) connectionScreen.SetActive(false);
    }

    void UpdateStatus(string mesaj)
    {
        if (statusText != null) statusText.text = mesaj;
        Debug.Log(mesaj);
    }
}