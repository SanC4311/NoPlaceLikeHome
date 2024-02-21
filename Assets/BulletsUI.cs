using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletsUI : MonoBehaviour
{
    public PlayerChar playerChar;
    public GameObject bulletUIPrefab; // Reference to the bullet UI prefab
    public HorizontalLayoutGroup bulletsContainer; // Parent container for bullet UIs
    public GameObject[] bulletUIPool; // Pool of bullet UI elements
    public int maxAmmo = 20; // Set this to your player's maximum ammo capacity

    private void Start()
    {
        InitializeBulletPool();
        playerChar.OnAmmoChanged += Bullets_OnAmmoChanged;
    }

    private void InitializeBulletPool()
    {
        bulletUIPool = new GameObject[maxAmmo];
        for (int i = 0; i < maxAmmo; i++)
        {
            GameObject bulletUI = Instantiate(bulletUIPrefab, bulletsContainer.transform);
            bulletUI.SetActive(false); // Start with all bullet UIs disabled
            bulletUIPool[i] = bulletUI;
        }
    }

    private void Bullets_OnAmmoChanged(object sender, PlayerChar.OnAmmoChangedEventArgs e)
    {
        UpdateBulletUI(e.Ammo);
    }

    private void UpdateBulletUI(int bulletCount)
    {
        for (int i = 0; i < bulletUIPool.Length; i++)
        {
            bulletUIPool[i].SetActive(i < bulletCount);
        }
    }

    private void OnDestroy()
    {
        playerChar.OnAmmoChanged -= Bullets_OnAmmoChanged; // Unsubscribe to prevent memory leaks
    }
}

