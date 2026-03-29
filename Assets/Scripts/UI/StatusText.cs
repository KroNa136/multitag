using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class StatusText : MonoBehaviour
{
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _errorColor = Color.red;
    [SerializeField] private Color _successColor = Color.green;
    [SerializeField] private Color _disabledColor = Color.gray;
    [SerializeField] private TMP_Text _text;

    private void Start()
    {
        _text = GetComponent<TMP_Text>();
    }

    public void SetMessage(string message) => SetStatus(_normalColor, message);
    public void SetError(string message) => SetStatus(_errorColor, message);
    public void SetSuccess(string message) => SetStatus(_successColor, message);
    public void SetDisabled(string message) => SetStatus(_disabledColor, message);

    private void SetStatus(Color color, string message)
    {
        _text.color = color;
        _text.text = message;
    }
}
