using UnityEngine;

public class CoinSpin : MonoBehaviour
{
    public float rotationSpeed;
    
    // Update is called once per frame
    void FixedUpdate()
    {
        this.transform.Rotate(this.rotationSpeed * Time.fixedDeltaTime, 0f, 0f);
    }
}