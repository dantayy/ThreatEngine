using System.Collections;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public void TransitionToScene()
    {
        SceneManager.LoadSceneAsync(1);
    }
}
