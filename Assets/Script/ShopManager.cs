using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    [System.Serializable]
    public class GunShopItem
    {
        public GunData gunData;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI dmgText;
        public TextMeshProUGUI cdText;
        public TextMeshProUGUI priceText;
        public Button buyButton;
        public TextMeshProUGUI buyButtonText;
        public Image buyButtonImage;
    }

    [Header("Shop Items")]
    public GunShopItem[] items;

    [Header("UI")]
    public TextMeshProUGUI cashText;

    [Header("Colors")]
    public Color equippedColor = Color.cyan;
    public Color ownedColor    = Color.green;
    public Color canBuyColor   = Color.yellow;
    public Color noCashColor   = Color.red;

    void Start()
    {
        // Bind ปุ่มทุกตัวใน code เลย ไม่ต้องตั้งใน Inspector
        for (int i = 0; i < items.Length; i++)
        {
            int index = i; // capture ค่า i ไว้ใน closure
            items[i].buyButton.onClick.RemoveAllListeners();
            items[i].buyButton.onClick.AddListener(() => BuyGun(index));
        }

        if (GameManager.Instance == null)
            Debug.LogError("GameManager.Instance is NULL! ต้องมี GameManager ใน scene ก่อนหน้า (Menu)");

        RefreshShop();
    }

    void RefreshShop()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager is NULL ใน RefreshShop");
            return;
        }

        cashText.text = "Cash: " + GameManager.Instance.cash.ToString("0");

        for (int i = 0; i < items.Length; i++)
        {
            GunShopItem item = items[i];
            GunData gun = item.gunData;

            bool owned      = GameManager.Instance.IsOwned(i);
            bool isEquipped = GameManager.Instance.equippedGun == gun;
            bool isFree     = gun.price <= 0;
            bool canAfford  = GameManager.Instance.cash >= gun.price;

            // แสดงข้อมูลปืน
            if (item.nameText)  item.nameText.text  = gun.gunName;
            if (item.dmgText)   item.dmgText.text   = "DMG: " + gun.damage;
            if (item.cdText)    item.cdText.text     = "CD: "  + gun.cooldown;
            if (item.priceText) item.priceText.text  = isFree ? "FREE" : gun.price + "$";

            // สถานะปุ่ม
            if (isEquipped)
            {
                SetButton(item, "Equipped", equippedColor, false);
            }
            else if (owned || isFree)
            {
                SetButton(item, "Equip", ownedColor, true);
            }
            else if (canAfford)
            {
                SetButton(item, "Buy", canBuyColor, true);
            }
            else
            {
                SetButton(item, "No Cash", noCashColor, false);
            }
        }
    }

    void SetButton(GunShopItem item, string label, Color color, bool interactable)
    {
        if (item.buyButtonText)  item.buyButtonText.text     = label;
        if (item.buyButtonImage) item.buyButtonImage.color   = color;
        if (item.buyButton)      item.buyButton.interactable = interactable;
    }

    public void BuyGun(int index)
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager is NULL ใน BuyGun");
            return;
        }

        GunShopItem item = items[index];
        GunData gun = item.gunData;
        bool owned  = GameManager.Instance.IsOwned(index);
        bool isFree = gun.price <= 0;

        // ซื้อถ้ายังไม่มีและมีเงินพอ
        if (!owned && !isFree)
        {
            if (GameManager.Instance.cash >= gun.price)
            {
                GameManager.Instance.cash -= gun.price;
                GameManager.Instance.BuyGun(index);
                owned = true;
                Debug.Log($"ซื้อ {gun.gunName} สำเร็จ เงินคงเหลือ: {GameManager.Instance.cash}");
            }
            else
            {
                Debug.Log("เงินไม่พอ");
                return;
            }
        }

        // Equip
        if (owned || isFree)
        {
            GameManager.Instance.equippedGun = gun;
            Debug.Log($"Equip {gun.gunName}");
        }

        RefreshShop();
    }

    public void BackToGame()
    {
        SceneManager.LoadScene("MainGame");
    }
}