using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiroir : MonoBehaviour
{

    [SerializeField] private MeshRenderer _imageMeshRenderer;
    [SerializeField] private MeshRenderer _cubeMeshRenderer;

    private string _filename;
    private string _imageName;
    private float _filesize;
    private float _averageHue;
    private float _width;
    private float _height;

    public void Initialize(string imageName, float[] imageValues, float tiroirHeight)
    {
        // Values : size (Ko), average hue, width, height
        _filename = imageName;
        _imageName = imageName[..^4];
        _filesize = imageValues[0];
        _averageHue = imageValues[1];
        _width = imageValues[2];
        _height = imageValues[3];
        
        transform.name = imageName[..^4];
        SetScale(tiroirHeight);
        SetTexture();
        
    }

    private void SetScale(float tiroirHeight)
    {
        // On change le scale du tiroir pour qu'il s'accorde au dimension de l'image
        Transform t = transform;
        float xScale = _width / 100;
        float yScale = tiroirHeight;
        float zScale = _height / 100;
        t.localScale = new Vector3(xScale, yScale, zScale);
    }

    private void SetTexture()
    {
        _imageMeshRenderer.material.mainTexture = Resources.Load<Texture2D>(_imageName); // [..^4] permet de retirer l'extension ".jpg"
    }
    
    private Vector3 _screenPoint;
    private Vector3 _offset;
    private bool dragging = false;
    
    private void OnMouseDown()
    {
        _screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        _offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z));
    }

    private void OnMouseDrag()
    {
        dragging = true;
        _cubeMeshRenderer.material.color = Color.green;
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + _offset;
        float x = Mathf.Clamp(curPosition.x, -(_width / 100), (_width / 100));
        float y = transform.position.y;
        float z = transform.position.z;
        Vector3 newPosition = new Vector3(x, y, z);
        transform.position = newPosition;
    }

    private void OnMouseUp()
    {
        _cubeMeshRenderer.material.color = Color.red;
        dragging = false;
        // Reviens sur sa position d'origine lentement
        StartCoroutine(ReturnToOriginalPosition());
    }

    private void OnMouseEnter()
    {
        _cubeMeshRenderer.material.color = Color.red;
    }
    
    private void OnMouseExit()
    {
        _cubeMeshRenderer.material.color = Color.white;
    }

    private IEnumerator ReturnToOriginalPosition()
    {
        Vector3 originalPosition = new Vector3(0, transform.position.y, 0);
        while (transform.position != originalPosition && dragging == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, 0.1f);
            yield return null;
        }
    }
}
