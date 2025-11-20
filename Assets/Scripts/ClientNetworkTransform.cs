using UnityEngine;
using Unity.Netcode.Components;

// Bu script, standart NetworkTransform'un "Sadece Sunucu Yönetir" kuralını bozar.
// Client'ın kendi karakterini hareket ettirmesine izin verir.
public class ClientNetworkTransform : NetworkTransform
{
    // "Patron Sunucu mu?" sorusuna "HAYIR" cevabı veriyoruz.
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}