# 🙈 Blind Walker (Görmeyen Yolcu)

**Unity 6** ve **Firebase Realtime Database** kullanılarak geliştirilmiş, iletişim odaklı 2 kişilik bir asimetrik işbirliği (co-op) oyunudur.

![Unity](https://img.shields.io/badge/Unity-2022.3%2B-black?style=for-the-badge&logo=unity)
![Firebase](https://img.shields.io/badge/Firebase-Realtime_DB-orange?style=for-the-badge&logo=firebase)

## 🎮 Oyunun Mantığı

Bu oyun, birbirini görmeyen iki oyuncunun sürekli iletişim kurarak (bağırarak) bir labirenti tamamlaması üzerine kuruludur.

* **Oyuncu 1 (Yürüteç / Walker):** Oyunu FPS (Birinci Şahıs) kamerasından görür. Ancak **yürüdüğü platformlar onun ekranında GÖRÜNMEZDİR.** Sadece boşluğu görür ve önündeki tehlikeyi bilemez. "Kör" bir şekilde yürür.
* **Oyuncu 2 (Rehber / Guide):** Oyunu tepeden (Kuş bakışı) görür. Tüm haritayı, gizli yolları ve Oyuncu 1'in o anki konumunu (kırmızı bir işaretçi ile) görebilir. Görevi, Oyuncu 1'e sesli komutlar vererek ("Düz git, şimdi dur, sola dön!") onu boşluğa düşürmeden hedefe ulaştırmaktır.

## 🚀 Kullanılan Teknolojiler

* **Oyun Motoru:** Unity 6 (6000.0.62f1)
* **Networking:** Google Firebase Realtime Database (Anlık pozisyon senkronizasyonu için)
* **Mekanik:** Unity Layers & Camera Culling Mask (Görünmezlik mekaniği için)
* **Version Control:** Unity Plastic SCM

## 🛠️ Nasıl Çalışır?

1.  **Firebase Bağlantısı:** Yürüteç hareket ettiğinde, `(x,y,z)` koordinatları anlık olarak Firebase veritabanına JSON formatında yazılır.
2.  **Veri Okuma:** Rehber'in oyunu, veritabanındaki bu değişikliği dinler (`ValueChanged`) ve Rehber'in ekranındaki kırmızı işaretçiyi anında günceller.
3.  **Görünmezlik:** Yürüteç'in kamerası `Culling Mask` ayarı sayesinde "Platform" katmanını render etmezken, Rehber'in kamerası her şeyi render eder.

## 📸 Ekran Görüntüleri

*(Buraya oyununuzdan alacağınız 1-2 ekran görüntüsünü eklerseniz harika olur)*

---

## 🇬🇧 English Description

**Blind Walker** is a 2-player asymmetric co-op game where trust and communication are key. Built with Unity 6 and Firebase.

### Gameplay
* **The Walker:** Plays in First-Person view but **cannot see the floor**. The platforms are invisible to them via Camera Culling masks. They must trust the Guide.
* **The Guide:** Plays in Top-Down view. They can see the entire map, the safe paths, and the Walker's position. They must verbally guide the Walker to the goal.

### Tech Stack
* Unity 6
* Firebase Realtime Database (for position syncing)
* C# Scripting
