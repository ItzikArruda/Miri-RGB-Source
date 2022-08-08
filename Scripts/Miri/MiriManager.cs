using System.Collections;
using Experiments.Global.Managers;
using UnityEngine.SceneManagement;
using Experiments.Global.Audio;
using Experiments.Global.IO;
using Experiments.MiriRGB.Gameplay;
using Experiments.MiriRGB.Visuals;
using UnityEngine;
using TMPro;

namespace Experiments.MiriRGB.Managers
{
    public class MiriManager : Manager<MiriManager>
    {
        [Space]
        public Miri miri;
        public CameraFollow Cam;
        [System.Serializable]
        public class Progress : SaveFile
        {
            [Space]
            public float HIscore;
            public int GamesPlayed;
        }
        public Progress progress;

        [Header("UI")]
        public GameObject GameTitle;
        enum TooltipTextMode { StartScreen, Tutorial, Score, Empty }
        public TMP_Text TooltipText;
        public GameObject NewHIText;
        public GameObject FadeOut;
        float Score;
        bool HIScoreBeat;
        bool PassedTutorial;

        // Start is called before the first frame update
        void Start()
        {
            Init(this);

            AudioManager.Instance.InteractWithSFX("Startup", SoundEffectBehaviour.Play);

            Resolution GameRes = Screen.resolutions[Screen.resolutions.Length - 1];
            Screen.SetResolution(GameRes.width, GameRes.height, true, GameRes.refreshRate);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Progress LoadedPorgress = Saver.Load(progress) as Progress;
            if(LoadedPorgress != null) { progress = LoadedPorgress; }
        }

        string GetTooltipText(TooltipTextMode Mode)
        {
            string Result = "";
            switch (Mode)
            {
                case TooltipTextMode.StartScreen:
                {
                    Result = "Best Distance:" + progress.HIscore.ToString("00") + " | Total Played:" + progress.GamesPlayed.ToString("00");
                    Result += "\nPress Any Key To Start.";
                    break;
                }
                case TooltipTextMode.Tutorial:
                {
                    Result = "Press The R/G/B Keys According";
                    Result += "\nTo Miris Color To Jump.";
                    break;
                }
                case TooltipTextMode.Score:
                {
                    Result = "Distance:" + Score.ToString("00") + " | Best Distance:" + progress.HIscore.ToString("00");
                    break;
                }
                case TooltipTextMode.Empty:
                {
                    Result = " ";
                    break;
                }
            }

            return Result;
        }

        // Update is called once per frame
        void Update()
        {
            GameTitle.SetActive(!miri.Playing);

            if(miri != null)
            {
                Score = miri.transform.position.y;
                if(Score > 10f) { PassedTutorial = true; }
                if(Score > progress.HIscore)
                {
                    progress.HIscore = Score;
                    if(!HIScoreBeat)
                    {
                        HIScoreBeat = true;
                        NewHIText.SetActive(true);
                        AudioManager.Instance.InteractWithSFX("NewHI", SoundEffectBehaviour.Play);
                    }
                }
            }

            if(miri == null) { TooltipText.text = GetTooltipText(TooltipTextMode.Empty); }
            else if(!miri.Playing && !PassedTutorial) { TooltipText.text = GetTooltipText(TooltipTextMode.StartScreen); }
            else { TooltipText.text = GetTooltipText((!PassedTutorial) ? TooltipTextMode.Tutorial : TooltipTextMode.Score); }

            progress.Save();

            if(miri != null)
            {
                if(!miri.Playing && Input.GetKeyDown(KeyCode.Escape)) { QuitGame(); }
            }
        }

        public void Die()
        {
            Cam.FlashRate = 1f;
            StartCoroutine(RestartScene());
        }
        IEnumerator RestartScene()
        {
            yield return new WaitForSeconds(5f);
            FadeOut.SetActive(true);
            yield return new WaitForSeconds(1f);
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }

        void QuitGame()
        {
            Application.Quit();
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.ExitPlaymode();
            #endif
        }
    }
}