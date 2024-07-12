using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiroir : MonoBehaviour
{

    [SerializeField] private MeshRenderer _imageMeshRenderer;
    [SerializeField] private MeshRenderer _cubeMeshRenderer;

    private Vector3 _originPosition = Vector3.zero;

    private string _filename;
    private string _imageName;
    private float _filesize;
    private float _averageHue;
    private float _width;
    private float _height;

    private float _tiroirHeight;
    private float _dimensionScale;

    public void Initialize(string imageName, float[] imageValues, float tiroirHeight, float dimensionScale)
    {
        _originPosition = transform.position;
        
        // Values : size (Ko), average hue, width, height
        _filename = imageName;
        _imageName = imageName[..^4];
        _filesize = imageValues[0];
        _averageHue = imageValues[1];
        _width = imageValues[2];
        _height = imageValues[3];

        _tiroirHeight = tiroirHeight;
        _dimensionScale = dimensionScale;
        
        transform.name = imageName[..^4];
        SetScale();
        SetTexture();
        
    }

    private void SetScale()
    {
        // On change le scale du tiroir pour qu'il s'accorde au dimension de l'image
        Transform t = transform;
        float xScale = _width * _dimensionScale;
        float yScale = _tiroirHeight;
        float zScale = _height * _dimensionScale;
        t.localScale = new Vector3(xScale, yScale, zScale);
    }

    private void SetTexture()
    {
        _cubeMeshRenderer.materials[0].color = Color.HSVToRGB(_averageHue, 1, 1);
        _imageMeshRenderer.material.mainTexture = Resources.Load<Texture2D>(_imageName); // [..^4] permet de retirer l'extension ".jpg"
    }

    public void LoadTexture()
    {
        _imageMeshRenderer.gameObject.SetActive(true);
    }

    public void UnloadTexture()
    {
        _imageMeshRenderer.gameObject.SetActive(false);
    }

    public void Resize(float dimensionScale, bool imageAlign)
    {
        _dimensionScale = dimensionScale;
        if (imageAlign)
        {
            Vector3 position = transform.position;
            position.z = - _height * _dimensionScale / 2;
            _originPosition.z = - _height * _dimensionScale / 2;
            transform.position = position;
        }
        SetScale();
    }
    
    private Vector3 _screenPoint;
    private Vector3 _offset;
    private bool dragging = false;
    
    private void OnMouseDown()
    {
        _screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        _offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,_screenPoint.z, Input.mousePosition.y ));
    }

    private void OnMouseDrag()
    {
        dragging = true;
        _cubeMeshRenderer.materials[0].color = Color.green;
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x,_screenPoint.z, Input.mousePosition.y );
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + _offset;
        float x = transform.position.x;
        float y = transform.position.y;
        float z = Mathf.Clamp(curPosition.z, -(_height * _dimensionScale), (_height * _dimensionScale));
        Vector3 newPosition = new Vector3(x, y, z);
        transform.position = newPosition;
    }

    private void OnMouseUp()
    {
        _cubeMeshRenderer.materials[0].color = Color.HSVToRGB(_averageHue, 1, 1);
        dragging = false;
        // Reviens sur sa position d'origine lentement
        StartCoroutine(ReturnToOriginalPosition());
    }

    private void OnMouseEnter()
    {
        _cubeMeshRenderer.materials[0].color = Color.red;
    }
    
    private void OnMouseExit()
    {
        _cubeMeshRenderer.materials[0].color = Color.HSVToRGB(_averageHue, 1, 1);
    }

    private IEnumerator ReturnToOriginalPosition()
    {
        while (transform.position != _originPosition && dragging == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, _originPosition, _width * _dimensionScale / 100);
            yield return null;
        }
    }
}
