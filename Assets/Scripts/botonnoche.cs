using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class botonnoche : MonoBehaviour
{    public GameObject confirmPopup;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
          confirmPopup.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
         if (Input.GetMouseButtonDown(0))
            confirmPopup.SetActive(true);
    }
    }

