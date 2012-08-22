
--/****** Object:  ForeignKey [FK_InterestingPoints_Tiles] ******/
ALTER TABLE [dbo].[InterestingPoints] DROP CONSTRAINT [FK_InterestingPoints_Tiles]
GO
ALTER TABLE [dbo].[InterestingPoints]  WITH CHECK ADD  CONSTRAINT [FK_InterestingPoints_Tiles] FOREIGN KEY([Tile_Id]) REFERENCES [dbo].[Tiles] ([Id]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[InterestingPoints] CHECK CONSTRAINT [FK_InterestingPoints_Tiles]
GO

/****** Object:  ForeignKey [FK_Places_Tiles] ******/
ALTER TABLE [dbo].[Places]  DROP CONSTRAINT [FK_Places_Tiles]
GO
ALTER TABLE [dbo].[Places]  WITH CHECK ADD  CONSTRAINT [FK_Places_Tiles] FOREIGN KEY([Tile_Id]) REFERENCES [dbo].[Tiles] ([Id]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Places] CHECK CONSTRAINT [FK_Places_Tiles]
GO

/****** Object:  ForeignKey [FK_PointPlaces_Point] ******/
ALTER TABLE [dbo].[PlaceInterestingPoints]  DROP CONSTRAINT [FK_PointPlaces_Point]
GO
ALTER TABLE [dbo].[PlaceInterestingPoints]  WITH CHECK ADD  CONSTRAINT [FK_PointPlaces_Point] FOREIGN KEY([InterestingPoint_Id]) REFERENCES [dbo].[InterestingPoints] ([Id])
GO
ALTER TABLE [dbo].[PlaceInterestingPoints] CHECK CONSTRAINT [FK_PointPlaces_Point]
GO

/****** Object:  ForeignKey [FK_PointPlaces_Place] ******/
ALTER TABLE [dbo].[PlaceInterestingPoints]  DROP  CONSTRAINT [FK_PointPlaces_Place]
GO
ALTER TABLE [dbo].[PlaceInterestingPoints]  WITH CHECK ADD  CONSTRAINT [FK_PointPlaces_Place] FOREIGN KEY([Place_Id]) REFERENCES [dbo].[Places] ([Id]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PlaceInterestingPoints] CHECK CONSTRAINT [FK_PointPlaces_Place]
GO
