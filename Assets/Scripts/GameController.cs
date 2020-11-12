using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json;
using UnityEngine;
using Path = System.IO.Path;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    private const string LevelDataDirectory = "LevelData", LevelDataFileName = "level_data";

    [SerializeField] private GameObject pointGameObject, ropeGameObject, levelButtonObject, emptyLevelErrorGameObject;
    [SerializeField] private Transform pointParent, ropeParent, levelParent;
    [SerializeField] private CanvasGroup gameCanvasGroup, menuCanvasGroup, errorCanvasGroup;

    [Header("Settings")] [SerializeField] private float ropeAnimateTime;
    [SerializeField] private float fadeAnimationTime;
    [SerializeField] private Ease ropeEaseType;

    private static readonly List<Point> Points = new List<Point>();
    private static readonly Queue<Rope> Ropes = new Queue<Rope>();

    private static int _currentPointIndex;
    private static Levels _levels;

    private void Awake() => Instance = this;

    private void Start()
    {
        //Set initial window values.
        gameCanvasGroup.alpha = 0f;
        menuCanvasGroup.alpha = 1f;
        errorCanvasGroup.alpha = 0f;

        emptyLevelErrorGameObject.SetActive(false);

        //Try to get level data.
        var levelData = Resources.Load<TextAsset>(Path.Combine(LevelDataDirectory, LevelDataFileName));

        //If level data is not found shot an error.
        if (levelData == null || string.IsNullOrEmpty(levelData.text))
        {
            Debug.Log("Level data file could not be found.");

            errorCanvasGroup.DOFade(1f, fadeAnimationTime).OnComplete(() =>
            {
                errorCanvasGroup.blocksRaycasts = true;
            });
            return;
        }

        //Save data for later use.
        _levels = JsonConvert.DeserializeObject<Levels>(levelData.text);

        PopulateLevels();
    }

    /// <summary>
    /// Populates level buttons.
    /// </summary>
    private void PopulateLevels()
    {
        for (var i = 0; i < _levels.levels.Count; i++)
        {
            var levelIndex = i;
            Instantiate(levelButtonObject, levelParent).GetComponent<LevelButton>().Initialize(i + 1, () =>
            {
                InitializeLevel(levelIndex, pointGameObject, emptyLevelErrorGameObject, pointParent, ropeParent, () =>
                {
                    //Hide menu
                    menuCanvasGroup.DOFade(0f, fadeAnimationTime);
                    menuCanvasGroup.blocksRaycasts = false;

                    //Show game.
                    gameCanvasGroup.DOFade(1f, fadeAnimationTime);
                    gameCanvasGroup.blocksRaycasts = true;
                });
            });
        }
    }

    /// <summary>
    /// Hides game window and shows menu window.
    /// </summary>
    public void FinishLevel()
    {
        gameCanvasGroup.blocksRaycasts = false;
        gameCanvasGroup.DOFade(0f, fadeAnimationTime);

        menuCanvasGroup.blocksRaycasts = true;
        menuCanvasGroup.DOFade(1f, fadeAnimationTime);
    }

    /// <summary>
    /// Initializes the specified level.
    /// </summary>
    /// <param name="levelIndex"></param>
    /// <param name="pointGameObject"></param>
    /// <param name="parent"></param>
    /// <param name="onComplete"></param>
    public static void InitializeLevel(int levelIndex, GameObject pointGameObject, GameObject emptyLevelError, Transform pointParent,
        Transform ropeParent, Action onComplete = null)
    {
        if (pointGameObject == null)
        {
            Debug.Log("Point game object is not set, ignoring request.");
            return;
        }

        //Remove all children for fresh start.
        RemoveChildren(pointParent);
        RemoveChildren(ropeParent);

        //Clear out lists.
        Ropes.Clear();
        Points.Clear();

        _currentPointIndex = 0;

        //If empty, show error.
        if (emptyLevelError != null && _levels.levels[levelIndex].GetCoordinates().Count <= 0)
        {
            emptyLevelError.SetActive(true);
            onComplete?.Invoke();
            return;
        }

        //Initialize all specified level points.
        foreach (var coordinate in _levels.levels[levelIndex].GetCoordinates())
            Points.Add(Instantiate(pointGameObject, pointParent).GetComponent<Point>()
                .Initialize(Points.Count + 1, coordinate));
        
        //Turn off error message in case it was active before.
        if (emptyLevelError != null)
            emptyLevelError.SetActive(false);

        onComplete?.Invoke();
    }

    /// <summary>
    /// Removes parent children.
    /// </summary>
    private static void RemoveChildren(IEnumerable parent)
    {
        foreach (Transform child in parent)
            Destroy(child.gameObject);
    }

    /// <summary>
    /// Returns whether the point with a specified index is the next point.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public static bool IsNextPoint(int index)
    {
        if (!(_currentPointIndex).Equals(index)) return false;

        var rope = Instantiate(Instance.ropeGameObject, Instance.ropeParent).GetComponent<Rope>();
        rope.OnAnimationFinished += () => { ContinueQueue(index); };

        if (_currentPointIndex != 0)
        {
            //If no ropes in queue, instantly animate the rope.
            if (Ropes.Count <= 0)
                rope.Animate(Points[index - 1].AnchoredPosition,
                    Points[index].AnchoredPosition,
                    Instance.ropeAnimateTime, Instance.ropeEaseType);

            Ropes.Enqueue(rope);
        }

        _currentPointIndex++;
        return true;
    }

    /// <summary>
    /// Animates next available rope in the queue.
    /// </summary>
    /// <param name="index"></param>
    private static void ContinueQueue(int index)
    {
        Ropes.Dequeue();

        //Check if the last point has been reached.
        //If so, draw rope from the last point to the first one.
        if (index.Equals(Points.Count - 1))
        {
            var lastRope = Instantiate(Instance.ropeGameObject, Instance.ropeParent).GetComponent<Rope>().Animate(
                Points[index].AnchoredPosition,
                Points[0].AnchoredPosition,
                Instance.ropeAnimateTime, Instance.ropeEaseType);

            //Once finish animation, finish the level.
            lastRope.OnAnimationFinished += Instance.FinishLevel;
        }

        //If empty ignore to prevent unnecessary errors.
        if (Ropes.Count.Equals(0))
            return;

        Ropes.Peek().Animate(Points[index].AnchoredPosition, Points[index + 1].AnchoredPosition,
            Instance.ropeAnimateTime, Instance.ropeEaseType);
    }

    /// <summary>
    /// Exits the application.
    /// </summary>
    public void ExitGame() => Application.Quit();
}