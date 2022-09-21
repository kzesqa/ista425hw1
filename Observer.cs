using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
    [Tooltip("Line of sight in local (object) space")]
    public Vector3 lineOfSight = Vector3.zero;

    private float time2 = 0f;
    public float displayMessage = 5.0f;
    public float rotationSpeed = 0.75f;
    public float velocityModifier = 1f;
    
    GameObject gc;
    GameObject hero;
    Vector3 currentOrientation;

    // direction to active target (in world space)
    Vector3 targetDir  = Vector3.zero;

    // A vector defining the surface normal in world space.
    public Vector3 getLineOfSight()
    {
        return transform.localToWorldMatrix * lineOfSight;
    }

    // Start is called before the first frame update
    void Start()
    {
        gc   = GameObject.FindGameObjectWithTag("GameController");
        hero = gc.GetComponent<GameManager> ().playerShip;
        Vector3 currentRotation = gameObject.transform.localEulerAngles;
        currentOrientation = Vector3.Normalize(Vector3.Cross(currentRotation, Vector3.left));
        rotationSpeed = rotationSpeed * Mathf.Deg2Rad;

        Debug.Log("My orientation is " + currentOrientation);
        Debug.Log("Angle sin: " + Mathf.Sin(0) + " Angle cos: " + Mathf.Cos(0));
    }

    // Update is called once per frame
    void Update()
    {
        time2 += Time.deltaTime;

        GameObject[] torpedos = GameObject.FindGameObjectsWithTag("Torpedo");
        GameObject currentFocus = hero;
        if(torpedos.Length > 0)
        {
            currentFocus = torpedos[0];
        }
        Vector3 currentFocusVector = Vector3.Normalize(currentFocus.transform.position - transform.position);
        float dotProduct = Vector3.Dot(currentFocusVector, currentOrientation);
        if(dotProduct >= 1)
        {
            dotProduct = 1;
        }
        else if(dotProduct <= -1)
        {
            dotProduct = -1;
        }
        float angle = Mathf.Acos(dotProduct);

        float splicedAngle = rotationSpeed;
        
        if (Vector3.Cross(currentOrientation, currentFocusVector).z < 0)
        {
            angle = -angle;
            splicedAngle = -splicedAngle;
        }
        
        if (Mathf.Abs(splicedAngle) >= Mathf.Abs(angle))
        {
            //Debug.Log("Switched by rotationspeed >= angle if statement: " + currentOrientation + " Current Focus Vector: " + currentFocusVector + " Angle: " + angle * Mathf.Rad2Deg);
            currentOrientation = currentFocusVector;
            splicedAngle = angle;
        }
        else
        {
            Vector3 newVectorCoord = currentOrientation.x * new Vector3(Mathf.Cos(splicedAngle), Mathf.Sin(splicedAngle)) + currentOrientation.y * new Vector3(-Mathf.Sin(splicedAngle), Mathf.Cos(splicedAngle));
            //Debug.Log("New Vector Coord: " + newVectorCoord + " Current Focus Vector: " + currentFocusVector);
            currentOrientation = newVectorCoord;
        }
        gameObject.transform.Rotate(0, 0, splicedAngle * Mathf.Rad2Deg);
        transform.position += currentOrientation * velocityModifier * Time.deltaTime;
    }	
}
