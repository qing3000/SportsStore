﻿CREATE TABLE [dbo].[Products] (
    [ID]			INT	IDENTITY (1, 1) NOT NULL,
    [Title]			NVARCHAR			NOT NULL,
	[TitleCN]		NVARCHAR			NOT NULL,
    [Brand]			NVARCHAR			NOT NULL, 
	[ProductID]		CHAR(100)			NOT NULL,
	[Sex]			TINYINT				NOT NULL,
    [Description]	NVARCHAR (MAX)		NOT NULL,
	[DescriptionCN]	NVARCHAR (MAX)		NOT NULL,
	[Material]		NVARCHAR (MAX)		NOT NULL,
	[MaterialCN]	NVARCHAR (MAX)		NOT NULL,
    [Category]		NCHAR				NOT NULL,
	[CategoryCN]	NCHAR				NOT NULL,
    [SizePrice]		VARBINARY(MAX)		NOT NULL,
	[MinimumAge]	REAL				NOT NULL,
	[MaximumAge]	REAL				NOT NULL,
    [URL]			NVARCHAR (MAX)		NOT NULL,
	[ThumbnailLink]	NVARCHAR (MAX)		NOT NULL,
    [ImageLinks]	NVARCHAR (MAX)		NOT NULL,
	[EnterDate]		DATETIME			NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);