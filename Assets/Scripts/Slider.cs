using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Slider : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _tmPro;

    public void SetText(float val)
    {
        _tmPro.text = val.ToString("0.00");
    }
}
