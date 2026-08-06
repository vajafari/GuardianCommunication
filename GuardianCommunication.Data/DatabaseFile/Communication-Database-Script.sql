---- SCRIPT LOGICAL REGION -----
USE [master]
GO
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'PadisCommunication')
BEGIN
	CREATE DATABASE [PadisCommunication] COLLATE SQL_Latin1_General_CP1_CI_AS
END
GO

ALTER DATABASE [PadisCommunication] SET RECOVERY SIMPLE 
GO

USE [PadisCommunication]
GO
---- SCRIPT LOGICAL REGION -----

/****** Object:  Table [dbo].[Attendance]    Script Date: 10/26/2021 09:47:38 ب.ظ ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Attendance]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Attendance](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[EmployeeNumber] [bigint] NOT NULL,
	[VerificationStyle] [int] NOT NULL,
	[AttendanceDateTime] [datetime2](7) NOT NULL,
	[IoRetrieveCode] [int] NOT NULL,
	[StatusCode] [int] NOT NULL,
	[DeviceNumber] [int] NOT NULL,
	[IsSent] [bit] NOT NULL,
	[IsInvalid] [bit] NOT NULL,
 CONSTRAINT [PK_Attendance] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
---- SCRIPT LOGICAL REGION -----

/****** Object:  Table [dbo].[AttendanceHookSystem]    Script Date: 10/26/2021 09:47:38 ب.ظ ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AttendanceHookSystem]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AttendanceHookSystem](
	[AttendanceId] [bigint] NOT NULL,
	[HookSystemId] [int] NOT NULL,
	[IsSent] [bit] NOT NULL,
	[RetryCount] [int] NOT NULL,
	[SentTime] [datetime2](7) NULL,
 CONSTRAINT [PK_AttendanceHookSystem] PRIMARY KEY CLUSTERED 
(
	[AttendanceId] ASC,
	[HookSystemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
---- SCRIPT LOGICAL REGION -----

/****** Object:  Table [dbo].[DeviceCommand]    Script Date: 10/26/2021 09:47:38 ب.ظ ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeviceCommand]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DeviceCommand](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DeviceSerialNumber] [nvarchar](200) NOT NULL,
	[CommandContent] [nvarchar](max) NOT NULL,
	[CommitTime] [datetime2](7) NOT NULL,
	[SendTime] [datetime2](7) NULL,
	[ResponseTime] [datetime2](7) NULL,
	[ResponseValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_DeviceCommand] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
---- SCRIPT LOGICAL REGION -----

/****** Object:  Table [dbo].[DeviceCommunicationData]    Script Date: 10/26/2021 09:47:38 ب.ظ ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeviceCommunicationData]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DeviceCommunicationData](
	[DeviceNumber] [int] NOT NULL,
	[LastLogId] [bigint] NULL,
	[LastAttendanceLogId] [bigint] NULL,
	[LastLogDateTime] [datetime2](7) NULL,
	[LastAttendanceLogDateTime] [datetime2](7) NULL,
 CONSTRAINT [PK_DeviceCommunicationData] PRIMARY KEY CLUSTERED 
(
	[DeviceNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

---- SCRIPT LOGICAL REGION -----

/****** Object:  Table [dbo].[CameraCommunicationData]    Script Date: 10/26/2021 09:47:38 ب.ظ ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CameraCommunicationData]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CameraCommunicationData](
	[CameraId] [int] NOT NULL,
	[LastAttendanceLogDateTime] [datetime2](7) NULL,
 CONSTRAINT [PK_CameraCommunicationData] PRIMARY KEY CLUSTERED 
(
	[CameraId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

---- SCRIPT LOGICAL REGION -----

/****** Object:  Table [dbo].[HookSystem]    Script Date: 10/26/2021 09:47:38 ب.ظ ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HookSystem]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[HookSystem](
	[Id] [int] IDENTITY(1,1)  NOT NULL,
	[SystemName] [nvarchar](500) NOT NULL,
	[AuthorizationUsername] [nvarchar](500) NOT NULL,
	[AuthorizationPassword] [nvarchar](500) NOT NULL,
	[AuthorizationType] [int] NOT NULL,
	[AuthorizationToken] [nvarchar](4000) NOT NULL,
	[UseProxy] [bit] NOT NULL,
	[ProxyUrl] [nvarchar](4000) NOT NULL,
	[ProxyUsername] [nvarchar](500) NOT NULL,
	[ProxyPassword] [nvarchar](500) NOT NULL,
	[BypassProxyOnLocal] [bit] NOT NULL,
	[UseDefaultCredentials] [bit] NOT NULL,
 CONSTRAINT [PK_HookSystem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
---- SCRIPT LOGICAL REGION -----

/****** Object:  Table [dbo].[HookSystemDetail]    Script Date: 10/26/2021 09:47:38 ب.ظ ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HookSystemDetail]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[HookSystemDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HookSystemId] [int] NOT NULL,
	[ApplicationId] [int] NOT NULL,
	[DetailType] [int] NOT NULL,
	[EndPointUrl] [nvarchar](4000) NOT NULL,
	[HttpMethod] [int] NOT NULL,
	[QueryStringTemplate] [nvarchar](max) NOT NULL,
	[HeaderTemplate] [nvarchar](max) NOT NULL,
	[BodyTemplate] [nvarchar](max) NOT NULL,
	[DateFormat] [nvarchar](500) NOT NULL,
	[RetryCount] [int] NOT NULL,
	[RequestTimeoutInSeconds] [int] NOT NULL,
	[ResponseResultJsonPath] [nvarchar](4000) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_HookSystemDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
---- SCRIPT LOGICAL REGION -----

/****** Object:  Table [dbo].[SystemConfig]    Script Date: 10/26/2021 09:47:38 ب.ظ ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SystemConfig]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[SystemConfig](
	[ConfigName] [nvarchar](100) NOT NULL,
	[ConfigValue] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_SystemConfig] PRIMARY KEY CLUSTERED 
(
	[ConfigName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
---- SCRIPT LOGICAL REGION -----

IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'AttendanceHookTimerInterval')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'AttendanceHookTimerInterval', N'30')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'AttendanceSendToKarnamaTimerInterval')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'AttendanceSendToKarnamaTimerInterval', N'30')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'AttendanceSendToKarnamaTimerRecordCount')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'AttendanceSendToKarnamaTimerRecordCount', N'200')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'AttendanceHookTimerRecordCount')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'AttendanceHookTimerRecordCount', N'200')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'AttendanceRegisterInterval')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'AttendanceRegisterInterval', N'0')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'AttendanceRegisterIntervalForParking')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'AttendanceRegisterIntervalForParking', N'0')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'AttendanceRegisterIntervalForTimeAttendance')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'AttendanceRegisterIntervalForTimeAttendance', N'0')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'AttendanceRegisterIntervalForAccessControl')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'AttendanceRegisterIntervalForAccessControl', N'0')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'AutomaticCollectAttendanceTimerInterval')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'AutomaticCollectAttendanceTimerInterval', N'30')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'KarnamaAuthorizationToken')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'KarnamaAuthorizationToken', N'')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'KarnamaServiceUrl')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'KarnamaServiceUrl', N'http://localhost:9080')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'OnlineDeviceTimerInterval')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'OnlineDeviceTimerInterval', N'30')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'OnlineMonitoringDevicesIntervalFromLastDataToReset')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'OnlineMonitoringDevicesIntervalFromLastDataToReset', N'30')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'OnlineMonitoringDevicesTimerCheckLastDataIntervalInMinutes')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'OnlineMonitoringDevicesTimerCheckLastDataIntervalInMinutes', N'30')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'OnlineMonitoringDevicesWaitAfterPingIsConnectedAgainInSecond')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'OnlineMonitoringDevicesWaitAfterPingIsConnectedAgainInSecond', N'20')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'OnlineMonitoringDevicesSleepAfterPingInSecond')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'OnlineMonitoringDevicesSleepAfterPingInSecond', N'5')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'OnlineMonitoringDevicesPingTimeoutInMillisecond')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'OnlineMonitoringDevicesPingTimeoutInMillisecond', N'3000')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'IsNetworkPingActive')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'IsNetworkPingActive', N'true')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ResetRetryCountDayBefore')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'ResetRetryCountDayBefore', N'2')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'KeepCommandAfterResponse')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'KeepCommandAfterResponse', N'false')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'SleepBetweenSendsIfCommandNotExistsInMilliSeconds')	
	INSERT [SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'SleepBetweenSendsIfCommandNotExistsInMilliSeconds', N'2000')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'KarnamaAppUsername')	
	INSERT [SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'KarnamaAppUsername', N'CommunicationApp')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'KarnamaAppPassword')	
	INSERT [SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'KarnamaAppPassword', N'#cSWaK85jx7AB0dx%GL#&MIb')
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'PullDevicesResetPullEventsTimerInterval')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'PullDevicesResetPullEventsTimerInterval'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'PullDevicesCheckConnectionTimerInterval')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'PullDevicesCheckConnectionTimerInterval'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'PullDevicesIntervalFromLastDataToReset')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'PullDevicesIntervalFromLastDataToReset'
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'IsDeleteFailedCommandsActive')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'IsDeleteFailedCommandsActive', N'false')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'DeleteUnsentCommandsIntervalInDays')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'DeleteUnsentCommandsIntervalInDays', N'5')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'DeleteUnsentCommandsTimerIntervalInHours')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'DeleteUnsentCommandsTimerIntervalInHours', N'12')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'DeleteUnsentCommandsJustDeleteFailed')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'DeleteUnsentCommandsJustDeleteFailed', N'true')
GO
-- SUPREMA SDK 1 SETTINGS
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'SupremaSdk1ServerMaxConnection')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'SupremaSdk1ServerMaxConnection', N'200')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'SupremaSdk1ServerPort')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'SupremaSdk1ServerPort', N'51211')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'SupremaSdk1ServerCheckConnectionTimerIntervalInSeconds')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'SupremaSdk1ServerCheckConnectionTimerIntervalInSeconds', N'300')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'SupremaSdk1DeadlineInMinutes')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'SupremaSdk1DeadlineInMinutes', N'7200')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxRetryForSupremaSdk1UserCommands')	
	INSERT [SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'MaxRetryForSupremaSdk1UserCommands', N'20')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxRetryForSupremaSdk1OtherCommands')	
	INSERT [SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'MaxRetryForSupremaSdk1OtherCommands', N'10')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'SupremaSdk1WaitInCommandLoopInMilliseconds')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'SupremaSdk1WaitInCommandLoopInMilliseconds', N'500')
GO


-- SUPREMA SDK 2 SETTINGS
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'SupremaSdk2ServerPort')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'SupremaSdk2ServerPort', N'51212')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'SupremaSdk2ServerReconnectTimerInterval')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'SupremaSdk2ServerReconnectTimerInterval', N'30')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'SupremaSdk2DeadlineInMinutes')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'SupremaSdk2DeadlineInMinutes', N'7200')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxRetryForSupremaSdk2UserCommands')	
	INSERT [SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'MaxRetryForSupremaSdk2UserCommands', N'20')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxRetryForSupremaSdk2OtherCommands')	
	INSERT [SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'MaxRetryForSupremaSdk2OtherCommands', N'10')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'SupremaSdk2WaitInCommandLoopInMilliseconds')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'SupremaSdk2WaitInCommandLoopInMilliseconds', N'200')
GO


-- TIMY SETTINGS
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'TimyNormalCommandTimeoutInSecond')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'TimyNormalCommandTimeoutInSecond', N'20')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'TimyLongCommandTimeoutInSecond')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'TimyLongCommandTimeoutInSecond', N'50')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'TimyGetCommandTimerIntervalInMillisecond')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'TimyGetCommandTimerIntervalInMillisecond', N'500')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'TimyWaitBetweenCommandSendInMilliseconds')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'TimyWaitBetweenCommandSendInMilliseconds', N'20')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'TimyWaitBetweenCommandSendInMilliseconds')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'TimyWaitBetweenCommandSendInMilliseconds', N'20')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'TimyMaxRetryForOtherCommand')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'TimyMaxRetryForOtherCommand', N'10')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'TimyMaxRetryForUserCommand')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'TimyMaxRetryForUserCommand', N'20')
GO

-- ZK SDK SETTINGS
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkPushServerIp')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'ZkPushServerIp', N'127.0.0.1')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkPushServerPort')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'ZkPushServerPort', N'4370')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkWaitInCommandLoopInMilliseconds')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'ZkWaitInCommandLoopInMilliseconds', N'200')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxZkCommandCount')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'MaxZkCommandCount', N'8')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxRetryForZkUserCommands')	
	INSERT [SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'MaxRetryForZkUserCommands', N'20')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxRetryForZkOtherCommand')	
	INSERT [SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'MaxRetryForZkOtherCommand', N'10')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkDeadlineInMinutes')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'ZkDeadlineInMinutes', N'7200')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkPushSleepBetweenSocketsInMillisecond')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'ZkPushSleepBetweenSocketsInMillisecond', N'5')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkPushSleepOnContinueInMillisecond')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'ZkPushSleepOnContinueInMillisecond', N'5000')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkPushReceiveTimeoutInMillisecond')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'ZkPushReceiveTimeoutInMillisecond', N'30000')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkPushStamp')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'ZkPushStamp', N'0')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkPushOpStamp')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'ZkPushOpStamp', N'0')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkPushPhotoStamp')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'ZkPushPhotoStamp', N'0')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkPushErrorDelay')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'ZkPushErrorDelay', N'120')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkPushDelay')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'ZkPushDelay', N'10')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkPushTransTimes')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'ZkPushTransTimes', N'')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkPushTransInterval')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'ZkPushTransInterval', N'30')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkPushSyncTime')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'ZkPushSyncTime', N'0')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkPushRealtime')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'ZkPushRealtime', N'1')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkPushAttendanceLogStamp')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'ZkPushAttendanceLogStamp', N'0')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkPushOperationLogStamp')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'ZkPushOperationLogStamp', N'0')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkPushAttendancePhotoStamp')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'ZkPushAttendancePhotoStamp', N'0')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkPushMultiBioDataSupport')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'ZkPushMultiBioDataSupport', N'1:1:1:1:1:1:1:1:1:1')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkPushMultiBioPhotoSupport')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'ZkPushMultiBioPhotoSupport', N'0:0:0:0:0:0:0:0:0:1')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkPushWaitForSocketData')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'ZkPushWaitForSocketData', N'700')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkIntervalForConsiderDeviceOnlineInSecond')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'ZkIntervalForConsiderDeviceOnlineInSecond', N'60')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'AttendanceSaveKarnamaSendResult')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'AttendanceSaveKarnamaSendResult', N'false')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkPushGetCommandTimerIntervalInMillisecond')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'ZkPushGetCommandTimerIntervalInMillisecond', N'500')
GO
-- PW SDK 1 SETTINGS
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'PwDeadlineInMinutes')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'PwDeadlineInMinutes', N'7200')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxRetryForPwUserCommands')	
	INSERT [SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'MaxRetryForPwUserCommands', N'20')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxRetryForPwOtherCommands')	
	INSERT [SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'MaxRetryForPwOtherCommands', N'10')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'PwWaitInCommandLoopInMilliseconds')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'PwWaitInCommandLoopInMilliseconds', N'200')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'PwSleepTimeInAutoCollectInSeconds')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'PwSleepTimeInAutoCollectInSeconds', N'0')
GO

-- VIRDI SDK 1 SETTINGS
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'VirdiDeadlineInMinutes')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'VirdiDeadlineInMinutes', N'7200')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxRetryForVirdiUserCommands')	
	INSERT [SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'MaxRetryForVirdiUserCommands', N'20')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxRetryForVirdiOtherCommands')	
	INSERT [SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'MaxRetryForVirdiOtherCommands', N'10')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'VirdiServerPort')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'VirdiServerPort', N'9780')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'VirdiWaitInCommandLoopInMilliseconds')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'VirdiWaitInCommandLoopInMilliseconds', N'200')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'VirdiSyncOperationTimeoutInMilliseconds')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'VirdiSyncOperationTimeoutInMilliseconds', N'5000')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'VirdiMaxVisibleLightImageSizeInKb')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'VirdiMaxVisibleLightImageSizeInKb', N'300')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'VirdiMaxVisibleLightImageSizeWidth')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'VirdiMaxVisibleLightImageSizeWidth', N'300')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'VirdiMaxVisibleLightImageSizeHeight')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'VirdiMaxVisibleLightImageSizeHeight', N'700')
GO

-- PADIS CONTROLLER


        

IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'PadisControllerGetCommandTimerIntervalInMillisecond')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'PadisControllerGetCommandTimerIntervalInMillisecond', N'500')
GO

-- PUSH

IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'PadisControllerPushServerPushAddress')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'PadisControllerPushServerPushAddress', N'http://192.168.1.1:3040/')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'PadisControllerPushServerMaxCommandCount')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'PadisControllerPushServerMaxCommandCount', N'20')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'PadisControllerPushServerMaxCommandLength')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'PadisControllerPushServerMaxCommandLength', N'2097152')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'PadisControllerPushServerIntervalForConsiderDeviceOnlineInSecond')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'PadisControllerPushServerIntervalForConsiderDeviceOnlineInSecond', N'60')
GO

-- GRPC
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'PadisControllerGrpcServerCommandCount')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'PadisControllerGrpcServerCommandCount', N'1')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'PadisControllerGrpcServerPort')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'PadisControllerGrpcServerPort', N'37426')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'PadisControllerGrpcCommandTimerIntervalInMillisecond')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'PadisControllerGrpcCommandTimerIntervalInMillisecond', N'1000')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'PadisControllerGrpcServerTimeoutShortInMillisecond')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'PadisControllerGrpcServerTimeoutShortInMillisecond', N'5000')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'PadisControllerGrpcServerTimeoutLongInMillisecond')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'PadisControllerGrpcServerTimeoutLongInMillisecond', N'15000')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'PadisControllerGrpcServerTimeoutVeryLongInMillisecond')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'PadisControllerGrpcServerTimeoutVeryLongInMillisecond', N'60000')
GO


-- SELF SETTINGS
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'SelfTimerIntervalForSendMealsToDeviceInMinute')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'SelfTimerIntervalForSendMealsToDeviceInMinute', N'30')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'SelfOffsetForFutureMealsInMinute')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'SelfOffsetForFutureMealsInMinute', N'90')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'SelfPrinterPingTimeoutInSecond')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'SelfPrinterPingTimeoutInSecond', N'100')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'SelfPrinterSleepAfterPingCircleInMillisecond')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'SelfPrinterSleepAfterPingCircleInMillisecond', N'1000')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'SelfPrinterSleepWhenQueueIsEmptyInMillisecond')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'SelfPrinterSleepWhenQueueIsEmptyInMillisecond', N'200')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'SelfPrinterPrinterPerQueue')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'SelfPrinterPrinterPerQueue', N'4')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'SelfSendSingleFoodTitle')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'SelfSendSingleFoodTitle', N'false')
GO

-- PDIS METAL DETECTOR
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'PadisMetalDetectorGateServerPushPort')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'PadisMetalDetectorGateServerPushPort', N'43724')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'PadisMetalDetectorGateServerPushIp')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'PadisMetalDetectorGateServerPushIp', N'127.0.0.1')
GO

-- XRAY
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'XRayIntervalToRetrySendInSecond')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'XRayIntervalToRetrySendInSecond', N'3600')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'XRaySleepAfterNoFileInMilliSecond')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'XRaySleepAfterNoFileInMilliSecond', N'1000')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'XRayWaitBeforeAddToQueueInMilliSecond')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'XRayWaitBeforeAddToQueueInMilliSecond', N'1000')
GO
-- KARABIN CAMERA
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'KarabinCameraIntervalForSendAccessListInSecond')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'KarabinCameraIntervalForSendAccessListInSecond', N'600')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'KarabinCameraAutoCollectIntervalInSecond')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'KarabinCameraAutoCollectIntervalInSecond', N'1800')
GO
IF NOT EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'KarabinCameraAccessFileBasePath')	
	INSERT [dbo].[SystemConfig] ([ConfigName], [ConfigValue]) VALUES (N'KarabinCameraAccessFileBasePath', N'C:\PadisFiles\Communication Service\Camera\Karabin')
GO


-- REMOVE UNUSED SETTINGS
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionCameraMaxRetryForUserCommand')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionCameraMaxRetryForUserCommand'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionCameraMaxRetryForOtherCommand')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionCameraMaxRetryForOtherCommand'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionDataMoonUsername')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionDataMoonUsername'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionDataMoonPassword')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionDataMoonPassword'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionDataMoonCommandCountInEachFetch')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionDataMoonCommandCountInEachFetch'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionDataMoonSleepBetweenSendsIfCommandNotExistsInMilliSeconds')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionDataMoonSleepBetweenSendsIfCommandNotExistsInMilliSeconds'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionDataMoonWaitBetweenCommandSendInMilliseconds')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionDataMoonWaitBetweenCommandSendInMilliseconds'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionDataMoonServerBaseAddress')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionDataMoonServerBaseAddress'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionDataMoonSleepAfterACommandLoopInMilliSeconds')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionDataMoonSleepAfterACommandLoopInMilliSeconds'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionPadisUsername')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionPadisUsername'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionPadisPassword')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionPadisPassword'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionPadisCommandCountInEachFetch')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionPadisCommandCountInEachFetch'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionPadisSleepBetweenSendsIfCommandNotExistsInMilliSeconds')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionPadisSleepBetweenSendsIfCommandNotExistsInMilliSeconds'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionPadisWaitBetweenCommandSendInMilliseconds')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionPadisWaitBetweenCommandSendInMilliseconds'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionPadisServerBaseAddress')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionPadisServerBaseAddress'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionPadisSleepAfterACommandLoopInMilliSeconds')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'FaceDetectionPadisSleepAfterACommandLoopInMilliSeconds'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxOtherDevicesCommandCount')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxOtherDevicesCommandCount'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'OtherDevicesDeadlineInMinutes')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'OtherDevicesDeadlineInMinutes'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxRetryForOtherDevicesUserCommands')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxRetryForOtherDevicesUserCommands'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxRetryForOtherDevicesOtherCommands')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxRetryForOtherDevicesOtherCommands'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxSupremaSdk1CommandCount')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxSupremaSdk1CommandCount'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxSupremaSdk2CommandCount')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxSupremaSdk2CommandCount'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxZkOnDemandCommandCount')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxZkOnDemandCommandCount'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxPwCommandCount')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxPwCommandCount'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxVirdiCommandCount')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxVirdiCommandCount'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxVirdiCommandCount')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'MaxVirdiCommandCount'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'SupremaSdk1ServerReconnectTimerInterval')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'SupremaSdk1ServerReconnectTimerInterval'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'KarnamaAppSystemId')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'KarnamaAppSystemId'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'KarnamaAppSecurityKey')
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'KarnamaAppSecurityKey'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'SleepBetweenSendsIfNoConnectedDeviceInMilliSeconds')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'SleepBetweenSendsIfNoConnectedDeviceInMilliSeconds'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'IsZkOnDemandCommandsEnabled')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'IsZkOnDemandCommandsEnabled'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'IsPwCommandsEnabled')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'IsPwCommandsEnabled'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'IsSupremaSdk1CommandsEnabled')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'IsSupremaSdk1CommandsEnabled'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'IsSupremaSdk2CommandsEnabled')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'IsSupremaSdk2CommandsEnabled'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'IsVirdiCommandsEnabled')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'IsVirdiCommandsEnabled'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'SleepTimeInNotActiveModeInSeconds')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'SleepTimeInNotActiveModeInSeconds'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'OnlineMonitoringDevicesCheckConnectionTimerInterval')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'OnlineMonitoringDevicesCheckConnectionTimerInterval'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkSleepBetweenSocketsInMillisecond')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkSleepBetweenSocketsInMillisecond'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkSleepOnContinueInMillisecond')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkSleepOnContinueInMillisecond'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkAttPhotoStamp')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkAttPhotoStamp'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkPushSleepForSocketDataInMillisecond')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'ZkPushSleepForSocketDataInMillisecond'
GO
IF EXISTS (SELECT * FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'SelfSendMealsInMinute')	
	DELETE FROM [dbo].[SystemConfig] WHERE [ConfigName] = N'SelfSendMealsInMinute'
GO


---- SCRIPT LOGICAL REGION -----
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_HookSystem_SystemName]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[HookSystem] ADD  CONSTRAINT [DF_HookSystem_SystemName]  DEFAULT ('') FOR [SystemName]
END
GO
---- SCRIPT LOGICAL REGION -----

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_HookSystem_AuthorizationUsername]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[HookSystem] ADD  CONSTRAINT [DF_HookSystem_AuthorizationUsername]  DEFAULT ('') FOR [AuthorizationUsername]
END
GO
---- SCRIPT LOGICAL REGION -----

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_HookSystem_AuthorizationPassword]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[HookSystem] ADD  CONSTRAINT [DF_HookSystem_AuthorizationPassword]  DEFAULT ('') FOR [AuthorizationPassword]
END
GO
---- SCRIPT LOGICAL REGION -----

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_HookSystem_AuthorizationType]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[HookSystem] ADD  CONSTRAINT [DF_HookSystem_AuthorizationType]  DEFAULT ('') FOR [AuthorizationType]
END
GO
---- SCRIPT LOGICAL REGION -----

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_HookSystem_AuthorizationToken]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[HookSystem] ADD  CONSTRAINT [DF_HookSystem_AuthorizationToken]  DEFAULT ('') FOR [AuthorizationToken]
END
GO
---- SCRIPT LOGICAL REGION -----

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_HookSystem_UseProxy]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[HookSystem] ADD  CONSTRAINT [DF_HookSystem_UseProxy]  DEFAULT ((0)) FOR [UseProxy]
END
GO
---- SCRIPT LOGICAL REGION -----

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_HookSystem_ProxyUrl]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[HookSystem] ADD  CONSTRAINT [DF_HookSystem_ProxyUrl]  DEFAULT ('') FOR [ProxyUrl]
END
GO
---- SCRIPT LOGICAL REGION -----

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_HookSystem_ProxyUsername]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[HookSystem] ADD  CONSTRAINT [DF_HookSystem_ProxyUsername]  DEFAULT ('') FOR [ProxyUsername]
END
GO
---- SCRIPT LOGICAL REGION -----

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_HookSystem_ProxyPassword]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[HookSystem] ADD  CONSTRAINT [DF_HookSystem_ProxyPassword]  DEFAULT ('') FOR [ProxyPassword]
END
GO
---- SCRIPT LOGICAL REGION -----

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_HookSystem_BypassProxyOnLocal]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[HookSystem] ADD  CONSTRAINT [DF_HookSystem_BypassProxyOnLocal]  DEFAULT ((0)) FOR [BypassProxyOnLocal]
END
GO
---- SCRIPT LOGICAL REGION -----

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_HookSystem_UseDefaultCredentials]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[HookSystem] ADD  CONSTRAINT [DF_HookSystem_UseDefaultCredentials]  DEFAULT ((0)) FOR [UseDefaultCredentials]
END
GO
---- SCRIPT LOGICAL REGION -----

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_HookSystemDetail_DetailType]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[HookSystemDetail] ADD  CONSTRAINT [DF_HookSystemDetail_DetailType]  DEFAULT ((0)) FOR [DetailType]
END
GO
---- SCRIPT LOGICAL REGION -----

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_HookSystemDetail_EndPointUrl]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[HookSystemDetail] ADD  CONSTRAINT [DF_HookSystemDetail_EndPointUrl]  DEFAULT ('') FOR [EndPointUrl]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_HookSystemDetail_HttpMethod]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[HookSystemDetail] ADD  CONSTRAINT [DF_HookSystemDetail_HttpMethod]  DEFAULT ((1)) FOR [HttpMethod]
END
GO
---- SCRIPT LOGICAL REGION -----

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_HookSystemDetail_QueryStringTemplate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[HookSystemDetail] ADD  CONSTRAINT [DF_HookSystemDetail_QueryStringTemplate]  DEFAULT ('') FOR [QueryStringTemplate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_HookSystemDetail_HeaderTemplate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[HookSystemDetail] ADD  CONSTRAINT [DF_HookSystemDetail_HeaderTemplate]  DEFAULT ('') FOR [HeaderTemplate]
END
GO
---- SCRIPT LOGICAL REGION -----

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_HookSystemDetail_BodyTemplate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[HookSystemDetail] ADD  CONSTRAINT [DF_HookSystemDetail_BodyTemplate]  DEFAULT ('') FOR [BodyTemplate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_HookSystemDetail_DateFormat]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[HookSystemDetail] ADD  CONSTRAINT [DF_HookSystemDetail_DateFormat]  DEFAULT ('') FOR [DateFormat]
END
GO
---- SCRIPT LOGICAL REGION -----

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_HookSystemDetail_RetryCount]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[HookSystemDetail] ADD  CONSTRAINT [DF_HookSystemDetail_RetryCount]  DEFAULT ((5)) FOR [RetryCount]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_HookSystemDetail_RequestTimeoutInSeconds]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[HookSystemDetail] ADD  CONSTRAINT [DF_HookSystemDetail_RequestTimeoutInSeconds]  DEFAULT ((60)) FOR [RequestTimeoutInSeconds]
END
GO
---- SCRIPT LOGICAL REGION -----

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_HookSystemDetail_ResponseResultJsonPath]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[HookSystemDetail] ADD  CONSTRAINT [DF_HookSystemDetail_ResponseResultJsonPath]  DEFAULT ('') FOR [ResponseResultJsonPath]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_HookSystemDetail_IsActive]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[HookSystemDetail] ADD  CONSTRAINT [DF_HookSystemDetail_IsActive]  DEFAULT ((0)) FOR [IsActive]
END
GO
---- SCRIPT LOGICAL REGION -----

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AttendanceHookSystem_Attendance]') AND parent_object_id = OBJECT_ID(N'[dbo].[AttendanceHookSystem]'))
ALTER TABLE [dbo].[AttendanceHookSystem]  WITH CHECK ADD  CONSTRAINT [FK_AttendanceHookSystem_Attendance] FOREIGN KEY([AttendanceId])
REFERENCES [dbo].[Attendance] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AttendanceHookSystem_Attendance]') AND parent_object_id = OBJECT_ID(N'[dbo].[AttendanceHookSystem]'))
ALTER TABLE [dbo].[AttendanceHookSystem] CHECK CONSTRAINT [FK_AttendanceHookSystem_Attendance]
GO
---- SCRIPT LOGICAL REGION -----

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AttendanceHookSystem_HookSystem]') AND parent_object_id = OBJECT_ID(N'[dbo].[AttendanceHookSystem]'))
ALTER TABLE [dbo].[AttendanceHookSystem]  WITH CHECK ADD  CONSTRAINT [FK_AttendanceHookSystem_HookSystem] FOREIGN KEY([HookSystemId])
REFERENCES [dbo].[HookSystem] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AttendanceHookSystem_HookSystem]') AND parent_object_id = OBJECT_ID(N'[dbo].[AttendanceHookSystem]'))
ALTER TABLE [dbo].[AttendanceHookSystem] CHECK CONSTRAINT [FK_AttendanceHookSystem_HookSystem]
GO
---- SCRIPT LOGICAL REGION -----

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_HookSystemDetail_HookSystem]') AND parent_object_id = OBJECT_ID(N'[dbo].[HookSystemDetail]'))
ALTER TABLE [dbo].[HookSystemDetail]  WITH CHECK ADD  CONSTRAINT [FK_HookSystemDetail_HookSystem] FOREIGN KEY([HookSystemId])
REFERENCES [dbo].[HookSystem] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_HookSystemDetail_HookSystem]') AND parent_object_id = OBJECT_ID(N'[dbo].[HookSystemDetail]'))
ALTER TABLE [dbo].[HookSystemDetail] CHECK CONSTRAINT [FK_HookSystemDetail_HookSystem]






---- SCRIPT LOGICAL REGION -----
GO
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[DeviceCommand]') 
         AND name = 'CommandType'
)
BEGIN 
	ALTER TABLE [dbo].[DeviceCommand]
	ADD [CommandType] SMALLINT NOT NULL 
	CONSTRAINT [DF_DeviceCommand_CommandType] DEFAULT 0
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_DeviceCommand_CommandType]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DeviceCommand] ADD  CONSTRAINT [DF_DeviceCommand_CommandType]  DEFAULT ((0)) FOR [CommandType]
END





---- SCRIPT LOGICAL REGION -----
GO
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[DeviceCommand]') 
         AND name = 'EmployeeNumber'
)
BEGIN 
	ALTER TABLE [dbo].[DeviceCommand]
	ADD [EmployeeNumber] BIGINT
END



---- SCRIPT LOGICAL REGION -----
GO
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[DeviceCommand]') 
         AND name = 'RetryCount'
)
BEGIN 
	ALTER TABLE [dbo].[DeviceCommand]
	ADD [RetryCount] TINYINT NOT NULL 
	CONSTRAINT [DF_DeviceCommand_RetryCount] DEFAULT 0
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_DeviceCommand_RetryCount]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DeviceCommand] ADD  CONSTRAINT [DF_DeviceCommand_RetryCount]  DEFAULT ((0)) FOR [RetryCount]
END



---- SCRIPT LOGICAL REGION -----
GO
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[DeviceCommand]') 
         AND name = 'Priority'
)
BEGIN 
	ALTER TABLE [dbo].[DeviceCommand]
	ADD [Priority] TINYINT NOT NULL 
	CONSTRAINT [DF_DeviceCommand_Priority] DEFAULT 1
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_DeviceCommand_Priority]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DeviceCommand] ADD  CONSTRAINT [DF_DeviceCommand_Priority]  DEFAULT ((2)) FOR [Priority]
END



---- SCRIPT LOGICAL REGION -----
GO
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[DeviceCommand]') 
         AND name = 'MaxRetry'
)
BEGIN 
	ALTER TABLE [dbo].[DeviceCommand]
	ADD [MaxRetry] TINYINT NOT NULL 
	CONSTRAINT [DF_DeviceCommand_MaxRetry] DEFAULT 1
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_DeviceCommand_MaxRetry]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DeviceCommand] ADD  CONSTRAINT [DF_DeviceCommand_MaxRetry]  DEFAULT ((1)) FOR [MaxRetry]
END



---- SCRIPT LOGICAL REGION -----
GO
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[DeviceCommand]') 
         AND name = 'DeviceNumber'
)
BEGIN 
	ALTER TABLE [dbo].[DeviceCommand]
	ADD [DeviceNumber] INT NOT NULL 
	CONSTRAINT [DF_DeviceCommand_DeviceNumber] DEFAULT 0
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_DeviceCommand_DeviceNumber]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DeviceCommand] ADD  CONSTRAINT [DF_DeviceCommand_DeviceNumber]  DEFAULT ((0)) FOR [DeviceNumber]
END




---- SCRIPT LOGICAL REGION -----
GO
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[DeviceCommand]') 
         AND name = 'DeviceContent'
)
BEGIN 
	ALTER TABLE [dbo].[DeviceCommand]
	ADD [DeviceContent] NVARCHAR(MAX) NOT NULL 
	CONSTRAINT [DF_DeviceCommand_DeviceContent] DEFAULT ('')
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_DeviceCommand_DeviceContent]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DeviceCommand] ADD  CONSTRAINT [DF_DeviceCommand_DeviceContent]  DEFAULT (('')) FOR [DeviceContent]
END


---- SCRIPT LOGICAL REGION -----
GO
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[DeviceCommand]') 
         AND name = 'Deadline'
)
BEGIN 
	ALTER TABLE [dbo].[DeviceCommand]
	ADD [Deadline] [datetime2](7)
END




---- SCRIPT LOGICAL REGION -----
GO
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[DeviceCommand]') 
         AND name = 'ProducerNumber'
)
BEGIN 
	ALTER TABLE [dbo].[DeviceCommand]
	ADD [ProducerNumber] SMALLINT NOT NULL 
	CONSTRAINT [DF_DeviceCommand_ProducerNumber] DEFAULT 1
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_DeviceCommand_ProducerNumber]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DeviceCommand] ADD  CONSTRAINT [DF_DeviceCommand_ProducerNumber]  DEFAULT ((1)) FOR [ProducerNumber]
END





---- SCRIPT LOGICAL REGION -----
GO
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[DeviceCommand]') 
         AND name = 'SdkVersion'
)
BEGIN 
	ALTER TABLE [dbo].[DeviceCommand]
	ADD [SdkVersion] SMALLINT NOT NULL 
	CONSTRAINT [DF_DeviceCommand_SdkVersion] DEFAULT 1
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_DeviceCommand_SdkVersion]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DeviceCommand] ADD  CONSTRAINT [DF_DeviceCommand_SdkVersion]  DEFAULT ((1)) FOR [SdkVersion]
END


---- SCRIPT LOGICAL REGION -----
GO
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[DeviceCommand]') AND name = N'IX_UnsentDeviceCommand')
	EXEC( 'DROP INDEX [IX_UnsentDeviceCommand] ON [dbo].[DeviceCommand]')

---- SCRIPT LOGICAL REGION -----
GO
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[DeviceCommand]') AND name = N'IX_DeviceCommandFailedCommands')
	EXEC( 'DROP INDEX [IX_DeviceCommandFailedCommands] ON [dbo].[DeviceCommand]')


---- SCRIPT LOGICAL REGION -----
GO
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[DeviceCommand]') 
         AND name = 'VisiblilityTime'
)
BEGIN 
	ALTER TABLE [dbo].[DeviceCommand]
	ADD [VisiblilityTime] [datetime2](7)
END

---- SCRIPT LOGICAL REGION -----
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[DeviceCommand]') AND name = N'IX_NotSentDeviceCommands')
	CREATE NONCLUSTERED INDEX [IX_NotSentDeviceCommands]
	ON [dbo].[DeviceCommand] ([ResponseTime],[DeviceNumber],[ProducerNumber],[SdkVersion],[Deadline],[VisiblilityTime])
	INCLUDE ([DeviceSerialNumber],[CommandContent],[CommitTime],[CommandType],[RetryCount],[Priority],[MaxRetry],[DeviceContent])

---- SCRIPT LOGICAL REGION -----
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[DeviceCommand]') AND name = N'IX_ZkNotSentDeviceCommands')
	CREATE NONCLUSTERED INDEX [IX_ZkNotSentDeviceCommands]
	ON [dbo].[DeviceCommand] ([ResponseTime],[ProducerNumber],[DeviceSerialNumber],[Deadline],[VisiblilityTime])

---- SCRIPT LOGICAL REGION -----
GO
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Attendance]') 
         AND name = 'ApplicationId'
)
BEGIN 
	ALTER TABLE [dbo].[Attendance]
	ADD [ApplicationId] [INT]
END




---- SCRIPT LOGICAL REGION -----

GO
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Attendance]') 
         AND name = 'InsertDateTime'
)
BEGIN 
	ALTER TABLE [dbo].[Attendance]
	ADD [InsertDateTime] DATETIME2 NOT NULL 
	CONSTRAINT [DF_Attendance_InsertDateTime] DEFAULT GETDATE();

	EXEC('UPDATE [dbo].[Attendance]
		SET [InsertDateTime] = [AttendanceDateTime];')

END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_Attendance_InsertDateTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Attendance] ADD  CONSTRAINT [DF_Attendance_InsertDateTime]  DEFAULT (GETDATE()) FOR [InsertDateTime]
END


---- SCRIPT LOGICAL REGION -----

GO
IF EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Attendance]') 
         AND name = 'VerificationStyle'
)
BEGIN 
	ALTER TABLE [dbo].[Attendance]
	ALTER COLUMN [VerificationStyle] INT NULL
END



---- SCRIPT LOGICAL REGION -----

GO
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Attendance]') 
         AND name = 'DoorId'
)
BEGIN 
	ALTER TABLE [dbo].[Attendance]
	ADD [DoorId] INT NULL
END



---- SCRIPT LOGICAL REGION -----

GO
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Attendance]') 
         AND name = 'ReaderDeviceNumber'
)
BEGIN 
	ALTER TABLE [dbo].[Attendance]
	ADD [ReaderDeviceNumber] INT NULL
END



---- SCRIPT LOGICAL REGION -----

GO
IF EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Attendance]') 
         AND name = 'DeviceNumber'
)
BEGIN 
	ALTER TABLE [dbo].[Attendance]
	ALTER COLUMN [DeviceNumber] INT NULL
END



IF NOT EXISTS(
    SELECT 1 FROM sys.indexes 
    WHERE object_id = OBJECT_ID(N'[dbo].[Attendance]') 
        AND name = 'UQ_Attendance'
)
BEGIN 
	
	DECLARE @TempIds TABLE
	(
		Id BIGINT  
	)

	INSERT INTO @TempIds
	SELECT Id
	FROM dbo.[Attendance] att1
	WHERE EXISTS (
		SELECT Id
		FROM dbo.[Attendance] att2
		WHERE	att1.[EmployeeNumber] = att2.[EmployeeNumber]
				AND att1.[AttendanceDateTime] = att2.[AttendanceDateTime]
				AND att1.[Id] > att2.[Id]
	)

	DELETE FROM dbo.[AttendanceHookSystem] WHERE [AttendanceId] IN (SELECT Id FROM @TempIds)

	DELETE FROM dbo.[Attendance] WHERE [Id] IN (SELECT Id FROM @TempIds)

    ALTER TABLE [dbo].[Attendance] ADD  CONSTRAINT [UQ_Attendance] UNIQUE NONCLUSTERED 
	(
		[EmployeeNumber] ASC,
		[AttendanceDateTime] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END 




---- SCRIPT LOGICAL REGION -----

GO
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[AttendanceHookSystem]') 
         AND name = 'Id'
)
BEGIN 
	ALTER TABLE [dbo].[AttendanceHookSystem]
	ADD [Id] BIGINT NOT NULL IDENTITY (1, 1)

	ALTER TABLE [dbo].[AttendanceHookSystem] DROP CONSTRAINT [PK_AttendanceHookSystem] WITH ( ONLINE = OFF )

	ALTER TABLE [dbo].[AttendanceHookSystem] ADD  CONSTRAINT [PK_AttendanceHookSystem] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END


---- SCRIPT LOGICAL REGION -----

IF NOT EXISTS(
    SELECT 1 FROM sys.indexes 
    WHERE object_id = OBJECT_ID(N'[dbo].[AttendanceHookSystem]') 
        AND name = 'UQ_AttendanceId_HookSystemId'
)
BEGIN 
    ALTER TABLE [dbo].[AttendanceHookSystem] ADD  CONSTRAINT [UQ_AttendanceId_HookSystemId] UNIQUE NONCLUSTERED 
	(
		[AttendanceId] ASC,
		[HookSystemId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END 




---- SCRIPT LOGICAL REGION -----
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[DeviceCommand]') AND name = N'IX_DeviceNumber')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_DeviceNumber] ON [dbo].[DeviceCommand]
	(
		[DeviceNumber] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END



---- SCRIPT LOGICAL REGION -----

GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[DeviceCommand]') AND name = N'IX_DeviceSerialNumber')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_DeviceSerialNumber] ON [dbo].[DeviceCommand]
	(
		[DeviceSerialNumber] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END


---- SCRIPT LOGICAL REGION -----
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ScheduledApiCallTask]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[ScheduledApiCallTask](
		[Id] [int] NOT NULL,
		[ApiName] [nvarchar](500) NULL,
		[Cron] [nvarchar](500) NOT NULL,
		[AuthorizationUsername] [nvarchar](500) NULL,
		[AuthorizationPassword] [nvarchar](500) NULL,
		[AuthorizationType] [int] NOT NULL,
		[AuthorizationToken] [nvarchar](4000) NULL,
		[EndPointUrl] [nvarchar](4000) NOT NULL,
		[HttpMethod] [int] NOT NULL,
		[QueryString] [nvarchar](max) NULL,
		[Header] [nvarchar](max) NULL,
		[Body] [nvarchar](max) NULL,
		[DateFormat] [nvarchar](500) NULL,
		[RequestTimeoutInSeconds] [int] NOT NULL,
	 CONSTRAINT [PK_ScheduledApiCallTask] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_ScheduledApiCallTask_RequestTimeoutInSeconds]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ScheduledApiCallTask] ADD  CONSTRAINT [DF_ScheduledApiCallTask_RequestTimeoutInSeconds]  DEFAULT ((60)) FOR [RequestTimeoutInSeconds]
END
GO


---- SCRIPT LOGICAL REGION -----

GO
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Attendance]') 
         AND name = 'RfCardNumber'
)
BEGIN 
	ALTER TABLE [dbo].[Attendance]
	ADD [RfCardNumber] NVARCHAR(100) NULL
END


---- SCRIPT LOGICAL REGION -----

GO
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Attendance]') 
         AND name = 'IoType'
)
BEGIN 
	ALTER TABLE [dbo].[Attendance]
	ADD [IoType] SMALLINT NOT NULL 
	CONSTRAINT [DF_Attendance_IoType] DEFAULT (0);
	EXEC ('
			UPDATE  [dbo].[Attendance]
				SET [IoType] = ISNULL((
								SELECT	TOP 1 dvc.[IoType] 
								FROM	[Karnama].[gen].[Device] dvc
								WHERE	dvc.[DeviceNumber] = [dbo].[Attendance].[DeviceNumber]
							), 0)
		')
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_Attendance_IoType]') AND type = 'D')
BEGIN
	ALTER TABLE [dbo].[Attendance] ADD  CONSTRAINT [DF_Attendance_IoType]  DEFAULT (0) FOR [IoType]
END
GO



---- SCRIPT LOGICAL REGION -----

GO
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Attendance]') 
         AND name = 'AttendanceSource'
)
BEGIN 
	ALTER TABLE [dbo].[Attendance]
	ADD [AttendanceSource] SMALLINT NOT NULL 
	CONSTRAINT [DF_Attendance_AttendanceSource] DEFAULT (1);


	EXEC('
			UPDATE [dbo].[Attendance]
				SET [AttendanceSource] = 
					CASE 
						WHEN [IoRetrieveCode] = 1 OR [IoRetrieveCode] = 2 OR [IoRetrieveCode] = 3 THEN 1
						WHEN [IoRetrieveCode] = 4 THEN 3
						WHEN [IoRetrieveCode] = 5 THEN 2
						WHEN [IoRetrieveCode] = 6 THEN 4
						ELSE 1
					END
		')

END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_Attendance_AttendanceSource]') AND type = 'D')
BEGIN
	ALTER TABLE [dbo].[Attendance] ADD  CONSTRAINT [DF_Attendance_AttendanceSource]  DEFAULT (1) FOR [AttendanceSource]
END
GO


---- SCRIPT LOGICAL REGION -----


IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Attendance]') 
         AND name = 'DeviceAttendanceIoRetrieveType'
)
BEGIN 
	ALTER TABLE [dbo].[Attendance]
	ADD [DeviceAttendanceIoRetrieveType] SMALLINT NULL 


	EXEC('
			UPDATE [dbo].[Attendance]
				SET [DeviceAttendanceIoRetrieveType] = [IoRetrieveCode]
			WHERE [IoRetrieveCode] IN (1, 2, 3)
		')

END




---- SCRIPT LOGICAL REGION -----


IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Attendance]') 
         AND name = 'CameraId'
)
BEGIN 
	ALTER TABLE [dbo].[Attendance]
	ADD [CameraId] INT NULL 

END


---- SCRIPT LOGICAL REGION -----

IF EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Attendance]') 
         AND name = 'IoRetrieveCode'
)
BEGIN 
	ALTER TABLE [dbo].[Attendance]
	DROP COLUMN [IoRetrieveCode] 
END




---- SCRIPT LOGICAL REGION -----
GO
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[DeviceCommand]') 
         AND name = 'Description'
)
BEGIN 
	ALTER TABLE [dbo].[DeviceCommand]
	ADD [Description] NVARCHAR(MAX) NULL 
END



---- SCRIPT LOGICAL REGION -----
GO
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[DeviceCommand]') 
         AND name = 'CommandIdentifier'
)
BEGIN 
	ALTER TABLE [dbo].[DeviceCommand]
	ADD [CommandIdentifier] UNIQUEIDENTIFIER NULL 
END


---- SCRIPT LOGICAL REGION -----


GO
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Attendance]') 
         AND name = 'ReaderDeviceNumber'
)
BEGIN 
	ALTER TABLE [dbo].[Attendance]
	ADD [ReaderDeviceNumber] INT NULL 
END


---- SCRIPT LOGICAL REGION -----

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AttendanceSendToKarnamaResult]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AttendanceSendToKarnamaResult](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AttendanceId] [int] NOT NULL,
	[SendTime] [datetime] NOT NULL,
	[IsSuccessful] [bit] NOT NULL,
	[StatusCode] [int] NOT NULL,
	[ExceptionMessage] [nvarchar](max) NULL,
 CONSTRAINT [PK_AttendanceSendToKarnamaResult] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

---- SCRIPT LOGICAL REGION -----
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OtherHardwareCommand]') AND type IN (N'U'))
BEGIN
CREATE TABLE [dbo].[OtherHardwareCommand](
	[Id] [INT] IDENTITY(1,1) NOT NULL,
	[HardwareSerialNumber] [NVARCHAR](200) NULL,
	[HardwareId] [BIGINT] NULL,
	[CommandContent] [NVARCHAR](MAX) NOT NULL,
	[HardwareType] [SMALLINT] NOT NULL,
	[CommitTime] [DATETIME2](7) NOT NULL,
	[SendTime] [DATETIME2](7) NULL,
	[ResponseTime] [DATETIME2](7) NULL,
	[ResponseValue] [NVARCHAR](MAX) NULL,
	[CommandType] [SMALLINT] NOT NULL,
	[ObjectId] [BIGINT] NULL,
	[RetryCount] [TINYINT] NOT NULL,
	[Priority] [TINYINT] NOT NULL,
	[MaxRetry] [TINYINT] NOT NULL,
	[HardwareContent] [NVARCHAR](MAX) NULL,
	[Deadline] [DATETIME2](7) NULL,
	[VisiblilityTime] [DATETIME2](7) NULL,
	[Description] [NVARCHAR](MAX) NULL,
	[CommandIdentifier] [UNIQUEIDENTIFIER] NULL,
 CONSTRAINT [PK_OtherHardwareCommand] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OtherHardwareCommand]') AND name = N'IX_OtherHardwareCommand_HardwareTypeDeviceId')
CREATE NONCLUSTERED INDEX [IX_OtherHardwareCommand_HardwareTypeDeviceId] ON [dbo].[OtherHardwareCommand]
(
	[HardwareType] ASC,
	[HardwareId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OtherHardwareCommand]') AND name = N'IX_OtherHardwareCommand_HardwareTypeDeviceSerialNumber')
CREATE NONCLUSTERED INDEX [IX_OtherHardwareCommand_HardwareTypeDeviceSerialNumber] ON [dbo].[OtherHardwareCommand]
(
	[HardwareType] ASC,
	[HardwareSerialNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO



---- SCRIPT LOGICAL REGION -----
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Attendance]') AND name = N'IX_InsertDateTime')
	CREATE NONCLUSTERED INDEX [IX_InsertDateTime]
	ON [dbo].[Attendance] ([InsertDateTime])


---- SCRIPT LOGICAL REGION -----

IF TYPE_ID(N'dbo.StringValueListTable') IS NULL
BEGIN
    CREATE TYPE dbo.StringValueListTable AS TABLE
    (
        StringValue NVARCHAR(200) NOT NULL PRIMARY KEY
    );
END;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeviceCommandCountByDeviceSerialNumber]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[DeviceCommandCountByDeviceSerialNumber] AS' 
END
GO
ALTER PROCEDURE [dbo].[DeviceCommandCountByDeviceSerialNumber] 
	@Producer INT,
    @SdkVersion INT,
    @DeviceSerialNumbersList dbo.StringValueListTable READONLY
AS 
BEGIN
	SET NOCOUNT ON;
    SELECT
        s.StringValue AS DeviceSerialNumber,
        COUNT(dc.DeviceSerialNumber) AS CommandCount
    FROM @DeviceSerialNumbersList AS s
		LEFT JOIN dbo.DeviceCommand AS dc
			ON dc.DeviceSerialNumber = s.StringValue
				AND dc.ProducerNumber = @Producer
				AND dc.SdkVersion = @SdkVersion
    GROUP BY
        s.StringValue;
END


GO
IF TYPE_ID(N'dbo.IntegerValueListTable') IS NULL
BEGIN
    CREATE TYPE dbo.IntegerValueListTable AS TABLE
    (
        IntegerValue INT NOT NULL PRIMARY KEY
    );
END;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeviceCommandCountByDeviceNumber]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[DeviceCommandCountByDeviceNumber] AS' 
END
GO

ALTER PROCEDURE [dbo].[DeviceCommandCountByDeviceNumber] 
	@Producer INT,
    @SdkVersion INT,
    @DeviceNumbersList dbo.IntegerValueListTable READONLY
AS 
BEGIN
	SET NOCOUNT ON;
    SELECT
        s.IntegerValue AS DeviceNumber,
        COUNT(dc.DeviceNumber) AS CommandCount
    FROM @DeviceNumbersList AS s
		LEFT JOIN dbo.DeviceCommand AS dc
			ON dc.DeviceNumber = s.IntegerValue
				AND dc.ProducerNumber = @Producer
				AND dc.SdkVersion = @SdkVersion
    GROUP BY
        s.IntegerValue;
END

---- SCRIPT LOGICAL REGION -----




---- SCRIPT LOGICAL REGION -----

GO
USE [master]
GO
ALTER DATABASE [PadisCommunication] SET  READ_WRITE 
GO

