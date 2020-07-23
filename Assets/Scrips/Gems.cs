using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gems : MonoBehaviour
{
    private static Color selectedColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
    private static Gems previousSelected = null;
    private SpriteRenderer spriteRenderer;
    private bool isSelected = false;
    public AudioSource audioSource;
    public AudioClip[] audioClips;
    public int selectGems;
    private Vector2[] direcciones = new Vector2[]
    {
        Vector2.up,
        Vector2.right,
        Vector2.down,
        Vector2.left
    };

    public int id;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void SelectGems()
    {      
        isSelected = true;
        spriteRenderer.color = selectedColor;
        previousSelected = gameObject.GetComponent<Gems>();
        audioSource.clip = audioClips[selectGems];
        audioSource.enabled = true;
    }

    private void DeselectGems()
    {
        isSelected = false;
        spriteRenderer.color = Color.white;
        previousSelected = null;
    }

    private void OnMouseDown()
    {
        if (spriteRenderer.sprite == null || BoardManager.sharedInstance.isShifting)
        {
            return;
        }
        else
        {
            if (isSelected)
            {
                audioSource.Play();
                DeselectGems();
            }
            else
            {
                if (previousSelected == null)
                {
                    SelectGems();
                    audioSource.enabled = false;

                }
                else
                {
                    if (CanSwipe())
                    {
                       
                        SwapSprite(previousSelected);
                        
                        previousSelected.FindAllMatches();
                        previousSelected.DeselectGems();
                        FindAllMatches();
                        Pontuacao.sharedInstance.MovimentacaoContador--;
                        
                    }
                    else
                    {
                        previousSelected.DeselectGems();
                     
                    }
                }
            }
        }
    }

    public void SwapSprite(Gems newGems)
    {
        if (spriteRenderer.sprite == newGems.GetComponent<SpriteRenderer>().sprite) 
        {
            return;
        }

            Sprite temp = this.spriteRenderer.sprite;
            this.spriteRenderer.sprite = newGems.spriteRenderer.sprite;
            newGems.spriteRenderer.sprite = temp;
            int nid = this.id;
            this.id = newGems.id;
            newGems.id = nid;

    }

    private GameObject GetNeighbor(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direction);
        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }
    }

    private List<GameObject> GetAllNeighbors()
    {
        List<GameObject> neighbors = new List<GameObject>();

        foreach(Vector2 direction in direcciones)
        {
            neighbors.Add(GetNeighbor(direction));

        }

        return neighbors;
    }

    private bool CanSwipe()
    {
        return GetAllNeighbors().Contains(previousSelected.gameObject);
    }

    private List<GameObject> FindMatch(Vector2 direction)
    {
        List<GameObject> matchingGems = new List<GameObject>();

        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direction);
        while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == spriteRenderer.sprite)
        {
            matchingGems.Add(hit.collider.gameObject);
            hit = Physics2D.Raycast(hit.collider.transform.position, direction);
        }
        return matchingGems;
    }

    private bool ClearMatch(Vector2[] directions)
    {
        List<GameObject> matchingGems = new List<GameObject>();
        foreach(Vector2 direction in directions)
        {
            matchingGems.AddRange(FindMatch(direction));
        }
        if (matchingGems.Count >= BoardManager.minGemsToMatch)
        {
            foreach(GameObject gems in matchingGems)
            {
                gems.GetComponent<SpriteRenderer>().sprite = null;
            }
            return true;
        }
        else
        {
            return false;
        }

    }

    public void FindAllMatches()
    {
        if (spriteRenderer.sprite == null)
        {
            return;
        }

        bool hMatch = ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
        bool vMatch = ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });

        if (hMatch || vMatch)
        {
            spriteRenderer.sprite = null;
            StopCoroutine(BoardManager.sharedInstance.FindNullGems());
            StartCoroutine(BoardManager.sharedInstance.FindNullGems());
        }
    }
}