using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    public enum CellContentTypes
    {
        EMPTY,
        Dot1,
        Dot2,
        Dot3
    }

    [SerializeField] SpriteRenderer _renderer;
    [SerializeField] CellContentTypes _content = CellContentTypes.EMPTY;
    [SerializeField] bool _isBlocked;
    [SerializeField] Vector2Int _gridPosition;
    private Dot _dotHeld;

    [HideInInspector] public CellContentTypes Content { get => _content; }
    [HideInInspector] public bool IsBlocked { get => _isBlocked; }
    [HideInInspector] public Vector2Int GridPosition { get => _gridPosition; }

    public void Init(bool gray, int x, int y)
    {
        _gridPosition = new Vector2Int(x, y);
        if (Random.Range(0, 4) == 0)
        {
            _renderer.color = Color.black;
            _isBlocked = true;
        }
        else if (gray)
        {
            _renderer.color = Color.gray;
        }
    }

    public void AttachDot(Dot dot)
    {
        _dotHeld = dot;
        _content = dot.Type;
    }

    public void RemoveDot()
    {
        _content = CellContentTypes.EMPTY;
        Destroy(_dotHeld.gameObject);
    }
}
