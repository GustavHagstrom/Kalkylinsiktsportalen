using BidconAccessModule.SdkAccess.Utilities;
using BidCon.SDK.Database;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace BidconAccessModule.SdkAccess.Services;
internal class BidconReflectionService : IBidconReflectionService
{
    public DatabaseUser CreateUser()
    {
        var connection = ConnectionFromReflection();
        var usr = (DatabaseUser)RuntimeHelpers.GetUninitializedObject(typeof(DatabaseUser));

        ReflectionHelper.SetFieldValue(usr, "<Name>k__BackingField", "Admin");
        ReflectionHelper.SetFieldValue(usr, "<ID>k__BackingField", 2);
        ReflectionHelper.SetFieldValue(usr, "<UserGroup>k__BackingField", 2);

        ReflectionHelper.SetFieldValue(usr, "<EstimationAccess>k__BackingField", CreateEstimationAccess(connection, usr));
        ReflectionHelper.SetFieldValue(usr, "_estimationReader", CreateEstimationReader(connection, usr));
        ReflectionHelper.SetFieldValue(usr, "_systemReader", CreateSystemReader(connection));

        Type keyType = typeof(string);
        Type valueType = Type.GetType("BidCon.SDK.Database.DatabaseEstimationWriter, BidCon.SDK, Version=2023.2.0.0, Culture=neutral, PublicKeyToken=d5af2ed775e84a93")!;
        Type dictionaryType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
        object dictionary = Activator.CreateInstance(dictionaryType)!;

        ReflectionHelper.SetFieldValue(usr, "EstimationWriters", dictionary);
        ReflectionHelper.SetFieldValue(usr, "<Connection>k__BackingField", connection);

        return usr;
    }
    private object CreateEstimationAccess(object connection, DatabaseUser user)
    {
        var accessType = Type.GetType("BidCon.SDK.Database.DatabaseEstimationAccess, BidCon.SDK, Version=2023.2.0.0, Culture=neutral, PublicKeyToken=d5af2ed775e84a93");
        var instance = ReflectionHelper.CreateInstance(accessType!, [user, connection]);
        return instance;
    }
    private DatabaseEstimationReader CreateEstimationReader(object connection, DatabaseUser user)
    {
        var reader = ReflectionHelper.CreateInstance<DatabaseEstimationReader>(user, connection);
        return reader;
    }
    private DatabaseSystemReader CreateSystemReader(object connection)
    {
        var reader = ReflectionHelper.CreateInstance<DatabaseSystemReader>(connection);
        return reader;
    }
    private object ConnectionFromIni()
    {
        var app = BidCon.SDK.Activator.CreateApp();
        app.InitConnectionFromIni();
        return ReflectionHelper.GetFieldOrPropertyValue<object>(app, "_connection");
    }
    private object ConnectionFromReflection()
    {
        var app = BidCon.SDK.Activator.CreateApp();
        var system = GetSystemConnection();
        var estimation = GetEstimationConnection();
        ReflectionHelper.InvokeMethod(app, "InitConnection", [system, estimation]);
        return ReflectionHelper.GetFieldOrPropertyValue<object>(app, "_connection");
    }
    private SqlConnection GetSystemConnection()
    {
        return new SqlConnection("Data Source=RHUSAPP02\\ELECOSOFT;Initial Catalog=BidConSystem;User ID=sa;Password=Putlig@15;Connect Timeout=30;Encrypt=False;");
    }
    private SqlConnection GetEstimationConnection()
    {
        return new SqlConnection("Data Source=RHUSAPP02\\ELECOSOFT;Initial Catalog=BidConEstimation;User ID=sa;Password=Putlig@15;Connect Timeout=30;Encrypt=False;");
    }

}
