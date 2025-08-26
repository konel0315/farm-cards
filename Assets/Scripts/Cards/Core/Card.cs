using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Card : MonoBehaviour
{
    [Header("Card Container")]
    [SerializeField] private GameObject CardContainer;

    [Header("Card Objects")]
    [SerializeField] private GameObject scenarioCard;
    [SerializeField] private Image screenImage;
    [SerializeField] private TMP_Text scenarioText;

    [SerializeField] private GameObject choiceCard;
    [SerializeField] private TMP_Text choiceText;
    [SerializeField] private Image choiceImage;
    [SerializeField] private GameObject back;

    [Header("Card Data")]
    public List<CardData> currentCards = new List<CardData>();
    public List<CardOption> choicePool = new List<CardOption>(); // 최대 4개 랜덤 카드 저장
    private int choiceIndex = 0;

    private bool isFlipping = false;
    private ChoiceCardTilt followMouse;
    
    public event System.Action OnCardChoiceCompleted;

    
    private void Update()
    {
        if (choiceCard.activeSelf && Input.GetMouseButtonDown(0))
        {
            CheckChoice();
        }
    }


    public void CardSetup(CardGroup group)
    {
        screenImage.sprite = Resources.Load<Sprite>($"Arts/Cards/{group.TitleImage}");
        scenarioText.text = group.Name + "\n" + group.Description;

        currentCards.Add(new CardData());

        choicePool.Clear();
        if (group.Options == null || group.Options.Count == 0)
            return;

        List<CardOption> optionsCopy = new List<CardOption>(group.Options);
        int maxCount = Mathf.Min(4, optionsCopy.Count);

        for (int i = 0; i < maxCount; i++)
        {
            int randIndex = Random.Range(0, optionsCopy.Count);
            choicePool.Add(optionsCopy[randIndex]);
            optionsCopy.RemoveAt(randIndex);
        }

        choiceIndex = 0;
        UpdateChoiceCard();
    }

    public void OpenCardWithAnimation()
    {
        
        OpenCard();
        StartCoroutine(SpawnAndFlip());
    }


    public void OpenCard()
    {
        if (CardContainer != null)
            CardContainer.transform.localPosition = Vector3.zero;

        back.SetActive(true);
        scenarioCard.SetActive(false);
        choiceCard.SetActive(false);

        scenarioCard.GetComponent<Button>().onClick.AddListener(OnScenarioClicked);
        followMouse = choiceCard.AddComponent<ChoiceCardTilt>();
    }

    private void closeCard()
    {
        back.SetActive(false);
        scenarioCard.SetActive(false);
        choiceCard.SetActive(false);
    }

    private void UpdateChoiceCard()
    {
        if (choicePool == null || choicePool.Count == 0 || choiceIndex >= choicePool.Count)
            return;

        CardOption option = choicePool[choiceIndex];
        CardVariant variant = null;

        if (option.Variants != null && option.Variants.Count > 0)
        {
            float total = 0f;
            foreach (var v in option.Variants)
                total += v.Probability;

            float r = Random.Range(0f, total);
            float sum = 0f;
            foreach (var v in option.Variants)
            {
                sum += v.Probability;
                if (r <= sum)
                {
                    variant = v;
                    break;
                }
            }
        }

        choiceText.text = option.OptionText;
        if (!string.IsNullOrEmpty(option.OptionImage))
            choiceImage.sprite = Resources.Load<Sprite>($"Arts/Cards/{option.OptionImage}");
    }

    private IEnumerator SpawnAndFlip()
    {
        if (isFlipping) yield break;
        isFlipping = true;

        back.SetActive(true);
        scenarioCard.SetActive(false);
        choiceCard.SetActive(false);

        yield return new WaitForSeconds(0.2f);

        yield return back.transform.DORotate(new Vector3(0, 90, 0), 0.25f).WaitForCompletion();

        scenarioCard.transform.localEulerAngles = new Vector3(0, -90, 0);
        scenarioCard.SetActive(true);
        back.SetActive(false);
        
        yield return scenarioCard.transform.DORotate(Vector3.zero, 0.25f).WaitForCompletion();

        
        isFlipping = false;
    }

    private IEnumerator Flip()
    {
        if (isFlipping) yield break;
        isFlipping = true;

        yield return scenarioCard.transform.DORotate(new Vector3(0, 90, 0), 0.25f).WaitForCompletion();

        choiceCard.transform.localEulerAngles = new Vector3(0, -90, 0);
        choiceCard.SetActive(true);
        scenarioCard.SetActive(false);

        yield return choiceCard.transform.DORotate(Vector3.zero, 0.25f).WaitForCompletion();

        isFlipping = false;
    }

    private void OnScenarioClicked()
    {
        StartCoroutine(Flip());
    }

    private IEnumerator DoChoiceFall(float zRotationOffset, bool goNext)
    {
        var tilt = choiceCard.GetComponent<ChoiceCardTilt>();
        if (tilt) Destroy(tilt);

        Sequence seq = DOTween.Sequence();
        seq.Join(choiceCard.transform.DOLocalMoveY(choiceCard.transform.localPosition.y - 600f, 0.6f));
        seq.Join(choiceCard.transform.DOLocalRotate(
            new Vector3(0, 0, choiceCard.transform.localEulerAngles.z + zRotationOffset), 0.6f));
        seq.Join(choiceCard.GetComponent<CanvasGroup>().DOFade(0f, 0.6f));

        yield return seq.WaitForCompletion();

        if (goNext)
        {
            choiceIndex++;
            UpdateChoiceCard();
            ResetChoiceCardTransform();
        }
        else
        {
            ResetChoiceCardTransform();
            command();
            closeCard();

            OnCardChoiceCompleted?.Invoke();
        }
    }

    private void ResetChoiceCardTransform()
    {
        choiceCard.transform.localPosition = new Vector3(0, -350f, 0);
        choiceCard.transform.localRotation = Quaternion.identity;
        choiceCard.GetComponent<CanvasGroup>().alpha = 1f;
        choiceCard.AddComponent<ChoiceCardTilt>();
    }

    private void command()
    {
        // 필요시 추가
    }

    private void CheckChoice()
    {
        bool isLastChoice = choiceIndex == choicePool.Count - 1;
        float rotZ = choiceCard.transform.localRotation.eulerAngles.z;
        if (rotZ > 180f) rotZ -= 360f;

        if (rotZ <= -10f)
        {
            if (!isLastChoice)
                StartCoroutine(DoChoiceFall(-45f, true));
            else
            {
                // 불가능 소리 처리
            }
        }
        else if (rotZ >= 10f)
        {
            StartCoroutine(DoChoiceFall(45f, false));
        }
    }
}
