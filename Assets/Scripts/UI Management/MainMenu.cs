using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI titleObj;

    [SerializeField]
    Vector2 startPos;


    void Awake()
    {
        DOTween.Init();
    }

    void Start()
    {
        //animate text down into position
        titleObj.rectTransform.DOAnchorPos(startPos, 1).From();

    }
    public void TransitionToScene()
    {
        SceneManager.LoadSceneAsync(1);
    }
}
