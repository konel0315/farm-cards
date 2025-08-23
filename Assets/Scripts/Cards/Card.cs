using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Card : MonoBehaviour
{
    [Header("Card Objects")]
    [SerializeField] private GameObject scenarioCard;
    [SerializeField] private GameObject choiceCard;
    [SerializeField] private GameObject back;

    [SerializeField] private Text choiceText;

    [Header("Card Data")]
    public List<CardData> currentCards = new List<CardData>();
    private CardData activeData;

    private bool isFlipping = false;

    private ChoiceCardTilt followMouse;

    private void Start()
    {
        back.SetActive(true);
        scenarioCard.SetActive(false);
        choiceCard.SetActive(false);
        
        scenarioCard.GetComponent<Button>().onClick.AddListener(OnScenarioClicked);

        followMouse = choiceCard.AddComponent<ChoiceCardTilt>();
    }

    private IEnumerator SpawnAndFlip()
    {
        if (isFlipping) yield break;
        isFlipping = true;

        currentCards.Add(activeData);

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

    private void Update()
    {
        if (currentCards.Count <= 0 && !isFlipping)
        {
            StartCoroutine(SpawnAndFlip());
        }

        if (choiceCard.activeSelf && Input.GetMouseButtonDown(0))
        {
            CheckChoice();
        }
    }

    private void CheckChoice()
    {
        float rotZ = choiceCard.transform.localRotation.eulerAngles.z;
        if (rotZ > 180f) rotZ -= 360f;

        if (rotZ <= -5f)
        {
            //제작 예정
        }
        else if (rotZ >= 5f)
        {
            //제작 예정
        }
    }
}