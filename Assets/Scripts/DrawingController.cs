using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class DrawingController : MonoBehaviour
{
    private bool isDrawing = false;
    private LineRenderer currentLine;
    private int vertexCount = 0;

    public LineRenderer linePrefab; // LineRenderer object
    public Transform drawingParent; // The parent object under which to put the drawn lines

    private bool canStartDrawing = false;

    void Update()
    {
        // only draw within the allowed area
        if (Input.GetMouseButtonDown(0) && IsMouseOverDrawingArea())
        {
            canStartDrawing = true;
            StartDrawing();
        }
        else if (Input.GetMouseButton(0) && isDrawing && canStartDrawing)
        {
            ContinueDrawing();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            canStartDrawing = false;
            StopDrawing();
        }
    }

    private bool IsMouseOverDrawingArea()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        // list to store raycast results
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // Check if any of the results are under the drawing area container
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.transform.IsChildOf(drawingParent))
            {
                return true;
            }
        }

        return false;
    }

    private void StartDrawing()
    {
        isDrawing = true;
        currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        currentLine.transform.SetParent(drawingParent);
        vertexCount = 0;
    }

    private void ContinueDrawing()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        if (currentLine != null)
        {
            currentLine.positionCount = vertexCount + 1;
            currentLine.SetPosition(vertexCount, mousePos);
            vertexCount++;
        }
    }

    private void StopDrawing()
    {
        isDrawing = false;
        currentLine = null;
    }
}
