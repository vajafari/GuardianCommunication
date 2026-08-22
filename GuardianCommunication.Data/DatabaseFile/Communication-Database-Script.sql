USE [Guardian]
GO
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'com')
EXEC sys.sp_executesql N'CREATE SCHEMA [com]'
GO
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N'IntegerValueListTable' AND ss.name = N'dbo')
CREATE TYPE [dbo].[IntegerValueListTable] AS TABLE(
	[IntegerValue] [int] NOT NULL,
	PRIMARY KEY CLUSTERED 
(
	[IntegerValue] ASC
)WITH (IGNORE_DUP_KEY = OFF)
)
GO
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N'StringValueListTable' AND ss.name = N'dbo')
CREATE TYPE [dbo].[StringValueListTable] AS TABLE(
	[StringValue] [nvarchar](200) NOT NULL,
	PRIMARY KEY CLUSTERED 
(
	[StringValue] ASC
)WITH (IGNORE_DUP_KEY = OFF)
)
GO
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N'UniqueIdentifierValueListTable' AND ss.name = N'dbo')
CREATE TYPE [dbo].[UniqueIdentifierValueListTable] AS TABLE(
	[UniqueIdentifierValue] [uniqueidentifier] NOT NULL,
	PRIMARY KEY CLUSTERED 
(
	[UniqueIdentifierValue] ASC
)WITH (IGNORE_DUP_KEY = OFF)
)
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[Attendance]') AND type in (N'U'))
BEGIN
CREATE TABLE [com].[Attendance](
	[Id] [uniqueidentifier] NOT NULL,
	[UserIdOnDevice] [bigint] NOT NULL,
	[LogIdOnDevice] [bigint] NULL,
	[AttendanceDateTime] [datetime2](7) NOT NULL,
	[DeviceId] [uniqueidentifier] NULL,
	[CameraId] [uniqueidentifier] NULL,
	[DoorId] [uniqueidentifier] NULL,
	[ReaderDeviceId] [uniqueidentifier] NULL,
	[LocationId] [uniqueidentifier] NOT NULL,
	[VerificationStyle] [int] NULL,
	[RfCardNumber] [nvarchar](100) NULL,
	[StatusCode] [int] NULL,
	[IsSentToGuardian] [bit] NOT NULL,
	[SentToGuardianRetryCount] [int] NOT NULL,
	[ModuleId] [int] NOT NULL,
	[IoType] [smallint] NOT NULL,
	[AttendanceSource] [smallint] NOT NULL,
	[DeviceAttendanceIoRetrieveType] [smallint] NULL,
	[InsertedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_Attendance_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Attendance] UNIQUE NONCLUSTERED 
(
	[UserIdOnDevice] ASC,
	[AttendanceDateTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[AttendanceHookDefinition]') AND type in (N'U'))
BEGIN
CREATE TABLE [com].[AttendanceHookDefinition](
	[Id] [uniqueidentifier] NOT NULL,
	[AttendanceId] [uniqueidentifier] NOT NULL,
	[HookDefinitionId] [uniqueidentifier] NULL,
	[IsSent] [bit] NOT NULL,
	[RetryCount] [int] NOT NULL,
	[SentTime] [datetime2](7) NULL,
	[InsertedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_AttendanceHookDefinition] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_AttendanceHookDefinition] UNIQUE NONCLUSTERED 
(
	[AttendanceId] ASC,
	[HookDefinitionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[AttendanceSendToGuardianResult]') AND type in (N'U'))
BEGIN
CREATE TABLE [com].[AttendanceSendToGuardianResult](
	[Id] [uniqueidentifier] NOT NULL,
	[AttendanceId] [uniqueidentifier] NOT NULL,
	[SendTime] [datetime2](7) NOT NULL,
	[IsSuccessful] [bit] NOT NULL,
	[StatusCode] [int] NOT NULL,
	[ExceptionMessage] [nvarchar](max) NULL,
	[InsertedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_AttendanceSendToGuardianResult_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DeviceCommand]') AND type in (N'U'))
BEGIN
CREATE TABLE [com].[DeviceCommand](
	[Id] [uniqueidentifier] NOT NULL,
	[NumericId] [bigint] IDENTITY(1,1) NOT NULL,
	[DeviceId] [uniqueidentifier] NOT NULL,
	[DeviceNumber] [int] NOT NULL,
	[DeviceContent] [nvarchar](max) NOT NULL,
	[DeviceSerialNumber] [nvarchar](200) NOT NULL,
	[UserIdOnDevice] [bigint] NULL,
	[CommandContent] [nvarchar](max) NOT NULL,
	[CommitTime] [datetime2](7) NOT NULL,
	[SendTime] [datetime2](7) NULL,
	[ResponseTime] [datetime2](7) NULL,
	[ResponseValue] [nvarchar](max) NULL,
	[CommandType] [smallint] NOT NULL,
	[RetryCount] [tinyint] NOT NULL,
	[Priority] [tinyint] NOT NULL,
	[MaxRetry] [tinyint] NOT NULL,
	[Deadline] [datetime2](7) NULL,
	[ProducerNumber] [smallint] NOT NULL,
	[SdkVersion] [smallint] NOT NULL,
	[VisiblilityTime] [datetime2](7) NULL,
	[Description] [nvarchar](max) NULL,
	[CommandIdentifier] [uniqueidentifier] NULL,
	[InsertedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_DeviceCommand] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DeviceCommunicationData]') AND type in (N'U'))
BEGIN
CREATE TABLE [com].[DeviceCommunicationData](
	[Id] [uniqueidentifier] NOT NULL,
	[DeviceId] [uniqueidentifier] NOT NULL,
	[DeviceCommunicationDataInJson] [nvarchar](max) NULL,
	[InsertedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_DeviceCommunicationData_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[HookDefinition]') AND type in (N'U'))
BEGIN
CREATE TABLE [com].[HookDefinition](
	[Id] [uniqueidentifier] NOT NULL,
	[HookType] [smallint] NOT NULL,
	[EndPointUrl] [nvarchar](4000) NOT NULL,
	[HttpMethod] [int] NOT NULL,
	[AuthorizationType] [smallint] NOT NULL,
	[AuthorizationUsername] [nvarchar](200) NULL,
	[AuthorizationPassword] [nvarchar](200) NULL,
	[QueryStringTemplate] [nvarchar](max) NOT NULL,
	[HeaderTemplate] [nvarchar](max) NOT NULL,
	[BodyTemplate] [nvarchar](max) NOT NULL,
	[DateFormat] [nvarchar](500) NOT NULL,
	[RetryCount] [int] NOT NULL,
	[RequestTimeoutInSeconds] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[InsertedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_HookDefinition] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[core].[Device]') AND type in (N'U'))
BEGIN
CREATE TABLE [core].[Device](
	[Id] [uniqueidentifier] NOT NULL,
	[DeviceNumber] [int] NOT NULL,
	[DeviceTypeId] [uniqueidentifier] NOT NULL,
	[LocationId] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](1000) NOT NULL,
	[SerialNumber] [nvarchar](200) NULL,
	[DevicePassword] [nvarchar](200) NULL,
	[ConnectionMode] [smallint] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[ConnectionType] [smallint] NOT NULL,
	[DeviceIp] [nvarchar](25) NULL,
	[TcpPort] [int] NULL,
	[ConnectTimeout] [int] NOT NULL,
	[ModuleId] [int] NOT NULL,
	[IoType] [smallint] NOT NULL,
	[DeviceSettingInJson] [nvarchar](max) NOT NULL,
	[TimeZone] [int] NOT NULL,
	[DeviceDescription] [nvarchar](4000) NULL,
	[InsertedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_Device] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[core].[DeviceDoorBase]') AND type in (N'U'))
BEGIN
CREATE TABLE [core].[DeviceDoorBase](
	[Id] [uniqueidentifier] NOT NULL,
	[DeviceId] [uniqueidentifier] NOT NULL,
	[DoorNumber] [int] NOT NULL,
	[Title] [nvarchar](200) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[OpenDoorDelay] [int] NOT NULL,
	[DeviceSpecificDoorSettingInJson] [nvarchar](max) NOT NULL,
	[ReaderDeviceId] [uniqueidentifier] NULL,
	[ReaderCameraId] [uniqueidentifier] NULL,
	[InsertedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_DeviceDoorBase] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[com].[Attendance]') AND name = N'IX_InsertDateTime')
CREATE NONCLUSTERED INDEX [IX_InsertDateTime] ON [com].[Attendance]
(
	[InsertedAt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[com].[DeviceCommand]') AND name = N'IX_DeviceSerialNumber')
CREATE NONCLUSTERED INDEX [IX_DeviceSerialNumber] ON [com].[DeviceCommand]
(
	[DeviceSerialNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[com].[DeviceCommand]') AND name = N'IX_ZkNotSentDeviceCommands')
CREATE NONCLUSTERED INDEX [IX_ZkNotSentDeviceCommands] ON [com].[DeviceCommand]
(
	[ResponseTime] ASC,
	[ProducerNumber] ASC,
	[DeviceSerialNumber] ASC,
	[Deadline] ASC,
	[VisiblilityTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[core].[Device]') AND name = N'UQ_Device')
CREATE NONCLUSTERED INDEX [UQ_Device] ON [core].[Device]
(
	[DeviceNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_Attendance_IoType]') AND type = 'D')
BEGIN
ALTER TABLE [com].[Attendance] ADD  CONSTRAINT [DF_Attendance_IoType]  DEFAULT ((0)) FOR [IoType]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_Attendance_AttendanceSource]') AND type = 'D')
BEGIN
ALTER TABLE [com].[Attendance] ADD  CONSTRAINT [DF_Attendance_AttendanceSource]  DEFAULT ((1)) FOR [AttendanceSource]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_Attendance_InsertDateTime]') AND type = 'D')
BEGIN
ALTER TABLE [com].[Attendance] ADD  CONSTRAINT [DF_Attendance_InsertDateTime]  DEFAULT (getdate()) FOR [InsertedAt]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_Attendance_InsertDateTime1]') AND type = 'D')
BEGIN
ALTER TABLE [com].[Attendance] ADD  CONSTRAINT [DF_Attendance_InsertDateTime1]  DEFAULT (getdate()) FOR [UpdatedAt]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_AttendanceHookSystem_InsertedAt]') AND type = 'D')
BEGIN
ALTER TABLE [com].[AttendanceHookDefinition] ADD  CONSTRAINT [DF_AttendanceHookSystem_InsertedAt]  DEFAULT (getdate()) FOR [InsertedAt]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_AttendanceHookSystem_UpdatedAt]') AND type = 'D')
BEGIN
ALTER TABLE [com].[AttendanceHookDefinition] ADD  CONSTRAINT [DF_AttendanceHookSystem_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_AttendanceSendToGuardianResult_InsertedAt]') AND type = 'D')
BEGIN
ALTER TABLE [com].[AttendanceSendToGuardianResult] ADD  CONSTRAINT [DF_AttendanceSendToGuardianResult_InsertedAt]  DEFAULT (getdate()) FOR [InsertedAt]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_AttendanceSendToGuardianResult_UpdatedAt]') AND type = 'D')
BEGIN
ALTER TABLE [com].[AttendanceSendToGuardianResult] ADD  CONSTRAINT [DF_AttendanceSendToGuardianResult_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_DeviceCommand_DeviceContent]') AND type = 'D')
BEGIN
ALTER TABLE [com].[DeviceCommand] ADD  CONSTRAINT [DF_DeviceCommand_DeviceContent]  DEFAULT ('') FOR [DeviceContent]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_DeviceCommand_CommandType]') AND type = 'D')
BEGIN
ALTER TABLE [com].[DeviceCommand] ADD  CONSTRAINT [DF_DeviceCommand_CommandType]  DEFAULT ((0)) FOR [CommandType]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_DeviceCommand_RetryCount]') AND type = 'D')
BEGIN
ALTER TABLE [com].[DeviceCommand] ADD  CONSTRAINT [DF_DeviceCommand_RetryCount]  DEFAULT ((0)) FOR [RetryCount]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_DeviceCommand_Priority]') AND type = 'D')
BEGIN
ALTER TABLE [com].[DeviceCommand] ADD  CONSTRAINT [DF_DeviceCommand_Priority]  DEFAULT ((1)) FOR [Priority]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_DeviceCommand_MaxRetry]') AND type = 'D')
BEGIN
ALTER TABLE [com].[DeviceCommand] ADD  CONSTRAINT [DF_DeviceCommand_MaxRetry]  DEFAULT ((1)) FOR [MaxRetry]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_DeviceCommand_ProducerNumber]') AND type = 'D')
BEGIN
ALTER TABLE [com].[DeviceCommand] ADD  CONSTRAINT [DF_DeviceCommand_ProducerNumber]  DEFAULT ((1)) FOR [ProducerNumber]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_DeviceCommand_SdkVersion]') AND type = 'D')
BEGIN
ALTER TABLE [com].[DeviceCommand] ADD  CONSTRAINT [DF_DeviceCommand_SdkVersion]  DEFAULT ((1)) FOR [SdkVersion]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_DeviceCommand_InsertedAt]') AND type = 'D')
BEGIN
ALTER TABLE [com].[DeviceCommand] ADD  CONSTRAINT [DF_DeviceCommand_InsertedAt]  DEFAULT (getdate()) FOR [InsertedAt]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_DeviceCommand_UpdatedAt]') AND type = 'D')
BEGIN
ALTER TABLE [com].[DeviceCommand] ADD  CONSTRAINT [DF_DeviceCommand_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_DeviceCommunicationData_InsertedAt]') AND type = 'D')
BEGIN
ALTER TABLE [com].[DeviceCommunicationData] ADD  CONSTRAINT [DF_DeviceCommunicationData_InsertedAt]  DEFAULT (getdate()) FOR [InsertedAt]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_DeviceCommunicationData_UpdatedAt]') AND type = 'D')
BEGIN
ALTER TABLE [com].[DeviceCommunicationData] ADD  CONSTRAINT [DF_DeviceCommunicationData_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_HookDefinition_EndPointUrl]') AND type = 'D')
BEGIN
ALTER TABLE [com].[HookDefinition] ADD  CONSTRAINT [DF_HookDefinition_EndPointUrl]  DEFAULT ('') FOR [EndPointUrl]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_HookDefinition_HttpMethod]') AND type = 'D')
BEGIN
ALTER TABLE [com].[HookDefinition] ADD  CONSTRAINT [DF_HookDefinition_HttpMethod]  DEFAULT ((1)) FOR [HttpMethod]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_HookDefinition_AuthorizationType]') AND type = 'D')
BEGIN
ALTER TABLE [com].[HookDefinition] ADD  CONSTRAINT [DF_HookDefinition_AuthorizationType]  DEFAULT ((0)) FOR [AuthorizationType]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_HookDefinition_QueryStringTemplate]') AND type = 'D')
BEGIN
ALTER TABLE [com].[HookDefinition] ADD  CONSTRAINT [DF_HookDefinition_QueryStringTemplate]  DEFAULT ('') FOR [QueryStringTemplate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_HookDefinition_HeaderTemplate]') AND type = 'D')
BEGIN
ALTER TABLE [com].[HookDefinition] ADD  CONSTRAINT [DF_HookDefinition_HeaderTemplate]  DEFAULT ('') FOR [HeaderTemplate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_HookDefinition_BodyTemplate]') AND type = 'D')
BEGIN
ALTER TABLE [com].[HookDefinition] ADD  CONSTRAINT [DF_HookDefinition_BodyTemplate]  DEFAULT ('') FOR [BodyTemplate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_HookDefinition_DateFormat]') AND type = 'D')
BEGIN
ALTER TABLE [com].[HookDefinition] ADD  CONSTRAINT [DF_HookDefinition_DateFormat]  DEFAULT ('') FOR [DateFormat]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_HookDefinition_RetryCount]') AND type = 'D')
BEGIN
ALTER TABLE [com].[HookDefinition] ADD  CONSTRAINT [DF_HookDefinition_RetryCount]  DEFAULT ((5)) FOR [RetryCount]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_HookDefinition_RequestTimeoutInSeconds]') AND type = 'D')
BEGIN
ALTER TABLE [com].[HookDefinition] ADD  CONSTRAINT [DF_HookDefinition_RequestTimeoutInSeconds]  DEFAULT ((60)) FOR [RequestTimeoutInSeconds]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_HookDefinition_IsActive]') AND type = 'D')
BEGIN
ALTER TABLE [com].[HookDefinition] ADD  CONSTRAINT [DF_HookDefinition_IsActive]  DEFAULT ((0)) FOR [IsActive]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_HookDefinition_InsertedAt]') AND type = 'D')
BEGIN
ALTER TABLE [com].[HookDefinition] ADD  CONSTRAINT [DF_HookDefinition_InsertedAt]  DEFAULT (getdate()) FOR [InsertedAt]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DF_HookDefinition_UpdatedAt]') AND type = 'D')
BEGIN
ALTER TABLE [com].[HookDefinition] ADD  CONSTRAINT [DF_HookDefinition_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[core].[DF_Device_IoType]') AND type = 'D')
BEGIN
ALTER TABLE [core].[Device] ADD  CONSTRAINT [DF_Device_IoType]  DEFAULT ((0)) FOR [IoType]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[core].[DF_Device_DeviceSettings]') AND type = 'D')
BEGIN
ALTER TABLE [core].[Device] ADD  CONSTRAINT [DF_Device_DeviceSettings]  DEFAULT ((0)) FOR [DeviceSettingInJson]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[core].[DF_Device_TimeZone]') AND type = 'D')
BEGIN
ALTER TABLE [core].[Device] ADD  CONSTRAINT [DF_Device_TimeZone]  DEFAULT ((330)) FOR [TimeZone]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[core].[DF_Device_InsertedAt]') AND type = 'D')
BEGIN
ALTER TABLE [core].[Device] ADD  CONSTRAINT [DF_Device_InsertedAt]  DEFAULT (getdate()) FOR [InsertedAt]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[core].[DF_Device_UpdatedAt]') AND type = 'D')
BEGIN
ALTER TABLE [core].[Device] ADD  CONSTRAINT [DF_Device_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[core].[DF_DeviceDoorBase_IsActive]') AND type = 'D')
BEGIN
ALTER TABLE [core].[DeviceDoorBase] ADD  CONSTRAINT [DF_DeviceDoorBase_IsActive]  DEFAULT ((1)) FOR [IsActive]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[core].[DF_DeviceDoorBase_OpenDoorDelay]') AND type = 'D')
BEGIN
ALTER TABLE [core].[DeviceDoorBase] ADD  CONSTRAINT [DF_DeviceDoorBase_OpenDoorDelay]  DEFAULT ((0)) FOR [OpenDoorDelay]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[core].[DF_DeviceDoorBase_DeviceSpecificDoorSettingInJson]') AND type = 'D')
BEGIN
ALTER TABLE [core].[DeviceDoorBase] ADD  CONSTRAINT [DF_DeviceDoorBase_DeviceSpecificDoorSettingInJson]  DEFAULT ('') FOR [DeviceSpecificDoorSettingInJson]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[core].[DF_DeviceDoorBase_InsertedAt]') AND type = 'D')
BEGIN
ALTER TABLE [core].[DeviceDoorBase] ADD  CONSTRAINT [DF_DeviceDoorBase_InsertedAt]  DEFAULT (getdate()) FOR [InsertedAt]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[core].[DF_DeviceDoorBase_UpdatedAt]') AND type = 'D')
BEGIN
ALTER TABLE [core].[DeviceDoorBase] ADD  CONSTRAINT [DF_DeviceDoorBase_UpdatedAt]  DEFAULT (getdate()) FOR [UpdatedAt]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[com].[FK_AttendanceHookDefinition_Attendance]') AND parent_object_id = OBJECT_ID(N'[com].[AttendanceHookDefinition]'))
ALTER TABLE [com].[AttendanceHookDefinition]  WITH CHECK ADD  CONSTRAINT [FK_AttendanceHookDefinition_Attendance] FOREIGN KEY([AttendanceId])
REFERENCES [com].[Attendance] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[com].[FK_AttendanceHookDefinition_Attendance]') AND parent_object_id = OBJECT_ID(N'[com].[AttendanceHookDefinition]'))
ALTER TABLE [com].[AttendanceHookDefinition] CHECK CONSTRAINT [FK_AttendanceHookDefinition_Attendance]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[com].[FK_AttendanceHookDefinition_HookDefinition]') AND parent_object_id = OBJECT_ID(N'[com].[AttendanceHookDefinition]'))
ALTER TABLE [com].[AttendanceHookDefinition]  WITH CHECK ADD  CONSTRAINT [FK_AttendanceHookDefinition_HookDefinition] FOREIGN KEY([HookDefinitionId])
REFERENCES [com].[HookDefinition] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[com].[FK_AttendanceHookDefinition_HookDefinition]') AND parent_object_id = OBJECT_ID(N'[com].[AttendanceHookDefinition]'))
ALTER TABLE [com].[AttendanceHookDefinition] CHECK CONSTRAINT [FK_AttendanceHookDefinition_HookDefinition]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[com].[FK_AttendanceSendToGuardianResult_Attendance]') AND parent_object_id = OBJECT_ID(N'[com].[AttendanceSendToGuardianResult]'))
ALTER TABLE [com].[AttendanceSendToGuardianResult]  WITH CHECK ADD  CONSTRAINT [FK_AttendanceSendToGuardianResult_Attendance] FOREIGN KEY([AttendanceId])
REFERENCES [com].[Attendance] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[com].[FK_AttendanceSendToGuardianResult_Attendance]') AND parent_object_id = OBJECT_ID(N'[com].[AttendanceSendToGuardianResult]'))
ALTER TABLE [com].[AttendanceSendToGuardianResult] CHECK CONSTRAINT [FK_AttendanceSendToGuardianResult_Attendance]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[core].[FK_Device_DeviceType]') AND parent_object_id = OBJECT_ID(N'[core].[Device]'))
ALTER TABLE [core].[Device]  WITH CHECK ADD  CONSTRAINT [FK_Device_DeviceType] FOREIGN KEY([DeviceTypeId])
REFERENCES [core].[DeviceType] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[core].[FK_Device_DeviceType]') AND parent_object_id = OBJECT_ID(N'[core].[Device]'))
ALTER TABLE [core].[Device] CHECK CONSTRAINT [FK_Device_DeviceType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[core].[FK_DeviceDoorBase_Device]') AND parent_object_id = OBJECT_ID(N'[core].[DeviceDoorBase]'))
ALTER TABLE [core].[DeviceDoorBase]  WITH CHECK ADD  CONSTRAINT [FK_DeviceDoorBase_Device] FOREIGN KEY([DeviceId])
REFERENCES [core].[Device] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[core].[FK_DeviceDoorBase_Device]') AND parent_object_id = OBJECT_ID(N'[core].[DeviceDoorBase]'))
ALTER TABLE [core].[DeviceDoorBase] CHECK CONSTRAINT [FK_DeviceDoorBase_Device]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DeviceCommandCountByDeviceNumber]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [com].[DeviceCommandCountByDeviceNumber] AS' 
END
GO


ALTER PROCEDURE [com].[DeviceCommandCountByDeviceNumber] 
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
		LEFT JOIN com.DeviceCommand AS dc ON dc.DeviceNumber = s.IntegerValue
	WHERE
			dc.[ResponseTime] IS NULL
		AND dc.[RetryCount] < dc.[MaxRetry]
		AND (
				dc.[VisiblilityTime] IS NULL
				OR dc.[VisiblilityTime] <= GETDATE()
			)
		AND (
				dc.[Deadline] IS NULL 
				OR dc.[Deadline] >= GETDATE()
			)
		AND dc.ProducerNumber = @Producer
		AND dc.SdkVersion = @SdkVersion
    GROUP BY
        s.IntegerValue;
END


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DeviceCommandCountByDeviceId]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [com].[DeviceCommandCountByDeviceId] AS' 
END
GO


ALTER PROCEDURE [com].[DeviceCommandCountByDeviceId] 
	@Producer INT,
    @SdkVersion INT,
    @DeviceIdsList dbo.UniqueIdentifierValueListTable READONLY
AS 
BEGIN
	SET NOCOUNT ON;
    SELECT
        s.UniqueIdentifierValue AS DeviceId,
        COUNT(dc.DeviceId) AS DeviceId
    FROM @DeviceIdsList AS s
		LEFT JOIN com.DeviceCommand AS dc ON dc.DeviceId = s.UniqueIdentifierValue
	WHERE
			dc.[ResponseTime] IS NULL
		AND dc.[RetryCount] < dc.[MaxRetry]
		AND (
				dc.[VisiblilityTime] IS NULL
				OR dc.[VisiblilityTime] <= GETDATE()
			)
		AND (
				dc.[Deadline] IS NULL 
				OR dc.[Deadline] >= GETDATE()
			)
		AND dc.ProducerNumber = @Producer
		AND dc.SdkVersion = @SdkVersion
    GROUP BY
        s.UniqueIdentifierValue;
END



---- SCRIPT LOGICAL REGION -----

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DeviceCommandCountByDeviceSerialNumber]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [com].[DeviceCommandCountByDeviceSerialNumber] AS' 
END
GO

ALTER PROCEDURE [com].[DeviceCommandCountByDeviceSerialNumber] 
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
		LEFT JOIN com.DeviceCommand AS dc
			ON dc.DeviceSerialNumber = s.StringValue
	WHERE
			dc.[ResponseTime] IS NULL
		AND dc.[RetryCount] < dc.[MaxRetry]
		AND (
				dc.[VisiblilityTime] IS NULL
				OR dc.[VisiblilityTime] <= GETDATE()
			)
		AND (
				dc.[Deadline] IS NULL 
				OR dc.[Deadline] >= GETDATE()
			)
		AND dc.ProducerNumber = @Producer
		AND dc.SdkVersion = @SdkVersion
    GROUP BY
        s.StringValue;
END


GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'Attendance', N'COLUMN',N'Id'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'شناسه رکورد در دیتابیس' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'Attendance', @level2type=N'COLUMN',@level2name=N'Id'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'Attendance', N'COLUMN',N'UserIdOnDevice'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'شماره فرد بر روی دستگاه- این شماره ای است که فرد با آن بر روی سخت افزار تردد می زند' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'Attendance', @level2type=N'COLUMN',@level2name=N'UserIdOnDevice'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'Attendance', N'COLUMN',N'AttendanceDateTime'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'تاریخ و ساعت تردد' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'Attendance', @level2type=N'COLUMN',@level2name=N'AttendanceDateTime'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'Attendance', N'COLUMN',N'DeviceId'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'شناسه دستگاه- این شناسه ارجاع به جدول core.Device دارد' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'Attendance', @level2type=N'COLUMN',@level2name=N'DeviceId'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'Attendance', N'COLUMN',N'CameraId'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'شناسه دوربین- این شناسه ارجاع به جدول core.Camera دارد' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'Attendance', @level2type=N'COLUMN',@level2name=N'CameraId'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'Attendance', N'COLUMN',N'ReaderDeviceId'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'شناسه دستگاه-درحالتی که دستگاه اصلی کنترلر باشد، ممکن است تعداد زیادی Reader به آن وصل باشد. این فیلد مشخص می کند تردد بر روی کدام Reader ثبت شده است' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'Attendance', @level2type=N'COLUMN',@level2name=N'ReaderDeviceId'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'Attendance', N'COLUMN',N'VerificationStyle'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'نحوه احراز هویت
تعریف نشده = 0
اثر انگشت = 1
چهره = 2
کارت = 3
شناسه و رمز = 4
شناسه و اثر انگشت = 5
شناسه و اثر انگشت و رمز = 6
شناسه و چهره = 7
شناسه، چهره و رمز = 8
کارت و رمز = 9
کارت و اثر انگشت = 10
کارت و اثر انگشت و رمز = 11
کارت و چهره = 12
کارت و چهره و رمز = 13
اثر انگشت و رمز = 14
چهره و رمز = 15
چهره و اثر انگشت = 16
کف دست = 17
فقط شناسه = 18
کف دست و کارت = 19
کف دست و اثر انگشت = 20
کف دست و چهره = 21
عنبیه = 22
عنبیه و کارت = 23' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'Attendance', @level2type=N'COLUMN',@level2name=N'VerificationStyle'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'Attendance', N'COLUMN',N'RfCardNumber'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'شماره کارت RFID' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'Attendance', @level2type=N'COLUMN',@level2name=N'RfCardNumber'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'Attendance', N'COLUMN',N'StatusCode'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'کد کلید عملیات بر روی دستگاه' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'Attendance', @level2type=N'COLUMN',@level2name=N'StatusCode'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'Attendance', N'COLUMN',N'IsSentToGuardian'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'آیا تردد به سیستم گاردین منتقل شده است' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'Attendance', @level2type=N'COLUMN',@level2name=N'IsSentToGuardian'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'Attendance', N'COLUMN',N'ModuleId'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'کد ماژول
1= پایه
2 = حسابداری
4 = پرداخت
8 = مراجعین
16 = آسانسور
32 = کمدداری
64 = گزارش ساز
128 = تشخبص چهره
256 = پارکینگ
این فیلد به صورت Flag می باشد' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'Attendance', @level2type=N'COLUMN',@level2name=N'ModuleId'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'Attendance', N'COLUMN',N'IoType'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'نوع ورود و خروج
هیچکدام = 0
ورود = 1
خروج = 2
ورود و خروج = 3
' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'Attendance', @level2type=N'COLUMN',@level2name=N'IoType'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'Attendance', N'COLUMN',N'AttendanceSource'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'مشا تردد
1 = دستگاه
2 = دوربین تشخیص پلاک
3 = ثبت دستی تردد
4 = Device Server Match - در سیستم پارکینگ و در حالت تردد با انتن و یا تگ کاربرد دارد
5 = دوربین تشخیص چهره
' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'Attendance', @level2type=N'COLUMN',@level2name=N'AttendanceSource'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'Attendance', N'COLUMN',N'InsertedAt'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'تاریخ درج' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'Attendance', @level2type=N'COLUMN',@level2name=N'InsertedAt'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'Attendance', N'COLUMN',N'UpdatedAt'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'تاریخ اخرین به روزرسانی' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'Attendance', @level2type=N'COLUMN',@level2name=N'UpdatedAt'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'Attendance', NULL,NULL))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'این جدول در سیستم Gurdian.Communication کاربرد دارد
در حقیقت وقتی تردد ها از دستگاه های مختلف جمع آوری می شوند، در این حدول ذخیره می شوند تا از دست نروند. سپس به مقصد ها مختلف ارسال خواهند شد' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'Attendance'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'AttendanceHookDefinition', N'COLUMN',N'Id'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'شناسه رکورد در دیتابیس' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'AttendanceHookDefinition', @level2type=N'COLUMN',@level2name=N'Id'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'AttendanceHookDefinition', N'COLUMN',N'AttendanceId'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'شناسه تردد' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'AttendanceHookDefinition', @level2type=N'COLUMN',@level2name=N'AttendanceId'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'AttendanceHookDefinition', N'COLUMN',N'HookDefinitionId'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'شناسه رکورد هوک' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'AttendanceHookDefinition', @level2type=N'COLUMN',@level2name=N'HookDefinitionId'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'AttendanceHookDefinition', N'COLUMN',N'IsSent'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'آیا رکورد با موفقیت ارسال شده است یا خیر' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'AttendanceHookDefinition', @level2type=N'COLUMN',@level2name=N'IsSent'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'AttendanceHookDefinition', N'COLUMN',N'RetryCount'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'تعداد دفعات تلاش برای ارسال. ' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'AttendanceHookDefinition', @level2type=N'COLUMN',@level2name=N'RetryCount'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'AttendanceHookDefinition', N'COLUMN',N'SentTime'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'زمان ارسال موفق' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'AttendanceHookDefinition', @level2type=N'COLUMN',@level2name=N'SentTime'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'AttendanceHookDefinition', N'COLUMN',N'InsertedAt'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'تاریخ درج' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'AttendanceHookDefinition', @level2type=N'COLUMN',@level2name=N'InsertedAt'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'AttendanceHookDefinition', N'COLUMN',N'UpdatedAt'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'تاریخ اخرین به روزرسانی' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'AttendanceHookDefinition', @level2type=N'COLUMN',@level2name=N'UpdatedAt'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'AttendanceHookDefinition', NULL,NULL))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'تردد های برخی از دستگاه ها می بایست به سایر سیستم ها Hook شوند. تردد هایی که می بایست Hook شوند می بایست در این جدول رکوردی داشته باشند تا مشخص شوند اولا کدام تردد ها هستتند و ثانیا وضعیت Hook آنها به چه صورت است' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'AttendanceHookDefinition'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'AttendanceSendToGuardianResult', N'COLUMN',N'Id'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'شناسه رکورد در دیتابیس' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'AttendanceSendToGuardianResult', @level2type=N'COLUMN',@level2name=N'Id'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'AttendanceSendToGuardianResult', N'COLUMN',N'AttendanceId'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'شناسه تردد' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'AttendanceSendToGuardianResult', @level2type=N'COLUMN',@level2name=N'AttendanceId'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'AttendanceSendToGuardianResult', N'COLUMN',N'SendTime'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'زمان ارسال' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'AttendanceSendToGuardianResult', @level2type=N'COLUMN',@level2name=N'SendTime'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'AttendanceSendToGuardianResult', N'COLUMN',N'IsSuccessful'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'آیا ارسال موفقیت آمیز بوده است یا خیر' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'AttendanceSendToGuardianResult', @level2type=N'COLUMN',@level2name=N'IsSuccessful'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'AttendanceSendToGuardianResult', N'COLUMN',N'StatusCode'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'کد HTTP بازگردانده شده' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'AttendanceSendToGuardianResult', @level2type=N'COLUMN',@level2name=N'StatusCode'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'AttendanceSendToGuardianResult', N'COLUMN',N'ExceptionMessage'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'پیام خطا' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'AttendanceSendToGuardianResult', @level2type=N'COLUMN',@level2name=N'ExceptionMessage'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'AttendanceSendToGuardianResult', N'COLUMN',N'InsertedAt'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'تاریخ درج' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'AttendanceSendToGuardianResult', @level2type=N'COLUMN',@level2name=N'InsertedAt'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'AttendanceSendToGuardianResult', N'COLUMN',N'UpdatedAt'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'تاریخ اخرین به روزرسانی' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'AttendanceSendToGuardianResult', @level2type=N'COLUMN',@level2name=N'UpdatedAt'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'AttendanceSendToGuardianResult', NULL,NULL))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'نتیجه ارسال تردد ها به سیستم Gaurdian در این حدول نگه داری می شود. 
ممکن است دفعه اولی که تردد ارسال می شود، ارسال موفقیت آمیزی نداشته باشد. 
نتیجه عملیات ارسال هر جه باشد، در این جدول ذحیره می شود' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'AttendanceSendToGuardianResult'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommand', N'COLUMN',N'Id'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'شناسه رکورد در دیتابیس' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommand', @level2type=N'COLUMN',@level2name=N'Id'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommand', N'COLUMN',N'DeviceId'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'شناسه دستگاه- این شناسه ارجاع به جدول core.Device دارد' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommand', @level2type=N'COLUMN',@level2name=N'DeviceId'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommand', N'COLUMN',N'DeviceNumber'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'شماره دستگاه' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommand', @level2type=N'COLUMN',@level2name=N'DeviceNumber'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommand', N'COLUMN',N'DeviceContent'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'مشخصات دستگاه' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommand', @level2type=N'COLUMN',@level2name=N'DeviceContent'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommand', N'COLUMN',N'DeviceSerialNumber'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'شماره سریال دستگاه' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommand', @level2type=N'COLUMN',@level2name=N'DeviceSerialNumber'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommand', N'COLUMN',N'UserIdOnDevice'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'در صورتی که دستور مربوط به افزار باشد، شماره فرد بر روی دستگاه را مشخص می کند' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommand', @level2type=N'COLUMN',@level2name=N'UserIdOnDevice'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommand', N'COLUMN',N'CommandContent'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'محتویات دستور' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommand', @level2type=N'COLUMN',@level2name=N'CommandContent'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommand', N'COLUMN',N'CommitTime'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'زمان ثبت دستور' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommand', @level2type=N'COLUMN',@level2name=N'CommitTime'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommand', N'COLUMN',N'SendTime'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'زمان ارسال دستور به سخت افزار' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommand', @level2type=N'COLUMN',@level2name=N'SendTime'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommand', N'COLUMN',N'ResponseTime'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'زمان دریافت پاسخ سخت افزار' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommand', @level2type=N'COLUMN',@level2name=N'ResponseTime'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommand', N'COLUMN',N'ResponseValue'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'پاسخ سخت افزار به دستور' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommand', @level2type=N'COLUMN',@level2name=N'ResponseValue'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommand', N'COLUMN',N'CommandType'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'نوع دستور' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommand', @level2type=N'COLUMN',@level2name=N'CommandType'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommand', N'COLUMN',N'RetryCount'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'تعداد دفعات ارسال دستور - این فیلد نشان می دهد تا کنون چند با سعی به ارسال دستور شده است' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommand', @level2type=N'COLUMN',@level2name=N'RetryCount'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommand', N'COLUMN',N'Priority'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'اولویت ارسال دستور' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommand', @level2type=N'COLUMN',@level2name=N'Priority'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommand', N'COLUMN',N'MaxRetry'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'حداکثر تعداد دفعات مجاز ارسال دستور' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommand', @level2type=N'COLUMN',@level2name=N'MaxRetry'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommand', N'COLUMN',N'Deadline'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'حداکثر زمان مجاز ارسال دستورات' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommand', @level2type=N'COLUMN',@level2name=N'Deadline'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommand', N'COLUMN',N'ProducerNumber'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'کد تولید کننده سحت افزار
1 = ZK
2 = Suprema
4 = Virdi
8 = Timy
16 = Pouya Fanavaran - یک تولید کننده نرم افزار تشخیص پلاک ایرانی است
32 - Asa Guard - یک تولید کننده بورد های کنترلر ایرانی می باشد' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommand', @level2type=N'COLUMN',@level2name=N'ProducerNumber'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommand', N'COLUMN',N'SdkVersion'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'نسخه SDK سخت افزار
برخی از برند ها بیش از یک نسخه از سحت افزار دارند و منطق ارتباط آنها با یکدیگر فرق می کند
این فیلد به منظور تفکیک نحوه ارتباط میان دستگاه های یک برند به کار می رود' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommand', @level2type=N'COLUMN',@level2name=N'SdkVersion'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommand', N'COLUMN',N'VisiblilityTime'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'زمان شروع ارسال - در صورتی که این فیلد مقدار نداشته باشد، در همان لحظه به سخت افزار ارسال می شود، در غیر این صورت در زمان ذکر شده در این فیلد برای سیستم ارسال دستورات قابل مشاهده می شود' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommand', @level2type=N'COLUMN',@level2name=N'VisiblilityTime'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommand', N'COLUMN',N'Description'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'شرح' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommand', @level2type=N'COLUMN',@level2name=N'Description'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommand', N'COLUMN',N'CommandIdentifier'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'شناسه یکتای دستورات - این شناسه در بسیاری از مواقع به منظور ایجاد ارتباط میان دستورات متفاوت استفاده می شود' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommand', @level2type=N'COLUMN',@level2name=N'CommandIdentifier'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommand', N'COLUMN',N'InsertedAt'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'تاریخ درج' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommand', @level2type=N'COLUMN',@level2name=N'InsertedAt'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommand', N'COLUMN',N'UpdatedAt'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'تاریخ اخرین به روزرسانی' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommand', @level2type=N'COLUMN',@level2name=N'UpdatedAt'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommand', NULL,NULL))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'برای اطمیان از اینکه دستورات به دیستگاه ها ارسال می شوند، می بایست ابتدا آنها را در این جدول ذحیره کرد و سپس برای ارسال به هر سخت افزار اقدام کرد' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommand'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommunicationData', N'COLUMN',N'Id'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'شناسه رکورد در دیتابیس' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommunicationData', @level2type=N'COLUMN',@level2name=N'Id'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommunicationData', N'COLUMN',N'DeviceId'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'شناسه دستگاه- این شناسه ارجاع به جدول core.Device دارد' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommunicationData', @level2type=N'COLUMN',@level2name=N'DeviceId'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommunicationData', N'COLUMN',N'InsertedAt'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'تاریخ درج' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommunicationData', @level2type=N'COLUMN',@level2name=N'InsertedAt'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommunicationData', N'COLUMN',N'UpdatedAt'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'تاریخ اخرین به روزرسانی' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommunicationData', @level2type=N'COLUMN',@level2name=N'UpdatedAt'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'DeviceCommunicationData', NULL,NULL))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'برای ارتباط با دستگاه ها، گاهی نیاز است تنظیمات را از خود دستگاه و یا اخرین وضعیت ارتباطی آن ذحیره کرد که در این جدول قرار می گیرد' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'DeviceCommunicationData'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'HookDefinition', N'COLUMN',N'Id'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'شناسه رکورد در دیتابیس' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'HookDefinition', @level2type=N'COLUMN',@level2name=N'Id'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'HookDefinition', N'COLUMN',N'HookType'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'نوع داده ای که باید هوک شود
1 = تردد' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'HookDefinition', @level2type=N'COLUMN',@level2name=N'HookType'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'HookDefinition', N'COLUMN',N'EndPointUrl'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'آدرس سیستم مقصد' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'HookDefinition', @level2type=N'COLUMN',@level2name=N'EndPointUrl'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'HookDefinition', N'COLUMN',N'HttpMethod'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'HTTP Method مقصد' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'HookDefinition', @level2type=N'COLUMN',@level2name=N'HttpMethod'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'HookDefinition', N'COLUMN',N'AuthorizationType'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'نوع احراز هویت درخواست به سیستم مقصد' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'HookDefinition', @level2type=N'COLUMN',@level2name=N'AuthorizationType'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'HookDefinition', N'COLUMN',N'AuthorizationUsername'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'نام کاربری برای احراز هویت با سیستم مقصد' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'HookDefinition', @level2type=N'COLUMN',@level2name=N'AuthorizationUsername'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'HookDefinition', N'COLUMN',N'AuthorizationPassword'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'کلمه عبور برای احراز هویت با سیستم مقصد' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'HookDefinition', @level2type=N'COLUMN',@level2name=N'AuthorizationPassword'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'HookDefinition', N'COLUMN',N'QueryStringTemplate'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'قالب Query String' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'HookDefinition', @level2type=N'COLUMN',@level2name=N'QueryStringTemplate'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'HookDefinition', N'COLUMN',N'HeaderTemplate'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'قالب Header در Http Request' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'HookDefinition', @level2type=N'COLUMN',@level2name=N'HeaderTemplate'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'HookDefinition', N'COLUMN',N'BodyTemplate'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'قالب Body در Http Request' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'HookDefinition', @level2type=N'COLUMN',@level2name=N'BodyTemplate'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'HookDefinition', N'COLUMN',N'DateFormat'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'فرمت تاریخ' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'HookDefinition', @level2type=N'COLUMN',@level2name=N'DateFormat'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'HookDefinition', N'COLUMN',N'RetryCount'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'حداکثر تعداد دفعات مجاز ارسال داده' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'HookDefinition', @level2type=N'COLUMN',@level2name=N'RetryCount'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'HookDefinition', N'COLUMN',N'RequestTimeoutInSeconds'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Timeout برای Http Request' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'HookDefinition', @level2type=N'COLUMN',@level2name=N'RequestTimeoutInSeconds'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'HookDefinition', N'COLUMN',N'IsActive'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'آیا این رکورد در حال حاضر فعال است؟ برخی از موجودیت های سیستم قابلیت حذف شدن را به دلیل اینکه اطلاعات و وایستگی ها را ندارد. به همین دلیل باید آنها را غیر فعال نمود که در این فیلد مشخص می شود ' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'HookDefinition', @level2type=N'COLUMN',@level2name=N'IsActive'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'HookDefinition', N'COLUMN',N'InsertedAt'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'تاریخ درج' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'HookDefinition', @level2type=N'COLUMN',@level2name=N'InsertedAt'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'HookDefinition', N'COLUMN',N'UpdatedAt'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'تاریخ اخرین به روزرسانی' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'HookDefinition', @level2type=N'COLUMN',@level2name=N'UpdatedAt'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'com', N'TABLE',N'HookDefinition', NULL,NULL))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'بخش ارتباطات نرم افزار به دلیل اینکه یک سیستم سرویس Based و زنده است امکان تعریف Hook های مختلف برای ارسال اطلاعات برای سیستم های مختلف را دارد. این جدول در بر گیرنده سیستم های مقصد برای ارسال اطلاعات می باشد. ' , @level0type=N'SCHEMA',@level0name=N'com', @level1type=N'TABLE',@level1name=N'HookDefinition'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'Device', N'COLUMN',N'Id'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'شناسه رکورد در دیتابیس' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Device', @level2type=N'COLUMN',@level2name=N'Id'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'Device', N'COLUMN',N'DeviceNumber'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'شماره دستگاه - برای درک بهتر کاربر و جستجو برای هر دستگاه یک شماره در نظر گرفته می شود' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Device', @level2type=N'COLUMN',@level2name=N'DeviceNumber'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'Device', N'COLUMN',N'DeviceTypeId'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'شناسه نوع دستگاه - این فیلد به جدول core.DeviceType ارجاع دار' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Device', @level2type=N'COLUMN',@level2name=N'DeviceTypeId'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'Device', N'COLUMN',N'LocationId'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'شناسه محل نصب دستگاه - این فیلد به جدول core.Location ارجاع دارد' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Device', @level2type=N'COLUMN',@level2name=N'LocationId'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'Device', N'COLUMN',N'Title'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'عنوان' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Device', @level2type=N'COLUMN',@level2name=N'Title'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'Device', N'COLUMN',N'SerialNumber'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'شماره سریال' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Device', @level2type=N'COLUMN',@level2name=N'SerialNumber'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'Device', N'COLUMN',N'DevicePassword'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'کلمه عبور' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Device', @level2type=N'COLUMN',@level2name=N'DevicePassword'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'Device', N'COLUMN',N'ConnectionMode'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'نوع اتصال
1 = Standalone
2 = Push
' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Device', @level2type=N'COLUMN',@level2name=N'ConnectionMode'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'Device', N'COLUMN',N'ConnectionType'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'بستر ارتباط' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Device', @level2type=N'COLUMN',@level2name=N'ConnectionType'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'Device', N'COLUMN',N'DeviceIp'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'آدرس IP دستگاه' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Device', @level2type=N'COLUMN',@level2name=N'DeviceIp'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'Device', N'COLUMN',N'TcpPort'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'پورت' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Device', @level2type=N'COLUMN',@level2name=N'TcpPort'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'Device', N'COLUMN',N'ConnectTimeout'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'مدت زمان انتظار برای اتصال' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Device', @level2type=N'COLUMN',@level2name=N'ConnectTimeout'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'Device', N'COLUMN',N'ModuleId'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'کد ماژول
1= پایه
2 = حسابداری
4 = پرداخت
8 = مراجعین
16 = آسانسور
32 = کمدداری
64 = گزارش ساز
128 = تشخبص چهره
256 = پارکینگ
این فیلد به صورت Flag می باشد' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Device', @level2type=N'COLUMN',@level2name=N'ModuleId'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'Device', N'COLUMN',N'IoType'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'نوع ورود و خروج
هیچکدام = 0
ورود = 1
خروج = 2
ورود و خروج = 3
' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Device', @level2type=N'COLUMN',@level2name=N'IoType'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'Device', N'COLUMN',N'DeviceSettingInJson'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'تنظیمات دستگاه - این تنظیمات به صورت رشته و در فرمت JSON ذخیره می شوند. به دلیل اینکه دستگاه های ممکن است تنظیمات احتصاصی و یا مشترک زیادی داشته باشند، امکان اضافه یک فیلد برای هر تنظیم وجود ندارد. به همین دلیل این تنظیمات به صورت یک فیلد JSON نگه داری می شود. ' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Device', @level2type=N'COLUMN',@level2name=N'DeviceSettingInJson'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'Device', N'COLUMN',N'TimeZone'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'منطقه زمانی که دستگاه نصب است' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Device', @level2type=N'COLUMN',@level2name=N'TimeZone'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'Device', N'COLUMN',N'DeviceDescription'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'توضیحات' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Device', @level2type=N'COLUMN',@level2name=N'DeviceDescription'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'Device', N'COLUMN',N'InsertedAt'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'تاریخ درج' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Device', @level2type=N'COLUMN',@level2name=N'InsertedAt'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'Device', N'COLUMN',N'UpdatedAt'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'تاریخ اخرین به روزرسانی' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Device', @level2type=N'COLUMN',@level2name=N'UpdatedAt'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'Device', NULL,NULL))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'مشخصات دستگاه ها' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Device'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'DeviceDoorBase', N'COLUMN',N'Id'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'شناسه رکورد در دیتابیس' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'DeviceDoorBase', @level2type=N'COLUMN',@level2name=N'Id'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'DeviceDoorBase', N'COLUMN',N'DeviceId'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'شناسه دستگاه- این شناسه ارجاع به جدول core.Device دارد' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'DeviceDoorBase', @level2type=N'COLUMN',@level2name=N'DeviceId'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'DeviceDoorBase', N'COLUMN',N'DoorNumber'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'شماره درب - برای درک بهتر کاربر و جستجو برای هر درب یک شماره در نظر گرفته می شود' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'DeviceDoorBase', @level2type=N'COLUMN',@level2name=N'DoorNumber'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'DeviceDoorBase', N'COLUMN',N'Title'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'عنوان' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'DeviceDoorBase', @level2type=N'COLUMN',@level2name=N'Title'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'DeviceDoorBase', N'COLUMN',N'IsActive'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'آیا این رکورد در حال حاضر فعال است؟ برخی از موجودیت های سیستم قابلیت حذف شدن را به دلیل اینکه اطلاعات و وایستگی ها را ندارد. به همین دلیل باید آنها را غیر فعال نمود که در این فیلد مشخص می شود ' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'DeviceDoorBase', @level2type=N'COLUMN',@level2name=N'IsActive'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'DeviceDoorBase', N'COLUMN',N'OpenDoorDelay'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'مدت زمان تاخیر برای بسته شدن درب پس از باز شدن (بر حسب ثانیه)' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'DeviceDoorBase', @level2type=N'COLUMN',@level2name=N'OpenDoorDelay'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'DeviceDoorBase', N'COLUMN',N'DeviceSpecificDoorSettingInJson'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'تنظیمات اختصاصی درب بر حسب نوع دستگاه، به صورت Json' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'DeviceDoorBase', @level2type=N'COLUMN',@level2name=N'DeviceSpecificDoorSettingInJson'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'DeviceDoorBase', N'COLUMN',N'InsertedAt'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'تاریخ درج' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'DeviceDoorBase', @level2type=N'COLUMN',@level2name=N'InsertedAt'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'DeviceDoorBase', N'COLUMN',N'UpdatedAt'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'تاریخ اخرین به روزرسانی' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'DeviceDoorBase', @level2type=N'COLUMN',@level2name=N'UpdatedAt'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'core', N'TABLE',N'DeviceDoorBase', NULL,NULL))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'مشخصات مشترک درب های ' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'DeviceDoorBase'
GO
