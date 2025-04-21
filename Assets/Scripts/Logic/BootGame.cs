using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootGame : MonoBehaviour
{
    [SerializeField] private SceneField logicPersistance;
    [SerializeField] private SceneField MainMenu;

    private void Awake()
    {
        SceneManager.LoadScene(logicPersistance, LoadSceneMode.Additive);
        SceneManager.LoadScene(MainMenu, LoadSceneMode.Additive);
    }
}
