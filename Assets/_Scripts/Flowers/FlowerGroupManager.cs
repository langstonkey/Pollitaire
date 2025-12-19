using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class FlowerGroupManager : MonoBehaviour
{
    public static FlowerGroupManager Instance;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    [SerializeField] List<FlowerType> flowerTypes;
    [SerializeField] List<FlowerGroup> flowerGroups;
    [SerializeField] List<FlowerType> flowerDeck;
    [field: SerializeField] public int SetSize { get; private set; }
    [SerializeField] int depth = 2;

    public UnityEvent OnGroupsComplete;

    private void Start()
    {
        GenerateFlowerDeck();
        DealFlowerDeck();
        InitFlowerGroups();
    }

    [Button]
    public void GenerateFlowerDeck()
    {
        //generate depth unique sets of setSize flower types per group
        flowerDeck.Clear();
        for (int i = 0; i < flowerGroups.Count; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                int randomIndex = Random.Range(0, flowerTypes.Count) + depth;
                int index = (int)Mathf.Repeat(randomIndex, flowerTypes.Count);
                flowerDeck.AddRange(CreateFlowerSet(SetSize, flowerTypes[index]));
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
                for (int k = 0; k < SetSize; k++)
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

    [Button]
    public void InitFlowerGroups()
    {
        foreach (FlowerGroup flowerGroup in flowerGroups)
        {
            flowerGroup.Init();
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

    public FlowerType GetRandomType()
    {
        return flowerTypes[Random.Range(0, flowerTypes.Count)];
    }

    public void CheckFlowerGroups()
    {
        if (FlowerGroupsComplete())
        {
            OnGroupsComplete?.Invoke();
        }
    }

    public bool FlowerGroupsComplete()
    {
        foreach (FlowerGroup group in flowerGroups)
        {
            if (group.gameObject.activeInHierarchy) return false;
        }

        return true;
    }
}
