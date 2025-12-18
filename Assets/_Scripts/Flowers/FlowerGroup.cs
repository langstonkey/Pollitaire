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
    public void AddFlower(Card card)
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

            StartCoroutine(PopulateNextFlowerSet());
        }
    }

    IEnumerator PopulateNextFlowerSet()
    {
        yield return null;

        if (flowerSets.Count > 0)
        {
            StartCoroutine(PopulateCardGroupWithFlowers(flowerSets[0].flowerTypes.ToList()));
            flowerSets.RemoveAt(0);
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
            cardGroup.AddCard(card);
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
