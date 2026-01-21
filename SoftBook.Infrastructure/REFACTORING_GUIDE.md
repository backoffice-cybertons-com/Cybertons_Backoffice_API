# BaseDAL Refactoring Guide - Pure ADO.NET

## Overview
I've created a **BaseDAL** class that contains all common database operations using **pure ADO.NET** (SqlDataReader, SqlCommand). All your 24 DAL classes can now inherit from this base class.

## BaseDAL Features

### Common Methods Available:
1. **ExecuteReader<T>()** - Returns List<T> from stored procedure
2. **ExecuteSingleReader<T>()** - Returns single object from stored procedure
3. **ExecuteNonQuery()** - Executes Insert/Update/Delete
4. **ExecuteScalar<T>()** - Returns scalar value (COUNT, SUM, etc.)
5. **ExecuteInTransaction()** - Executes in transaction context
6. **Transaction Methods** - StartTransaction, CommitTransaction, RollbackTransaction

## Code Comparison

### BEFORE (Old approach - 95 lines):
```csharp
public class UnitDAL
{
    private readonly string _connectionString;
    
    public UnitDAL(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public List<Unit> GetAll()
    {
        List<Unit> units = new List<Unit>();
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            SqlCommand cmd = new SqlCommand("sp_GetAllUnits", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                units.Add(new Unit
                {
                    UnitId = Convert.ToInt32(reader["UnitId"]),
                    UnitName = reader["UnitName"]?.ToString(),
                    UQC = reader["UQC"]?.ToString()
                });
            }
        }
        return units;
    }
    
    // ... 70 more lines of repetitive code
}
```

### AFTER (New approach - 55 lines):
```csharp
public class UnitDAL : BaseDAL
{
    public UnitDAL(IConfiguration configuration) : base(configuration) { }

    public List<Unit> GetAll()
    {
        return ExecuteReader("sp_GetAllUnits", null, reader => new Unit
        {
            UnitId = Convert.ToInt32(reader["UnitId"]),
            UnitName = reader["UnitName"]?.ToString(),
            UQC = reader["UQC"]?.ToString()
        });
    }

    public Unit GetById(int id)
    {
        return ExecuteSingleReader("sp_GetUnitById", 
            cmd => cmd.Parameters.AddWithValue("@UnitId", id),
            reader => new Unit
            {
                UnitId = Convert.ToInt32(reader["UnitId"]),
                UnitName = reader["UnitName"]?.ToString(),
                UQC = reader["UQC"]?.ToString()
            });
    }

    public void Insert(Unit unit)
    {
        ExecuteNonQuery("sp_InsertUnit", cmd =>
        {
            cmd.Parameters.AddWithValue("@UnitName", unit.UnitName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@UQC", unit.UQC ?? (object)DBNull.Value);
        });
    }
}
```

## Benefits

✅ **42% less code** - From 95 lines to 55 lines
✅ **No repetitive connection handling** - BaseDAL handles it
✅ **Consistent pattern** - All DALs work the same way
✅ **Transaction support** - Built-in transaction methods
✅ **Pure ADO.NET** - No external dependencies
✅ **Easier to maintain** - Change once in BaseDAL, affects all 24 DALs

## Pattern for All DALs

```csharp
public class [Master]DAL : BaseDAL
{
    public [Master]DAL(IConfiguration configuration) : base(configuration) { }

    // GetAll - Returns List
    public List<[Model]> GetAll()
    {
        return ExecuteReader("sp_GetAll[Masters]", null, reader => new [Model]
        {
            Field1 = reader["Field1"]?.ToString(),
            Field2 = Convert.ToInt32(reader["Field2"])
        });
    }

    // GetById - Returns single object
    public [Model] GetById(int id)
    {
        return ExecuteSingleReader("sp_Get[Master]ById", 
            cmd => cmd.Parameters.AddWithValue("@IdField", id),
            reader => new [Model]
            {
                Field1 = reader["Field1"]?.ToString()
            });
    }

    // Insert
    public void Insert([Model] item)
    {
        ExecuteNonQuery("sp_Insert[Master]", cmd =>
        {
            cmd.Parameters.AddWithValue("@Field1", item.Field1);
            cmd.Parameters.AddWithValue("@Field2", item.Field2);
        });
    }

    // Update
    public void Update([Model] item)
    {
        ExecuteNonQuery("sp_Update[Master]", cmd =>
        {
            cmd.Parameters.AddWithValue("@IdField", item.IdField);
            cmd.Parameters.AddWithValue("@Field1", item.Field1);
        });
    }

    // Delete
    public void Delete(int id)
    {
        ExecuteNonQuery("sp_Delete[Master]", cmd =>
        {
            cmd.Parameters.AddWithValue("@IdField", id);
        });
    }
}
```

## Next Steps

Would you like me to:
1. ✅ Refactor all 23 remaining DAL classes to use BaseDAL
2. ✅ Test the build to ensure everything works
3. ✅ Create a summary document

This will make your codebase much cleaner and easier to maintain!
