namespace MultiDBAcademy.Domain.Entities;

public enum DbEngineType
{
    MySQL = 1,
    PostgreSQL = 2,
    MongoDB = 3,
    Redis = 4,
    SQLServer = 5
}

public enum DbInstanceStatus
{
    Creating = 1,
    Active = 2,
    Stopped = 3,
    Error = 4,
    Deleted = 5
}