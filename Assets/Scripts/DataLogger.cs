using System.IO;
using RenderHeads.Media.AVProVideo;
using UnityEngine;

public class DataLogger : MonoBehaviour
{
    private DisplayVideo displayVideo;
    private TextDisplayer _textDisplay;

    public string loggingFilePath;
    public string pngLoggingFilePath;

    public float DangerThreshold;

    public float timeBetweenScreenshots;

    public float timeSinceLastScreenshot;
    public bool canTakeScreenshots = true;

    private float _timeStamp;
    public bool trackTime;

    private InputManager _inputManager;
    private MediaPlayer mediaPlayer;

    private string logText;
    private string pngTimeString;

    public DirectoryInfo newDirectoryInfo;

    public void Update()
    {
        if (trackTime)
        {
            _timeStamp += Time.deltaTime;
        }

        if (!canTakeScreenshots)
        {
            timeSinceLastScreenshot += Time.deltaTime;

            if (timeSinceLastScreenshot >= timeBetweenScreenshots)
            {
                canTakeScreenshots = true;
            }
        }
    }

    private void OnEnable()
    {
        _inputManager.OnDataReceived += LogData;
    }

    private void OnDisable()
    {
        _inputManager.OnDataReceived -= LogData;
    }

    private void Awake()
    {
        _inputManager = FindObjectOfType<InputManager>();
        displayVideo = FindObjectOfType<DisplayVideo>();
        _textDisplay = FindObjectOfType<TextDisplayer>();
    }

    public void LogData(string data)
    {
        if (!trackTime) return;
        var split = data.Split(";");
        if (float.Parse(split[1]) >= DangerThreshold && canTakeScreenshots)
        {
            timeSinceLastScreenshot = 0;
            canTakeScreenshots = false;
            pngTimeString = displayVideo.currentVideoPath.Split(".")[0] + "_" + _timeStamp + "_" + split[1] + "\n";
            //ScreenCapture.CaptureScreenshot(displayVideo.currentVideoPath.Split(".")[0] + "_" + _timeStamp + "_" + split[1] + ".png");
        }

        logText += _timeStamp + ";" + data + "\n";
        _textDisplay.DisplayDangerScore((float.Parse(data.Split(";")[1])).ToString());
    }

    public void StartLogging()
    {
        loggingFilePath = displayVideo.currentVideoPath.Split(".")[0] + ".csv";
        pngLoggingFilePath = displayVideo.currentVideoPath.Split(".")[0] + ".txt";

        _timeStamp = 0.0f;
        logText = "";
        trackTime = true;
        canTakeScreenshots = true;
        timeSinceLastScreenshot = 0;

        _textDisplay.DisplayDangerScore("50");
    }

    public void StopLogging()
    {
        trackTime = false;
    }


    public void FinishLogging(string newDirectory)
    {
        trackTime = false;
        _textDisplay.ClearDangerScore();
        //File.AppendAllText(pngLoggingFilePath, pngTimeString);
        File.AppendAllText(loggingFilePath, logText);

        // bool pathExists = false;
        // int counter = 0;
        string newDirectoryPath = Path.Join(Directory.GetCurrentDirectory() + "/ParticipantLogs", newDirectory);
        // do
        int counter = 0;

        do
        {
            if (counter == 0)
            {
                newDirectoryPath = Path.Join(Directory.GetCurrentDirectory() + "/ParticipantLogs", newDirectory);
            }
            else
            {
                newDirectoryPath = Path.Join(Directory.GetCurrentDirectory() + "/ParticipantLogs",
                    newDirectory + counter);
            }

            counter++;
        } while (Directory.Exists(newDirectoryPath));

        newDirectoryInfo = Directory.CreateDirectory(newDirectoryPath);

        File.Move(
            loggingFilePath, Path.Join(newDirectoryPath,
                Path.GetFileName(loggingFilePath)));
        // File.Move(
        //     pngLoggingFilePath, Path.Join(newDirectoryInfo.ToString(),
        //         Path.GetFileName(pngLoggingFilePath)));
    }
}