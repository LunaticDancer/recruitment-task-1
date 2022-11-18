using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [SerializeField] GridCell.CellContentTypes _type;
    [SerializeField] float _duration = 1;

    [HideInInspector] public GridCell.CellContentTypes Type { get => _type; }

    public void StartMoving(GridCell target)
    {
        StartCoroutine(MoveToSpot(target));
    }

    private IEnumerator MoveToSpot(GridCell target)
    {
        Vector3 goToPosition = target.transform.position;
        Vector3 startPosition = transform.position;
        target.AttachDot(this);
        float timer = 0;
        while (timer < _duration)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, goToPosition, timer / _duration);
            yield return null;
        }
        transform.position = goToPosition;
    }
}
