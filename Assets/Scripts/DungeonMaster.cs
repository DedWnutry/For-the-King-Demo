using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEditor;

public class DungeonMaster : MonoBehaviour
{
    public GameObject player;
    public Camera mainCam;
    public GameObject HUD;
    public SoundManager SM;
    public bool paused;
    public LayerMask environmentMask;
    public LayerMask actorsMask;
    public LayerMask waterMask;
    [SerializeField] private Texture2D _cursorStandart;
    [SerializeField] private Texture2D _cursorDrag;
    [SerializeField] private Texture2D _cursorPick;
    [SerializeField] private Texture2D _cursorFether;
    private int _size = 50;
    private Vector2 _offset;
    [SerializeField] private GameObject _pauseMenu;
    public GameObject inventoryMenu;
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _charMenu;
    [SerializeField] private GameObject _journalMenu;
    [SerializeField] private GameObject _diaryMenu;
    [SerializeField] private GameObject _mapMenu;
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private GameObject _controlsMenu;
    [SerializeField] private GameObject _graphicMenu;
    [SerializeField] private GameObject _soundMenu;
    [SerializeField] private GameObject _exitMenu;
    [Range(0f, 1f)] public float soundOverall = 1f;
    [Range(0f, 1f)] public float musicOverall = 1f;
    [Range(0f, 1f)] public float effectsOverall = 1f;
    [Range(0f, 1f)] public float dialogueOverall = 1f;
    [SerializeField] private int _fallingTimer;
    [SerializeField] private int _maxFallTime;
    public Transform deathScreen;
    public float gravity;
    [SerializeField] private Transform _sun;
    [SerializeField] private float _timeSpeed = .1f;
    [SerializeField] private Gradient _sunColor;
    [SerializeField] private bool _day;
    [SerializeField] private bool _dayCycleActive;
    [SerializeField] private Transform _compass;
    [SerializeField] private Sprite _sunIcon;
    [SerializeField] private Sprite _moonIcon;
    [SerializeField] private Transform _timeWidget;
    [SerializeField] private Transform _miniMapCam;

    [Header("Controls:")]
    public float mouseSensitivity = 100f;
    public KeyCode menuKey;
    public KeyCode moveForward;
    public KeyCode moveBackward;
    public KeyCode moveRight;
    public KeyCode moveLeft;
    public KeyCode sprintKey;
    public KeyCode walkKey;
    public KeyCode sneakKey;
    public KeyCode jumpKey;
    public KeyCode kickKey;
    public KeyCode drawWeaponKey;
    public KeyCode interactKey;
    public KeyCode throwKey;

    void Awake()
    {
        //player = GameObject.FindGameObjectWithTag("Player");

        //mainCam = Camera.main;

        Cursor.visible = false;
    }

    void Start()
    {
        CloseAllMenus(); //Need to be in Start() for picking up items

        SM = GetComponent<SoundManager>();

        if (SceneManager.GetActiveScene().buildIndex == 0) 
        {
            SM.PlaySound("MainMenuTheme");
        }
        else SM.PlaySound("Ambient");
    }

    void Update()
    {
        if (Input.GetKeyDown(menuKey))
        {
            if (paused == true) CloseAllMenus();
            else if (player.GetComponent<Character>().grounded)
            {
                //Cursor.visible = false;

                _pauseMenu.SetActive(true);

                player.GetComponent<Character>().headConstraint.weight = 0;

                //mainCam.GetComponent<CameraControl>().ChangeToInventoryCam(true);
            }

            paused = !paused;
        }

        if (_dayCycleActive) DayNightCycle();

        //InfiniteFall();

        /*
        if (paused == false && Time.timeScale != 1f)
        {
            Time.timeScale = 1f;

            //Time.fixedDeltaTime = Time.timeScale / 0.02f;
        }
        else if (paused == true && player.GetComponent<PlayerStats>().curHP > 0)
        {
            Time.timeScale = 0.33f;

            //Time.fixedDeltaTime = Time.timeScale * 0.02f;  
        }*/

        if (SceneManager.GetActiveScene().buildIndex != 0) _compass.transform.rotation = Quaternion.Euler(0, 0, mainCam.transform.eulerAngles.y);
    }

    void LateUpdate()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            Vector3 newPos = player.transform.position;

            newPos.y = _miniMapCam.transform.position.y;

            _miniMapCam.transform.position = newPos;

            _miniMapCam.transform.rotation = Quaternion.Euler(90f, player.transform.eulerAngles.y, 0f);
        }       
    }

    void CloseAllMenus()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            _pauseMenu.SetActive(false);

            inventoryMenu.SetActive(false);

            _journalMenu.SetActive(false);

            _mapMenu.SetActive(false);

            _cursorStandart = null;

            //mainCam.GetComponent<CameraControl>().ChangeToInventoryCam(false);

            player.GetComponent<Character>().headConstraint.weight = 1;

            player.GetComponent<Animator>().ResetTrigger("Pause");

            player.GetComponent<Animator>().SetLayerWeight(1, 1f);
        }
    }

    void DayNightCycle()
    {
        //if (_sun.localRotation.x < 0) _sun.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));


        if (_sun.transform.rotation.x < 1)
        {
            _sun.transform.Rotate(_timeSpeed, 0f, 0f, Space.World);

            _timeWidget.transform.Rotate(0f, 0f, -_timeSpeed, Space.World);

            //_timeWidget.rotation = new Quaternion(0f, 0f, _sun.transform.rotation.x, 1f);
        }
        else 
        {
            _sun.transform.rotation = Quaternion.identity;

            //_timeWidget.transform.rotation = Quaternion.identity;

            if (_day) _timeWidget.transform.rotation = new Quaternion(0f, 0f, _timeWidget.transform.rotation.z - 180, 1f);
            

            _day = !_day;
        }

        if (_day)
        {
            _sun.GetComponent<Light>().color = _sunColor.Evaluate(_sun.transform.localRotation.x / 2);

            _sun.GetComponent<Light>().intensity = (1 / _sunColor.Evaluate(_sun.transform.localRotation.x)[0]) - 1;

            _timeWidget.GetComponent<Image>().sprite = _sunIcon;
        }
        else 
        {
            _sun.GetComponent<Light>().color = _sunColor.Evaluate(0.51f);

            _sun.GetComponent<Light>().intensity = 0.5f;

            _timeWidget.GetComponent<Image>().sprite = _moonIcon;
        }   
    }

    void InfiniteFall()
    {
        if (player.GetComponent<Character>().falling == true & _fallingTimer == 0)
        {
            StartCoroutine(FallCountdown());
        }
        else if (player.GetComponent<Character>().falling == false)
        {
            StopCoroutine(FallCountdown());

            _fallingTimer = 0;
        }

        if (_fallingTimer > _maxFallTime) GameOver();
    }

    IEnumerator FallCountdown()
    {
        yield return new WaitForSeconds(1.0f);

        _fallingTimer++;
    }

    public void NewGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Unpause()
    {

        paused = !paused;
    }

    public void Continue()
    {
        Unpause();

        CloseAllMenus();
    }

    public void Inventory()
    {
        if (inventoryMenu.activeSelf == false)
        {
            _cursorStandart = _cursorPick;

            _journalMenu.SetActive(false);

            _mapMenu.SetActive(false);

            inventoryMenu.SetActive(true);
        }
        else inventoryMenu.SetActive(false);
    }

    public void Journal()
    {
        if (_journalMenu.activeSelf == false)
        {
            _cursorStandart = _cursorFether;

            _journalMenu.SetActive(true);

            _mapMenu.SetActive(false);

            inventoryMenu.SetActive(false);
        }
        else _journalMenu.SetActive(false);

        mainCam.transform.LookAt(_journalMenu.transform);

        player.GetComponent<Animator>().SetTrigger("Pause");

        mainCam.fieldOfView = 45;

        player.GetComponent<Animator>().SetLayerWeight(1, 0f);

        _journalMenu.GetComponent<Animation>().Play();
    }

    public void Map()
    {
        if (_mapMenu.activeSelf == false)
        {
            _cursorStandart = _cursorFether;

            _mapMenu.SetActive(true);

            _journalMenu.SetActive(false);

            inventoryMenu.SetActive(false);
        }
        else _mapMenu.SetActive(false);

        mainCam.transform.LookAt(_mapMenu.transform);

        player.GetComponent<Animator>().SetTrigger("Pause");

        mainCam.fieldOfView = 45;

        player.GetComponent<Animator>().SetLayerWeight(1, 0f);

        //mapMenu.GetComponent<Animation>().Play();
    }

    public void CharacterMenu()
    {
        if (_charMenu.activeSelf == false)
        {
            _diaryMenu.SetActive(false);

            _charMenu.SetActive(true);
        }
        else _charMenu.SetActive(false);
    }

    public void Diary()
    {
        if (_diaryMenu.activeSelf == false)
        {
            _diaryMenu.SetActive(true);

            _charMenu.SetActive(false);
        }
        else _diaryMenu.SetActive(false);
    }

    public void Settings()
    {
        if (_settingsMenu.activeSelf == false) 
        {
            _settingsMenu.SetActive(true);

            _mainMenu.SetActive(false);
        }
        else 
        {
            _settingsMenu.SetActive(false);

            _mainMenu.SetActive(true);
        }
    }

    public void Controls()
    {
        if (_controlsMenu.activeSelf == false) 
        {
            _controlsMenu.SetActive(true);

            _graphicMenu.SetActive(false);

            _soundMenu.SetActive(false);
        }
        else _controlsMenu.SetActive(false);
    }

    public void Graphics()
    {
        if (_graphicMenu.activeSelf == false)
        {
            _controlsMenu.SetActive(false);

            _graphicMenu.SetActive(true);

            _soundMenu.SetActive(false);
        }
        else _graphicMenu.SetActive(false);
    }

    public void ExitMenu()
    {
        if (_exitMenu.activeSelf == false) _exitMenu.SetActive(true);
        else _exitMenu.SetActive(false);
    }

    public void Sounds()
    {
        if (_soundMenu.activeSelf == false)
        {
            _controlsMenu.SetActive(false);

            _graphicMenu.SetActive(false);

            _soundMenu.SetActive(true);
        }
        else _soundMenu.SetActive(false);
    }

    public void ChangeOverallSoundVolume(float value)
    {
        soundOverall = value;

       foreach (SoundManager manager in GameObject.FindObjectsOfType<SoundManager>())
       {
           foreach (Sound sound in manager.sounds) sound.source.volume = soundOverall;
       }
    }

    public void ChangeMusicSoundVolume(float value)
    {
        musicOverall = value;

        foreach (SoundManager manager in GameObject.FindObjectsOfType<SoundManager>())
        {
            foreach (Sound sound in manager.sounds) if (sound.category == SoundCategory.OST) sound.source.volume = musicOverall * soundOverall;
        }
    }

    public void ChangeEffectsSoundVolume(float value)
    {
        effectsOverall = value;

        foreach (SoundManager manager in GameObject.FindObjectsOfType<SoundManager>())
        {
            foreach (Sound sound in manager.sounds) if (sound.category == SoundCategory.Effect) sound.source.volume = effectsOverall * soundOverall;
        }
    }

    public void ChangeDialogueSoundVolume(float value)
    {
        dialogueOverall = value;

        foreach (SoundManager manager in GameObject.FindObjectsOfType<SoundManager>())
        {
            foreach (Sound sound in manager.sounds) if (sound.category == SoundCategory.Voice) sound.source.volume = dialogueOverall * soundOverall;
        }
    }

    public void SetCursor(bool value)
    {
        _offset = new Vector2(-_size / 2, -_size / 2);
        if (inventoryMenu.activeSelf == true)
        {
            if (value == true) _cursorStandart = _cursorPick;
            else _cursorStandart = _cursorDrag;
        }   
    }

    public void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        EditorApplication.isPlaying = false;

        Application.Quit();
    }

    void OnGUI()
    {
        Vector2 mousePos = Event.current.mousePosition;

        GUI.depth = 999;

        GUI.Label(new Rect(mousePos.x + _offset.x, mousePos.y + _offset.y, _size, _size), _cursorStandart);
    }
}