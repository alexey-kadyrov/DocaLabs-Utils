-- **************************************
-- dropping objects if they already exist
-- **************************************

IF (OBJECT_ID('[AddConnectionString]') IS NOT NULL)
  DROP PROCEDURE [AddConnectionString]
GO

IF (OBJECT_ID('[GetApplicationConnectionStrings]') IS NOT NULL)
  DROP PROCEDURE [GetApplicationConnectionStrings]
GO

IF (OBJECT_ID('[PartitionConnectionStrings]') IS NOT NULL)
  DROP TABLE [PartitionConnectionStrings]
GO


-- **************************************
-- creating
-- **************************************

-- table: [PartitionConnectionStrings]
CREATE TABLE [PartitionConnectionStrings] (
	[Partition] [int] NOT NULL,
	[ProviderName] [nvarchar](4000) NULL,
	[ConnectionString] [nvarchar](4000) NOT NULL,
	CONSTRAINT [PK_PartitionConnectionStrings] PRIMARY KEY CLUSTERED ([Partition] ASC) 
) 
GO

-- -- stored procedure: [GetApplicationConnectionStrings]
CREATE PROCEDURE [GetApplicationConnectionStrings]
AS
BEGIN
	SET NOCOUNT ON
	SELECT [Partition], [ProviderName], [ConnectionString] FROM [PartitionConnectionStrings] ORDER BY [Partition]
END
GO

-- -- stored procedure: [AddConnectionString]
CREATE PROCEDURE [AddConnectionString]
	@partition [int],
	@providerName [nvarchar](4000),
	@connectionString [nvarchar](4000)
AS
BEGIN
	SET NOCOUNT ON
	INSERT INTO [PartitionConnectionStrings] ([Partition], [ProviderName], [ConnectionString]) VALUES (@partition, @providerName, @connectionString)
END
GO
