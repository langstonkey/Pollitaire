using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class FlowerCardVisual : MonoBehaviour
{
    [SerializeField] float followSpeed;
    [SerializeField] Transform followTarget;
    [SerializeField] Image flowerImage;
    GameObject particleEffect;

    Vector3 targetPosition;

    public void Start()
    {
        transform.localPosition = Vector3.zero;
    }

    IEnumerator FollowTarget()
    {
        while (true)
        {
            if (followTarget == null) break;
            targetPosition = followTarget.position;
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void SetFlowerVisual(FlowerCard flower, Transform target)
    {
        flowerImage.sprite = flower.Type.Sprite;
        followTarget = target;
        particleEffect = flower.Type.ParticleEffect;
        StartCoroutine(FollowTarget());
    }

    public void SetFlowerVisual(FlowerType flower)
    {
        flowerImage.sprite = flower.Sprite;
    }

    public void OnDestroy()
    {
        Transform effectTransform = Instantiate(particleEffect, transform.parent).transform;
        effectTransform.position = transform.position;

        Image effectImage = effectTransform.GetComponent<Image>();
        if (effectImage != null) effectImage.sprite = flowerImage.sprite;
    }
}
