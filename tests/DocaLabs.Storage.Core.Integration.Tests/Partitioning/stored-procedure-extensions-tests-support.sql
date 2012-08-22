CREATE PROCEDURE [RunTest]
	@param1 [int],
	@param2 [nvarchar](1000)
AS
BEGIN

	SET NOCOUNT ON

	IF @param2 IS NULL
		RETURN 200
	ELSE
		RETURN @param1
		
END
GO
