using Unity.Collections;

using UnityEngine;

public class BoardTile : MonoBehaviour
{
    private static Color LEFT_COLOR = new Color(163.0f / 255.0f, 1.0f, 1.0f);
    private static Color RIGHT_COLOR = new Color(174.0f / 255.0f, 57.0f / 255.0f, 197.0f / 255.0f);

    private (int, int) m_Coords;
    private SpriteRenderer m_Back;

    public bool EndGame = false;
    public int BreakOn = 100;
    public Board Board { get; set; }
    public Color m_StartingTileColor;
    public Transform m_Edge;
    public Transform m_Selection;
    public SpriteRenderer m_Content;
    public PieceInstance Piece = new();

    public Transform m_Points;
    public TMPro.TextMeshPro m_PointsText;
    public TMPro.TextMeshPro m_ScoreText;
    public int Points = 0;
    private int m_PointsCache = 0;
    
    public (int, int) Coords => m_Coords;

    [ReadOnly] public string Coordinates;
    public TurnPhase Phase = TurnPhase.Upkeep;

    public void Awake()
    {
        m_Content = transform.GetChild(1).GetComponent<SpriteRenderer>();
        m_Edge = transform.GetChild(2);
        m_Selection = transform.GetChild(3);

        m_Points = transform.GetChild(4);
        m_PointsText = m_Points.GetComponentInChildren<TMPro.TextMeshPro>();

        m_ScoreText = transform.GetChild(5).GetComponentInChildren<TMPro.TextMeshPro>();

        m_Back = GetComponent<SpriteRenderer>();
        m_StartingTileColor = m_Back.color;

        var x = int.Parse(this.name);
        var y = int.Parse(this.transform.parent.name);
        m_Coords = (x, y);

        Coordinates = $"{m_Coords}";
    }

    public void ColorEdge(Color color)
    {
        var renderer = m_Selection.gameObject.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = color;
        }
    }

    public void Update()
    {
        UpdatePoints();
        UpdateScore();
    }

    private void UpdatePoints()
    {
        if (m_PointsCache != Points)
        {
            m_PointsText.text = "";
            m_Points.gameObject.SetActive(false);

            if (Points > 0 && Piece.Empty)
            {
                m_PointsText.text = $"{Points}";
                m_PointsText.color = Color.blue;

                m_Points.gameObject.SetActive(true);
            }
        }

        m_PointsCache = Points;
    }

    private void UpdateScore()
    {
        if (this.EndGame)
        {
            if (Piece.Empty)
            {
                m_ScoreText.text = "";
            }
            else
            {
                m_ScoreText.text = $"{Piece.Cost}";
            }
        }
    }

    public void SetPiece(Piece piece, TurnPhase phase)
    {
        m_PointsText.text = "";
        Piece.Set(piece);
        m_Content.gameObject.SetActive(true);
        Phase = phase;
        m_Content.sprite = Piece.Image;
        ColorEdge(phase == TurnPhase.Left ? LEFT_COLOR : RIGHT_COLOR);
    }

    public void SetPiece(PieceInstance piece, TurnPhase phase)
    {
        m_PointsText.text = "";
        Piece.Set(piece);
        m_Content.gameObject.SetActive(true);
        Phase = phase;
        m_Content.sprite = Piece.Image;
        ColorEdge(phase == TurnPhase.Left ? LEFT_COLOR : RIGHT_COLOR);
    }

    public void UnsetPiece()
    {
        Piece.Set((PieceInstance)null);
        m_Content.gameObject.SetActive(false);
    }

    public void Tint(Color c)
    {
        m_Back.color = Color.Lerp(m_StartingTileColor, c, 0.5f);
    }

    public void Untint()
    {
        m_Back.color = m_StartingTileColor;
    }

#if !UNITY_ANDROID
    public void OnMouseEnter()
    {
        if (this.Piece.Empty)
        {
            GameManager.Instance.PreviewSelectedCardEffect(this);
        }
        else
        {
            GameManager.Instance.PreviewBoardTile(this);
        }
    }

    public void OnMouseExit()
    {
        GameManager.Instance.PreviewSelectedCardEffect(null);
        GameManager.Instance.PreviewBoardTile(null);
    }

    public void OnMouseUpAsButton()
    {
        Board.ToggleSelection(this);
    }
#else
    static BoardTile SelectedBoardTile = null;
    static bool SelectionConfirmed = false;

    public void OnMouseDown()
    {
        if (SelectedBoardTile != null)
        {
            if (SelectedBoardTile != this)
            {
                SelectedBoardTile = null;
                SelectionConfirmed = false;
            }
            else
            {
                SelectionConfirmed = true;
            }
        }

        GameManager.Instance.PreviewBoardTile(null);
        GameManager.Instance.PreviewSelectedCardEffect(null);

        if (this.Piece.Empty)
        {
            SelectedBoardTile = this;
            GameManager.Instance.PreviewSelectedCardEffect(this);
        }
        else
        {
            GameManager.Instance.PreviewBoardTile(this);
        }
    }

    public void OnMouseUpAsButton()
    {
        if (SelectionConfirmed)
        {
            Board.ToggleSelection(this);
        }
    }
#endif

    public void ShowSelectionEdge()
    {
        m_Edge.gameObject.SetActive(true);
    }

    public void HideSelectionEdge()
    {
        m_Edge.gameObject.SetActive(false);
    }

    public void ShowSelected()
    {
        m_Selection.gameObject.SetActive(true);
    }

    public void HideSelected()
    {
        m_Selection.gameObject.SetActive(false);
    }
}
