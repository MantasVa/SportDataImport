namespace SportDataImport.Mongo.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class BsonCollectionNameAttribute : Attribute
{
    private readonly string _name;
    private readonly double _version;

    public string Name => _name;

    public BsonCollectionNameAttribute(string name)
    {
        _name = name;
        _version = 1.0;
    }
}
