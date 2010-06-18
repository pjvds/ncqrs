using System;
using System.ServiceModel;
using Commands;

namespace CommandService
{
    [ServiceContract]
    public interface IMyNotesCommandService
    {
        [OperationContract]
        void CreateNewNote(CreateNewNote command);

        [OperationContract]
        void ChangeNoteText(ChangeNoteText command);
    }
}
