using TMPro;
using UnityEngine;

public class TextDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textOnScreen;
    [SerializeField] private TextMeshProUGUI dangerScore;
    
    
    public void DisplayText(string text)
    {
        textOnScreen.text = text;
    }
    
    public void DisplayDangerScore(string text)
    {
        dangerScore.text = text;
    }
    
    
    public void ClearText()
    {
        textOnScreen.text = "";
    }
    
    public void ClearDangerScore()
    {
        dangerScore.text = "";
    }
    
    
    public void SetColorDangerScore(Color color)
    {
        dangerScore.color = color;
    }

    public void DisableTexts()
    {
        textOnScreen.enabled = false;
        dangerScore.enabled = false;
    }

    public void EnableTexts()
    {
        textOnScreen.enabled = false;
        dangerScore.enabled = false;
    }
}