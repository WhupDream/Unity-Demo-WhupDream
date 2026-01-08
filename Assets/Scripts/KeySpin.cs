using UnityEngine;

public class KeySpin : MonoBehaviour
{
    public float rotationSpeed;
    
    // Update is called once per frame
    void FixedUpdate()
    {
        this.transform.Rotate(0f, this.rotationSpeed * Time.fixedDeltaTime, 0f);
    }
}
