using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Death Screen")]
    public Canvas deathUI;
    public Button resetButton;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI moneyText;

    [Header("Win Screen")]
    public Canvas winUI;
    public Button winResetButton;

    [Header("Entities")]
    public GameObject menuBoss;
    private GameObject menuBossInstance;
    public GameObject playerPrefab;
    private GameObject playerInstance;
    public GameObject enemyPrefab;
    public GameObject goliathPrefab;
    public GameObject blockDestroyerPrefab;
    private PlayerController playerInstanceController;
    private GameObject blockDestroyerInstance;
    public GameObject origin;

    [Header("Game UI")]
    public Sprite shieldIcon;
    public Sprite buckshotIcon;
    public Sprite lockedIcon;
    public Image shieldImage;
    public Image buckshotImage;
    public Canvas menuUI;
    public Button startButton;
    public Canvas mainUI;
    private Slider progressSlider;
    private Slider shieldSlider;
    private Slider dashSlider;
    private Slider buckshotSlider;

    [Header("Music UI")]
    public AudioClip gameMusic;

    private List<GameObject> enemies;
    private int enemyCount;
    private int wave;
    private bool gameRunning = false;
    private int totalMoney;

    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        resetButton.onClick.AddListener(OnResetClick);
        winResetButton.onClick.AddListener(OnResetClick);
        progressSlider = mainUI.GetComponentInChildren<Slider>();
        shieldSlider = mainUI.GetComponentsInChildren<Slider>()[1];
        shieldImage.sprite = lockedIcon;
        dashSlider = mainUI.GetComponentsInChildren<Slider>()[2];
        buckshotSlider = mainUI.GetComponentsInChildren<Slider>()[3];
        buckshotImage.sprite = lockedIcon;
        menuBossInstance = Instantiate(menuBoss, new Vector3(0f, 3f, 0f), Quaternion.identity);
    }

    void Update()
    {
        if (gameRunning)
        {
            // check enemy count
            enemyCount = 0;

            foreach (GameObject enemy in enemies)
            {
                if (enemy != null)
                {
                    enemyCount += 1;
                }
            }

            progressSlider.value = 1 - enemyCount / (float)enemies.Count;

            if (enemyCount <= 0)
            {
                Enemy blockDestroyerEnemy = blockDestroyerInstance.GetComponent<Enemy>();
                blockDestroyerEnemy.SetHealth(blockDestroyerEnemy.health - 1);

                int money = PlayerPrefs.GetInt("Money", 0);
                money += enemies.Count;
                totalMoney += enemies.Count;

                PlayerPrefs.SetInt("Money", money);

                SpawnWave();
            }

            // check for player
            if (playerInstance == null)
            {
                EndGame();
            }
            else
            {
                shieldSlider.value = 1 - playerInstanceController.shieldCooldown / 15f;
                dashSlider.value = 1 - playerInstanceController.dashCooldown / 5f;
                buckshotSlider.value = 1 - playerInstanceController.buckshotCooldown / 30f;
            }
        }
        else
        {
            origin.transform.Rotate(new Vector3(0f, Time.deltaTime * 5f, 0f));
        }
    }

    private void StartGame()
    {
        playerInstance = Instantiate(playerPrefab, new Vector3(0f, 2f, -20f), Quaternion.identity);
        playerInstanceController = playerInstance.GetComponent<PlayerController>();
        blockDestroyerInstance = Instantiate(blockDestroyerPrefab, new Vector3(0f, 5f, 25f), Quaternion.identity);

        menuUI.gameObject.SetActive(false);
        mainUI.gameObject.SetActive(true);

        AudioSource audioSource = Camera.main.GetComponent<AudioSource>();
        audioSource.clip = gameMusic;
        audioSource.Play();

        Destroy(menuBossInstance);

        SpawnWave();

        origin.transform.rotation = Quaternion.identity;

        gameRunning = true;
    }

    private void EndGame()
    {
        mainUI.gameObject.SetActive(false);
        deathUI.enabled = true;
        deathUI.gameObject.SetActive(true);
        waveText.text = $"You died on wave {wave}";
        moneyText.text = $"gained: {totalMoney}";
        gameRunning = false;
    }

    private void SpawnWave()
    {
        enemies = new List<GameObject>();
        wave += 1;

        if (wave > 10)
        {
            winUI.enabled = true;
            winUI.gameObject.SetActive(true);
            playerInstanceController.health = 999999;
            return;
        }

        for (int i = 0; i < wave + 2; i++)
        {
            enemies.Add(Instantiate(enemyPrefab, GetEnemySpawn(), Quaternion.identity));
        }

        if (wave > 2)
        {
            for (int i = 0; i < wave - 2; i++)
            {
                enemies.Add(Instantiate(goliathPrefab, GetEnemySpawn(), Quaternion.identity));
            }
        }

        if (playerInstanceController.health < 3)
        {
            playerInstanceController.health += 1;
        }

        if (wave == 5)
        {
            playerInstanceController.canShield = true;
            shieldImage.sprite = shieldIcon;
        }

        if (wave == 8)
        {
            playerInstanceController.canBuckshot = true;
            buckshotImage.sprite = buckshotIcon;
        }
    }

    private Vector3 GetEnemySpawn()
    {
        Vector2 spawnPosition;
        do
        {
            spawnPosition = Random.insideUnitSphere * 25f;
        }
        while (spawnPosition.y < 0f);

        return new Vector3(spawnPosition.x, 2.5f, spawnPosition.y);
    }

    private void OnResetClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
