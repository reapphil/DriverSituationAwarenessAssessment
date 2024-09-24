using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RenderHeads.Media.AVProVideo;
using UnityEngine;

public class DisplayVideo : MonoBehaviour
{
    public string videoFolderName;
    public string videoFolderPath;
    private TextDisplayer _textDisplay;

    public string currentVideoPath;


    public bool finishedPlaying;
    public bool firstVideo = true;

    [SerializeField] DisplayUGUI _displayUGUI;


    private InputManager _inputManager;
    private GameManager _gameManager;
    private DataLogger _dataLogger;
    private MediaPlayer _videoPlayer;

    private List<string> videoFileBag = new();

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


    // public List<string> videoFileBag = new();


    private void Awake()
    {
        _inputManager = FindObjectOfType<InputManager>();
        _gameManager = FindObjectOfType<GameManager>();
        _dataLogger = FindObjectOfType<DataLogger>();
        _videoPlayer = FindObjectOfType<MediaPlayer>();
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

    // Start is called before the first frame update
    void Start()
    {
        videoFolderPath = Application.dataPath + "/StreamingAssets/" + videoFolderName;

        _textDisplay = FindObjectOfType<TextDisplayer>();
        List<string> videosInStreamingAssetsFolder = Directory
            .GetFiles(videoFolderPath, "*.mp4").Select(Path.GetFileName).ToList();

        videoFileBag.AddRange(videosInStreamingAssetsFolder.OrderBy(a => Guid.NewGuid()).ToList());
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

    private void OnStateChangeHandler(States newval)
    {
        if (newval == States.DisplayAndLog)
        {
            StartStudy();
        }
        else
        {
            DisableStudy();
        }
    }

    private void DisableStudy()
    {
    }

    private void OnButtonPressHandler()
    {
        if ((firstVideo || finishedPlaying) && _gameManager.CurrentState == States.DisplayAndLog)
        {
            ContinueTrigger = true;
            firstVideo = false;
        }
    }

    public void StartStudy()
    {
        StartCoroutine(WorkOnFileBag());
    }

    public void PlayNextClip()
    {
        _displayUGUI.color = Color.white;
        _videoPlayer.Play();
        _dataLogger.StartLogging();
    }

    public void SetNextClip()
    {
        finishedPlaying = false;

        Debug.Log("Setting next clip");
        currentVideoPath = videoFolderPath + "/" + videoFileBag[0];
        //_videoPlayer.clip = videoFileBag[0];
        _videoPlayer.OpenMedia(
            new MediaPath(currentVideoPath, MediaPathType.RelativeToStreamingAssetsFolder), autoPlay: false);

        videoFileBag.RemoveAt(0);
    }

    public void StudyStopped()
    {
        _dataLogger.StopLogging();
        _gameManager.CurrentState = States.ReplayVideos;
    }

    private IEnumerator DisplayName()
    {
        //_textDisplay.DisplayText("Press button to start next video");
        // yield return new WaitUntil(() => ContinueTrigger);
        _textDisplay.DisplayText("Playing in 3");
        yield return new WaitForSeconds(1);
        _textDisplay.DisplayText("Playing in 2");
        yield return new WaitForSeconds(1);
        _textDisplay.DisplayText("Playing in 1");
        yield return new WaitForSeconds(1);

        _textDisplay.ClearText();

        PlayNextClip();
    }

    private IEnumerator WorkOnFileBag()
    {
        _textDisplay.DisplayText("Video display Mode.\nPress button to start.");
        yield return new WaitUntil(() => ContinueTrigger);
        firstVideo = true;
        
        while (videoFileBag.Count > 0)
        {
            _displayUGUI.color = Color.clear;
            _textDisplay.DisplayText("Set the dial to 50 and\nPress button to start next video");
            yield return new WaitUntil(() => ContinueTrigger);

            yield return new WaitUntil(() => _inputManager.tempValue is < 52 and > 48);
            
            SetNextClip();
            StartCoroutine(DisplayName());
            yield return new WaitUntil(() => finishedPlaying);
            yield return new WaitForSeconds(0.5f);
            _displayUGUI.color = Color.clear;
            _dataLogger.FinishLogging(_gameManager.participantName);
        }

        StudyStopped();
    }
}