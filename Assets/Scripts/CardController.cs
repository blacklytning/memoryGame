using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class CardsController : MonoBehaviour
{
    [SerializeField] Card cardPrefab;
    [SerializeField] Transform gridTransform;
    [SerializeField] Sprite[] sprites;
    [Header("Optional UI")]
    [SerializeField] GameObject winPanel;

    private List<Sprite> spritePairs;

    Card firstSelected;
    Card secondSelected;

    int matchCounts;
    bool winShown = false;

    private void Start()
    {
        PrepareSprites();
        CreateCards();
        SetupWinPanel();
    }

    Button playAgainButton;

    void SetupWinPanel()
    {
        if (winPanel == null) return;

        // Look for any Button inside the panel (include inactive children)
        playAgainButton = winPanel.GetComponentInChildren<UnityEngine.UI.Button>(true);
        if (playAgainButton != null)
        {
            // Remove previous listeners so the inspector or prefab doesn't duplicate behavior
            playAgainButton.onClick.RemoveAllListeners();
            playAgainButton.onClick.AddListener(() => {
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            });
        }
    }

    void OnDestroy()
    {
        if (playAgainButton != null)
        {
            playAgainButton.onClick.RemoveAllListeners();
        }
    }

    private void PrepareSprites()
    {
        spritePairs = new List<Sprite>();
        for (int i = 0; i < sprites.Length; i++)
        {
            // adding sprite 2 times to make it pair
            spritePairs.Add(sprites[i]);
            spritePairs.Add(sprites[i]);
        }

        ShuffleSprites(spritePairs);
    }

    void CreateCards()
    {
        for (int i = 0; i < spritePairs.Count; i++)
        {
            Card card = Instantiate(cardPrefab, gridTransform);
            card.SetIconSprite(spritePairs[i]);
            card.controller = this;
        }
    }

    public void SetSelected(Card card)
    {
        if (card.isSelected == false)
        {
            card.Show();

            if (firstSelected == null)
            {
                firstSelected = card;
                return;
            }
            if (secondSelected == null)
            {
                secondSelected = card;

                StartCoroutine(CheckMatching(firstSelected, secondSelected));
                firstSelected = null;
                secondSelected = null;
            }
        }
    }

    IEnumerator CheckMatching(Card a, Card b)
    {
        yield return new WaitForSeconds(0.3f);

        if (a.iconSprite == b.iconSprite)
        {
            matchCounts++;
            if (matchCounts >= sprites.Length)
            {
                Debug.Log("All matched!");
                PrimeTween.Sequence.Create()
                    .Chain(PrimeTween.Tween.Scale(gridTransform, Vector3.one * 1.2f, 0.2f, ease: PrimeTween.Ease.OutBack))
                    .Chain(PrimeTween.Tween.Scale(gridTransform, Vector3.one, 0.1f));
                if (winPanel != null && !winShown)
                {
                    winShown = true;
                    winPanel.SetActive(true);
                }
            }
        }
        else
        {
            a.Hide();
            b.Hide();
        }

    }

    // Method to shuffle a list of sprites
    void ShuffleSprites(List<Sprite> spriteList)
    {
        for (int i = spriteList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            // Swap the elements at i and randomIndex
            Sprite temp = spriteList[i];
            spriteList[i] = spriteList[randomIndex];
            spriteList[randomIndex] = temp;
        }
    }
}
