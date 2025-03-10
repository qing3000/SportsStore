﻿CREATE TABLE [dbo].[Products] (
    [ID]				INT	IDENTITY (1, 1) NOT NULL,
    [Title]				NVARCHAR (1000)		NOT NULL,
	[TitleCN]			NVARCHAR (1000)		NOT NULL,
    [Brand]				NVARCHAR (100)		NOT NULL, 
	[ProductID]			VARCHAR(100)		NOT NULL,
	[Gender]			INT					NOT NULL,
    [Description]		NVARCHAR (MAX)		NOT NULL,
	[DescriptionCN]		NVARCHAR (MAX)		NOT NULL,
	[Material]			NVARCHAR (MAX)		NOT NULL,
	[MaterialCN]		NVARCHAR (MAX)		NOT NULL,
    [Category]			INT					NOT NULL,
	[Color]				INT					NOT NULL,	
    [SizePricesBinary]	VARBINARY(MAX)		NOT NULL,
	[MinimumAge]		REAL				NOT NULL,
	[MaximumAge]		REAL				NOT NULL,
    [URL]				NVARCHAR (MAX)		NOT NULL,
	[ThumbnailLink]		NVARCHAR (MAX)		NOT NULL,
    [ImageLinks]		NVARCHAR (MAX)		NOT NULL,
	[InsertTime]		DATETIME2			NOT NULL,
	[UpdateTime]		DATETIME2			NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);