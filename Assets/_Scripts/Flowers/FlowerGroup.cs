using NaughtyAttributes;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public struct FlowerSet
{
    public List<FlowerType> flowerTypes;
}

public class FlowerGroup : MonoBehaviour
{
    [SerializeField] List<FlowerCard> currentFlowers = new List<FlowerCard>();
    [SerializeField] List<FlowerSet> flowerSets;
    [SerializeField] CardGroup cardGroup;
    [SerializeField] GameObject flowerCardPrefab;
    [SerializeField] GameObject flowerPreviewPrefab;
    [SerializeField] RectTransform previewRoot;

    [Header("Visuals")]
    [SerializeField] float flowerDelay = 0.5f;

    public void Init()
    {
        StartCoroutine(PopulateNextFlowerSet());
    }

    public void RemoveFlower(Card card)
    {
        FlowerCard flower = card.GetComponent<FlowerCard>();
        if (flower) currentFlowers.Remove(flower);
    }
    public void AddFlower(Card card, bool autoAdded)
    {
        FlowerCard flower = card.GetComponent<FlowerCard>();
        if (flower) currentFlowers.Add(flower);
    }

    [Button]
    public void CheckFlowers()
    {
        if (FlowerTypesMatch())
        {
            //delete current flowers and get the next flower set if there are any.
            cardGroup.ClearGroup();
            currentFlowers.Clear();

            if (flowerSets.Count > 0)
            {
                StartCoroutine(PopulateNextFlowerSet());
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    IEnumerator PopulateNextFlowerSet()
    {
        yield return null;

        //clear preview if there is one
        StartCoroutine(ClearPreview());

        StartCoroutine(PopulateCardGroupWithFlowers(flowerSets[0].flowerTypes.ToList()));
        flowerSets.RemoveAt(0);

        //if there are still flower sets remaining after this one gets populated
        if (flowerSets.Count > 0)
        {
            yield return new WaitForSeconds(flowerDelay * FlowerGroupManager.Instance.SetSize);
            StartCoroutine(PopulatePreviewWithFlowers(flowerSets[0].flowerTypes.ToList()));
        }
    }

    IEnumerator PopulateCardGroupWithFlowers(List<FlowerType> flowerTypes)
    {
        foreach (FlowerType type in flowerTypes)
        {
            yield return new WaitForSeconds(flowerDelay);
            Card card = Instantiate(flowerCardPrefab, cardGroup.transform).GetComponentInChildren<Card>();
            FlowerCard flowerCard = card.GetComponent<FlowerCard>();
            flowerCard.SetFlowerType(type);
            cardGroup.AddCard(card, true);
        }
    }

    IEnumerator PopulatePreviewWithFlowers(List<FlowerType> flowerTypes)
    {
        foreach (FlowerType type in flowerTypes)
        {
            yield return new WaitForSeconds(flowerDelay);
            FlowerCardVisual visual = Instantiate(flowerPreviewPrefab, previewRoot).GetComponentInChildren<FlowerCardVisual>();
            visual.SetFlowerVisual(type);
        }
    }

    IEnumerator ClearPreview()
    {
        List<GameObject> children = new List<GameObject>();

        //populate seperate list of children
        for (int i  = 0; i < previewRoot.childCount; i++)
        {
            children.Add(previewRoot.GetChild(i).gameObject);
        }

        //put it into an array to destroy
        GameObject[] childrenToDestroy = children.ToArray();
        for (int i = 0; i < childrenToDestroy.Length; i++)
        {
            yield return new WaitForSeconds(flowerDelay);
            Destroy(childrenToDestroy[i]);
        }
    }

    bool FlowerTypesMatch()
    {
        //ignore if its not a full set;
        if (currentFlowers.Count < FlowerGroupManager.Instance.SetSize) return false;

        FlowerType type = currentFlowers[0].Type;
        for (int i = 1; i < currentFlowers.Count; i++)
        {
            if (currentFlowers[i].Type != type) return false;
        }

        return true;
    }

    public void PopulateFlowerSets(List<FlowerSet> set)
    {
        flowerSets = set;
    }
}
