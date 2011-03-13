using System;
using Events;
using Ncqrs.Spec;

namespace Tests
{
    public class when_serializing_NewNoteAdded
        : JsonEventSerializationFixture<NewNoteAdded>
    {
        protected override NewNoteAdded GivenEvent()
        {
            return new NewNoteAdded()
                       {
                           CreationDate = DateTime.Now,
                           NoteId = Guid.NewGuid(),
                           Text = "Note text goes here"
                       };
        }
    }
}
