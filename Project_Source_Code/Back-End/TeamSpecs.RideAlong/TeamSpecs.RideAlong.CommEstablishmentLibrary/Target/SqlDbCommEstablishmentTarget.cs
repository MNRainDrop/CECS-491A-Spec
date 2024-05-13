using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.CommEstablishmentLibrary.Target
{
    public class SqlDbCommEstablishmentTarget
    {
        private readonly ISqlServerDAO _dao;

        public SqlDbCommEstablishmentTarget(ISqlServerDAO dao)
        {
            _dao = dao;
        }

        public IResponse RetrieveSeller()
        {

        }

    }
}
