using System;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string participantName;

    public TMP_InputField inputField;

    public delegate void OnStatesChangeDelegate(States newVal);

    public event OnStatesChangeDelegate OnStateChange;

    [SerializeField] private States _currentState = States.Idle;

    private InputManager _inputManager;
    private TextDisplayer _textDisplayer;

    public States CurrentState
    {
        get => _currentState;
        set
        {
            _currentState = value;
            OnStateChange?.Invoke(_currentState);
        }
    }

    private void Awake()
    {
        _inputManager = FindObjectOfType<InputManager>();
        _textDisplayer = FindObjectOfType<TextDisplayer>();
    }

    private void Start()
    {
        CurrentState = States.Idle;
        
        inputField.Select();
    }

    private void OnEnable()
    {
        OnStateChange += OnStateChangeHandler;
        _inputManager.OnButtonPress += ButtonPressed;
    }
    

    private void ButtonPressed()
    {
        if (CurrentState != States.Idle || inputField.text.Length <= 0) return;
        participantName = inputField.text;
        inputField.gameObject.SetActive(false);
        CurrentState = States.DisplayAndLog;
    }

    private void OnDisable()
    {
        OnStateChange -= OnStateChangeHandler;
        _inputManager.OnButtonPress -= ButtonPressed;
    }

    private void OnStateChangeHandler(States newval)
    {
        if (newval == States.Idle)
        {
            _textDisplayer.DisplayText("Enter your name then\npress the button to start the study");
        }
    }
}