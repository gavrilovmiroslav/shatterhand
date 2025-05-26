using UnityEngine;

public class CardDetails : MonoBehaviour
{
    public bool IsSpecialLayer = false;
    private static readonly string[] s_Layers = new string[] {
        "Card1", "Card2", "Card3", "Card4", "Card5",
    };

    private static readonly string s_LayerSelected = "SelectedCard";

    public TurnPhase Phase = TurnPhase.Upkeep;
    public PieceInstance Piece = new();
    public int Index;
    public bool IsSelected;
    
    private SpriteRenderer Background;
    private string m_CachedPiece;
    private TMPro.TextMeshPro Cost;
    private TMPro.TextMeshPro Title;
    private TMPro.TextMeshPro Description;
    private SpriteRenderer Image;
    private Transform Grid;
    private int m_SelectedCardLayer;
    public Sprite ImageEmpty;
    public Sprite ImageFull;
    public Sprite ImagePositive;
    
    private void Start()
    {
        Background = this.GetComponent<SpriteRenderer>();
        Image = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
        Title = this.transform.GetChild(1).gameObject.GetComponent<TMPro.TextMeshPro>();
        Cost = this.transform.GetChild(2).GetComponent<TMPro.TextMeshPro>();
        Description = this.transform.GetChild(3).GetComponent<TMPro.TextMeshPro>();
        Grid = this.transform.GetChild(4);
        m_SelectedCardLayer = SortingLayer.NameToID("SelectedCard");
    }

    private void UpdateLayers()
    {
        if (IsSpecialLayer) return;
        var layer = SortingLayer.NameToID(s_Layers[Index]);
        if (IsSelected) { layer = m_SelectedCardLayer; }
        
        Background.sortingLayerID = layer;
        Image.sortingLayerID = layer;

        for (int i = 0; i < Image.transform.childCount; i++)
        {
            var renderer = Image.transform.GetChild(i).GetComponent<SpriteRenderer>();
            renderer.sortingLayerID = layer;
        }
        
        Title.sortingLayerID = layer;
        Title.GetComponentInChildren<SpriteRenderer>().sortingLayerID = layer;
        Cost.sortingLayerID = layer;
        Cost.GetComponentInChildren<TMPro.TextMeshPro>().sortingLayerID = layer;
        Description.sortingLayerID = layer;

        for (int i = 0; i < Grid.transform.childCount; i++)
        {
            var renderer = Grid.transform.GetChild(i).GetComponent<SpriteRenderer>();
            renderer.sortingLayerID = layer;
        }
    }

    private void OnMouseEnter()
    {
        if (GameManager.Instance.Phase == this.Phase)
        {
            IsSelected = true;
            var t = this.transform.localPosition;
            t.x = -0.12f + (this.Phase == TurnPhase.Left ? -0.28f : 0.28f);
            this.transform.localPosition = t;
        }
    }

    private void OnMouseExit()
    {
        if (GameManager.Instance.Phase == this.Phase)
        {
            IsSelected = false;
            var t = this.transform.localPosition;
            t.x = -0.12f;
            this.transform.localPosition = t;
        }
    }

    private void ChooseTileAndPutUnit(BoardTile tile)
    {
        Board.Instance.OnTileSelected.RemoveListener(ChooseTileAndPutUnit);
        BoardFunctions.ChooseNothing(Board.Instance);

        tile.SetPiece(this.Piece, this.Phase);
        GameManager.Instance.RegisterSpawn(tile);
        BoardFunctions.AnimateNewTargetsOfTile(tile, this);
        Board.Instance.UpdateValues(tile.Phase);
        this.transform.parent.GetComponent<PlayerHand>().UpdateHandAfterUsing(this);
    }

    void Update()
    {
        UpdateLayers();

        if (Piece.Name != m_CachedPiece)
        {
            this.gameObject.SetActive(Piece != null);

            if (Piece != null)
            {
                Image.sprite = Piece.Image;
                Title.text = Piece.Name;
                Cost.text = $"{Piece.Cost}";
                Description.text = Piece.Description;

                for (int i = 0; i < 49; i++)
                {
                    var dx = i % 7;
                    if (Phase == TurnPhase.Right)
                    {
                        dx = 6 - dx;
                    }
                    var dy = i / 7;
                    var j = dx + dy * 7;
                    var piece = Grid.GetChild(j);
                    var renderer = piece.GetComponent<SpriteRenderer>();
                    renderer.sprite = ImageEmpty;

                    if (i == 24)
                    {
                        renderer.sprite = ImageFull;
                    }
                    else if (Piece.Effect[i])
                    {
                        renderer.sprite = ImagePositive;
                    }
                }
            }

            m_CachedPiece = Piece.Name;
        }

        if (GameManager.Instance.Phase == this.Phase && IsSelected)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Board.CancelPutUnit();
                var selected = this.transform.parent.parent.GetChild(2);
                GameManager.Instance.SelectedPiece.Set(this.Piece);
                GameManager.Instance.SelectedPieceHolder = selected;
                selected.gameObject.SetActive(true);
                var cardDetails = selected.GetComponent<CardDetails>();
                cardDetails.Phase = this.Phase;
                cardDetails.Piece.Set(this.Piece);
                BoardFunctions.ChooseAllEmptyWithPoints(Board.Instance, this.Piece.Cost);
                Board.Instance.OnTileSelected.AddListener(ChooseTileAndPutUnit);
            }
        }
    }
}
