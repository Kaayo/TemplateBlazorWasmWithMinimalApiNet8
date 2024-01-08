using MessagePack;
using System;

namespace TemplateBlazorWasmHostedNet8.Shared.Dtos;


public record DogDto(string message, string status);

//[MessagePackObject]
//public record DogDto(string Message, string Status)
//{
//    [Key(0)]
//    public string Message { get; init; } = Message;

//    [Key(1)]
//    public string Status { get; init; } = Status;
//};