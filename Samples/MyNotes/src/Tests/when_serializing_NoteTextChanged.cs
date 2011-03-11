using Events;
using Ncqrs.Spec;

namespace Tests
{
    public class when_serializing_NoteTextChanged
        : JsonEventSerializationFixture<NoteTextChanged>
    {
        protected override NoteTextChanged GivenEvent()
        {
            return new NoteTextChanged()
                       {
                           NewText = "New text goes here."
                       };
        }
    }
}
