using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour
{
    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject PrepairedMenu;
    [SerializeField] private GameObject CustomMenu;

    [SerializeField] private Text RandomSeedButtonText;

    public int seed = 3232;
    private void Start()
    {
        SetRandomSeed();
        BackToMainMenu();
    }

    public void BackToMainMenu ()
    {
        PrepairedMenu.SetActive(false);
        CustomMenu.SetActive(false);
        MainMenu.SetActive(true);
    }

    public void EnablePrepairedMenu ()
    {
        PrepairedMenu.SetActive(true);
        CustomMenu.SetActive(false);
        MainMenu.SetActive(false);
    }

    public void EnableCustomMenu ()
    {
        PrepairedMenu.SetActive(false);
        CustomMenu.SetActive(true);
        MainMenu.SetActive(false);
    }

    public void StartExamplePuzzle()
    {
        PuzzleDataManager.instance.LoadExampleData();
        LoadingManager.instance.LoadGameScene();
    }

    public void StartPremadePuzzle()
    {
        PuzzleDataManager.instance.LoadPremadeData();
        LoadingManager.instance.LoadGameScene();
    }

    public void StartRandomPuzzle()
    {
        PuzzleDataManager.instance.LoadRandomData(seed);
        LoadingManager.instance.LoadGameScene();
    }

    public void SetRandomSeed ()
    {
        seed = (int) Random.Range(0, 10000);
        RandomSeedButtonText.text = "Random Seed: \n" + seed;
    }

    public void Eixt ()
    {
        Application.Quit();
    }
}
