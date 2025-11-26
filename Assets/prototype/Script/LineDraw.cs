using UnityEngine;

public class LineDraw : MonoBehaviour
{
    [SerializeField] private LineRenderer rend;
    [SerializeField] private Camera cam;

    private int _posCount = 0;
    private float _interval = 0.1f;

    private void Update()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButton(0))  SetPosition(mousePos);
        else if (Input.GetMouseButtonUp(0)) _posCount = 0;
    }

    private void SetPosition(Vector2 pos)
    {
        if (!PosCheck(pos)) return;

        _posCount++;
        rend.positionCount = _posCount;
        rend.SetPosition(_posCount - 1, pos);
    }

    private bool PosCheck(Vector2 pos)
    {
        if (_posCount == 0) return true;

        float distance = Vector2.Distance(rend.GetPosition(_posCount - 1), pos);
        if (distance > _interval) return true;
        else return false;

    }
}