using UnityEngine;

public class ScaleYWithCurve : MonoBehaviour
{
    public AnimationCurve curve;
    public float targetY = 1.0f;
    public float duration;

    private float timeStart;
    private Vector3 initialScale;
    private Vector3 targetScale;
    private bool inverted;

    public bool playOnAwake;

    private void Awake()
    {
        Initialize();

        if (playOnAwake)
            StartAnimation();
    }

    public void Update()
    {
        ProcessAnimation();
    }

    private void Initialize()
    {
        enabled = false;
        initialScale = transform.localScale;
        //initialScale.y = 0.0f;
        //transform.localScale = initialScale;

        targetScale = transform.localScale;
        targetScale.y = targetY;

    }

    public void StartAnimation()
    {
        timeStart = Time.timeSinceLevelLoad;
        enabled = true;
        inverted = false;
    }

    public void StartAnimationInverted()
    {
        timeStart = Time.timeSinceLevelLoad;
        enabled = true;
        inverted = true;
    }

    public void StopAnimation()
    {
        enabled = false;
    }

    private void ProcessAnimation()
    {
        float progress = (Time.timeSinceLevelLoad - timeStart) / duration;

        if (progress >= 1.0f)
        {
            progress = 1.0f;
            StopAnimation();
        }

        if (inverted)
            transform.localScale = Vector3.Lerp(targetScale, initialScale, curve.Evaluate(progress) );
        else
            transform.localScale = Vector3.Lerp(initialScale, targetScale, curve.Evaluate(progress) );

        Debug.Log(curve.Evaluate(progress));
    }

}
