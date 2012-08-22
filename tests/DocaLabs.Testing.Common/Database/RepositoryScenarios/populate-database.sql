INSERT INTO [dbo].[Tiles] 
			([Id])
     VALUES 
			('F8027143-F8B9-41B4-9A49-6A6A74BED529')
GO

INSERT INTO [dbo].[InterestingPoints]
           ([Id]
           ,[Category]
           ,[Tile_Id])
     VALUES
           ('07709830-323E-4277-B604-7B6980C65919'
           ,'Blue'
           ,'F8027143-F8B9-41B4-9A49-6A6A74BED529')
GO

INSERT INTO [dbo].[InterestingPoints]
           ([Id]
           ,[Category]
           ,[Tile_Id])
     VALUES
           ('FE3D1F1F-7E4E-4DBE-9894-C063602664A4'
           ,'Red'
           ,'F8027143-F8B9-41B4-9A49-6A6A74BED529')
GO

INSERT INTO [dbo].[InterestingPoints]
           ([Id]
           ,[Category]
           ,[Tile_Id])
     VALUES
           ('1368B5E6-3B87-4078-868F-4A1CA4B50622'
           ,'Green'
           ,'F8027143-F8B9-41B4-9A49-6A6A74BED529')
GO

INSERT INTO [dbo].[Places]
           ([Id]
           ,[Name]
           ,[Tile_Id])
     VALUES
           ('F4145B48-F796-4373-8AF3-F9F9B8983C09'
           ,'Place 0'
           ,'F8027143-F8B9-41B4-9A49-6A6A74BED529')
GO

INSERT INTO [dbo].[Places]
           ([Id]
           ,[Name]
           ,[Tile_Id])
     VALUES
           ('2AB17DE7-FD5F-4DD4-9F36-DC2ECE6F5A8F'
           ,'Place 1'
           ,'F8027143-F8B9-41B4-9A49-6A6A74BED529')
GO

INSERT INTO [dbo].[Places]
           ([Id]
           ,[Name]
           ,[Tile_Id])
     VALUES
           ('1C92AB7B-FF5F-4588-A304-05F07870B0A6'
           ,'Place 2'
           ,'F8027143-F8B9-41B4-9A49-6A6A74BED529')
GO

INSERT INTO [dbo].[Places]
           ([Id]
           ,[Name]
           ,[Tile_Id])
     VALUES
           ('9BC9770F-2862-42D9-B7F2-1065C2F27EBA'
           ,'Place 3'
           ,'F8027143-F8B9-41B4-9A49-6A6A74BED529')
GO

INSERT INTO [dbo].[Places]
           ([Id]
           ,[Name]
           ,[Tile_Id])
     VALUES
           ('07B7629C-A0C0-46B1-A815-9927A8555E5B'
           ,'Place 4'
           ,'F8027143-F8B9-41B4-9A49-6A6A74BED529')
GO

INSERT INTO [dbo].[Places]
           ([Id]
           ,[Name]
           ,[Tile_Id])
     VALUES
           ('7C2E64CA-5ECC-4D7D-82DB-2593D7293CD8'
           ,'Place 5'
           ,'F8027143-F8B9-41B4-9A49-6A6A74BED529')
GO

INSERT INTO [dbo].[PlaceInterestingPoints]
           ([InterestingPoint_Id]
           ,[Place_Id])
     VALUES
           ('1368B5E6-3B87-4078-868F-4A1CA4B50622'
           ,'9BC9770F-2862-42D9-B7F2-1065C2F27EBA')
GO

INSERT INTO [dbo].[PlaceInterestingPoints]
           ([InterestingPoint_Id]
           ,[Place_Id])
     VALUES
           ('07709830-323E-4277-B604-7B6980C65919'
           ,'1C92AB7B-FF5F-4588-A304-05F07870B0A6')
GO

INSERT INTO [dbo].[PlaceInterestingPoints]
           ([InterestingPoint_Id]
           ,[Place_Id])
     VALUES
           ('07709830-323E-4277-B604-7B6980C65919'
           ,'07B7629C-A0C0-46B1-A815-9927A8555E5B')
GO

INSERT INTO [dbo].[PlaceInterestingPoints]
           ([InterestingPoint_Id]
           ,[Place_Id])
     VALUES
           ('07709830-323E-4277-B604-7B6980C65919'
           ,'F4145B48-F796-4373-8AF3-F9F9B8983C09')
GO

INSERT INTO [dbo].[PlaceInterestingPoints]
           ([InterestingPoint_Id]
           ,[Place_Id])
     VALUES
           ('FE3D1F1F-7E4E-4DBE-9894-C063602664A4'
           ,'1C92AB7B-FF5F-4588-A304-05F07870B0A6')
GO

INSERT INTO [dbo].[PlaceInterestingPoints]
           ([InterestingPoint_Id]
           ,[Place_Id])
     VALUES
           ('FE3D1F1F-7E4E-4DBE-9894-C063602664A4'
           ,'9BC9770F-2862-42D9-B7F2-1065C2F27EBA')
GO

INSERT INTO [dbo].[PlaceInterestingPoints]
           ([InterestingPoint_Id]
           ,[Place_Id])
     VALUES
           ('FE3D1F1F-7E4E-4DBE-9894-C063602664A4'
           ,'2AB17DE7-FD5F-4DD4-9F36-DC2ECE6F5A8F')
GO
