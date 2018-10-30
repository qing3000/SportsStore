SET IDENTITY_INSERT [dbo].[Products] ON
INSERT INTO [dbo].[Products] ([ProductID], [Name], [Brand], [Sex], [Description], [Category], [Price], [SizeFormat], [ImageLinks], [URL]) VALUES (1, N'test', N'Next', 0, N'test description', N'test category', CAST(1.00 AS Decimal(16, 2)), 1, N'', N'http://www.next.co.uk/g642432s4#936888')
SET IDENTITY_INSERT [dbo].[Products] OFF
