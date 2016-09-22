﻿GO
CREATE TABLE [dbo].[ELMAH_Error](
	[ErrorId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_ELMAH_Error_ErrorId]  DEFAULT (newid()),
	[Application] [nvarchar](60) NOT NULL,
	[Host] [nvarchar](50) NOT NULL,
	[Type] [nvarchar](100) NOT NULL,
	[Source] [nvarchar](60) NOT NULL,
	[Message] [nvarchar](500) NOT NULL,
	[User] [nvarchar](50) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[TimeUtc] [datetime] NOT NULL,
	[Sequence] [int] IDENTITY(1,1) NOT NULL,
	[AllXml] [ntext] NOT NULL,
 CONSTRAINT [PK_ELMAH_Error] PRIMARY KEY CLUSTERED 
(
	[ErrorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
) 

GO
CREATE NONCLUSTERED INDEX [IX_ELMAH_Error_App_Time_Seq] ON [dbo].[ELMAH_Error]
(
	[Application] ASC,
	[TimeUtc] DESC,
	[Sequence] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]



GO
CREATE PROCEDURE [dbo].[ELMAH_GetErrorXml]
(
	@Application NVARCHAR(60),
	@ErrorId UNIQUEIDENTIFIER
)
AS

	SET NOCOUNT ON

	SELECT 
		[AllXml]
	FROM 
		[ELMAH_Error]
	WHERE
		[ErrorId] = @ErrorId
	AND
		[Application] = @Application


GO
CREATE PROCEDURE [dbo].[ELMAH_GetErrorsXml]
(
	@Application NVARCHAR(60),
	@PageIndex INT = 0,
	@PageSize INT = 15,
	@TotalCount INT OUTPUT
)
AS 

	SET NOCOUNT ON

	DECLARE @FirstTimeUTC DATETIME
	DECLARE @FirstSequence INT
	DECLARE @StartRow INT
	DECLARE @StartRowIndex INT

	SELECT 
		@TotalCount = COUNT(1) 
	FROM 
		[ELMAH_Error]
	WHERE 
		[Application] = @Application

	-- Get the ID of the first error for the requested page

	SET @StartRowIndex = @PageIndex * @PageSize + 1

	IF @StartRowIndex <= @TotalCount
	BEGIN

		SET ROWCOUNT @StartRowIndex

		SELECT  
			@FirstTimeUTC = [TimeUtc],
			@FirstSequence = [Sequence]
		FROM 
			[ELMAH_Error]
		WHERE   
			[Application] = @Application
		ORDER BY 
			[TimeUtc] DESC, 
			[Sequence] DESC

	END
	ELSE
	BEGIN

		SET @PageSize = 0

	END

	-- Now set the row count to the requested page size and get
	-- all records below it for the pertaining application.

	SET ROWCOUNT @PageSize

	SELECT 
		errorId     = [ErrorId], 
		application = [Application],
		host        = [Host], 
		type        = [Type],
		source      = [Source],
		message     = [Message],
		[user]      = [User],
		statusCode  = [StatusCode], 
		time        = CONVERT(VARCHAR(50), [TimeUtc], 126) + 'Z'
	FROM 
		[ELMAH_Error] error
	WHERE
		[Application] = @Application
	AND
		[TimeUtc] <= @FirstTimeUTC
	AND 
		[Sequence] <= @FirstSequence
	ORDER BY
		[TimeUtc] DESC, 
		[Sequence] DESC
	FOR
		XML AUTO


GO
CREATE PROCEDURE [dbo].[ELMAH_LogError]
(
	@ErrorId UNIQUEIDENTIFIER,
	@Application NVARCHAR(60),
	@Host NVARCHAR(30),
	@Type NVARCHAR(100),
	@Source NVARCHAR(60),
	@Message NVARCHAR(500),
	@User NVARCHAR(50),
	@AllXml NTEXT,
	@StatusCode INT,
	@TimeUtc DATETIME
)
AS

	SET NOCOUNT ON

	INSERT
	INTO
		[ELMAH_Error]
		(
			[ErrorId],
			[Application],
			[Host],
			[Type],
			[Source],
			[Message],
			[User],
			[AllXml],
			[StatusCode],
			[TimeUtc]
		)
	VALUES
		(
			@ErrorId,
			@Application,
			@Host,
			@Type,
			@Source,
			@Message,
			@User,
			@AllXml,
			@StatusCode,
			@TimeUtc
		)

