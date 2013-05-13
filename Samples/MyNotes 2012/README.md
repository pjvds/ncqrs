MY NOTES 2012 SAMPLE APPLICATION
================================

This sample application contains the basic components of a CQRS + Event Sourcing application.
It is built with ASP.NET MVC4, the Ncqrs Framework, and Entity Framework 5.0.

GETTING IT UP AND RUNNING
-------------------------

What do you need to run this sample app?

0. Visual Studio 2010 or 2012
1. Microsoft MVC4 (Included with VS 2012)
2. IIS Express
3. Microsoft SQL 2005 Express or higher

Follow these steps to get the website running.

0. Run BUILD.bat in the root folder (two up).
1. Run MyNotesReadModel.sql in the "sqlexpress" folder to create the read model database.
2. Open MyNotes 2012.sln with Visual Studio 2012 (Run as Administrator: http://stackoverflow.com/questions/6898597/visual-studio-unable-to-open-port-to-host-wcf-service).
3. If needed (this step is not needed when you run the default SQL express), 
   update the "MyNotes Read Model" connection string in 
	- ApplicationService\app.config
	- Website\Web.config
4. Set Website and ApplicationService as default startup project.
5. Hit F5 and have fun!

CONTACT
-------

If you have any questions or feedback, contact me via:
	- Twitter: <http://twitter.com/theBoringCoder>
	- Mail: ncqrs-dev@googlegroups.com