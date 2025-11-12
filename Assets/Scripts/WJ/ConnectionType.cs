using UnityEngine;

[CreateAssetMenu(fileName = "NewConnectionType", menuName = "Connection System/Connection Type")]
public class ConnectionType : ScriptableObject
{
    public string typeName;
    public Color typeColor = Color.white;
    public Material lineMaterial;
}
