using DG.Tweening;
using UnityEngine;

public class ButtonScale : MonoBehaviour
{
    [SerializeField] private float scaleDownDuration = 0.5f;

    private void Start()
    {
        AnimationUI();
    }

    public void AnimationUIZero()
    {
       gameObject.transform.DOScale(Vector3.zero, scaleDownDuration)
                     .SetEase(Ease.InBack)
                     .OnComplete(() => Destroy(gameObject));
    }

    public void AnimationUI()
    {
        gameObject.transform.DOScale(Vector3.one, scaleDownDuration)
                     .SetEase(Ease.OutBack);
    }
}
