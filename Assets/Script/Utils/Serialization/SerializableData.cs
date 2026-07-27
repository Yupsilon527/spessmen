using System;

[Serializable]
public abstract class SerializableData<T>
{
    public SerializableData(T data) { }
    public virtual void Deserialize(T output) { }
    public virtual T Deserialize() { return default(T); }

}


[Serializable]
public class ScriptableSerializable : SerializableData<ScriptableBase>
{
    public string internalName;
    public ScriptableSerializable(ScriptableBase data) : base(data)
    {
        internalName = data.name;
    }
}