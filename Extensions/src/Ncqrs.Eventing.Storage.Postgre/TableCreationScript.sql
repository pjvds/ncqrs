CREATE TABLE Events
(
  SequentialId serial NOT NULL,
  Id uuid NOT NULL,
  TimeStamp timestamp NOT NULL,
  Name text NOT NULL,
  Version text NOT NULL,
  EventSourceId uuid NOT NULL,
  Sequence bigint NULL,
  Data text NOT NULL,
  CONSTRAINT Events_pkey PRIMARY KEY (SequentialId)
)
WITH (
  OIDS=FALSE
);

CREATE TABLE EventSources
(
  Id uuid NOT NULL,
  Type character varying(255) NOT NULL,
  Version int NOT NULL
)
WITH (
  OIDS=FALSE
);

CREATE TABLE Snapshots
(
  EventSourceId uuid NOT NULL,
  Version bigint,
  TimeStamp timestamp NOT NULL,
  Type character varying(255) NOT NULL,
  Data bytea NOT NULL
)
WITH (
  OIDS=FALSE
);

CREATE TABLE PipelineState
(
  BatchId serial NOT NULL,
  PipelineName character varying(255) NOT NULL,
  LastProcessedEventId uuid NOT NULL,
  CONSTRAINT PipelineState_pkey PRIMARY KEY (BatchId)
)
WITH (
  OIDS=FALSE
);