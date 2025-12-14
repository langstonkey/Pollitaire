using NaughtyAttributes;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct FlowerSet
{
    public List<FlowerType> flowerTypes;
}

public class FlowerGroup : MonoBehaviour
{
    [SerializeField] List<Flower> currentFlowers = new List<Flower>();
    [SerializeField] List<FlowerSet> flowerSets;
    [SerializeField] CardGroup cardGroup;
    [SerializeField] GameObject flowerCardPrefab;

    public void RemoveFlower(Card card)
    {
        Flower flower = card.GetComponent<Flower>();
        if (flower) currentFlowers.Remove(flower);
    }
    public void AddFlower(Card card)
    {
        Flower flower = card.GetComponent<Flower>();
        if (flower) currentFlowers.Add(flower);
    }

    [Button]
    public void CheckFlowers()
    {
        if (FlowerTypesMatch())
        {
            //delete current flowers and get the next flower set if there are any.
            cardGroup.ClearGroup();

            if (flowerSets.Count > 0)
            {
                PopulateCardGroupWithFlowers(flowerSets[0].flowerTypes);
                flowerSets.RemoveAt(0);
            }
        }
    }

    public void PopulateCardGroupWithFlowers(List<FlowerType> flowerTypes)
    {
        foreach (FlowerType type in flowerTypes)
        {
            Card card = Instantiate(flowerCardPrefab, cardGroup.transform).GetComponent<Card>();
            cardGroup.AddCard(card);
        }
    }

    bool FlowerTypesMatch()
    {
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
