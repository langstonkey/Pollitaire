using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class FlowerGroupManager : MonoBehaviour
{
    [SerializeField] List<FlowerGroup> flowerGroups;
    [SerializeField] List<FlowerType> flowerDeck;
    [SerializeField] int setSize = 3;
    [SerializeField] int depth = 2;

    private void Start()
    {
        GenerateFlowerDeck();
        DealFlowerDeck();
    }

    [Button]
    public void GenerateFlowerDeck()
    {
        //generate depth unique sets of setSize flower types per group
        flowerDeck.Clear();
        for (int i = 0; i < flowerGroups.Count; i++)
        {
            int maxFlowerType = (int)Enum.GetValues(typeof(FlowerType)).Cast<FlowerType>().Max() + 1;
            Debug.Log(maxFlowerType);
            for (int j = 0; j < depth; j++)
            {
                flowerDeck.AddRange(CreateFlowerSet(setSize, (FlowerType)Mathf.Repeat(Random.Range(0, maxFlowerType) + depth, maxFlowerType)));
            }
        }
    }
    [Button]
    public void DealFlowerDeck()
    {
        //deal the flower deck
        for (int i = 0; i < flowerGroups.Count; i++)
        {
            //create a list of flower sets
            List<FlowerSet> flowerSets = new List<FlowerSet>();

            //create as many flower sets as there are depth
            for (int j = 0; j < depth; j++)
            {
                FlowerSet flowerSet = new FlowerSet();
                flowerSet.flowerTypes = new List<FlowerType>();

                //create as many flower types as there are set size
                for (int k = 0; k < setSize; k++)
                {
                    //select a random flower from the flower deck and add it to the set and remove it from the deck
                    int randomIndex = Random.Range(0, flowerDeck.Count);
                    FlowerType randomFlowerType = flowerDeck[randomIndex];

                    //if it already has that flower type re re-roll
                    int itterationCount = 0;
                    while (flowerSet.flowerTypes.Contains(randomFlowerType) && itterationCount < 99)
                    {
                        randomIndex = Random.Range(0, flowerDeck.Count);
                        randomFlowerType = flowerDeck[randomIndex];
                        itterationCount++;
                    }

                    flowerSet.flowerTypes.Add(randomFlowerType);
                    flowerDeck.RemoveAt(randomIndex);
                }

                //add the flower set to the list of flower sets
                flowerSets.Add(flowerSet);
            }

            //add the generated flower sets to the flower groups
            flowerGroups[i].PopulateFlowerSets(flowerSets);
        }
    }

    public List<FlowerType> CreateFlowerSet(int size, FlowerType type)
    {
        List<FlowerType> typeList = new List<FlowerType>();

        for (int i = 0; i < size; i++)
        {
            typeList.Add(type);
        }

        return typeList;
    }
}
