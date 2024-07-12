using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameSystem : MonoBehaviour
{

    [SerializeField] private GameObject _gameInterface;
    [SerializeField] private GameObject _pauseInterface;

    public void TogglePauseUI(InputAction.CallbackContext context)
    {
        var timeScale = Time.timeScale == 1 ? Time.timeScale = 0 : Time.timeScale = 1;
        _gameInterface.SetActive(!_gameInterface.activeSelf);
        _pauseInterface.SetActive(!_pauseInterface.activeSelf);
    }

    public void OpenSourceURL()
    {
        Application.OpenURL("https://github.com/robillardstudio/laion2B-en-aesthetic-minified");
    }

    public void Quit()
    {
        Application.Quit();
    }

}
