USE [master]
GO

IF EXISTS(SELECT * FROM sys.databases WHERE name = N'MyNotesReadModel')
	BEGIN
		DROP DATABASE [MyNotesReadModel]
	END
GO

CREATE DATABASE [MyNotesReadModel]
GO

USE [MyNotesReadModel]
GO

CREATE TABLE [NoteItemSet]
(
	[Id]			[uniqueidentifier]		PRIMARY KEY,
	[Text]			[varchar](250)			NULL,
	[CreationDate]	[datetime]				NULL
) ON [PRIMARY]
GO

CREATE TABLE [TotalsPerDayItemSet]
(
	[Id]			[int] IDENTITY(1,1)		PRIMARY KEY,
	[Date]			[datetime]				NOT NULL,
	[NewCount]		[int]					NOT NULL,
	[EditCount]		[int]					NOT NULL
) ON [PRIMARY]
GO