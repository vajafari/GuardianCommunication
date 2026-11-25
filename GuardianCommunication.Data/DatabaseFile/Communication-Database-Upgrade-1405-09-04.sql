USE [Guardian]
GO
-- =====================================================================================================
-- ارتقای دیتابیس سرویس ارتباطات گاردین (1405/09/04)
-- دیتابیس موجود را با تغییرات این نسخه هم سان می کند:
--   1) ستون های Cron و LastRunAt جدول com.HookDefinition (زمان بند)
--   2) پیش فرض getutcdate برای ستون های InsertedAt و UpdatedAt جدول های com
--   3) رویه های شمارش دستورهای ارسال نشده با GETUTCDATE
--   4) تنظیمات تازه در com.SystemConfig: فاصله تیک زمان بند و بورد کنترلر گاردین
--   5) توضیح جدول و ستون های com.HookDefinition
--   6) گزارش وضعیت همه موارد بالا (در پایان؛ هر ردیفی که Status آن OK نیست هنوز درست نشده است)
-- اجرای چندباره بی خطر است: هر مورد فقط وقتی هنوز اعمال نشده اعمال می شود و مقدار تنظیم های موجود دست نمی خورد.
-- اگر نام دیتابیس سرویس ارتباطات Guardian نیست، خط USE بالا را عوض کنید.
-- =====================================================================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF OBJECT_ID(N'com.HookDefinition', N'U') IS NULL OR OBJECT_ID(N'com.DeviceCommand', N'U') IS NULL
BEGIN
	DECLARE @DatabaseName sysname = DB_NAME();
	RAISERROR(N'جدول های سرویس ارتباطات در دیتابیس %s نیستند؛ دیتابیس درست را انتخاب کنید. هیچ تغییری داده نشد', 16, 1, @DatabaseName);
	SET NOEXEC ON;
END
GO


-- 1) ستون های زمان بند: Cron عبارت پنج بخشی ردیف های نوع 2 و LastRunAt زمان شروع آخرین اجرا (UTC)
IF COL_LENGTH(N'com.HookDefinition', N'Cron') IS NULL
BEGIN
	ALTER TABLE [com].[HookDefinition] ADD [Cron] [nvarchar](100) NULL;
	PRINT N'ستون com.HookDefinition.Cron اضافه شد';
END
GO
IF COL_LENGTH(N'com.HookDefinition', N'LastRunAt') IS NULL
BEGIN
	ALTER TABLE [com].[HookDefinition] ADD [LastRunAt] [datetime2](7) NULL;
	PRINT N'ستون com.HookDefinition.LastRunAt اضافه شد';
END
GO


-- 2) پیش فرض InsertedAt و UpdatedAt جدول های com به getutcdate؛ پیش فرضی که چیز دیگری است برداشته و دوباره ساخته می شود
IF OBJECT_ID(N'tempdb..#UpgradeDefaults') IS NOT NULL
	DROP TABLE #UpgradeDefaults;
CREATE TABLE #UpgradeDefaults ([Order] int IDENTITY(1, 1) NOT NULL, [TableName] sysname NOT NULL, [ColumnName] sysname NOT NULL, [ConstraintName] sysname NOT NULL);
INSERT INTO #UpgradeDefaults ([TableName], [ColumnName], [ConstraintName])
VALUES
	  (N'Attendance', N'InsertedAt', N'DF_Attendance_InsertDateTime')
	, (N'Attendance', N'UpdatedAt', N'DF_Attendance_InsertDateTime1')
	, (N'AttendanceHookDefinition', N'InsertedAt', N'DF_AttendanceHookSystem_InsertedAt')
	, (N'AttendanceHookDefinition', N'UpdatedAt', N'DF_AttendanceHookSystem_UpdatedAt')
	, (N'AttendanceSendToGuardianResult', N'InsertedAt', N'DF_AttendanceSendToGuardianResult_InsertedAt')
	, (N'AttendanceSendToGuardianResult', N'UpdatedAt', N'DF_AttendanceSendToGuardianResult_UpdatedAt')
	, (N'DeviceCommand', N'InsertedAt', N'DF_DeviceCommand_InsertedAt')
	, (N'DeviceCommand', N'UpdatedAt', N'DF_DeviceCommand_UpdatedAt')
	, (N'DeviceCommunicationData', N'InsertedAt', N'DF_DeviceCommunicationData_InsertedAt')
	, (N'DeviceCommunicationData', N'UpdatedAt', N'DF_DeviceCommunicationData_UpdatedAt')
	, (N'HookDefinition', N'InsertedAt', N'DF_HookDefinition_InsertedAt')
	, (N'HookDefinition', N'UpdatedAt', N'DF_HookDefinition_UpdatedAt');

DECLARE @TableName sysname, @ColumnName sysname, @ConstraintName sysname, @CurrentName sysname, @CurrentDefinition nvarchar(max), @Sql nvarchar(max);
DECLARE DefaultCursor CURSOR LOCAL FAST_FORWARD FOR
	SELECT [TableName], [ColumnName], [ConstraintName] FROM #UpgradeDefaults;
OPEN DefaultCursor;
FETCH NEXT FROM DefaultCursor INTO @TableName, @ColumnName, @ConstraintName;
WHILE @@FETCH_STATUS = 0
BEGIN
	IF COL_LENGTH(N'com.' + @TableName, @ColumnName) IS NOT NULL
	BEGIN
		SELECT @CurrentName = NULL, @CurrentDefinition = NULL;
		SELECT @CurrentName = dc.[name], @CurrentDefinition = dc.[definition]
		FROM sys.default_constraints AS dc
			INNER JOIN sys.columns AS c ON c.[object_id] = dc.[parent_object_id] AND c.[column_id] = dc.[parent_column_id]
		WHERE dc.[parent_object_id] = OBJECT_ID(N'com.' + @TableName) AND c.[name] = @ColumnName;

		IF @CurrentName IS NOT NULL AND LOWER(REPLACE(@CurrentDefinition, N' ', N'')) <> N'(getutcdate())'
		BEGIN
			SET @Sql = N'ALTER TABLE [com].' + QUOTENAME(@TableName) + N' DROP CONSTRAINT ' + QUOTENAME(@CurrentName) + N';';
			EXEC sys.sp_executesql @Sql;
			SET @CurrentName = NULL;
		END
		IF @CurrentName IS NULL
		BEGIN
			SET @Sql = N'ALTER TABLE [com].' + QUOTENAME(@TableName) + N' ADD CONSTRAINT ' + QUOTENAME(@ConstraintName)
				+ N' DEFAULT (getutcdate()) FOR ' + QUOTENAME(@ColumnName) + N';';
			EXEC sys.sp_executesql @Sql;
			PRINT N'پیش فرض com.' + @TableName + N'.' + @ColumnName + N' به getutcdate تغییر کرد';
		END
	END
	FETCH NEXT FROM DefaultCursor INTO @TableName, @ColumnName, @ConstraintName;
END
CLOSE DefaultCursor;
DEALLOCATE DefaultCursor;
GO


-- 3) رویه های شمارش دستورهای ارسال نشده: Deadline و VisiblilityTime دستورها UTC ذخیره می شوند و با GETUTCDATE مقایسه می شوند.
-- ستون شمارش رویه DeviceCommandCountByDeviceId هم مثل دو رویه دیگر CommandCount است
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DeviceCommandCountByDeviceNumber]') AND type in (N'P', N'PC'))
	EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [com].[DeviceCommandCountByDeviceNumber] AS'
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
				OR dc.[VisiblilityTime] <= GETUTCDATE()
			)
		AND (
				dc.[Deadline] IS NULL
				OR dc.[Deadline] >= GETUTCDATE()
			)
		AND dc.ProducerNumber = @Producer
		AND dc.SdkVersion = @SdkVersion
    GROUP BY
        s.IntegerValue;
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DeviceCommandCountByDeviceId]') AND type in (N'P', N'PC'))
	EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [com].[DeviceCommandCountByDeviceId] AS'
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
        COUNT(dc.DeviceId) AS CommandCount
    FROM @DeviceIdsList AS s
		LEFT JOIN com.DeviceCommand AS dc ON dc.DeviceId = s.UniqueIdentifierValue
	WHERE
			dc.[ResponseTime] IS NULL
		AND dc.[RetryCount] < dc.[MaxRetry]
		AND (
				dc.[VisiblilityTime] IS NULL
				OR dc.[VisiblilityTime] <= GETUTCDATE()
			)
		AND (
				dc.[Deadline] IS NULL
				OR dc.[Deadline] >= GETUTCDATE()
			)
		AND dc.ProducerNumber = @Producer
		AND dc.SdkVersion = @SdkVersion
    GROUP BY
        s.UniqueIdentifierValue;
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[com].[DeviceCommandCountByDeviceSerialNumber]') AND type in (N'P', N'PC'))
	EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [com].[DeviceCommandCountByDeviceSerialNumber] AS'
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
				OR dc.[VisiblilityTime] <= GETUTCDATE()
			)
		AND (
				dc.[Deadline] IS NULL
				OR dc.[Deadline] >= GETUTCDATE()
			)
		AND dc.ProducerNumber = @Producer
		AND dc.SdkVersion = @SdkVersion
    GROUP BY
        s.StringValue;
END
GO


-- 4) تنظیمات تازه در com.SystemConfig؛ کلیدی که هست دست نمی خورد. جدول com.SystemConfig را این اسکریپت نمی سازد
IF OBJECT_ID(N'tempdb..#UpgradeConfigs') IS NOT NULL
	DROP TABLE #UpgradeConfigs;
CREATE TABLE #UpgradeConfigs ([Order] int IDENTITY(1, 1) NOT NULL, [ConfigName] nvarchar(200) NOT NULL, [ConfigValue] nvarchar(100) NOT NULL);
INSERT INTO #UpgradeConfigs ([ConfigName], [ConfigValue])
VALUES
	-- فاصله تیک زمان بند (CronHookTask) به ثانیه؛ یک دقیقه
	  (N'CronHookTimerIntervalInSeconds', N'60')
	-- بورد کنترلر گاردین
	, (N'GuardianControllerMaxRetryForUserCommands', N'20')
	, (N'GuardianControllerMaxRetryForOtherCommands', N'10')
	, (N'GuardianControllerGrpcServerCommandCount', N'1')
	, (N'GuardianControllerGrpcServerPort', N'37426')
	, (N'GuardianControllerGrpcCommandTimerIntervalInMillisecond', N'1000')
	, (N'GuardianControllerGrpcServerTimeoutShortInMillisecond', N'5000')
	, (N'GuardianControllerGrpcServerTimeoutLongInMillisecond', N'15000')
	, (N'GuardianControllerGrpcServerTimeoutVeryLongInMillisecond', N'60000');

IF OBJECT_ID(N'com.SystemConfig', N'U') IS NULL
	RAISERROR(N'جدول com.SystemConfig در این دیتابیس نیست؛ تنظیمات اضافه نشدند', 16, 1);
ELSE
BEGIN
	INSERT INTO [com].[SystemConfig] ([ConfigName], [ConfigValue])
	SELECT cfg.[ConfigName], cfg.[ConfigValue]
	FROM #UpgradeConfigs AS cfg
	WHERE NOT EXISTS (SELECT * FROM [com].[SystemConfig] AS sc WHERE sc.[ConfigName] = cfg.[ConfigName]);
	PRINT N'تعداد تنظیم های اضافه شده: ' + CAST(@@ROWCOUNT AS nvarchar(10));
END
GO


-- 5) توضیح جدول و ستون های com.HookDefinition (همان متن اسکریپت اصلی)
DECLARE @Description nvarchar(4000);

SET @Description = N'نوع داده ای که باید هوک شود
1 = تردد
2 = زمان بند (Cron): آدرس در زمان های عبارت Cron ردیف صدا زده می شود';
IF EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description', N'SCHEMA', N'com', N'TABLE', N'HookDefinition', N'COLUMN', N'HookType'))
	EXEC sys.sp_updateextendedproperty @name = N'MS_Description', @value = @Description, @level0type = N'SCHEMA', @level0name = N'com', @level1type = N'TABLE', @level1name = N'HookDefinition', @level2type = N'COLUMN', @level2name = N'HookType';
ELSE
	EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = @Description, @level0type = N'SCHEMA', @level0name = N'com', @level1type = N'TABLE', @level1name = N'HookDefinition', @level2type = N'COLUMN', @level2name = N'HookType';

SET @Description = N'عبارت Cron ردیف زمان بند (فقط نوع 2): پنج بخش دقیقه، ساعت، روز ماه، ماه و روز هفته (0 و 7 یکشنبه، 6 شنبه) به وقت محلی سرویس ارتباطات؛ هر بخش * یا عدد یا بازه a-b است، گام با /n و چند مقدار با , جدا می شوند. آدرسی که با / شروع شود مسیری روی Guardian است و با آدرس و کاربر Guardian در تنظیمات سرویس صدا زده می شود و آدرس کامل با احراز هویت خود ردیف';
IF EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description', N'SCHEMA', N'com', N'TABLE', N'HookDefinition', N'COLUMN', N'Cron'))
	EXEC sys.sp_updateextendedproperty @name = N'MS_Description', @value = @Description, @level0type = N'SCHEMA', @level0name = N'com', @level1type = N'TABLE', @level1name = N'HookDefinition', @level2type = N'COLUMN', @level2name = N'Cron';
ELSE
	EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = @Description, @level0type = N'SCHEMA', @level0name = N'com', @level1type = N'TABLE', @level1name = N'HookDefinition', @level2type = N'COLUMN', @level2name = N'Cron';

SET @Description = N'زمان شروع آخرین اجرای ردیف زمان بند (UTC)، چه فراخوانی موفق بوده باشد چه نه؛ خالی یعنی هنوز اجرا نشده و اولین تیک زمان بند اجرایش می کند. زمان های جاافتاده (مثلا وقتی سرویس خاموش بوده) روی هم یک اجرا می شوند';
IF EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description', N'SCHEMA', N'com', N'TABLE', N'HookDefinition', N'COLUMN', N'LastRunAt'))
	EXEC sys.sp_updateextendedproperty @name = N'MS_Description', @value = @Description, @level0type = N'SCHEMA', @level0name = N'com', @level1type = N'TABLE', @level1name = N'HookDefinition', @level2type = N'COLUMN', @level2name = N'LastRunAt';
ELSE
	EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = @Description, @level0type = N'SCHEMA', @level0name = N'com', @level1type = N'TABLE', @level1name = N'HookDefinition', @level2type = N'COLUMN', @level2name = N'LastRunAt';

SET @Description = N'بخش ارتباطات نرم افزار به دلیل اینکه یک سیستم سرویس Based و زنده است امکان تعریف Hook های مختلف برای ارسال اطلاعات برای سیستم های مختلف را دارد. این جدول در بر گیرنده سیستم های مقصد برای ارسال اطلاعات می باشد. ردیف های نوع زمان بند (Cron) آدرس هایی هستند که سرویس در زمان های عبارت Cron صدا می زند، مثل کارهای زمان دار Guardian (فرستادن صف اعلان ها، رویدادهای زمان دار، همگام سازی روزانه دسترسی قراردادها)؛ این ردیف ها فقط با اسکریپت ساخته می شوند';
IF EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description', N'SCHEMA', N'com', N'TABLE', N'HookDefinition', NULL, NULL))
	EXEC sys.sp_updateextendedproperty @name = N'MS_Description', @value = @Description, @level0type = N'SCHEMA', @level0name = N'com', @level1type = N'TABLE', @level1name = N'HookDefinition';
ELSE
	EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = @Description, @level0type = N'SCHEMA', @level0name = N'com', @level1type = N'TABLE', @level1name = N'HookDefinition';
GO


-- 6) گزارش وضعیت؛ هر ردیفی که Status آن OK نیست هنوز در دیتابیس نیست یا قدیمی است
DECLARE @Report TABLE ([Row] int IDENTITY(1, 1) NOT NULL, [Section] nvarchar(50) NOT NULL, [Item] nvarchar(300) NOT NULL, [Status] nvarchar(20) NOT NULL, [Value] nvarchar(400) NULL);

INSERT INTO @Report ([Section], [Item], [Status])
SELECT N'ستون', N'com.HookDefinition.' + col.[ColumnName]
	, CASE WHEN COL_LENGTH(N'com.HookDefinition', col.[ColumnName]) IS NULL THEN N'ندارد' ELSE N'OK' END
FROM (VALUES (1, N'Cron'), (2, N'LastRunAt')) AS col([Order], [ColumnName])
ORDER BY col.[Order];

INSERT INTO @Report ([Section], [Item], [Status], [Value])
SELECT N'پیش فرض', N'com.' + d.[TableName] + N'.' + d.[ColumnName]
	, CASE WHEN LOWER(REPLACE(dc.[definition], N' ', N'')) = N'(getutcdate())' THEN N'OK' ELSE N'ندارد' END
	, dc.[definition]
FROM #UpgradeDefaults AS d
	LEFT JOIN sys.columns AS c ON c.[object_id] = OBJECT_ID(N'com.' + d.[TableName]) AND c.[name] = d.[ColumnName]
	LEFT JOIN sys.default_constraints AS dc ON dc.[parent_object_id] = c.[object_id] AND dc.[parent_column_id] = c.[column_id]
ORDER BY d.[Order];

INSERT INTO @Report ([Section], [Item], [Status])
SELECT N'رویه', N'com.' + prc.[ProcedureName]
	, CASE
		WHEN m.[definition] IS NULL THEN N'ندارد'
		WHEN m.[definition] LIKE N'%GETUTCDATE()%'
			AND m.[definition] NOT LIKE N'%GETDATE()%'
			AND m.[definition] NOT LIKE N'%sp_executesql%'
			AND m.[definition] NOT LIKE N'%COUNT(dc.DeviceId) AS DeviceId%' THEN N'OK'
		ELSE N'قدیمی'
	  END
FROM (VALUES (1, N'DeviceCommandCountByDeviceNumber'), (2, N'DeviceCommandCountByDeviceId'), (3, N'DeviceCommandCountByDeviceSerialNumber')) AS prc([Order], [ProcedureName])
	LEFT JOIN sys.sql_modules AS m ON m.[object_id] = OBJECT_ID(N'com.' + prc.[ProcedureName])
ORDER BY prc.[Order];

IF OBJECT_ID(N'com.SystemConfig', N'U') IS NULL
	INSERT INTO @Report ([Section], [Item], [Status]) VALUES (N'تنظیم', N'com.SystemConfig', N'ندارد');
ELSE
	INSERT INTO @Report ([Section], [Item], [Status], [Value])
	SELECT N'تنظیم', cfg.[ConfigName]
		, CASE WHEN sc.[ConfigName] IS NULL THEN N'ندارد' ELSE N'OK' END
		, sc.[ConfigValue]
	FROM #UpgradeConfigs AS cfg
		LEFT JOIN [com].[SystemConfig] AS sc ON sc.[ConfigName] = cfg.[ConfigName]
	ORDER BY cfg.[Order];

SELECT [Section], [Item], [Status], [Value] FROM @Report ORDER BY [Row];

IF EXISTS (SELECT * FROM @Report WHERE [Status] <> N'OK')
	RAISERROR(N'بعضی موارد در دیتابیس نیستند یا قدیمی اند؛ ستون Status گزارش را ببینید', 16, 1);
ELSE
	PRINT N'همه تغییرات این نسخه در دیتابیس هست';

IF OBJECT_ID(N'tempdb..#UpgradeDefaults') IS NOT NULL
	DROP TABLE #UpgradeDefaults;
IF OBJECT_ID(N'tempdb..#UpgradeConfigs') IS NOT NULL
	DROP TABLE #UpgradeConfigs;
GO
SET NOEXEC OFF
GO
