using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ModifyTMP : MonoBehaviour {

    TextMeshProUGUI[] textMeshPro;

    void Awake()
    {
        textMeshPro = GetComponentsInChildren<TextMeshProUGUI>();
    }

    void OnMouseOver()
    {
        for (int i = 0; i < textMeshPro.Length; i++)
            textMeshPro[i].color = new Color32(0, 0, 0, 255);
    }

    void OnMouseExit()
    {
        for (int i = 0; i < textMeshPro.Length; i++)
            textMeshPro[i].color = new Color32(255, 255, 255, 255);
    }
}
