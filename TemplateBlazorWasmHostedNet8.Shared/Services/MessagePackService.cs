using MessagePack;
using MessagePack.Resolvers;
using System.Buffers;

namespace TemplateBlazorWasmHostedNet8.Shared.Services;

public class MessagePackService
{
    /// <summary>
    /// Para mais informações: https://github.com/neuecc/MessagePack-CSharp#security
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public ResultDto<T> Deserialize<T>(ReadOnlySequence<byte> data)
    {
        try
        {
            // Add segurança na deserialização do objeto
            var options = MessagePackSerializerOptions.Standard.WithSecurity(MessagePackSecurity.UntrustedData);

            // Resolvers (Conversores)
            var resolver = CompositeResolver.Create(

                // resolver custom types first
                PrimitiveObjectResolver.Instance,
                StandardResolverAllowPrivate.Instance,
                BuiltinResolver.Instance,
                NativeDateTimeResolver.Instance,
                DynamicGenericResolver.Instance,
                DynamicObjectResolver.Instance,
                DynamicObjectResolverAllowPrivate.Instance,
                NativeGuidResolver.Instance,
                NativeDecimalResolver.Instance,
                TypelessObjectResolver.Instance,

                // finally use standard resolver
                StandardResolver.Instance
            );

            options = MessagePackSerializerOptions.Standard.WithResolver(resolver);
            var result = MessagePackSerializer.Deserialize<T>(data, options);

            return new ResultDto<T>(Success: true, Data: result, Exception: null);
        }
        catch (Exception ex)
        {
            return new ResultDto<T>(Success: false, Data: default, Exception: ex);
        }
    }

    /// <summary>
    /// Para mais informações: https://github.com/neuecc/MessagePack-CSharp#lz4-compression
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public ResultDto<ReadOnlySequence<byte>> Serialize<T>(T data)
    {
        try
        {
            // Add compatação binária - LZ4 Compression
            var options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);

            // Resolvers (Conversores)
            var resolver = CompositeResolver.Create(

                // resolver custom types first
                PrimitiveObjectResolver.Instance,
                StandardResolverAllowPrivate.Instance,
                BuiltinResolver.Instance,
                NativeDateTimeResolver.Instance,
                DynamicGenericResolver.Instance,
                DynamicObjectResolver.Instance,
                DynamicObjectResolverAllowPrivate.Instance,
                NativeGuidResolver.Instance,
                NativeDecimalResolver.Instance,
                TypelessObjectResolver.Instance,

                // finally use standard resolver
                StandardResolver.Instance
            );

            options = MessagePackSerializerOptions.Standard.WithResolver(resolver);
            var bytes = MessagePackSerializer.Serialize<T>(data, options);

            var payload = new ReadOnlySequence<byte>(bytes);
            return new ResultDto<ReadOnlySequence<byte>>(Success: true, Data: payload, Exception: null);
        }
        catch (Exception ex)
        {
            return new ResultDto<ReadOnlySequence<byte>>(Success: false, Data: default, Exception: ex);
        }
    }

    #region MÉTODOS DE TESTE
    //public static ResultDto<T> DeserializeWithException<T>(ReadOnlySequence<byte> data)
    //{
    //    try
    //    {
    //        throw new Exception("DeserializeWithException - TESTE");
    //    }
    //    catch (Exception ex)
    //    {
    //        return new ResultDto<T>(Success: false, Data: default, Exception: ex);

    //    }

    //}
    #endregion
}
