CREATE TABLE [dbo].[Products] (
    [ProductID]   INT             IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (MAX)  NOT NULL,
    [Brand]	      NVARCHAR(50) NOT NULL, 
	[Sex]		  INT NOT NULL,
    [Description] NVARCHAR (MAX)  NOT NULL,
    [Category]    NVARCHAR (50)   NOT NULL,
    [Price]       DECIMAL (16, 2) NOT NULL,
    [SizeFormat]  INT             NOT NULL,
    [ImageLinks]  VARCHAR (MAX)   NULL,
    [URL]         VARCHAR (MAX)   NOT NULL,
    PRIMARY KEY CLUSTERED ([ProductID] ASC)
);