using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager  gm;

    [SerializeField] private Text _uiScore;
    [SerializeField] private Text _uiTime;    
    [SerializeField] private GameObject[] _uiLives;
    [SerializeField] private GameObject _uiLostPanel;
    [SerializeField] private Button _uiRestartBtn;
    [SerializeField] private Button _uiExitGameBtn;

    [SerializeField] private int _currentScore = 0;
    [SerializeField] private int _currentLives;

    public int currentPlayerLives{
        get{
            return _currentLives;
        }
    }

    private float _timerTime;

    public bool canPlay = true;

    private void Awake() {
        if(gm==null)
        {
            gm = this.GetComponent<GameManager>();
        }
        canPlay = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        _timerTime = 0;   
        _uiRestartBtn.GetComponent<Button>().onClick.AddListener(Restart);   
        _uiExitGameBtn.GetComponent<Button>().onClick.AddListener(ExitGame); 
        _currentLives = _uiLives.Length;        
    }

    // Update is called once per frame
    void Update()
    {
        _timerTime += Time.deltaTime;
        _uiTime.text = "Time: "+ _timerTime.ToString("0");        
    }

    public void SetScore(int value)
    {
        _currentScore += value;
        _uiScore.text = "Score: "+_currentScore;
    }

    public void SetLivesState()
    {
        for(int i=0; i<_uiLives.Length ; i++)
        {
            if(!_uiLives[i].activeInHierarchy)
            {
                _uiLives[i-1].SetActive(false);
                break;
            }         
            if(i == (_uiLives.Length-1))
            {
                _uiLives[i].SetActive(false);
            }               
        }
        _currentLives--;
        if(_currentLives <=0)
        {
            canPlay = false;
            _uiLostPanel.SetActive(true);
        }
    }
    
    private void Restart()
    {
        SceneManager.LoadScene("Level1");
    }
    private void ExitGame()
    {
        Application.Quit();
    }
}
