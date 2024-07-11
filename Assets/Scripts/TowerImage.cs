using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

public class TowerImage : MonoBehaviour
{

    [SerializeField] private GameObject tiroirPrefab;

    private Dictionary<string, float[]> imageInfo = new Dictionary<string, float[]>();

    // Start is called before the first frame update
    void Start()
    {
        TextAsset imageInfoRaw = Resources.Load<TextAsset>("ImageInfo/ImageInfo");
        string[] text = imageInfoRaw.text.Split("\n");
        
        foreach (var line in text.Skip(1))
        {
            string[] value = line.Split(new[]{',', '(', ')', '"', ' '}, StringSplitOptions.RemoveEmptyEntries);

            string imageName = value[0];
            Texture2D texture = Resources.Load<Texture2D>(imageName.Substring(0, imageName.Length-4));
            if (texture == null)
            {
                Resources.UnloadAsset(texture);
                continue;
            }
            // name, size (Ko), average hue, width, height
            imageInfo.Add(value[0], new float[] {float.Parse(value[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse(value[2], CultureInfo.InvariantCulture.NumberFormat), float.Parse(value[7], CultureInfo.InvariantCulture.NumberFormat), float.Parse(value[8], CultureInfo.InvariantCulture.NumberFormat)});
            Debug.Log(imageInfo[value[0]]);
        }

        GameObject tiroir1 = Instantiate(tiroirPrefab, transform);

        tiroir1.GetComponentInChildren<MeshRenderer>().material.mainTexture = Resources.Load<Texture2D>("image_78");
        
        Debug.Log(imageInfo["image_78.jpg"]);
    }

}
