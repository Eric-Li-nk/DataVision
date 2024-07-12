using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using yutokun;
using Object = UnityEngine.Object;

public class TowerImage : MonoBehaviour
{

    [SerializeField] private GameObject tiroirPrefab;
    [SerializeField] private TextAsset imageInfoRaw;

    [SerializeField] private float _maxHeight = 40.0f;
    [SerializeField] private float _minHeight = 0.01f;
    [SerializeField] private float _dimensionScale = 0.1f;

    [SerializeField] private UnityEngine.UI.Slider _sliderDimensionScale;

    private Dictionary<string, float[]> imageInfoList = new Dictionary<string, float[]>();

    private float _tiroirHeightPosition = 0.0f;
    private float _maxImageSize = 0.0f;
    private bool _imageAlign = false;
    private bool _showTexture = false;

    // Start is called before the first frame update
    private void Start()
    {
        _sliderDimensionScale.value = _dimensionScale;
        LoadImageInfoList();
        // On récupère la plus grande taille en Ko des images
        _maxImageSize = imageInfoList.Max(pair => pair.Value[0]);
        ImageInfoListOrderByName();
        GenerateTower();
    }

    private void GenerateTower()
    {
        // Réinitialise 
        ClearTower();
        _tiroirHeightPosition = 0.0f;
        
        // On récupère le nom des images dans une liste
        List<string> imageNameList = imageInfoList.Keys.ToList();

        // Iteration de tout les images
        for (int i = 0; i < imageInfoList.Count; i++)
        {
            // On récupère les info de l'image
            string imageName = imageNameList[i];
            float[] imageValues = imageInfoList[imageName];
            
            // On calcule la hauteur du tiroir
            float tiroirHeight = ComputeTiroirHeight(imageValues[0]);
            _tiroirHeightPosition += tiroirHeight / 2.0f;
            
            // Création du tiroir
            Vector3 position = new Vector3(0, _tiroirHeightPosition, 0);
            if (_imageAlign)
                position.z = - imageValues[3] * _dimensionScale / 2;
            GameObject tiroir = Instantiate(tiroirPrefab, position, Quaternion.identity, transform);
            Tiroir tiroirScript = tiroir.GetComponent<Tiroir>();
            tiroirScript.Initialize(imageName, imageValues, tiroirHeight, _dimensionScale);
            
            // Chargement du texture
            if(_showTexture)
                tiroirScript.LoadTexture();
            else
                tiroirScript.UnloadTexture();
            
            _tiroirHeightPosition += tiroirHeight / 2.0f;
        }
    }

    private void ClearTower()
    {
        foreach (Transform tiroir in transform)
            Destroy(tiroir.gameObject);
    }
    
    // Initialise le dictionaire contenant la liste des images disponibles dans ressources
    private void LoadImageInfoList()
    {
        var imageInfo = CSVParser.LoadFromString(imageInfoRaw.text);
        //Texture2D texture = Resources.Load<Texture2D>(name.Substring(0, name.Length-4));

        foreach (var line in imageInfo.Skip(1))
        {
            // Original : name,size (Ko),average hue,average value,author,date taken,location,Dimension
            // New list : name, size (Ko), average hue, width, height
            string imageName = line[0];

            // Don't add the imageinfo if unity doesn't have it in Resources
            Texture2D texture = Resources.Load<Texture2D>(imageName[..^4]);
            if (texture == null)
            {
                Resources.UnloadAsset(texture);
                continue;
            }

            float size =
                float.Parse(line[1],
                    CultureInfo.InvariantCulture.NumberFormat); // Since we have '.' as floating digit separator
            float averageHue = float.Parse(line[2], CultureInfo.InvariantCulture.NumberFormat);

            string[] dimensions = line[7].Split(new[] { ',', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
            int width = int.Parse(dimensions[0]);
            int height = int.Parse(dimensions[1]);

            imageInfoList.Add(imageName, new[] { size, averageHue, width, height });
        }
    }

    private float ComputeTiroirHeight(float imageSize)
    {
        // Produit en croix
        return imageSize / _maxImageSize * (_maxHeight - _minHeight) + _minHeight;
    }

    public void OrderSettings(int val)
    {
        switch (val)
        {
            case 0:
                ImageInfoListOrderByName();
                break;
            case 1:
                ImageInfoListOrderByAreaSize();
                break;
            case 2:
                ImageInfoListOrderByAverageHue();
                break;
        }
        GenerateTower();
    }

    public void SetImageAlign(bool val)
    {
        _imageAlign = val;
        GenerateTower();
    }

    public void SetImageTexture(bool val)
    {
        _showTexture = val;
        if (val)
        {
            foreach (Transform tiroir in transform)
            {
                Tiroir tiroirScript = tiroir.GetComponent<Tiroir>();
                tiroirScript.LoadTexture();
            }
        }
        else
        {
            foreach (Transform tiroir in transform)
            {
                Tiroir tiroirScript = tiroir.GetComponent<Tiroir>();
                tiroirScript.UnloadTexture();
            }
        }
    }

    public void SetDimensionScale(float val)
    {
        _dimensionScale = val;
        foreach (Transform tiroir in transform)
        {
            Tiroir tiroirScript = tiroir.GetComponent<Tiroir>();
            tiroirScript.Resize(val, _imageAlign);
        }
    }

    private void ImageInfoListOrderByName()
    {
        // Triage des images dans l'ordre croissant du nom des images
        imageInfoList = imageInfoList.OrderBy(pair => pair.Key.Length).OrderBy(pair => pair.Key).ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    private void ImageInfoListOrderByAreaSize()
    {
        imageInfoList = imageInfoList.OrderByDescending(pair => pair.Value[2] * pair.Value[3]).ToDictionary(pair => pair.Key, pair => pair.Value);
    }
    
    private void ImageInfoListOrderByAverageHue()
    {
        imageInfoList = imageInfoList.OrderBy(pair => pair.Value[1]).ToDictionary(pair => pair.Key, pair => pair.Value);
    }
    
}
