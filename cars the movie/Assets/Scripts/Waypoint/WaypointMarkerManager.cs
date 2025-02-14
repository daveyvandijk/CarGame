using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class WaypointMarkerManager : MonoBehaviour
{
    public GameObject leftMarker;
    public GameObject rightMarker;
    public BoxCollider boxCollider;

    private GameObject prefab;

    public void InitializeMarkers(GameObject prefab, BoxCollider collider)
    {
        this.prefab = prefab;
        boxCollider = collider;

        // Verwijder bestaande markers als ze bestaan
        if (leftMarker != null) DestroyImmediate(leftMarker);
        if (rightMarker != null) DestroyImmediate(rightMarker);

        // Instantiate markers
        leftMarker = Instantiate(prefab, transform);
        rightMarker = Instantiate(prefab, transform);

        UpdateMarkerPositions();
    }

    private void Update()
    {
        // Controleer continu of de markers gesynchroniseerd zijn met de collider
        if (boxCollider != null)
        {
            UpdateMarkerPositions();
        }
    }

    private void UpdateMarkerPositions()
    {
        if (boxCollider == null) return;

        // Bereken de nieuwe posities van de markers
        Vector3 colliderSize = boxCollider.size;
        Vector3 colliderCenter = boxCollider.center;

        Vector3 leftPosition = transform.TransformPoint(new Vector3(-colliderSize.x / 2, colliderCenter.y, colliderCenter.z));
        Vector3 rightPosition = transform.TransformPoint(new Vector3(colliderSize.x / 2, colliderCenter.y, colliderCenter.z));

        // Pas de posities van de markers aan
        leftMarker.transform.position = leftPosition;
        rightMarker.transform.position = rightPosition;

        // Zorg ervoor dat ze altijd recht staan
        leftMarker.transform.rotation = Quaternion.identity;
        rightMarker.transform.rotation = Quaternion.identity;
    }

}
