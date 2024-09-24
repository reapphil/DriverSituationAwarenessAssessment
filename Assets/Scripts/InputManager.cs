using Uduino;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public delegate void OnDataReceivedDelegate(string data);

    public event OnDataReceivedDelegate OnDataReceived;


    public delegate void OnButtonPressDelegate();

    public event OnButtonPressDelegate OnButtonPress;

    private UduinoManager _uduinoManager;

    public float tempValue;
    private string tempString = ";0";

    private void Awake()
    {
        _uduinoManager = FindObjectOfType<UduinoManager>();
    }

    private void OnEnable()
    {
        _uduinoManager.OnValueReceived += ProcessInput;
    }

    private void OnDisable()
    {
        _uduinoManager.OnValueReceived -= ProcessInput;
    }


    // private void Update()
    // {
    //     string pressedText = "";
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         pressedText = "Pressed";
    //         OnButtonPress?.Invoke();
    //     }
    //
    //     if (Input.GetKeyDown(KeyCode.UpArrow))
    //     {
    //         tempValue += 1f;
    //     }
    //
    //     if (Input.GetKeyDown(KeyCode.DownArrow))
    //     {
    //         tempValue -= 1f;
    //     }
    //     string newString = pressedText + ";" + Mathf.Clamp(tempValue, 0, 100);
    //     if (tempString != newString)
    //     {
    //         tempString = newString;
    //         OnDataReceived?.Invoke(tempString);
    //     }
    // }


    private void ProcessInput(string data, UduinoDevice device)
    {
        tempValue = float.Parse(data.Split(";")[1]);

        OnDataReceived?.Invoke(data);

        if (data.Contains("Pressed"))
        {
            OnButtonPress?.Invoke();
        }
    }
}