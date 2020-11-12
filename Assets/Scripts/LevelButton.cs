using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Button button;

    /// <summary>
    /// Initializes level button.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="onClick"></param>
    public void Initialize(int index, UnityAction onClick)
    {
        if (buttonText != null)
            buttonText.text = $"Level {index}";

        if (button != null)
            button.onClick.AddListener(onClick);
    }
}