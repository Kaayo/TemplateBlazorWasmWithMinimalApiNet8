using MessagePack;

namespace TemplateBlazorWasmHostedNet8.Shared.Dtos;

[MessagePackObject]
public record ResultDto(bool Success, Exception? Exception)
{
    [Key(0)]
    public bool Success { get; init; } = Success;

    [Key(1)]
    public Exception? Exception { get; init; } = Exception;
};


[MessagePackObject]
public record ResultDto<T>(bool Success, Exception? Exception, T? Data)
{
    [Key(0)]
    public bool Success { get; init; } = Success;

    [Key(1)]
    public Exception? Exception { get; init; } = Exception;

    [Key(2)]
    public T? Data { get; init; } = Data;
};
