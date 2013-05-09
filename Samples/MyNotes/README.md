MY NOTES SAMPLE APPLICATION
===========================

This sample application contains the basic components of a CQRS + Event Sourcing application.
It is built with ASP.NET MVC3 and the Ncqrs Framework.

GETTING IT UP AND RUNNING
-------------------------

What do you need to run this sample app?

0. Visual Studio 2010
1. Microsoft SQL 2005 Express or higher
2. Microsoft MVC3

Follow these steps to get the website running.

0. Run BUILD.bat in the root folder (two up).
1. Open MyNotes.sln with Visual Studio 2010 (Run as Administrator: http://stackoverflow.com/questions/6898597/visual-studio-unable-to-open-port-to-host-wcf-service).
2. Run MyNotesEventStore.sql to create the event store database.
3. Run MyNotesReadModel.sql to create the read model database.
4. If needed (this step is not needed when you run the default SQL express), 
   update ReadModelContainer connection string in 
	- ApplicationService\app.config
	- ReadModel\App.config
	- Website\Web.config
5. Set Website and ApplicationService as default startup project.
6. Hit F5 and have fun!

CONTACT
-------

If you have any questions or feedback, contact me via:
	- Twitter: <http://twitter.com/ncqrs>
	- Mail: ncqrs-dev@googlegroups.com