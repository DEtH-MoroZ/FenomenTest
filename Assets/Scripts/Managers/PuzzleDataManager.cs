using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleDataManager : MonoBehaviour
{
    public static PuzzleDataManager instance;

    public PuzzleData currentPuzzleData;

    private void Awake()
    {
        instance = this;
    }

    public void LoadExampleData()
    {
        currentPuzzleData = new PuzzleData("PremadePuzzleData/PuzzleExample");
    }

    public void LoadPremadeData()
    {
        currentPuzzleData = new PuzzleData("PremadePuzzleData/PuzzlePremade");
    }

    public void LoadRandomData (int seed)
    {
        currentPuzzleData = new PuzzleData(seed);
    }
}
