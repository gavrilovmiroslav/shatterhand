using UnityEngine;

public class VFXRegistry : MonoBehaviour
{
    public static VFXRegistry Instance;
    public GameObject FriendlySummon;
    public GameObject DestroyField;
    public GameObject Grow;
    public GameObject Shrink;
    public GameObject Buff;
    public GameObject Debuff;
    public GameObject Discard;
    public GameObject Hit;
    public GameObject Sustain;
    public GameObject Ruin;
    public GameObject Gift;

    public void Awake()
    {
        Instance = this;
    }
}
