using TMPro;
using UnityEngine;

public class PasteJoinCodeButton : MonoBehaviour
{
    [SerializeField] private TMP_InputField _joinCodeInputField;

    public void Paste()
    {
        _joinCodeInputField.text = ClipboardHelper.ClipBoard;
    }
}
