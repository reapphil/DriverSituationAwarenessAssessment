using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurveyStuff : MonoBehaviour
{
    TextDisplayer _textDisplayer;
    GameManager _gameManager;
    InputManager _inputManager;

    private void Awake()
    {
        _textDisplayer = FindObjectOfType<TextDisplayer>();
        _gameManager = FindObjectOfType<GameManager>();
        _inputManager = FindObjectOfType<InputManager>();
    }

    public void OnEnable()
    {
        _gameManager.OnStateChange += OnStateChangeHandler;
        _inputManager.OnButtonPress += OnButtonPressHandler;
    }

    private void OnButtonPressHandler()
    {
        if (_gameManager.CurrentState == States.SurveyStuff)
        {
            StartCoroutine(CloseApplicationInNSecs(3));
            _textDisplayer.DisplayText("Thank you for participating!\nHave a great day!");
        }
    }

    private IEnumerator CloseApplicationInNSecs(int i)
    {
        yield return new WaitForSeconds(i);
        Application.Quit();
    }

    public void OnDisable()
    {
        _gameManager.OnStateChange -= OnStateChangeHandler;
        _inputManager.OnButtonPress -= OnButtonPressHandler;
    }


    private void OnStateChangeHandler(States newval)
    {
        if (newval == States.SurveyStuff)
        {
            FindObjectOfType<DataLogger>().enabled = false;
            _textDisplayer.ClearDangerScore();
            StartSurveys();
        }
    }

    public void StartSurveys()
    {
        _textDisplayer.DisplayText("Press the button when you are\ndone with the surveys");
    }
}