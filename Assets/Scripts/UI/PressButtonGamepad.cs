using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PressButtonGamepad : MonoBehaviour
{
    public PlayerInput _playerInput;
    public Button yourButton;
    public GameObject camaraMovible;
    public string scrpt;
    private bool iniciado = false;
    // Start is called before the first frame update
    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        Button btn = yourButton.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (iniciado == false)
        {
            if (_playerInput.actions["Start"].WasPressedThisFrame())
            {
                yourButton.onClick.Invoke();
                (camaraMovible.GetComponent(scrpt) as MonoBehaviour).enabled = true;
                iniciado = true;
            }
        }
    }
}
