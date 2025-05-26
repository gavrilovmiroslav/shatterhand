using UnityEngine;

public class Shake : MonoBehaviour
{
    private Vector3 m_Position;

    public void Start()
    {
        m_Position = transform.position;
    }

    public void Update()
    {
        this.transform.position = m_Position + Random.onUnitSphere * 0.01f;
    }
}
