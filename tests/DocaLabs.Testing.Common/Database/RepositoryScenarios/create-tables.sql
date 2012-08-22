/****** Object:  Table [Tiles] ******/
CREATE TABLE [dbo].[Tiles](
	[Id] [uniqueidentifier] NOT NULL,
	[Version] [Timestamp] NOT NULL,
	CONSTRAINT [PK_Tiles] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

/****** Object:  Table [InterestingPoints] ******/
CREATE TABLE [dbo].[InterestingPoints](
	[Id] [uniqueidentifier] NOT NULL,
	[Category] [nvarchar](500) NULL,
	[Tile_Id] [uniqueidentifier] NOT NULL,
	[Version] [Timestamp] NOT NULL,
	CONSTRAINT [PK_InterestingPoints] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

/****** Object:  Table [Places] ******/
CREATE TABLE [dbo].[Places](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](500) NULL,
	[Tile_Id] [uniqueidentifier] NOT NULL,
	[Version] [Timestamp] NOT NULL,
	CONSTRAINT [PK_Places] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

/****** Object:  Table [PlaceInterestingPoints] ******/
CREATE TABLE [dbo].[PlaceInterestingPoints](
	[InterestingPoint_Id] [uniqueidentifier] NOT NULL,
	[Place_Id] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_PointPlaces] PRIMARY KEY CLUSTERED ([InterestingPoint_Id] ASC, [Place_Id] ASC)
)
GO


--/****** Object:  ForeignKey [FK_InterestingPoints_Tiles] ******/
ALTER TABLE [dbo].[InterestingPoints]  WITH CHECK ADD  CONSTRAINT [FK_InterestingPoints_Tiles] FOREIGN KEY([Tile_Id]) REFERENCES [dbo].[Tiles] ([Id])
GO
ALTER TABLE [dbo].[InterestingPoints] CHECK CONSTRAINT [FK_InterestingPoints_Tiles]
GO

/****** Object:  ForeignKey [FK_Places_Tiles] ******/
ALTER TABLE [dbo].[Places]  WITH CHECK ADD  CONSTRAINT [FK_Places_Tiles] FOREIGN KEY([Tile_Id]) REFERENCES [dbo].[Tiles] ([Id])
GO
ALTER TABLE [dbo].[Places] CHECK CONSTRAINT [FK_Places_Tiles]
GO

/****** Object:  ForeignKey [FK_PointPlaces_Point] ******/
ALTER TABLE [dbo].[PlaceInterestingPoints]  WITH CHECK ADD  CONSTRAINT [FK_PointPlaces_Point] FOREIGN KEY([InterestingPoint_Id]) REFERENCES [dbo].[InterestingPoints] ([Id])
GO
ALTER TABLE [dbo].[PlaceInterestingPoints] CHECK CONSTRAINT [FK_PointPlaces_Point]
GO

/****** Object:  ForeignKey [FK_PointPlaces_Place] ******/
ALTER TABLE [dbo].[PlaceInterestingPoints]  WITH CHECK ADD  CONSTRAINT [FK_PointPlaces_Place] FOREIGN KEY([Place_Id]) REFERENCES [dbo].[Places] ([Id])
GO
ALTER TABLE [dbo].[PlaceInterestingPoints] CHECK CONSTRAINT [FK_PointPlaces_Place]
GO
