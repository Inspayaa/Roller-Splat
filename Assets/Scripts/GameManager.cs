using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;
    public ParticleSystem victoryParticle;

    private GroundPiece[] allGroundPieces;

    // Start is called before the first frame update
    void Start()
    {
       SetUpNewLevels();
       victoryParticle.Play();
    }

    private void SetUpNewLevels()
    {
        allGroundPieces = FindObjectsOfType<GroundPiece>();
    }

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        } else if (singleton != null)
        {
            Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        SetUpNewLevels();
    }

    public void CheckComplete()
    {
        bool isFinished = true;

        for (int i = 0; i < allGroundPieces.Length; i++)
        {
            if (allGroundPieces[i].isColoured == false)
            {
                isFinished = false;
                break;
            }
        }

        if (isFinished)
        {
            // call next level
            NextLevel();
        }
    }

    private void NextLevel()
    {

        // as level increases remember to update 'buildIndex == 1'
        if (SceneManager.GetActiveScene().buildIndex == 6)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            
        }
        
    }

}
