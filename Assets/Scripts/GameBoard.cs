using NUnit.Framework.Internal.Builders;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameBoard : MonoBehaviour
{
    private static int boardWidth = 28;
    private static int boardHeight = 36;

    private bool didStartDeath = false;
    private bool didStartConsumed = false;

    public static int playerOneLevel = 1;
    public static int playerTwoLevel = 1;

    public int playerOnePelletsConsumed = 0;
    public int playerTwoPelletsConsumed = 0;

    public int totalPellets = 0;
    public int score = 0;
    public static int playerOneScore = 0;
    public static int playerTwoScore = 0;
    public int pacManLives = 3;

    public int totalKey = 5;
    public int playerOneKeyConsumed = 0;

    public static bool isPlayerOneUp = true;
    public bool shouldBlink = false;

    public float blinkIntervalTime = 0.1f;
    private float blinkIntervalTimer = 0;

    public AudioClip backgroundAudioNormal;
    public AudioClip backgroundAudioFrightened;
    public AudioClip backgroundAudioPacManDeath;
    public AudioClip consumedGhostAudioClip;

    public Sprite mazeBlue;
    public Sprite mazeWhite;

    public Text playerText;
    public Text readyText;

    public Text highScoreText;
    public Text playerOneUp;
    public Text playerTwoUp;
    public Text playerOneScoreText;
    public Text playerTwoScoreText;
    public Image playerLives2;
    public Image playerLives3;
    public Image playerKey1;
    public Image playerKey2;
    public Image playerKey3;
    public Image playerKey4;
    public Image playerKey5;

    public Text consumedGhostScoreText;
    public Text timer;
    public int secondsLeft = 15;
    public bool takingAway = true;

    public GameObject[,] board = new GameObject[boardWidth, boardHeight];

    private bool didIncrementLevel = false;

    bool didSpawnBonusItem1;
    bool didSpawnBonusItem2;

    bool didSpawnKeyItem;

    // Start is called before the first frame update
    void Start()
    {
        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));
        foreach (GameObject o in objects)
        {
            Vector2 pos = o.transform.position;

            if (o.name != "PacMan" && o.name != "Nodes" && o.name != "bottom_left_corner_single" && o.tag != "Ghost" && o.tag != "ghostHome" && o.name != "Canvas" && o.tag != "UIElements")
            {
                if(o.GetComponent<Tile>() != null)
                {
                    if(o.GetComponent<Tile>().isPellet || o.GetComponent<Tile>().isSuperPellet)
                    {
                        totalPellets++;
                    }
                }

                board [(int)pos.x, (int)pos.y] = o;
            }
            else
            {
                Debug.Log("Found PacMan at: " + pos);
            }
        }
        GameObject.Find("DoorOpen").transform.GetComponent<AudioSource>().enabled = false;
        if (isPlayerOneUp)
        {
            if (playerOneLevel == 1)
            {
                GetComponent<AudioSource>().Play();
            }
        }
        else
        {
            if (playerTwoLevel == 1)
            {
                GetComponent<AudioSource>().Play();
            }
        }

        StartGame();
    }

    void Update()
    {
        UpdateUI();

        //CheckPelletsConsumed();
        CheckFinishStage();

        //CheckShouldBlink();

        BonusItems();
        timeOutDeath();
    }

    void BonusItems()
    {
        if (GameMenu.isOnePlayerGame)
        {
            SpawnBonusItemForPlayer(1);
        }
        else
        {
            if (isPlayerOneUp)
            {
                SpawnBonusItemForPlayer(1);
            }
            else
            {
                SpawnBonusItemForPlayer(2);
            }
        }
    }

    void SpawnBonusItemForPlayer(int playernum)
    {
        if(playernum == 1)
        {
            if (SceneManager.GetActiveScene().name == "Level2")
            {
                if (!didSpawnKeyItem)
                {
                    didSpawnKeyItem = true;
                    SpawnKeyItem();
                }
            }
            if(playerOnePelletsConsumed>=70 && playerOnePelletsConsumed < 170)
            {
                if (!didSpawnBonusItem1)
                {
                    didSpawnBonusItem1 = true;
                    SpawnBonusItemForLevel(playerOneLevel);
                }
            }else if (playerOnePelletsConsumed >= 170)
            {
                if (!didSpawnBonusItem2)
                {
                    didSpawnBonusItem2 = true;
                    SpawnBonusItemForLevel(playerOneLevel);
                }
            }
        }
    }
    void SpawnBonusItemForLevel(int level)
    {
        GameObject bonusitem = null;
        if(level == 1)
        {
            bonusitem = Resources.Load("Prefabs/bonus_apple", typeof(GameObject)) as GameObject;
        }
        else if (level == 2)
        {
            bonusitem = Resources.Load("Prefabs/bonus_banana", typeof(GameObject)) as GameObject;
        }
        else
        {
            bonusitem = Resources.Load("Prefabs/bonus_melon", typeof(GameObject)) as GameObject;
        }

        Instantiate(bonusitem);
    }

    void SpawnKeyItem()
    {
        GameObject keyitem1 = null;
        GameObject keyitem2 = null;
        GameObject keyitem3 = null;
        GameObject keyitem4 = null;
        GameObject keyitem5 = null;
       
        keyitem1 = Resources.Load("Prefabs/key1", typeof(GameObject)) as GameObject;
        keyitem2 = Resources.Load("Prefabs/key2", typeof(GameObject)) as GameObject;
        keyitem3 = Resources.Load("Prefabs/key3", typeof(GameObject)) as GameObject;
        keyitem4 = Resources.Load("Prefabs/key4", typeof(GameObject)) as GameObject;
        keyitem5 = Resources.Load("Prefabs/key5", typeof(GameObject)) as GameObject;

        Instantiate(keyitem1);
        Instantiate(keyitem2);
        Instantiate(keyitem3);
        Instantiate(keyitem4);
        Instantiate(keyitem5);
    }

    void UpdateUI()
    {
        playerOneScoreText.text = playerOneScore.ToString();
        playerTwoScoreText.text = playerTwoScore.ToString();

        if (pacManLives == 3)
        {
            playerLives3.enabled = true;
            playerLives2.enabled = true;
        }else if (pacManLives == 2)
        {
            playerLives3.enabled = false;
            playerLives2.enabled = true;
        }
        else if (pacManLives == 1)
        {
            playerLives3.enabled = false;
            playerLives2.enabled = false;
        }

        if (SceneManager.GetActiveScene().name == "Level2")
        {
            if (playerOneKeyConsumed == 0)
            {
                playerKey1.enabled = true;
                playerKey2.enabled = true;
                playerKey3.enabled = true;
                playerKey4.enabled = true;
                playerKey5.enabled = true;
            }
            else if (playerOneKeyConsumed == 1)
            {
                playerKey5.enabled = false;
            }
            else if (playerOneKeyConsumed == 2)
            {
                playerKey4.enabled = false;
            }
            else if (playerOneKeyConsumed == 3)
            {
                playerKey3.enabled = false;
            }
            else if (playerOneKeyConsumed == 4)
            {
                playerKey2.enabled = false;
            }
            else if (playerOneKeyConsumed == 5)
            {
                playerKey1.enabled = false;
            }
        }
        if(SceneManager.GetActiveScene().name == "Level3")
        {
            if (takingAway == false && secondsLeft > 0)
            {
                StartCoroutine(TimerTake());
            }
        }
        
    }

    bool CheckPelletsConsumed()
    {
        if (isPlayerOneUp)
        {
            //player one is playing
            if(totalPellets == playerOnePelletsConsumed)
            {
                GameObject.Find("DoorClose").transform.GetComponent<SpriteRenderer>().enabled = false;
                if (!GameObject.Find("DoorOpen").transform.GetComponent<AudioSource>().enabled)
                {
                    GameObject.Find("DoorOpen").transform.GetComponent<AudioSource>().enabled = true;
                    GameObject.Find("DoorOpen").transform.GetComponent<AudioSource>().Play();
                }
                return true;
            }
        }
        else
        {
            //player two is playing
            /*if (totalPellets == playerTwoPelletsConsumed)
            {
                PlayerWin(2);
            }*/
            return false;
        }
        return false;
    }
    
    bool CheckKeyConsumed()
    {
        if (totalKey == playerOneKeyConsumed)
        {
            GameObject.Find("DoorClose").transform.GetComponent<SpriteRenderer>().enabled = false;
            if (!GameObject.Find("DoorOpen").transform.GetComponent<AudioSource>().enabled)
            {
                GameObject.Find("DoorOpen").transform.GetComponent<AudioSource>().enabled = true;
                GameObject.Find("DoorOpen").transform.GetComponent<AudioSource>().Play();
            }
            return true;
        }
        return false;
    }

    void CheckFinishStage()
    {
        
        if (playerOneLevel == 2)
        {
            if (CheckKeyConsumed() && GameObject.FindGameObjectWithTag("PacMan").transform.position == new Vector3(14, 0, 0))
                PlayerWin(1);
            return;
        }
        if(CheckPelletsConsumed() && GameObject.FindGameObjectWithTag("PacMan").transform.position == new Vector3(14, 0, 0))
        {
            PlayerWin(1);
        }
    }

    void PlayerWin(int playerNum)
    {
        if (playerNum == 1)
        {
            if (!didIncrementLevel)
            {
                didIncrementLevel = true;
                playerOneLevel++;
            }
        }
        else
        {
            if (!didIncrementLevel)
            {
                didIncrementLevel = true;
                playerTwoLevel++;
            }
        }

        StartCoroutine(ProcessWin(2));
    }

    IEnumerator ProcessWin(float delay)
    {
        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<PacMan>().canMove = false;
        pacMan.transform.GetComponent<Animator>().enabled = false;

        transform.GetComponent<AudioSource>().Stop();

        GameObject [] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach(GameObject ghost in o)
        {
            ghost.transform.GetComponent<Ghost>().canMove = false;
            ghost.transform.GetComponent<Animator>().enabled = false;
        }

        yield return new WaitForSeconds(delay);

        StartCoroutine(BlinkBoard(2));
    }

    IEnumerator BlinkBoard(float delay)
    {
        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in o)
        {
            ghost.transform.GetComponent<SpriteRenderer>().enabled = false;
        }

        //blink board
        shouldBlink = true;

        yield return new WaitForSeconds(delay);

        //Restart the game at the next level
        shouldBlink = false;
        StartNextLevel();
    }

    private void StartNextLevel()
    {
        if (playerOneLevel % 2 == 0)
        {
            SceneManager.LoadScene("Level2");
        }
        else if (playerOneLevel % 3 == 0)
        {
            SceneManager.LoadScene("Level3");
            secondsLeft = 30;
        }
        else SceneManager.LoadScene("Level1");
    }

    private void CheckShouldBlink()
    {
        if (shouldBlink)
        {
            if (blinkIntervalTimer < blinkIntervalTime)
            {
                blinkIntervalTimer += Time.deltaTime;
            }
            else
            {
                blinkIntervalTimer = 0;

                if(GameObject.Find("Maze").transform.GetComponent<SpriteRenderer>().sprite == mazeBlue)
                {
                    GameObject.Find("Maze").transform.GetComponent<SpriteRenderer>().sprite = mazeWhite;
                }
                else
                {
                    GameObject.Find("Maze").transform.GetComponent<SpriteRenderer>().sprite = mazeBlue;
                }
            }
        }
    }

    public void StartGame()
    {
        if (GameMenu.isOnePlayerGame)
        {
            playerTwoUp.GetComponent<Text>().enabled = false;
            playerTwoScoreText.GetComponent<Text>().enabled = false;
            
            //timer.text = "00:" + secondsLeft.ToString();
        } else
        {
            playerTwoUp.GetComponent<Text>().enabled = true;
            playerTwoScoreText.GetComponent<Text>().enabled = true;
        }

        if (isPlayerOneUp)
        {
            StartCoroutine(StartBlinking(playerOneUp));
        }
        else
        {
            StartCoroutine(StartBlinking(playerTwoUp));
        }

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in o)
        {
            ghost.transform.GetComponent<SpriteRenderer>().enabled = false;
            ghost.transform.GetComponent<Ghost>().canMove = false;
        }
        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;
        pacMan.transform.GetComponent<PacMan>().canMove = false;
        pacMan.transform.GetComponent<Animator>().enabled = false;

        StartCoroutine(ShowObjectAfter(2.25f));
    }

    public void StartConsumed(Ghost consumedGhost)
    {
        if (!didStartConsumed)
        {
            didStartConsumed = true;

            //pause all the ghosts
            GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

            foreach(GameObject ghost in o)
            {
                ghost.transform.GetComponent<Ghost>().canMove = false;
            }

            //pause Pacman
            GameObject pacMan = GameObject.Find("PacMan");
            pacMan.transform.GetComponent<PacMan>().canMove = false;

            //hide PacMan
            pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;

            //hide the consumed ghost
            consumedGhost.transform.GetComponent<SpriteRenderer>().enabled = false;

            //Stop background music
            transform.GetComponent<AudioSource>().Stop();

            Vector2 pos = consumedGhost.transform.position;

            Vector2 viewPortPoint = Camera.main.WorldToViewportPoint(pos);

            consumedGhostScoreText.GetComponent<RectTransform>().anchorMin = viewPortPoint;
            consumedGhostScoreText.GetComponent<RectTransform>().anchorMax = viewPortPoint;

            consumedGhostScoreText.GetComponent<Text>().enabled = true;

            //play consumed sound
            transform.GetComponent<AudioSource>().PlayOneShot(consumedGhostAudioClip);

            //wait for the audio clip to finish
            StartCoroutine(ProcessConsumedAfter(0.75f, consumedGhost));
        }
    }

    public void StartConsumedBonusItem (GameObject bonusItem, int scorevalue)
    {
        Vector2 pos = bonusItem.transform.position;
        Vector2 viewPortPoint = Camera.main.WorldToViewportPoint(pos);

        consumedGhostScoreText.GetComponent<RectTransform>().anchorMin = viewPortPoint;
        consumedGhostScoreText.GetComponent<RectTransform>().anchorMax = viewPortPoint;

        consumedGhostScoreText.text = scorevalue.ToString();

        consumedGhostScoreText.GetComponent<Text>().enabled = true;
        if (SceneManager.GetActiveScene().name == "Level3")
            secondsLeft += 10;

        Destroy(bonusItem.gameObject);

        StartCoroutine(ProcessConsumedBonusItem(0.75f));
    }

    public void StartConsumedKeyItem(GameObject keyItem)
    {
        Destroy(keyItem.gameObject);
    }

    IEnumerator ProcessConsumedBonusItem(float delay)
    {
        yield return new WaitForSeconds(delay);

        consumedGhostScoreText.GetComponent<Text>().enabled = false;
        consumedGhostScoreText.text = "200";
    }

    IEnumerator StartBlinking(Text blinkText)
    {
        yield return new WaitForSeconds(0.25f);

        blinkText.GetComponent<Text>().enabled = !blinkText.GetComponent<Text>().enabled;
        StartCoroutine(StartBlinking(blinkText));
    }

    IEnumerator ProcessConsumedAfter(float delay, Ghost consumedGhost)
    {
        yield return new WaitForSeconds(delay);

        //hide the score
        consumedGhostScoreText.GetComponent<Text>().enabled = false;

        //show pacman
        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<SpriteRenderer>().enabled = true;

        //show consumed ghost
        consumedGhost.transform.GetComponent<SpriteRenderer>().enabled = true;

        //resume all ghosts
        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in o)
        {
            ghost.transform.GetComponent<Ghost>().canMove = true;
        }

        //resume pacman
        pacMan.transform.GetComponent<PacMan>().canMove = true;

        //start background music
        transform.GetComponent<AudioSource>().Play();

        didStartConsumed = false;
    }

    IEnumerator ShowObjectAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in o)
        {
            ghost.transform.GetComponent<SpriteRenderer>().enabled = true;
        }
        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<SpriteRenderer>().enabled = true;

        playerText.transform.GetComponent<Text>().enabled = false;

        StartCoroutine(StartGameAfter(2));
    }

    IEnumerator StartGameAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in o)
        {
            ghost.transform.GetComponent<Ghost>().canMove = true;
        }
        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<Animator>().enabled = false;
        pacMan.transform.GetComponent<PacMan>().canMove = true;

        readyText.transform.GetComponent<Text>().enabled = false;

        transform.GetComponent<AudioSource>().clip = backgroundAudioNormal;
        transform.GetComponent<AudioSource>().Play();

        takingAway = false;
    }

    public void StartDeath()
    {
        if (!didStartDeath)
        {
            StopAllCoroutines();

            if (GameMenu.isOnePlayerGame)
            {
                playerOneUp.GetComponent<Text>().enabled = true;
            } else
            {
                playerOneUp.GetComponent<Text>().enabled = true;
                playerTwoUp.GetComponent<Text>().enabled = true;
            }

            GameObject bonusItem = GameObject.Find("bonusItem");

            if (bonusItem)
                Destroy(bonusItem.gameObject);

            didStartDeath = true;

            GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

            foreach (GameObject ghost in o)
            {
                ghost.transform.GetComponent<Ghost>().canMove = false;
            }

            GameObject pacMan = GameObject.Find("PacMan");
            pacMan.transform.GetComponent<PacMan>().canMove = false;

            pacMan.transform.GetComponent<Animator>().enabled = false;

            transform.GetComponent<AudioSource>().Stop();

            StartCoroutine(ProcessDeathAfter(2));
        }
    }

    IEnumerator ProcessDeathAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in o)
        {
            ghost.transform.GetComponent<SpriteRenderer>().enabled = false;
        }

        StartCoroutine(ProcessDeathAnimation(1.9f));
    }

    IEnumerator ProcessDeathAnimation(float delay)
    {
        GameObject pacMan = GameObject.Find("PacMan");

        pacMan.transform.localScale = new Vector3(1, 1, 1);
        pacMan.transform.localRotation = Quaternion.Euler(0, 0, 0);

        pacMan.transform.GetComponent<Animator>().runtimeAnimatorController = pacMan.transform.GetComponent<PacMan>().deathAnimation;
        pacMan.transform.GetComponent<Animator>().enabled = true;

        transform.GetComponent<AudioSource>().clip = backgroundAudioPacManDeath;
        transform.GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(delay);

        StartCoroutine(ProcessRestart(1));
    }

    IEnumerator ProcessRestart(float delay)
    {
        pacManLives -= 1;

        if (pacManLives == 0)
        {
            playerText.transform.GetComponent<Text>().enabled = false;

            readyText.transform.GetComponent<Text>().text = "GAME OVER";
            readyText.transform.GetComponent<Text>().color = Color.red;

            readyText.transform.GetComponent<Text>().enabled = true;

            GameObject pacMan = GameObject.Find("PacMan");
            pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;

            transform.GetComponent<AudioSource>().Stop();

            playerOneLevel = 1;

            StartCoroutine(ProcessGameOver(2));
        }
        else
        {

            playerText.transform.GetComponent<Text>().enabled = true;
            readyText.transform.GetComponent<Text>().enabled = true;

            GameObject pacMan = GameObject.Find("PacMan");
            pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;

            transform.GetComponent<AudioSource>().Stop();

            yield return new WaitForSeconds(delay);

            StartCoroutine(ProcessRestartShowObjects(1));
        }

    }

    IEnumerator ProcessGameOver(float delay)
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene("GameMenu");
    }

    IEnumerator ProcessRestartShowObjects(float delay)
    {
        playerText.transform.GetComponent<Text>().enabled = false;

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in o)
        {
            ghost.transform.GetComponent<SpriteRenderer>().enabled = true;
            ghost.transform.GetComponent<Ghost>().MoveToStartingPosition();
        }
        GameObject pacMan = GameObject.Find("PacMan");

        pacMan.transform.GetComponent<Animator>().enabled = false;
        pacMan.transform.GetComponent<SpriteRenderer>().enabled = true;
        pacMan.transform.GetComponent<PacMan>().MoveToStartingPosition();
        if (SceneManager.GetActiveScene().name == "Level3")
        {
            if (secondsLeft < 10)
            {
                timer.text = "00:0" + secondsLeft.ToString();
            }
            else timer.text = "00:" + secondsLeft.ToString();
        }

        yield return new WaitForSeconds(delay);

        Restart();
    }

    public void Restart()
    {
        readyText.transform.GetComponent<Text>().enabled = false;

        

        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<PacMan>().Restart();

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach(GameObject ghost in o)
        {
            ghost.transform.GetComponent<Ghost>().Restart();
        }
        transform.GetComponent<AudioSource>().clip = backgroundAudioNormal;
        transform.GetComponent<AudioSource>().Play();
        didStartDeath = false;
        takingAway = false;
        
    }

    IEnumerator TimerTake()
    {
        takingAway = true;
        yield return new WaitForSeconds(1);
        secondsLeft -= 1;
        if(secondsLeft % 3 == 0)
        {
            GameObject.Find("Main Camera").transform.GetComponent<CameraShake>().shakeDuration = 0.6f;
        }
        if(secondsLeft < 10)
        {
            timer.text = "00:0" + secondsLeft.ToString();
        }
        else timer.text = "00:" + secondsLeft.ToString();
        takingAway = false;
    }

    void timeOutDeath()
    {
        if(secondsLeft == 0)
        {
            pacManLives = 1;
            StartDeath();
            
        }
    }
}
