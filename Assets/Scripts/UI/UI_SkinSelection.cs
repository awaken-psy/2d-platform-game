using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct Skin
{
    public string name;
    public int price;
    public bool unlocked;
}

public class UI_SkinSelection : MonoBehaviour
{
    [SerializeField] private UI_LevelSelection levelSelection;
    [SerializeField] private UI_MainMenu mainMenu;

    [SerializeField] private Skin[] skins;

    [SerializeField] private int skinIndex;
    [SerializeField] private int maxIndex;
    [SerializeField] private Animator skinDisplay;

    [SerializeField] private TextMeshProUGUI buySelectText;
    [SerializeField] private GameObject Fruitprice;
    [SerializeField] private GameObject Fruitbank;
    private TextMeshProUGUI priceText;
    private TextMeshProUGUI bankText;

    private void Start() {
        priceText = Fruitprice.GetComponentInChildren<TextMeshProUGUI>(true);
        bankText = Fruitbank.GetComponentInChildren<TextMeshProUGUI>(true);
        LoadSkinUnlocks();
        UpdateSkinDisplay();
    }

    private void LoadSkinUnlocks() {
        skins[0].unlocked = true;
        for (int i = 0; i < skins.Length; i++) {
            string skinName = skins[i].name;
            skins[i].unlocked = PlayerPrefs.GetInt(skinName + "Unlocked", 0) == 1;
        }
    }

    public void SelectSkin() {
        if (skins[skinIndex].unlocked) {
            SkinManager.instance.setSkinId(skinIndex);
            mainMenu.SwitchToUI(levelSelection.gameObject);
        }
        else {
            BuySkin(skinIndex);
        }

        UpdateSkinDisplay();
    }

    public void NextSkin() {
        skinIndex++;
        if (skinIndex > maxIndex) {
            skinIndex = 0;
        }
        UpdateSkinDisplay();
    }

    public void PreviousSkin() {
        skinIndex--;
        if (skinIndex < 0) {
            skinIndex = maxIndex;
        }
        UpdateSkinDisplay();
    }

    private void UpdateSkinDisplay() {
        for (int i = 0; i <= maxIndex; i++) {
            skinDisplay.SetLayerWeight(i, i == skinIndex ? 1f : 0f);
        }


        if (skins[skinIndex].unlocked) {
            buySelectText.text = "Select";
            Fruitbank.SetActive(false);
            Fruitprice.SetActive(false);
        }
        else {
            buySelectText.text = "Buy";
            Fruitbank.SetActive(true);
            Fruitprice.SetActive(true);
            priceText.text = "Price: " + skins[skinIndex].price;
            bankText.text = "Bank: " + fruitsInBank();
        }

    }

    private void BuySkin(int index) {
        if (!HaveEnoughFruits()) {
            return;
        }

        string skinName = skins[index].name;
        skins[index].unlocked = true;
        PlayerPrefs.SetInt(skinName + "Unlocked", 1);
    }

    private bool HaveEnoughFruits() {
        if (fruitsInBank() >= skins[skinIndex].price) {
            PlayerPrefs.SetInt("TotalFruitsAmount", fruitsInBank() - skins[skinIndex].price);
            return true;
        }
        else {
            return false;
        }
    }

    private int fruitsInBank() => PlayerPrefs.GetInt("TotalFruitsAmount", 0);
}


