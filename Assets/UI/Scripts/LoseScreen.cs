using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoseScreen : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;


    void Start()
    {
        restartButton.onClick.AddListener(() => {
            GameManager.Instance.RespawnPlayer();
        });
        exitButton.onClick.AddListener(() => SceneManager.LoadScene(0));
    }

    
}
