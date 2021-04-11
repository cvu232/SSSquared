using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{
    public static Global instance;
    public Shader shader;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            Application.Quit();
    }

    // calculate "mouse" in world space //
    public static Vector3 getScreenToWorldMouse()
    {
        Vector3 worldMouse = new Vector3();
        Plane plane = new Plane(Vector3.forward, 0);

        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
            worldMouse = ray.GetPoint(distance);
        return worldMouse;
    }

    // get object where mouse clicked
    public static GameObject getMouseClickObject()
    {
        Vector3 worldMouse = getScreenToWorldMouse();
        RaycastHit2D hit = Physics2D.Raycast(worldMouse, Vector2.zero);
        if (hit)
            return hit.transform.gameObject;
        return null;
    }
}
