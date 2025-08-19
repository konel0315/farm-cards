using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
public class IntroManager : MonoBehaviour
{
    [Header("UI References")]
    public Image backgroundImage;
    public Image introImageUI;
    public TMP_Text gameTitleText;
    public TMP_Text madeByText;
    
    [Header("Intro Settings")]
    public List<string> introImageNames;
    public List<Color> backgroundColors;
    private float imageDisplayTime = 1.3f;
    private float fadeDuration = 1f;

    private List<Sprite> introSprites = new List<Sprite>();

    void Start()
    {
        introImageUI.color = new Color(1f, 1f, 1f, 0f);
        
        backgroundImage.color = backgroundColors[0];
        
        LoadIntroImages();
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
            
            seq.AppendCallback(() =>
            {
                introImageUI.sprite = introSprites[index];
            });

            seq.Append(introImageUI.DOFade(1f, fadeDuration))
                .Join(backgroundImage.DOColor(backgroundColors[index], fadeDuration));
            
            seq.AppendInterval(imageDisplayTime);
            if (i != introSprites.Count-1)
            {
                seq.Append(introImageUI.DOFade(0f, fadeDuration));
            }
            
        }

        
        seq.AppendCallback(() =>
        {
            gameTitleText.gameObject.SetActive(true);
            gameTitleText.text = "";
            
            seq.Append(gameTitleText
                .DOText("Harvest Tales", 2f, true, ScrambleMode.None)
                .OnComplete(() =>
                {
                    madeByText.gameObject.SetActive(true);
                    madeByText.alpha = 0f;
                    madeByText.DOFade(1f, 1f);
                }));
        });


        seq.Play();
    }
}
