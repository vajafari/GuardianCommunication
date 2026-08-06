IF NOT EXISTS (SELECT * FROM [PadisCommunication].[dbo].[ScheduledApiCallTask] WHERE [Id] = 2)
BEGIN
	INSERT [PadisCommunication].[dbo].[ScheduledApiCallTask] ([Id], [ApiName], [Cron], [AuthorizationUsername], [AuthorizationPassword], [AuthorizationType], [AuthorizationToken], [EndPointUrl], [HttpMethod], [QueryString], [Header], [Body], [DateFormat], [RequestTimeoutInSeconds]) VALUES (2, N'Call Visitor Employee Substitute Check Expired', N'0 * * * *', N'LiveAgentApiCall', N'Rff2pEZ2FdJxbpKiyoQ9BX8dvDiuGMSV', 4, NULL, N'https://localhost:44317/api/vt/VisitorEmployeeSubstitute/CheckExpired', 1, NULL, NULL, NULL, NULL, 30)
END
GO
IF NOT EXISTS (SELECT * FROM [PadisCommunication].[dbo].[ScheduledApiCallTask] WHERE [Id] = 3)
BEGIN
	INSERT [PadisCommunication].[dbo].[ScheduledApiCallTask] ([Id], [ApiName], [Cron], [AuthorizationUsername], [AuthorizationPassword], [AuthorizationType], [AuthorizationToken], [EndPointUrl], [HttpMethod], [QueryString], [Header], [Body], [DateFormat], [RequestTimeoutInSeconds]) VALUES (3, N'Call Visitor Form Check Expired', N'0 * * * *', N'LiveAgentApiCall', N'Rff2pEZ2FdJxbpKiyoQ9BX8dvDiuGMSV', 4, NULL, N'https://localhost:44317/api/vt/VisitorForm/CheckExpired', 1, NULL, NULL, NULL, NULL, 30)
END
GO
