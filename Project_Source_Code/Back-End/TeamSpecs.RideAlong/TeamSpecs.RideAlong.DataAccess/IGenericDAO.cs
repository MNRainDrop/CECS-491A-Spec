namespace TeamSpecs.RideAlong.DataAccess;

public interface IGenericDAO : IReadOnlyFromDataStore, IWriteOnlyFromDataStore, IReadOnlyFromFile, IWriteOnlyFromFile
{
}
