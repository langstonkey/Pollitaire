using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class FlowerCardVisual : MonoBehaviour
{
    [SerializeField] float followSpeed;
    [SerializeField] Transform followTarget;
    [SerializeField] Image flowerImage;

    IEnumerator FollowTarget()
    {
        while (true)
        {
            if (followTarget == null) break;
            transform.position = Vector3.Lerp(transform.position, followTarget.position, followSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void SetFlowerVisual(FlowerCard flower, Transform target)
    {
        flowerImage.sprite = flower.Type.Sprite;
        followTarget = target;
        StartCoroutine(FollowTarget());
    }

    public void SetFlowerVisual(FlowerType flower)
    {
        flowerImage.sprite = flower.Sprite;
    }
}
