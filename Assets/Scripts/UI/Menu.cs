using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class Menu : MonoBehaviour
{
    public UnityEvent OnActivated = new();
    public UnityEvent OnDeactivated = new();

    [SerializeField] private Menu _parentMenu;
    [SerializeField] private float _fadeTime = 0.5f;
    [SerializeField] private bool _isActiveByDefault = false;

    private CanvasGroup _canvasGroup;

    public bool IsActive { get; private set; }

    private Coroutine _fadeInCoroutine;
    private Coroutine _fadeOutCoroutine;

    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        if (_isActiveByDefault)
        {
            IsActive = true;

            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.alpha = 1f;
        }

        OnStart();
    }

    protected virtual void OnStart() { }

    public void Toggle()
    {
        if (IsActive)
            Deactivate();
        else
            Activate();
    }

    public void Activate()
    {
        _ = _parentMenu.Bind(menu => menu.Deactivate());

        IsActive = true;

        OnActivated.Invoke();

        if (_fadeOutCoroutine != null)
        {
            StopCoroutine(_fadeOutCoroutine);
            _fadeOutCoroutine = null;
        }

        _fadeInCoroutine = StartCoroutine(FadeIn());

        OnActivate();
    }

    protected virtual void OnActivate() { }

    public void Deactivate()
    {
        _ = _parentMenu.Bind(menu => menu.Activate());

        IsActive = false;

        OnDeactivated.Invoke();

        if (_fadeInCoroutine != null)
        {
            StopCoroutine(_fadeInCoroutine);
            _fadeInCoroutine = null;
        }

        _fadeOutCoroutine = StartCoroutine(FadeOut());

        OnDeactivate();
    }

    protected virtual void OnDeactivate() { }

    private IEnumerator FadeIn()
    {
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;

        float time = 0f;

        while (_canvasGroup.alpha < 1f)
        {
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, time / _fadeTime);

            time += Time.unscaledDeltaTime;
            yield return null;
        }

        _fadeInCoroutine = null;
    }

    private IEnumerator FadeOut()
    {
        float time = 0f;

        while (_canvasGroup.alpha > 0f)
        {
            _canvasGroup.alpha = Mathf.Lerp(1f, 0f, time / _fadeTime);

            time += Time.unscaledDeltaTime;
            yield return null;
        }

        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        _fadeOutCoroutine = null;
    }
}
