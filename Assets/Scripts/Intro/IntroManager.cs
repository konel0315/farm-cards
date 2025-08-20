using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    [Header("UI References")]
    public Image backgroundImage;
    public Image introImageUI1;
    public Image introImageUI2;
    public TMP_Text gameTitleText;
    public TMP_Text pressAnyKeyText;
    
    [Header("Intro Settings")]
    public List<string> introImageNames;
    public List<Color> backgroundColors;
    private float imageDisplayTime = 1.3f;
    private float fadeDuration = 1.4f;

    [Header("Audio")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioClip bgmClip;
    public AudioClip pencilSfxClip; 
    
    [Header("Press Any Key Settings")]
    private float blinkDuration = 0.8f;
    
    private List<Sprite> introSprites = new List<Sprite>();
    private bool useFirst = true;
    private bool pressAnyKeyActive = false;
    
    void Start()
    {
        introImageUI1.color = new Color(1f, 1f, 1f, 0f);
        introImageUI2.color = new Color(1f, 1f, 1f, 0f);
        backgroundImage.color = backgroundColors[0];
        gameTitleText.gameObject.SetActive(false);
        pressAnyKeyText.gameObject.SetActive(false);
        LoadIntroImages();
        
        if (bgmSource != null && bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
        
        PlayIntroSequence();
    }

    void LoadIntroImages()
    {
        introSprites.Clear();
        foreach (var name in introImageNames)
        {
            Sprite sprite = Resources.Load<Sprite>($"Arts/BackGround/{name}");
            if (sprite != null)
                introSprites.Add(sprite);
            else
                Debug.LogWarning($"Intro image '{name}' not found in Resources!");
        }
    }

    void PlayIntroSequence()
    {
        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < introSprites.Count; i++)
        {
            int index = i;
            Image active = useFirst ? introImageUI1 : introImageUI2;
            Image inactive = useFirst ? introImageUI2 : introImageUI1;

            seq.AppendCallback(() =>
            {
                active.sprite = introSprites[index];
                active.color = new Color(1, 1, 1, 0);
                active.gameObject.SetActive(true);
            });

            seq.Append(active.DOFade(1f, fadeDuration))
                .Join(inactive.DOFade(0f, fadeDuration))
                .Join(backgroundImage.DOColor(backgroundColors[index], fadeDuration));

            seq.AppendInterval(imageDisplayTime);

            useFirst = !useFirst;
        }

        
        seq.AppendCallback(() =>
        {
            if (sfxSource != null && pencilSfxClip != null)
            {
                sfxSource.clip = pencilSfxClip;
                sfxSource.loop = true;
                sfxSource.Play();
            }
            gameTitleText.gameObject.SetActive(true);
            gameTitleText.text = "";
        });
        
        seq.AppendInterval(0.5f);
        
        seq.Append(gameTitleText.DOText("Harvest Tales", 2f, true, ScrambleMode.None)
            .OnComplete(() =>
            {
                if (sfxSource != null)
                {
                    sfxSource.Stop();
                    sfxSource.loop = false;
                }
                DOVirtual.DelayedCall(2f, () =>
                {
                    ShowPressAnyKey();
                });
            })
        );

        seq.Play();

    }
    void ShowPressAnyKey()
    {
        pressAnyKeyText.gameObject.SetActive(true);
        pressAnyKeyActive = true;
        
        DOVirtual.DelayedCall(0.7f, () =>
        {
            pressAnyKeyText.DOFade(0.4f, blinkDuration)
                .SetLoops(-1, LoopType.Yoyo);
        });
    }
    void Update()
    {
        if (pressAnyKeyActive && Input.anyKeyDown)
        {
            LoadGameScene();
        }
    }

    void LoadGameScene()
    {
        SceneManager.LoadScene("Main");
    }
}
