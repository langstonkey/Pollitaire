using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class FlowerCardVisual : MonoBehaviour
{
    [SerializeField] float followSpeed;
    [SerializeField] Transform followTarget;
    [SerializeField] Image flowerImage;

    private void Update()
    {
        if (followTarget == null) return;
        transform.position = Vector3.Lerp(transform.position, followTarget.position, followSpeed * Time.deltaTime);
    }

    public void SetFlowerVisual(FlowerCard flower, Transform target)
    {
        flowerImage.sprite = flower.Type.Sprite;
        followTarget = target;
    }
}
