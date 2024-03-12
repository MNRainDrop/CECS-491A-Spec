
namespace TeamSpecs.RideAlong.DataAccess;

public interface ISqlServerDAO : IReadOnlyFromDataStore, IWriteOnlyFromDataStore
{
}
public interface IJsonFileDAO : IReadOnlyFromFile, IWriteOnlyFromFile
{
}
