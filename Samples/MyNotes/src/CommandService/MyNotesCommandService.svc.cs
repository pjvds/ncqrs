using System;
using Commands;
using Ncqrs;
using Ncqrs.Commanding.ServiceModel;

namespace CommandService
{
    public class MyNotesCommandService : IMyNotesCommandService
    {
        private static ICommandService _service;

        static MyNotesCommandService()
        {
            BootStrapper.BootUp();

            _service = NcqrsEnvironment.Get<ICommandService>();
        }

        public void CreateNewNote(CreateNewNote command)
        {
            _service.Execute(command);
        }

        public void ChangeNoteText(ChangeNoteText command)
        {
            _service.Execute(command);
        }
    }
}
