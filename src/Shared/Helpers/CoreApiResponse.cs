namespace Shared.Helpers.Helpers;

public class CoreApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T ?Data { get; set; }
}

public class HrmmApiResponse<T>
{
    public bool Success { get; set; }
    public string ?Message { get; set; }
    public T ?Data { get; set; }
}

public class HrmProApiResponse<T>
{
    public bool Success { get; set; }
    public string ?Message { get; set; }
    public T ?Data { get; set; }
}
