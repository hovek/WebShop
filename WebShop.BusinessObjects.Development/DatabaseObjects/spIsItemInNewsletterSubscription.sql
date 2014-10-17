CREATE PROCEDURE spIsItemInNewsletterSubscription
	@ItemId INT,
	@LanguageId INT
AS 
IF EXISTS ( SELECT	i.Id
			FROM	dbo.NewsletterSubscription ns
			INNER JOIN dbo.Item i ON ns.ItemId = i.Id
									 AND ns.LanguageId = @LanguageId
			INNER JOIN dbo.Item i2 ON i2.Node.IsDescendantOf(i.Node) = 1
									  AND i2.Id = @ItemId ) 
	BEGIN
		SELECT	CONVERT(BIT, 1)
	END
ELSE 
	BEGIN
		SELECT	CONVERT(BIT, 0)
	END