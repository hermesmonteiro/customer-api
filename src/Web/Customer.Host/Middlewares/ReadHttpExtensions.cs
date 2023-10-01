using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IO;

namespace Customer.Host.Middlewares
{
    public static class ReadHttpExtensions
    {
        public static string ReadStreamInChunks(this Stream stream) => While((
                readChunkBufferLength: 4096,
                textWriter: new StringWriter(),
                readChunk: new char[4096],
                reader: new StreamReader(stream),
                readChunkLength: 0),
            tuple =>
            {
                tuple.textWriter.Write(tuple.readChunk, 0, tuple.readChunkLength);

                return (tuple.readChunkBufferLength, tuple.textWriter, tuple.readChunk, tuple.reader,
                        tuple.reader.ReadBlock(tuple.readChunk,
                                               0,
                                               tuple.readChunkBufferLength));
            },
            tuple => tuple.readChunkLength > 0,
            value => value.textWriter?
                          .ToString());

        public static async Task<string> ReadRequestBody(this HttpContext context)
        {
            string requestBody;

            context.Request.EnableBuffering();
            await using var requestStream = new RecyclableMemoryStreamManager().GetStream();
            await context.Request.Body.CopyToAsync(requestStream);
            requestStream.Position = 0;
            requestBody = requestStream.ReadStreamInChunks();
            context.Request.Body.Position = 0;

            return requestBody;
        }

        public static TResult While<TData, TResult>(TData data,
                                                    Func<TData, TData> whileFunction,
                                                    Func<TData, bool> condition,
                                                    Func<TData, TResult> map)
        {
            TData result = whileFunction(data);
            while (condition(result)) result = whileFunction(result);

            return map(result);
        }

        public static TResult Map<TData, TResult>(this TData @this,
                                                  Func<TData, TResult> map)
            => map(@this);
    }
}
