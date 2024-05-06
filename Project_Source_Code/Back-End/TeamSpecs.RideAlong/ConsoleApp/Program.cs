
using Microsoft.Data.SqlClient;
using System.Data;

public class Program
{
    public static void Main(string[] args)
    {
        MyClass obj = new MyClass
        {
            Uid = 123456,
            Name = "John Doe",
            BirthDate = new DateTime(1990, 5, 1)
        };

        HashSet<SqlParameter> parameters = SqlParameterHelper.CreateSqlParameters(obj);

        foreach (var parameter in parameters)
        {
            Console.WriteLine($"var param = new SqlParameter(\"{parameter.ParameterName}\");");
            Console.WriteLine($"param.Value = {GetValueCode(parameter.Value)};");
            Console.WriteLine("parameters.Add(param);\n");
        }
    }

    private static string GetValueCode(object value)
    {
        if (value == null)
        {
            return "null";
        }
        else if (value is string)
        {
            return $"\"{value}\"";
        }
        else if (value is DateTime)
        {
            return $"DateTime.Parse(\"{((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss")}\")";
        }
        else
        {
            return value.ToString();
        }
    }
}

public class MyClass
{
    public long Uid { get; set; }
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    // Add more properties as needed
}

public class SqlParameterHelper
{
    public static HashSet<SqlParameter> CreateSqlParameters(object obj)
    {
        HashSet<SqlParameter> parameters = new HashSet<SqlParameter>();

        var properties = obj.GetType().GetProperties();

        foreach (var property in properties)
        {
            string paramName = "@" + property.Name;
            SqlDbType sqlType = GetSqlType(property.PropertyType);

            SqlParameter parameter = new SqlParameter(paramName, sqlType);
            parameter.Value = property.GetValue(obj);

            parameters.Add(parameter);
        }

        return parameters;
    }

    private static SqlDbType GetSqlType(Type type)
    {
        if (type == typeof(string))
        {
            return SqlDbType.VarChar;
        }
        else if (type == typeof(long))
        {
            return SqlDbType.BigInt;
        }
        else if (type == typeof(DateTime))
        {
            return SqlDbType.DateTime;
        }
        else
        {
            throw new ArgumentException("Unsupported data type for SQL parameter.");
        }
    }
}