using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Point : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image image;
    [SerializeField] private Sprite inActiveSprite, activeSprite;
    [SerializeField] private TextMeshProUGUI indexText;

    /// <summary>
    /// Returns the current point state.
    /// </summary>
    public bool CurrentState { get; private set; }

    /// <summary>
    /// Returns point index.
    /// </summary>
    public int Index { get; private set; }

    /// <summary>
    /// Returns point anchored position.
    /// </summary>
    public Vector2 AnchoredPosition => image.rectTransform.anchoredPosition;

    /// <summary>
    /// Initializes point object.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="coordinates"></param>
    public Point Initialize(int index, Vector2 coordinates)
    {
        Index = index - 1;

        //Set the point index for user to see.
        if (indexText != null)
            indexText.text = index.ToString();

        //Set button on click method.
        if (button != null)
            button.onClick.AddListener(() => { ToggleState(!CurrentState); });

        //Initial state is inactive.
        ToggleState(false, true);

        //Set anchored point position.
        image.rectTransform.anchoredPosition = coordinates;

        return this;
    }

    /// <summary>
    /// Toggles the state of the point object depending on the specified bool value.
    /// </summary>
    /// <param name="active"></param>
    public void ToggleState(bool active, bool initialSet = false)
    {
        if (!initialSet && !GameController.IsNextPoint(Index))
            return;

        image.sprite = active ? activeSprite : inActiveSprite;
        indexText.gameObject.SetActive(!active);

        CurrentState = false;
    }
}