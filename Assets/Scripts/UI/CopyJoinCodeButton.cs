using TMPro;
using UnityEngine;

public class CopyJoinCodeButton : MonoBehaviour
{
    [SerializeField] private TMP_InputField _joinCodeInputField;

    public void Copy()
    {
        ClipboardHelper.ClipBoard = _joinCodeInputField.text;
    }
}
