using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RenderHeads.Media.AVProVideo;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class VideoPartSelector : MonoBehaviour
{
    [SerializeField] private MediaPlayer _videoPlayer;
    [SerializeField] private float SecondsAroundTimeStamp;
    [SerializeField] private int numberOfVideosToPlay = 6;

    [SerializeField] private bool replayingVideo;

    public List<PathTime> paths;

    public PathTime currentPath;

    public float currentMediaDuration;

    private bool replayMode;

    private bool pauseVideo;
    private bool finishedPlaying;

    private List<(float, string)> _dangerScores;

    [Serializable]
    public struct PathTime
    {
        public string filePath;
        public float time;

        public PathTime(string filePath, float time)
        {
            this.filePath = filePath;
            this.time = time;
        }
    }


    private bool _continueTrigger;

    public bool ContinueTrigger
    {
        get
        {
            if (!_continueTrigger) return false;
            _continueTrigger = false;
            return true;
        }
        set => _continueTrigger = value;
    }


    private GameManager _gameManager;
    private InputManager _inputManager;
    private DataLogger _dataLogger;
    private TextDisplayer _textDisplayer;
    private DisplayVideo _displayVideo;
    private DisplayUGUI _displayUGUI;

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _inputManager = FindObjectOfType<InputManager>();
        _dataLogger = FindObjectOfType<DataLogger>();
        _textDisplayer = FindObjectOfType<TextDisplayer>();
        _videoPlayer = FindObjectOfType<MediaPlayer>();
        _displayVideo = FindObjectOfType<DisplayVideo>();
        _displayUGUI = FindObjectOfType<DisplayUGUI>();
        _videoPlayer.Events.AddListener(HandleEvent);
    }

    // This method is called whenever there is an event from the MediaPlayer
    void HandleEvent(MediaPlayer mp, MediaPlayerEvent.EventType eventType, ErrorCode code)
    {
        if (eventType == MediaPlayerEvent.EventType.FinishedPlaying)
        {
            finishedPlaying = true;
        }
        else if (eventType == MediaPlayerEvent.EventType.Started)
        {
            finishedPlaying = false;
        }
    }

    private void OnEnable()
    {
        _inputManager.OnButtonPress += OnButtonPressHandler;
        _gameManager.OnStateChange += OnStateChangeHandler;
    }

    private void OnDisable()
    {
        _inputManager.OnButtonPress -= OnButtonPressHandler;
        _gameManager.OnStateChange -= OnStateChangeHandler;
    }

    private void OnButtonPressHandler()
    {
        if (_gameManager.CurrentState == States.ReplayVideos)
        {
            // if (replayingVideo)
            // {
            //     TogglePause();
            // }
            //else
            {
                ContinueTrigger = true;
            }
        }
    }

    public void Update()
    {
        if (_gameManager.CurrentState == States.ReplayVideos && Input.GetKeyDown(KeyCode.Space) && replayingVideo)
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        if (pauseVideo)
        {
            _textDisplayer.ClearText();
            _videoPlayer.Play();
        }
        else
        {
            _textDisplayer.DisplayText("PAUSED!");
            _videoPlayer.Pause();
        }

        pauseVideo = !pauseVideo;
    }

    private void OnStateChangeHandler(States newval)
    {
        if (newval == States.ReplayVideos)
        {
            _displayUGUI.color = Color.clear;
            StartReplayingVideos();
        }
    }

    public void SetClip()
    {
        float time = currentPath.time - SecondsAroundTimeStamp;

        _videoPlayer.OpenMedia(
            new MediaPath(currentPath.filePath, MediaPathType.RelativeToStreamingAssetsFolder), autoPlay: false);

        float newTime = Mathf.Clamp(time, 0, currentMediaDuration);

        _videoPlayer.Control.SeekFast(newTime);
    }

    public void ResetTime()
    {
        float time = currentPath.time - SecondsAroundTimeStamp;

        float newTime = Mathf.Clamp(time, 0, currentMediaDuration);

        _videoPlayer.Control.SeekFast(newTime);
    }

    public void Play()
    {
        replayingVideo = true;
        StartDangerScoreDisplay();
        _videoPlayer.Play();
    }

    public void StartReplayingVideos()
    {
        if (replayMode) return;
        _displayUGUI.color = Color.clear;
        replayMode = true;

        paths = ReadFiles(_dataLogger.newDirectoryInfo.ToString());

        paths = paths.OrderBy(_ => Random.value).ToList();

        StartCoroutine(ReplayVideos());
    }

    private List<PathTime> ReadFiles(string directory)
    {
        var paths = new List<PathTime>();

        var files = Directory.GetFiles(directory, "*.csv");

        foreach (var file in files)
        {
            var lines = File.ReadAllLines(file);
            foreach (var line in lines)
            {
                var split = line.Split(';');
                if (split.Length == 3)
                {
                    if (float.TryParse(split[0], out var time))
                    {
                        if (split[1] != "")
                        {
                            paths.Add((new PathTime(
                                _displayVideo.videoFolderPath + "\\" + Path.GetFileNameWithoutExtension(file) + ".mp4",
                                time)));
                        }
                    }
                }
            }
        }

        return paths;
    }

    private IEnumerator DisplayDangerscore()
    {
        while (replayingVideo)
        {
            yield return new WaitForSeconds(0.1f);

            var time = (float)_videoPlayer.Control.GetCurrentTime();

            //find the closest danger score to the current time
            var closest = _dangerScores.OrderBy(x => Math.Abs(x.Item1 - time)).First();

            float.TryParse(closest.Item2, out float ds);

            _textDisplayer.DisplayDangerScore((ds) + "");
        }

        _textDisplayer.ClearDangerScore();
    }

    private void StartDangerScoreDisplay()
    {
        //red the time and danger score from the file
        _dangerScores = ReadDangerScores();

        StartCoroutine(DisplayDangerscore());
    }

    public List<(float, string)> ReadDangerScores()
    {
        var dangerScores = new List<(float, string)>();
        var lines = File.ReadAllLines(_dataLogger.newDirectoryInfo + "\\" +
                                      Path.GetFileNameWithoutExtension(_dataLogger.loggingFilePath) + ".csv");
        foreach (var line in lines)
        {
            var split = line.Split(";");
            if (split.Length == 3)
            {
                if (float.TryParse(split[0], out var time))
                {
                    dangerScores.Add((time, split[2]));
                }
            }
        }

        return dangerScores;
    }


    private IEnumerator ReplayVideos()
    {
        _textDisplayer.DisplayText("Situation replaying Mode.\nPress button to start.");
        yield return new WaitUntil(() => ContinueTrigger);

        int counter = 0;

        File.AppendAllText(_dataLogger.newDirectoryInfo + "\\replayedSituations.txt",
            paths.Select(path => Path.GetFileName(path.filePath) + " at " + path.time).Aggregate((c, p) => c + "\n" + p)
                .ToString());


        while (paths.Count > 0 && counter <= numberOfVideosToPlay)
        {
            counter++;

            currentPath = paths[0];
            paths.RemoveAt(0);
            _textDisplayer.DisplayText("Press button to replay next situation");
            currentMediaDuration = (float)_videoPlayer.Info.GetDuration();
            yield return new WaitForSeconds(0.5f);

            yield return new WaitUntil(() => ContinueTrigger);
            SetClip();

            _textDisplayer.ClearText();
            _displayUGUI.color = Color.white;


            while (!ContinueTrigger)
            {
                Play();
                Debug.Log("Playing: " + currentPath.filePath + " at " + currentPath.time);
                yield return new WaitUntil(() =>
                    _videoPlayer.Control.GetCurrentTime() >= Mathf.Clamp(currentPath.time + SecondsAroundTimeStamp, 0,
                        currentMediaDuration));

                _videoPlayer.Stop();
                _textDisplayer.DisplayText("Restarting Situation! Press button to continue to next.");
                ResetTime();

                yield return new WaitForSeconds(2f);

                _textDisplayer.ClearText();
            }

            _displayUGUI.color = Color.clear;


            // _videoPlayer.Stop();
            // replayingVideo = false;
        }

        _displayUGUI.color = Color.clear;
        _gameManager.CurrentState = States.SurveyStuff;
    }
}