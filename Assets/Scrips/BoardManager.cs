using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager sharedInstance;
    public const int minGemsToMatch = 2;

    public List<Sprite> prefabs = new List<Sprite>();
    public GameObject currentGems;
    public int xSize, ySize;
    public bool isShifting { get; set; }

    private GameObject[,] gems;
    private Gems SelectGems;

    void Start()
    {
        if (sharedInstance == null)
        {
            sharedInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        Vector2 offset = currentGems.GetComponent<BoxCollider2D>().size;
        CreateInitialBoard(offset);
    }

    private void CreateInitialBoard(Vector2 offset)
    {
        gems = new GameObject[xSize, ySize];

        float startX = this.transform.position.x;
        float startY = this.transform.position.y;

        int idX = -1;
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                GameObject newGems = Instantiate(currentGems, new Vector3(startX + (offset.x * i), startY + (offset.y * j), 0), currentGems.transform.rotation);
                newGems.name = string.Format("Gems[{0}],[{1}]", i, j);
                do
                {
                    idX = Random.Range(0, prefabs.Count);
                } while ((i > 0 && idX == gems[i - 1, j].GetComponent<Gems>().id) || (j > 0 && idX == gems[i, j - 1].GetComponent<Gems>().id));
                Sprite sprite = prefabs[idX];
                newGems.GetComponent<SpriteRenderer>().sprite = sprite;
                newGems.GetComponent<Gems>().id = idX;
                newGems.transform.parent = this.transform;
                gems[i, j] = newGems;
            }
        }
    }

    public IEnumerator FindNullGems()
    {
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                if (gems[i, j].GetComponent<SpriteRenderer>().sprite == null)
                {
                    yield return StartCoroutine(MakeGemsFall(i, j));
                    break;
                }
            }
        }
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                gems[i, j].GetComponent<Gems>().FindAllMatches();
            }
        }
    }

    private IEnumerator MakeGemsFall(int x, int yStart, float shiftDelay=0.05f)
    {
        isShifting = true;

        List<SpriteRenderer> renderes = new List<SpriteRenderer>();
        int nullGems = 0;

        for (int y = yStart; y < ySize; y++)
        {
            SpriteRenderer spriteRenderer = gems[x, y].GetComponent<SpriteRenderer>();
            if (spriteRenderer.sprite == null)
            {
                nullGems++;
            }
            renderes.Add(spriteRenderer);
        }
        for(int i = 0; i < nullGems; i++)
        {
            Pontuacao.sharedInstance.Pontos += 10;
            yield return new WaitForSeconds(shiftDelay);
            for(int j = 0; j < renderes.Count - 1; j++)
            {
                renderes[j].sprite = renderes[j + 1].sprite;
                renderes[j + 1].sprite = GetNewGems(x,ySize-1);
            }
        }
        isShifting = false;
    }

    private Sprite GetNewGems(int x, int y)
    {
        List<Sprite> possibleGems = new List<Sprite>();
        possibleGems.AddRange(prefabs);
        if (x > 0)
        {
            possibleGems.Remove(gems[x - 1, y].GetComponent<SpriteRenderer>().sprite);
        }
            if (x < xSize - 1)
            {
                possibleGems.Remove(gems[x + 1, y].GetComponent<SpriteRenderer>().sprite);
            }
                if (y > 0)
                {
                    possibleGems.Remove(gems[x, y - 1].GetComponent<SpriteRenderer>().sprite);
                }
        return possibleGems[Random.Range(0, possibleGems.Count)];
    }
}
