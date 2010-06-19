== MY NOTES SAMPLE APPLICATION ==
This sample application contains the basic componetents of an Ncqrs bases 
application. It is build with ASP.NET MVC2 and the Ncqrs Framework.

== GETTING IT UP AND RUNNING ==
Follow these steps to get the website running.

1. Open MyNotes.sln with Visual Studio 2010.
2. Run MyNotesEventStore.sql to create the event store database.
3. Run MyNotesReadModel.sql to create the read model database.
4. If needed (this step is not needed when you run the default SQL express), 
   update ReadModelContainer connection string in 
	- CommandService\Web.config
	- ReadModel\App.config
	- Website\Web.config
5. Set Website as default startup project.
6. Hit F5 and have fun!

== CONTACT ==
If you have any questions or feedback, contact me via:
	- Twitter: http://twitter.com/pjvds
	- GTalk: pjvandesande@born2code.net
	- MSN: pjvandesande@born2code.net
	- Mail: pjvandesande@born2code.net