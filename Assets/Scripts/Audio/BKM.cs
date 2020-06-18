using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BKM : MonoBehaviour
{
    [Header("Menu")]
    [FMODUnity.EventRef, SerializeField] private string musicMenu = "event:/BKM/Menu Music";
    
    [Space, Header("Farm")]
    [FMODUnity.EventRef, SerializeField] private string musicFarm = "event:/BKM/Farm Music";
    [FMODUnity.EventRef, SerializeField] private string farmTruckDrive = "event:/SFX_TRUCK/Farm Truck Driving";

    [Space, Header("Factory")]
    [FMODUnity.EventRef, SerializeField] private string musicFactory = "event:/BKM/Factory Music";
    [FMODUnity.EventRef, SerializeField] private string factoryHum = "event:/SFX_FACTORY/Conveyor Hum";

    [Space, Header("Kitchen")]
    [FMODUnity.EventRef, SerializeField] private string musicHome = "event:/BKM/Kitchen Music";
    
    [Space, Header("Transit Cutscenes")]
    [FMODUnity.EventRef, SerializeField] private string farmTruckTransit = "event:/SFX_TRUCK/Farm Truck Transit";

    private FMOD.Studio.EventInstance _instanceSong;
    private FMOD.Studio.EventInstance _instanceTransitionFx;

    public int debugLevelTest;
   
    //TODO: Remove this
    private int _debugPotatoes;
    
    private void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        switch (currentScene)
        {
            case "!Main Menu":
                debugLevelTest = 0;
                StopMusicImmediate();
                MenuMusic();
                break;
            
            case "!Farm Level":
                debugLevelTest = 1;
                _instanceSong.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _instanceSong.release();
                FarmMusic();
                break;
            
            case "!Factory Level":
                debugLevelTest = 2;
                _instanceSong.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _instanceSong.release();
                FactoryMusic();
                break;
            
            case "!Kitchen Level":
                debugLevelTest = 3;
                _instanceSong.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _instanceSong.release();
                HomeMusic();
                break;
            
            case "!Ending Scene":
                debugLevelTest = 3;
                _instanceSong.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _instanceSong.release();
                HomeMusic();
                break;
            
            default:
                _instanceSong.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                _instanceSong.release();
                break;
        }

        GameObject[] objs = GameObject.FindGameObjectsWithTag("AudioManager");
        if (objs.Length > 1)
            Destroy(objs[0]);
        
        DontDestroyOnLoad(this.gameObject);
        
        /*
        if (debugLevelTest == 0)
        {
            FarmMusic();
        }

        if (debugLevelTest == 1)
        {
            FactoryMusic();
        }

        if (debugLevelTest == 2)
        {
            FarmMusic();
            _instanceSong.setVolume(0.5f);
        }
        */
    }
    
    

    public void StopMusicImmediate()
    {
        _instanceSong.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _instanceSong.release();
    }

    public void StopMusicFade()
    {
        _instanceSong.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _instanceSong.release();
    }

    private void MenuMusic()
    {
        _instanceSong = FMODUnity.RuntimeManager.CreateInstance(musicMenu);
        _instanceSong.start();
    }

    #region Farm

    private void FarmMusic() 
    { 
        _instanceSong = FMODUnity.RuntimeManager.CreateInstance(musicFarm);
        _instanceSong.start();
    }

    #endregion

    #region Factory

    private void FactoryMusic()
    {
        _instanceSong = FMODUnity.RuntimeManager.CreateInstance(musicFactory);
        _instanceSong.start();
    }

    #endregion

    #region Home

    private void HomeMusic() 
    { 
        _instanceSong = FMODUnity.RuntimeManager.CreateInstance(musicHome);
        _instanceSong.start();
    }

    #endregion
    
    #region Transit

    public void TruckDriving()
    {
        _instanceTransitionFx = FMODUnity.RuntimeManager.CreateInstance(farmTruckDrive);
        _instanceTransitionFx.start();
        StartCoroutine(WaitForEnd(6.0f));
    }
    
    public void TruckTransition()
    {
        _instanceTransitionFx = FMODUnity.RuntimeManager.CreateInstance(farmTruckTransit);
        _instanceTransitionFx.start();
        _instanceTransitionFx.release();
    }

    #endregion
    
    // TODO: REMOVE THIS
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Factory/Potato_Packed"))
        {
            if (_debugPotatoes >= 2)
            {
                StopMusicFade();
            }
            else
            {
                _debugPotatoes++;
            }
        }
    }

    private IEnumerator WaitForEnd(float length)
    {
        yield return new WaitForSeconds(length);
        _instanceSong.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _instanceTransitionFx.release();
        _instanceSong.release();
    }
}
