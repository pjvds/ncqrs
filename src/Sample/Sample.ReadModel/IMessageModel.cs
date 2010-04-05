using System;
using MongoDB.Emitter;
namespace Sample.ReadModel
{
    public interface IMessageModel : IDocumentWrapper
    {
        DateTime CreationDate { get; set; }
        Guid Id { get; set; }
        string Text { get; set; }
    }
}
