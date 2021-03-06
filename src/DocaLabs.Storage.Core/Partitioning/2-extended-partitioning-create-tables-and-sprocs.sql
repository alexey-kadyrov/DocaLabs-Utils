-- **************************************
-- dropping objects if they already exist
-- **************************************

IF (OBJECT_ID('[GetOrAutoAssignPartition]') IS NOT NULL)
  DROP PROCEDURE [GetOrAutoAssignPartition]
GO

IF (OBJECT_ID('[AddKeyToManualPartition]') IS NOT NULL)
  DROP PROCEDURE [AddKeyToManualPartition]
GO

IF (OBJECT_ID('[AddNewPartition]') IS NOT NULL)
  DROP PROCEDURE [AddNewPartition]
GO

IF (OBJECT_ID('[GetConnectionString]') IS NOT NULL)
  DROP PROCEDURE [GetConnectionString]
GO

IF (OBJECT_ID('[PartitionCount]') IS NOT NULL)
  DROP TABLE [PartitionCount]
GO

IF (OBJECT_ID('[PartitionMap]') IS NOT NULL)
  DROP TABLE [PartitionMap]
GO


-- **************************************
-- creating
-- **************************************

-- table: [PartitionMap]
CREATE TABLE [PartitionMap] (
	[Key] [varchar](50) NOT NULL,
	[Partition] [int] NOT NULL,
	CONSTRAINT [PK_PartitionMap] PRIMARY KEY NONCLUSTERED ([Key] ASC)
) 
GO

-- table: [PartitionCount]
CREATE TABLE [PartitionCount] (
	[Partition] [int] NOT NULL,
	[Count] [int] NOT NULL,
	[AutoAssign] [bit] NOT NULL CONSTRAINT [DF_PartitionCount_AutoAssign]  DEFAULT ((1)),
	CONSTRAINT [PK_PartitionCount] PRIMARY KEY CLUSTERED ([Partition] ASC)
) 
GO

-- -- stored procedure: [GetConnectionString]
CREATE PROCEDURE [GetConnectionString]
	@partition [int]
AS
BEGIN

	SET NOCOUNT ON

	SELECT [Partition], [ProviderName], [ConnectionString] FROM [PartitionConnectionStrings] WHERE [Partition] = @partition
		
END
GO

-- stored procedure: [AddNewPartition]
CREATE PROCEDURE [AddNewPartition]
	@newPartition [int],
	@providerName [nvarchar](4000),
	@connectionString [nvarchar](4000),
	@autoAssign [bit] 
AS
BEGIN

	SET NOCOUNT ON

	INSERT INTO [PartitionCount] ([Partition], [Count], [AutoAssign]) VALUES(@newPartition, 0, @autoAssign)
		
	EXEC [AddConnectionString] @newPartition, @providerName, @connectionString

	RETURN (@newPartition)
	
END
GO

-- stored procedure: [AddKeyToManualPartition]
CREATE PROCEDURE [AddKeyToManualPartition] 
	@partition [int],
	@key [varchar](50)
AS
BEGIN

	SET NOCOUNT ON

	IF EXISTS(SELECT * FROM [PartitionMap] WITH (TABLOCKX, HOLDLOCK) WHERE [Key] = @key)
	BEGIN
		RAISERROR('The key already EXISTS', 18, 1)
		RETURN (-1)
	END

	DECLARE @existingAutoAssignFlag [bit]

	SELECT @existingAutoAssignFlag = [AutoAssign] FROM [PartitionCount] WITH (TABLOCKX, HOLDLOCK) WHERE [Partition] = @partition

	IF @existingAutoAssignFlag IS NOT NULL
	BEGIN
		IF @existingAutoAssignFlag <> 0
		BEGIN
			RAISERROR('The existing partition is auto assign', 18, 1)
			RETURN (-1)
		END

		UPDATE [PartitionCount] SET [Count] = [Count] + 1 WHERE [Partition] = @partition
	END
	ELSE
	BEGIN
		RAISERROR('The partition does not exist', 18, 1)
		RETURN (-1)
	END
		
	INSERT INTO [PartitionMap] ([Key], [Partition]) VALUES(@key, @partition)
	
	RETURN (@partition)

END
GO

-- stored procedure: [GetOrAutoAssignPartition]
CREATE PROCEDURE [GetOrAutoAssignPartition]
	@key [varchar](50)
AS
BEGIN

	SET NOCOUNT ON
	
	DECLARE @partition [int]
	
	-- very important to have exclusive lock for the table for the TRANSACTION's 
	-- duration as it's used in cluster environment to assign partitions to keys
	SELECT @partition = [Partition] FROM [PartitionMap] WITH (TABLOCKX, HOLDLOCK) WHERE [Key] = @key
			
	IF @partition IS NULL
	BEGIN
	
		SELECT TOP 1 @partition = [Partition] FROM [PartitionCount] WITH (TABLOCKX, HOLDLOCK) WHERE [AutoAssign] = 1 ORDER BY [Count] ASC
			
		IF @partition IS NULL
		BEGIN
			RAISERROR('There is no suitable partitions', 18, 1)
			RETURN (-1)
		END
			
		UPDATE [PartitionCount] SET [Count] = [Count] + 1 WHERE [Partition] = @partition

		INSERT INTO [PartitionMap] ([Key], [Partition]) VALUES(@key, @partition)

	END
		
	RETURN (@partition)
		
END
GO

