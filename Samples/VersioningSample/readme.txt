This is to explore common versioning issues with persisted events when using
event sourcing with ncqrs.


* AppV1 is the base app, run this first to create a person with an "old" event.


* AppV2 represents a common refactoring change to namespaces but does not alter
  the events themselves, only their names.

  This is to test that the old events can de-serialize correctly despite the
  namespace change.

  TODO: Also need to investigate a change to the event name (eg for correcting
  typeos)


* AppV3 represents a change to the event, this would be considered a new
  version of the event.

  This is to ensure that if fields are added and remove convertors can be used
  to reformat the old events to match the new event.

  NOTE: When changing an event, semantic changes should be a new event, this is
  just to ensure ncqrs can handle such changes correctly.
  See, http://bloggingabout.net/blogs/vagif/archive/2010/05/28/is-event-versioning-as-costly-as-sql-schema-versioning.aspx
  

  TODO: Need to investigate how these convertors will work and interact with
  the event store.
