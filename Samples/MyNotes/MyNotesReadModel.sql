USE [master]
GO

IF exists(SELECT * FROM sys.databases WHERE name = N'MyNotesReadModel')
BEGIN
	DROP DATABASE [MyNotesReadModel]
END
GO

CREATE DATABASE [MyNotesReadModel]
GO

USE [MyNotesReadModel]
GO

CREATE TABLE [dbo].[NoteItem](
	[Id] [uniqueidentifier] PRIMARY KEY,
	[Text] [varchar](250) NULL,
	[CreationDate] [datetime] NULL
) ON [PRIMARY]
GO
