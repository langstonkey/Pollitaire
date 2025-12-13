using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


public struct FlowerSet
{
    public List<Flower> flowers;
}

public class FlowerGroup : MonoBehaviour
{
    [SerializeField] List<Flower> currentFlowers = new List<Flower>();
    [SerializeField] List<FlowerSet> flowerSets;

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


    public void CheckFlowers()
    {
        if (FlowerTypesMatch())
        {
            //delete current flowers and get the next flower set if there are any.
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

    public void PopulateFlowerSet(List<FlowerSet> set)
    {
        flowerSets = set;
    }
}
