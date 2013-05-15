USE [master]
GO

IF EXISTS(SELECT * FROM sys.databases WHERE name = N'MyNotes')
	BEGIN
		Print 'Database "MyNotes" already exists.'
	END
ELSE
	BEGIN
		CREATE DATABASE [MyNotes]

		Print 'Database "MyNotes" was created.'
	END
GO

USE [MyNotes]
GO

IF EXISTS(SELECT * FROM sysobjects WHERE xtype = 'U' AND name = 'Note' AND uid = SCHEMA_ID(SCHEMA_NAME()))
	BEGIN
		PRINT 'The "Note" table already exists.'
	END
ELSE
	BEGIN
		CREATE TABLE [Note]
		(
			[Id]			[uniqueidentifier]		NOT NULL,
			[Text]			[varchar](250)			NULL,
			[CreationDate]	[datetime]				NULL
			CONSTRAINT [PK_Note] PRIMARY KEY CLUSTERED 
			(
				[Id]		ASC
			)
		) ON [PRIMARY]

		PRINT 'The "Note" table was created.'
	END
GO

IF EXISTS(SELECT * FROM sysobjects WHERE xtype = 'U' AND name = 'DailyStatistics' AND uid = SCHEMA_ID(SCHEMA_NAME()))
	BEGIN
		PRINT 'The "DailyStatistics" table already exists.'
	END
ELSE
	BEGIN
		CREATE TABLE [DailyStatistics]
		(
			[Id]			[int] IDENTITY(1,1)		PRIMARY KEY,
			[Date]			[datetime]				NOT NULL,
			[NewCount]		[int]					NOT NULL,
			[EditCount]		[int]					NOT NULL
		) ON [PRIMARY]

		PRINT 'The "DailyStatistics" table was created.'
	END
GO
