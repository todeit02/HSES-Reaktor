using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class MenuConfigManager : MonoBehaviour {

    //private List<GameObject> toggleObjects;

    /*public Transform subgameToggle;
    public Transform togglesPanelTransform;*/

    // Use this for initialization
    void Start () {


        /*togglesPanelTransform = togglesPanelTransform.GetComponent<Transform>();

        for (int i = 0; i < 1; i++)
        {
            string toggleName = string.Concat("toggle", i);
            Transform addingToggle = (Transform) Instantiate(subgameToggle, togglesPanelTransform.transform.position, Quaternion.identity);
            addingToggle.name = toggleName;
            addingToggle.transform.SetParent(togglesPanelTransform, false);
            //addingToggle.transform.Rotate(0,0,270);
            //toggleObjects.Add(addingToggle);
        }*/


    }

    void OnGUI()
    {

    }

    public void HandleGameStartClick()
    {
        GameManager.Instance.StartGame();
    }

    public void HandleMenuMainClick()
    {
        SceneManager.LoadScene("MenuMain");
    }
}