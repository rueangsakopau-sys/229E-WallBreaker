using UnityEngine;

[CreateAssetMenu(fileName = "GunData", menuName = "WallBreaker/GunData")]
public class GunData : ScriptableObject
{
    public string gunName  = "Basic Gun";
    public float  damage   = 1f;
    public float  cooldown = 5f;
    public float  price    = 0f;
    public bool   owned    = true;
}