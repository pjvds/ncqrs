
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 10/26/2010 22:39:10
-- Generated from EDMX file: C:\Projects\ncqrs\Samples\MyNotes\src\ReadModel\ReadModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [MyNotesReadModel];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[NoteItemSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NoteItemSet];
GO
IF OBJECT_ID(N'[dbo].[TotalsPerDayItemSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TotalsPerDayItemSet];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'NoteItemSet'
CREATE TABLE [dbo].[NoteItemSet] (
    [Id] uniqueidentifier  NOT NULL,
    [Text] varchar(250)  NULL,
    [CreationDate] datetime  NULL
);
GO

-- Creating table 'TotalsPerDayItemSet'
CREATE TABLE [dbo].[TotalsPerDayItemSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Date] datetime  NOT NULL,
    [NewCount] int  NOT NULL,
    [EditCount] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'NoteItemSet'
ALTER TABLE [dbo].[NoteItemSet]
ADD CONSTRAINT [PK_NoteItemSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TotalsPerDayItemSet'
ALTER TABLE [dbo].[TotalsPerDayItemSet]
ADD CONSTRAINT [PK_TotalsPerDayItemSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------