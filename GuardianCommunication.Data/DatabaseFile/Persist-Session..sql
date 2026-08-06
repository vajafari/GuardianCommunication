---- SCRIPT LOGICAL REGION -----

GO
IF NOT EXISTS (SELECT * FROM [PadisCommunication].[dbo].[ScheduledApiCallTask] WHERE [Id] = 1)
BEGIN
	INSERT [PadisCommunication].[dbo].[ScheduledApiCallTask] ([Id], [ApiName], [Cron], [AuthorizationUsername], [AuthorizationPassword], [AuthorizationType], [AuthorizationToken], [EndPointUrl], [HttpMethod], [QueryString], [Header], [Body], [DateFormat], [RequestTimeoutInSeconds]) VALUES (1, N'Call Session Managment Persist Cache', N'0 * * * *', N'LiveAgentApiCall', N'Rff2pEZ2FdJxbpKiyoQ9BX8dvDiuGMSV', 4, NULL, N'https://localhost:44317/api/general/Session/PersistCache', 1, NULL, NULL, NULL, NULL, 30)
END