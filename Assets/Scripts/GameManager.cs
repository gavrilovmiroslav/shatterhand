using Arrow;

using Cinemachine;

using DG.Tweening;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public enum TurnPhase
{
    Left,
    Right,
    Upkeep,
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public CardDetails HintCard;
    public CardDetails SelectedCard;

    public CinemachineVirtualCamera FrontCamera;

    public CinemachineVirtualCamera LeftCamera;
    public PlayerHand LeftPlayerHand;

    public CinemachineVirtualCamera RightCamera;
    public PlayerHand RightPlayerHand;

    public TMPro.TextMeshPro TurnCounter;
    public TMPro.TextMeshPro LeftScoreCounter;
    public TMPro.TextMeshPro RightScoreCounter;

    public TMPro.TextMeshPro LeftSacrificeCounter;
    public TMPro.TextMeshPro RightSacrificeCounter;

    public GameObject EndTurnButton;

    [Space(10)]
    public Deck LeftDeck;
    public BoardTile LeftSpawnTile;

    [Space(10)]
    public Deck RightDeck;
    public BoardTile RightSpawnTile;

    public List<Piece> LeftHand;
    public List<Piece> LeftDiscard;

    public List<Piece> RightHand;
    public List<Piece> RightDiscard;

    public TurnPhase Phase;
    public PieceInstance SelectedPiece = new();

    public GameObject EffectHint;

    public int Turn = 1;
    public int LeftScore = 0;
    public int RightScore = 0;

    public int LeftSacrifice = 0;
    public int RightSacrifice = 0;

    public Vector3 HandPivotUp;
    public Vector3 HandPivotDown;

    public Vector3 HintPivotUp;
    public Vector3 HintPivotDown;

    private bool Done = false;
    private HashSet<BoardTile> m_SpawnedTiles = new();

    public IEnumerator RunAbilitiesFor<T>(BoardTile tile) where T: CardEffectTrigger
    {
        if (tile.Piece != null && tile.Piece.Abilities != null)
        {
            foreach (var ability in tile.Piece.Abilities)
            {
                if (ability.Trigger is T)
                {
                    yield return ability.Effect.Run(tile);
                }
            }
        }
    }

    public IEnumerator RunAbilitiesFor<T>() where T: CardEffectTrigger
    {
        foreach (var (_, tile) in Board.Instance.m_Tiles)
        {
            yield return RunAbilitiesFor<T>(tile);
        }
    }

    public void TurnPhaseDone()
    {
        Done = true;
    }

    public static void ShuffleDeck(ref List<Piece> collection)
    {
        collection.Shuffle(4, 10, 0.5f);
        collection.Shuffle(0, 3, 1.0f);
    }

    public static void Transfer(ref List<Piece> from, ref List<Piece> to, int number = 1)
    {
        for (int i = 0; i < number; i++)
        {
            if (from.Count > 0)
            {
                to.Add(from[0]);
                from.RemoveAt(0);
            }
        }
    }

    public void Start()
    {
        Instance = this;

        HintPivotDown = HintCard.transform.localPosition;
        HintPivotUp = HintCard.transform.localPosition + new Vector3(0.7f, 7.0f, 0);

        HintCard.transform.position = HintPivotDown;

        HandPivotUp = LeftPlayerHand.transform.localPosition;
        HandPivotDown = LeftPlayerHand.transform.localPosition - new Vector3(0, 5, 0);

        LeftPlayerHand.transform.position = HandPivotDown;
        RightPlayerHand.transform.position = HandPivotDown;

        LeftSpawnTile.SetPiece(LeftDeck.Hero, TurnPhase.Left);
        RightSpawnTile.SetPiece(RightDeck.Hero, TurnPhase.Right);

        LeftPlayerHand.Load(LeftDeck);
        RightPlayerHand.Load(RightDeck);

        StartCoroutine(TurnStart());
    }

    private IEnumerator FocusMainCamera()
    {
        FrontCamera.Priority = 1;
        RightCamera.Priority = 0;
        LeftCamera.Priority = 0;
        yield return new WaitForSeconds(0.2f);
    }

    private IEnumerator FocusLeftCamera()
    {
        FrontCamera.Priority = 0;
        RightCamera.Priority = 0;
        LeftCamera.Priority = 1;
        yield return new WaitForSeconds(0.2f);
    }

    private IEnumerator FocusRightCamera()
    {
        FrontCamera.Priority = 0;
        LeftCamera.Priority = 0;
        RightCamera.Priority = 1;
        yield return new WaitForSeconds(0.2f);
    }

    public IEnumerator TurnStart()
    {
        yield return StartLeftPlayerTurn();
    }

    public IEnumerator StartLeftPlayerTurn()
    {
        Phase = TurnPhase.Left;
        LeftPlayerHand.gameObject.SetActive(true);
        LeftPlayerHand.transform.DOLocalMove(HandPivotUp, 0.5f);
        Board.Instance.UpdateValues(Phase);
        yield return FocusLeftCamera();
    }

    public IEnumerator StartRightPlayerTurn()
    {
        Phase = TurnPhase.Right;
        RightPlayerHand.gameObject.SetActive(true);
        RightPlayerHand.transform.DOLocalMove(HandPivotUp, 0.5f);
        LeftPlayerHand.transform.DOLocalMove(HandPivotDown, 0.5f);
        Board.Instance.UpdateValues(Phase);
        yield return FocusRightCamera();
    }

    public IEnumerator StartUpkeep()
    {
        LeftPlayerHand.transform.DOLocalMove(HandPivotDown, 0.5f);
        RightPlayerHand.transform.DOLocalMove(HandPivotDown, 0.5f);

        if (LeftSacrifice > 0) LeftSacrifice--;
        if (RightSacrifice > 0) RightSacrifice--;

        foreach (var (_, tile) in Board.Instance.m_Tiles)
        {
            if (!m_SpawnedTiles.Contains(tile))
            {
                yield return RunAbilitiesFor<Trigger_OnTurnEnd>(tile);
            }
        }

        Phase = TurnPhase.Upkeep;
        yield return FocusMainCamera();

        if (Turn == 9)
        {
            Turn = 10;
            LeftScore += LeftSacrifice;
            LeftSacrifice = 0;
            RightScore += RightSacrifice;
            RightSacrifice = 0;
            
            EndTurnButton.GetComponent<TMPro.TextMeshPro>().text = "Restart";
            yield return null;
        }
        else
        {
            Turn++;

            var arr = Board.Instance.GetComponentsInChildren<BoardTile>();
            arr.Shuffle(0, arr.Length, 1.0f);

            foreach (var tile in arr)
            {
                if (tile.BreakOn == Turn + 1)
                {
                    tile.GetComponent<SpriteRenderer>().color = Color.red;
                    tile.AddComponent<Shake>();
                    yield return RunAbilitiesFor<Trigger_OnDistress>(tile);
                }
                else if (tile.BreakOn == Turn)
                {
                    if (tile.Piece != null)
                    {
                        if (tile.Phase == TurnPhase.Left)
                        {
                            LeftSacrifice++;
                        }
                        else if (tile.Phase == TurnPhase.Right)
                        {
                            RightSacrifice++;
                        }
                    }

                    Destroy(GameObject.Instantiate(VFXRegistry.Instance.DestroyField, tile.gameObject.transform.position + new Vector3(0, 0, -0.1f), Quaternion.identity), 2.0f);
                    yield return RunAbilitiesFor<Trigger_OnUnspawned>(tile);
                    tile.Piece.Set((PieceInstance)null);
                    tile.gameObject.SetActive(false);
                    yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.15f));
                }
            }
            yield return new WaitForSeconds(0.5f);
            yield return StartLeftPlayerTurn();
        }
    }

    public void Update()
    {
        TurnCounter.text = $"Turn: {Turn}";
        LeftScoreCounter.text = $"{LeftScore}";
        RightScoreCounter.text = $"{RightScore}";

        if (LeftSacrifice > 0)
        {
            LeftSacrificeCounter.text = $"+{LeftSacrifice}";
        }
        else
        {
            LeftSacrificeCounter.text = "";
        }

        if (RightSacrifice > 0)
        {
            RightSacrificeCounter.text = $"+{RightSacrifice}";
        }
        else
        {
            RightSacrificeCounter.text = "";
        }

        if (Turn < 10)
        {
            this.LeftScore = 0;
            this.RightScore = 0;

            foreach (var (_, tile) in Board.Instance.m_Tiles)
            {
                if (tile != null && tile.EndGame && tile.Piece != null)
                {
                    if (tile.Phase == TurnPhase.Left)
                    {
                        this.LeftScore += tile.Piece.Cost;
                    }
                    else if (tile.Phase == TurnPhase.Right)
                    {
                        this.RightScore += tile.Piece.Cost;
                    }
                }
            }
        }

        this.LeftScoreCounter.text = $"{this.LeftScore}";
        this.RightScoreCounter.text = $"{this.RightScore}";

        if (Input.GetMouseButtonDown(1) && SelectedPiece != null)
        {
            CancelSelection();
        }

        if (Done)
        {
            Done = false;
            StartCoroutine(TurnEnd());
        }
    }

    public void CancelSelection()
    {
        Board.CancelPutUnit();
        ClearSelectedCard();
        SelectedCard.transform.DOLocalMove(HintPivotDown, 0.5f);
        if (this.Phase == TurnPhase.Left)
        {
            LeftPlayerHand.transform.DOLocalMove(HandPivotUp, 0.5f);
        }
        else if (this.Phase == TurnPhase.Right)
        {
            RightPlayerHand.transform.DOLocalMove(HandPivotUp, 0.5f);
        }
        Board.Instance.ClearSelectionFilter();
    }

    public void RegisterSpawn(BoardTile tile)
    {
        m_SpawnedTiles.Add(tile);
    }

    public IEnumerator TurnEnd()
    {
        foreach (var tile in m_SpawnedTiles)
        {
            yield return RunAbilitiesFor<Trigger_OnSpawned>(tile);
            if (tile.EndGame)
            {
                yield return RunAbilitiesFor<Trigger_OnSpawnedSafe>(tile);
            }
            else
            {
                yield return RunAbilitiesFor<Trigger_OnSpawnedUnsafe>(tile);
            }

            if (!Board.Instance.GetNeighbors(tile).Where(t => t != null && (!t.Piece.Empty && t.Phase == tile.Phase)).Any())
            {
                yield return RunAbilitiesFor<Trigger_OnSpawnedAlone>(tile);
            }
        }

        m_SpawnedTiles.Clear();

        switch (Phase)
        {
            case TurnPhase.Left:
                yield return StartRightPlayerTurn();
                break;
            case TurnPhase.Right:
                yield return StartUpkeep();
                break;
            case TurnPhase.Upkeep:
                yield return null;
                break;
        }
    }

    public void ClearSelectedCard()
    {
        SelectedPiece.Set((PieceInstance)null);
    }

    private List<GameObject> m_EffectHints = new();

    public void PreviewSelectedCardEffect(BoardTile tile)
    {
        if (tile != null && !SelectedPiece.Empty && tile.m_Edge.gameObject.activeSelf)
        {
            for (int i = 0; i < 49; i++)
            {
                if (SelectedPiece.Effect[i])
                {
                    var dx = i % 7 - 3;
                    var dy = i / 7 - 3;
                    var target = Board.Instance.GetAtOffset(tile, ((Phase == TurnPhase.Left ? 1 : -1) * dx, -dy));
                    if (target != null)
                    {
                        var hint = GameObject.Instantiate(EffectHint, target.transform.position, Quaternion.identity);
                        m_EffectHints.Add(hint);
                    }
                }
            }
        }
        
        if (tile == null)
        {
            if (m_EffectHints.Count > 0)
            {
                foreach (var hint in m_EffectHints)
                {
                    Destroy(hint);
                }

                m_EffectHints.Clear();
            }
        }
    }

    public void HardEndTurn()
    {
        if (Turn > 9)
        {
            SceneManager.LoadScene(0);
        }

        if (Phase == TurnPhase.Left)
        {
            for (int i = 0; i < LeftPlayerHand.Cards.Count; i++)
                LeftPlayerHand.DiscardCard(0);
            LeftPlayerHand.FillHand();
            LeftSacrifice += 2;
        }
        else if (Phase == TurnPhase.Right)
        {
            for (int i = 0; i < RightPlayerHand.Cards.Count; i++)
                RightPlayerHand.DiscardCard(0);
            RightPlayerHand.FillHand();
            RightSacrifice += 2;
        }
        
        TurnPhaseDone();
    }

    public void PreviewBoardTile(BoardTile tile)
    {
        if (tile != null)
        {
            HintCard.gameObject.SetActive(true);
            HintCard.Phase = tile.Phase;
            HintCard.Piece.Set(tile.Piece);
            HintCard.transform.position = tile.transform.position;
        }
        else
        {
            HintCard.gameObject.SetActive(false);
        }
    }
}
