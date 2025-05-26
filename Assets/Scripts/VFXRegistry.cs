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

    public void Awake()
    {
        Instance = this;
    }
}
